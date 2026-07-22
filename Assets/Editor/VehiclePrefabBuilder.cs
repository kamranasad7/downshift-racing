using UnityEditor;
using UnityEngine;
using Downshift;

namespace Downshift.EditorTools
{
    public static class VehiclePrefabBuilder
    {
        static readonly Color CabinCream = HexColor("E8DCC8");
        static readonly Color WindowDark = HexColor("2A2E38");

        [MenuItem("Downshift/Build/Hatchback Prefab")]
        public static void Build()
        {
            var stats = AssetDatabase.LoadAssetAtPath<VehicleStats>("Assets/Data/Hatchback.asset");
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

            CreateQuad(root, "Cabin", square, CabinCream, new Vector2(-0.35f, 0.55f), new Vector2(1.1f, 0.5f), 1);
            CreateQuad(root, "Window", square, WindowDark, new Vector2(-0.35f, 0.58f), new Vector2(0.8f, 0.28f), 2);
            CreateQuad(root, "Bumper", square, WindowDark, new Vector2(1.15f, -0.32f), new Vector2(0.25f, 0.12f), 1);

            var mat = new PhysicsMaterial2D("Wheel") { friction = stats.wheelFriction, bounciness = 0f };

            CreateWheel(root, stats, circle, mat, "WheelFront", new Vector2(0.85f, -0.55f));
            CreateWheel(root, stats, circle, mat, "WheelRear", new Vector2(-0.85f, -0.55f));

            var roof = new GameObject("RoofTrigger");
            roof.transform.SetParent(root.transform, false);
            roof.transform.localPosition = new Vector2(0f, 0.5f);
            var roofCol = roof.AddComponent<BoxCollider2D>();
            roofCol.isTrigger = true;
            roofCol.size = new Vector2(2.0f, 0.2f);
            roof.AddComponent<RoofCrashDetector>();

            var vc = root.AddComponent<VehicleController>();
            vc.stats = stats;

            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");

            string particleMatPath = "Assets/Prefabs/HeatParticleMaterial.mat";
            var particleMat = new Material(Shader.Find("Sprites/Default")) { mainTexture = circle.texture };
            if (AssetDatabase.LoadAssetAtPath<Material>(particleMatPath) != null)
                AssetDatabase.DeleteAsset(particleMatPath);
            AssetDatabase.CreateAsset(particleMat, particleMatPath);

            BuildHeatEffects(root, vc, particleMat);

            AssetDatabase.CreateAsset(mat, "Assets/Prefabs/WheelMaterial.physicsMaterial2D");
            PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Hatchback.prefab");
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
        }

        static void CreateQuad(GameObject root, string name, Sprite sprite, Color color, Vector2 localPos, Vector2 size, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(root.transform, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = sortingOrder;
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

            var hub = new GameObject("Hub");
            hub.transform.SetParent(wheel.transform, false);
            hub.transform.localPosition = Vector3.zero;
            hub.transform.localScale = Vector3.one * 0.25f;
            var hsr = hub.AddComponent<SpriteRenderer>();
            hsr.sprite = sprite;
            hsr.color = CabinCream;
            hsr.sortingOrder = 1;

            var joint = root.AddComponent<WheelJoint2D>();
            joint.connectedBody = wrb;
            joint.anchor = pos;
            var susp = joint.suspension;
            susp.frequency = stats.suspensionFrequency;
            susp.dampingRatio = stats.suspensionDamping;
            susp.angle = 90f;
            joint.suspension = susp;
        }

        static void BuildHeatEffects(GameObject root, VehicleController vc, Material particleMat)
        {
            var heatGo = new GameObject("HeatEffects");
            heatGo.transform.SetParent(root.transform, false);
            var heat = heatGo.AddComponent<HeatEffects>();
            heat.vehicle = vc;

            heat.brakeEmbersFront = CreateEmberSystem(root, "BrakeEmbersFront", new Vector2(0.73f, -0.5f), particleMat);
            heat.brakeEmbersRear = CreateEmberSystem(root, "BrakeEmbersRear", new Vector2(-0.97f, -0.5f), particleMat);
            heat.engineSmoke = CreateSmokeSystem(root, "EngineSmoke", new Vector2(0.9f, 0.35f), particleMat);
            heat.blowupBurst = CreateBurstSystem(root, "BlowupBurst", new Vector2(0.9f, 0.15f), particleMat);
        }

        static ParticleSystem CreateBaseSystem(GameObject root, string name, Vector2 localPos, Material particleMat)
        {
            var go = new GameObject(name);
            go.transform.SetParent(root.transform, false);
            go.transform.localPosition = localPos;
            var ps = go.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.loop = true;

            var emission = ps.emission;
            emission.rateOverTime = 0f;

            var shape = ps.shape;
            shape.enabled = false;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            var fadeOut = new Gradient();
            fadeOut.SetKeys(
                new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
            colorOverLifetime.color = fadeOut;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.sharedMaterial = particleMat;
            renderer.sortingOrder = 5;

            return ps;
        }

        static ParticleSystem CreateEmberSystem(GameObject root, string name, Vector2 localPos, Material particleMat)
        {
            var ps = CreateBaseSystem(root, name, localPos, particleMat);
            var main = ps.main;
            main.startLifetime = 0.4f;
            main.startSpeed = 0.3f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.15f);
            main.startColor = new ParticleSystem.MinMaxGradient(HexColor("FFAA33"), HexColor("CC3311"));

            var vel = ps.velocityOverLifetime;
            vel.enabled = true;
            vel.x = new ParticleSystem.MinMaxCurve(-0.6f, -0.3f);
            vel.y = new ParticleSystem.MinMaxCurve(-0.05f, 0.05f);
            vel.z = new ParticleSystem.MinMaxCurve(0f, 0f);
            return ps;
        }

        static ParticleSystem CreateSmokeSystem(GameObject root, string name, Vector2 localPos, Material particleMat)
        {
            var ps = CreateBaseSystem(root, name, localPos, particleMat);
            var main = ps.main;
            main.startLifetime = 1.2f;
            main.startSpeed = 0.2f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.35f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.6f, 0.6f, 0.6f, 0.6f), new Color(0.78f, 0.78f, 0.78f, 0.6f));

            var vel = ps.velocityOverLifetime;
            vel.enabled = true;
            vel.x = new ParticleSystem.MinMaxCurve(-0.05f, 0.05f);
            vel.y = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
            vel.z = new ParticleSystem.MinMaxCurve(0f, 0f);
            return ps;
        }

        static ParticleSystem CreateBurstSystem(GameObject root, string name, Vector2 localPos, Material particleMat)
        {
            var ps = CreateBaseSystem(root, name, localPos, particleMat);
            var main = ps.main;
            main.startLifetime = 0.6f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2.5f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.3f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.15f, 0.13f, 0.12f), new Color(0.35f, 0.3f, 0.25f));

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.05f;
            return ps;
        }

        static Color HexColor(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out var c);
            return c;
        }
    }
}
