using UnityEditor;
using UnityEngine;
using Downshift;

namespace Downshift.EditorTools
{
    public static class VehiclePrefabBuilder
    {
        [MenuItem("Downshift/Build/Hatchback Prefab")]
        public static void Build()
        {
            var stats = AssetDatabase.LoadAssetAtPath<VehicleStats>("Assets/Data/Hatchback.asset");
            // loading alone doesn't dirty the asset, so newly-added serialized fields
            // (their in-memory class defaults) never get written back without this.
            EditorUtility.SetDirty(stats);
            var square = SpriteFactory.Ensure("square", false);
            var circle = SpriteFactory.Ensure("circle", true);

            var root = new GameObject("Hatchback");
            root.tag = "Player";
            var rb = root.AddComponent<Rigidbody2D>();
            rb.mass = stats.chassisMass;
            var chassisCol = root.AddComponent<BoxCollider2D>();
            chassisCol.size = new Vector2(2.4f, 0.8f);
            var body = new GameObject("Body");
            body.transform.SetParent(root.transform, false);
            body.transform.localScale = new Vector3(2.4f, 0.8f, 1f);
            var sr = body.AddComponent<SpriteRenderer>();
            sr.sprite = square;
            sr.color = new Color(0.85f, 0.3f, 0.2f);

            var mat = new PhysicsMaterial2D("Wheel") { friction = stats.wheelFriction, bounciness = 0f };

            CreateWheel(root, stats, circle, mat, "WheelFront", new Vector2(0.85f, -0.55f));
            CreateWheel(root, stats, circle, mat, "WheelRear", new Vector2(-0.85f, -0.55f));

            var roof = new GameObject("RoofTrigger");
            roof.transform.SetParent(root.transform, false);
            roof.transform.localPosition = new Vector2(0f, 0.5f);
            var roofCol = roof.AddComponent<BoxCollider2D>();
            roofCol.isTrigger = true;
            roofCol.size = new Vector2(2.0f, 0.2f);

            var vc = root.AddComponent<VehicleController>();
            vc.stats = stats;

            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            AssetDatabase.CreateAsset(mat, "Assets/Prefabs/WheelMaterial.physicsMaterial2D");
            PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Hatchback.prefab");
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
        }

        static void CreateWheel(GameObject root, VehicleStats stats, Sprite sprite, PhysicsMaterial2D mat, string name, Vector2 pos)
        {
            var wheel = new GameObject(name);
            wheel.transform.SetParent(root.transform, false);
            wheel.transform.localPosition = pos;
            var wrb = wheel.AddComponent<Rigidbody2D>();
            wrb.mass = stats.chassisMass * 0.04f;
            var col = wheel.AddComponent<CircleCollider2D>();
            col.radius = stats.wheelRadius;
            col.sharedMaterial = mat;
            var wsr = wheel.AddComponent<SpriteRenderer>();
            wsr.sprite = sprite;
            wsr.color = Color.black;
            wheel.transform.localScale = Vector3.one * (stats.wheelRadius * 2f);

            var joint = root.AddComponent<WheelJoint2D>();
            joint.connectedBody = wrb;
            joint.anchor = pos;
            var susp = joint.suspension;
            susp.frequency = stats.suspensionFrequency;
            susp.dampingRatio = stats.suspensionDamping;
            susp.angle = 90f;
            joint.suspension = susp;
        }
    }
}
