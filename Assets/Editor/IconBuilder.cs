using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Downshift.EditorTools
{
    public static class IconBuilder
    {
        const int Size = 512;
        const string Path = "Assets/Art/icon.png";

        [MenuItem("Downshift/Build/App Icon")]
        public static void BuildIcon()
        {
            var top = HexColor("2E3A59");
            var horizon = HexColor("B8886B");
            var road = Color.white;
            var carColor = HexColor("D94D33");

            var waypoints = new[]
            {
                new Vector2(70, 30),
                new Vector2(440, 150),
                new Vector2(120, 260),
                new Vector2(440, 370),
                new Vector2(200, 480),
            };
            const float roadHalfWidth = 20f;
            const float carRadius = 22f;
            var carCenter = Vector2.Lerp(waypoints[3], waypoints[4], 0.65f);

            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            for (int py = 0; py < Size; py++)
            {
                int designY = Size - 1 - py;
                var bg = Color.Lerp(top, horizon, designY / (float)(Size - 1));

                for (int px = 0; px < Size; px++)
                {
                    var p = new Vector2(px, designY);
                    var color = bg;

                    float minDist = float.MaxValue;
                    for (int i = 0; i < waypoints.Length - 1; i++)
                        minDist = Mathf.Min(minDist, DistToSegment(p, waypoints[i], waypoints[i + 1]));
                    if (minDist <= roadHalfWidth)
                        color = road;

                    if (Vector2.Distance(p, carCenter) <= carRadius)
                        color = carColor;

                    tex.SetPixel(px, py, color);
                }
            }
            tex.Apply();

            var png = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);

            if (!AssetDatabase.IsValidFolder("Assets/Art"))
                AssetDatabase.CreateFolder("Assets", "Art");
            File.WriteAllBytes(Path, png);
            AssetDatabase.ImportAsset(Path, ImportAssetOptions.ForceUpdate);

            var importer = (TextureImporter)AssetImporter.GetAtPath(Path);
            importer.textureType = TextureImporterType.Default;
            importer.alphaIsTransparency = false;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();

            var imported = AssetDatabase.LoadAssetAtPath<Texture2D>(Path);
            PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new[] { imported }, IconKind.Any);

            var assigned = PlayerSettings.GetIcons(NamedBuildTarget.Unknown, IconKind.Any);
            bool verified = assigned.Length > 0 && assigned[0] == imported;
            Debug.Log(verified
                ? "IconBuilder: icon assigned and verified via PlayerSettings.GetIcons."
                : "IconBuilder: icon assignment FAILED verification.");
        }

        static float DistToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            var ab = b - a;
            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / ab.sqrMagnitude);
            var proj = a + t * ab;
            return Vector2.Distance(p, proj);
        }

        static Color HexColor(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out var c);
            return c;
        }
    }
}
