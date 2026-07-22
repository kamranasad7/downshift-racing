using UnityEngine;

namespace Downshift
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SkyGradient : MonoBehaviour
    {
        public Transform cam;
        public Color topColor = new Color32(0x2E, 0x3A, 0x59, 0xFF);
        public Color horizonColor = new Color32(0xB8, 0x88, 0x6B, 0xFF);
        public Vector2 worldSize = new Vector2(60f, 14f);
        public int textureHeight = 256;

        SpriteRenderer _sr;

        void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _sr.sortingOrder = -100;
            Build();
        }

        void Build()
        {
            var tex = new Texture2D(2, textureHeight, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            for (int y = 0; y < textureHeight; y++)
            {
                float t = y / (float)(textureHeight - 1);
                var c = Color.Lerp(horizonColor, topColor, t);
                tex.SetPixel(0, y, c);
                tex.SetPixel(1, y, c);
            }
            tex.Apply();

            var sprite = Sprite.Create(tex, new Rect(0, 0, 2, textureHeight), new Vector2(0.5f, 0.5f), 1f);
            _sr.sprite = sprite;
            transform.localScale = new Vector3(worldSize.x / 2f, worldSize.y / textureHeight, 1f);
        }

        void LateUpdate()
        {
            if (cam == null) return;
            var p = cam.position;
            transform.position = new Vector3(p.x, p.y, transform.position.z);
        }
    }
}
