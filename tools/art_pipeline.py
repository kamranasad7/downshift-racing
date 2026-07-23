import sys
from pathlib import Path
from PIL import Image

TOLERANCE = 28
PADDING = 2


def key_color(img):
    corners = [img.getpixel(p) for p in [(0, 0), (img.width - 1, 0), (0, img.height - 1), (img.width - 1, img.height - 1)]]
    return max(set(corners), key=corners.count)


def chroma_key(img, key, tolerance=TOLERANCE):
    img = img.convert("RGBA")
    data = img.getdata()
    kr, kg, kb = key[0], key[1], key[2]
    out = []
    for px in data:
        r, g, b, a = px
        if abs(r - kr) <= tolerance and abs(g - kg) <= tolerance and abs(b - kb) <= tolerance:
            out.append((r, g, b, 0))
        else:
            out.append((r, g, b, a))
    img.putdata(out)
    return img


def autocrop(img, padding=PADDING):
    bbox = img.getbbox()
    if bbox is None:
        return img
    left = max(0, bbox[0] - padding)
    top = max(0, bbox[1] - padding)
    right = min(img.width, bbox[2] + padding)
    bottom = min(img.height, bbox[3] + padding)
    return img.crop((left, top, right, bottom))


def process(src, dst_dir, max_size=None):
    img = Image.open(src)
    key = key_color(img.convert("RGBA"))
    img = chroma_key(img, key)
    img = autocrop(img)
    if max_size and max(img.size) > max_size:
        scale = max_size / max(img.size)
        img = img.resize((int(img.width * scale), int(img.height * scale)), Image.LANCZOS)
    dst = Path(dst_dir) / (Path(src).stem + ".png")
    dst.parent.mkdir(parents=True, exist_ok=True)
    img.save(dst)
    return dst, img.size


if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("usage: art_pipeline.py <src_file_or_dir> <dst_dir> [max_size]")
        sys.exit(1)
    src, dst_dir = sys.argv[1], sys.argv[2]
    max_size = int(sys.argv[3]) if len(sys.argv) > 3 else None
    sources = [Path(src)] if Path(src).is_file() else sorted(p for p in Path(src).iterdir() if p.suffix.lower() in (".png", ".jpg", ".jpeg", ".webp"))
    for s in sources:
        out, size = process(s, dst_dir, max_size)
        print(f"{s.name} -> {out} {size[0]}x{size[1]}")
