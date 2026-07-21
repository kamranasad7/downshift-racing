using UnityEditor;
using UnityEngine;
using System.IO;

namespace Downshift.EditorTools
{
    public static class SpriteFactory
    {
        public static Sprite Ensure(string name, bool circle)
        {
            string path = $"Assets/Art/{name}.png";
            if (!AssetDatabase.IsValidFolder("Assets/Art"))
                AssetDatabase.CreateFolder("Assets", "Art");
            if (AssetDatabase.LoadAssetAtPath<Sprite>(path) == null)
            {
                const int size = 64;
                var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                var center = new Vector2(size / 2f, size / 2f);
                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    bool inside = !circle || Vector2.Distance(new Vector2(x, y), center) <= size / 2f;
                    tex.SetPixel(x, y, inside ? Color.white : Color.clear);
                }
                tex.Apply();
                File.WriteAllBytes(path, tex.EncodeToPNG());
                AssetDatabase.ImportAsset(path);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 64f;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
