using UnityEditor;
using UnityEngine;

namespace Downshift.EditorTools
{
    public static class HazardAssetsBuilder
    {
        static readonly Color RockGrey = HexColor("6E6A63");
        static readonly Color RidgeGrey = HexColor("5A554C");
        static readonly Color SignBoard = HexColor("E8B84B");
        static readonly Color SignDark = HexColor("2A2E38");

        [MenuItem("Downshift/Build/Hazard Prefabs")]
        public static void BuildHazardPrefabs()
        {
            BuildRocks();
            BuildRidge();
            BuildWashboard();
            BuildSign();
        }

        static void BuildRocks()
        {
            var circle = SpriteFactory.Ensure("circle", true);
            var root = new GameObject("Rocks");
            AddCircleHazard(root, "RockA", circle, RockGrey, new Vector2(-0.3f, 0.15f - 0.28f), 0.28f);
            AddCircleHazard(root, "RockB", circle, RockGrey, new Vector2(0.22f, 0.1f - 0.2f), 0.2f);
            Save(root, "Rocks");
        }

        static void BuildRidge()
        {
            var square = SpriteFactory.Ensure("square", false);
            var root = new GameObject("Ridge");
            var child = new GameObject("Diamond");
            child.transform.SetParent(root.transform, false);
            child.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            child.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            child.transform.localScale = Vector3.one * 0.5f;
            var sr = child.AddComponent<SpriteRenderer>();
            sr.sprite = square;
            sr.color = RidgeGrey;
            sr.sortingOrder = -1;
            var col = child.AddComponent<BoxCollider2D>();
            col.isTrigger = false;
            Save(root, "Ridge");
        }

        static void BuildWashboard()
        {
            var circle = SpriteFactory.Ensure("circle", true);
            var root = new GameObject("Washboard");
            float[] offsets = { -2.4f, -1.2f, 0f, 1.2f, 2.4f };
            for (int i = 0; i < offsets.Length; i++)
                AddCircleHazard(root, $"Bump{i}", circle, RidgeGrey, new Vector2(offsets[i], 0.12f), 0.12f);
            Save(root, "Washboard");
        }

        static void AddCircleHazard(GameObject root, string name, Sprite circle, Color color, Vector2 localPos, float radius)
        {
            var go = new GameObject(name);
            go.transform.SetParent(root.transform, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = Vector3.one * (radius * 2f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = circle;
            sr.color = color;
            sr.sortingOrder = -1;
            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.5f;
            col.isTrigger = false;
        }

        static void BuildSign()
        {
            var square = SpriteFactory.Ensure("square", false);
            var root = new GameObject("Sign");
            AddQuad(root, "Pole", square, SignDark, new Vector3(0f, -1.0f, 0f), new Vector3(0.1f, 1.2f, 1f), 0f, -2);
            AddQuad(root, "Border", square, SignDark, new Vector3(0f, -0.3f, 0f), Vector3.one * 0.52f, 45f, -3);
            AddQuad(root, "Board", square, SignBoard, new Vector3(0f, -0.3f, 0f), Vector3.one * 0.42f, 45f, -2);
            Save(root, "Sign");
        }

        static void AddQuad(GameObject parent, string name, Sprite sprite, Color color, Vector3 localPos, Vector3 localScale, float zRot, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            go.transform.localRotation = Quaternion.Euler(0f, 0f, zRot);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = sortingOrder;
        }

        static void Save(GameObject root, string name)
        {
            string path = $"Assets/Prefabs/{name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        static Color HexColor(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out var c);
            return c;
        }
    }
}
