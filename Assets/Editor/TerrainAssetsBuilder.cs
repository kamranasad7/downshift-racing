using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.U2D;
using Downshift;

namespace Downshift.EditorTools
{
    public static class TerrainAssetsBuilder
    {
        [MenuItem("Downshift/Build/Pickup Prefabs")]
        public static void BuildPickupPrefabs()
        {
            BuildOne("Coin", true, new Color(1f, 0.85f, 0.1f), 0.35f, PickupKind.Coin);
            BuildOne("Coolant", true, new Color(0.2f, 0.6f, 1f), 0.45f, PickupKind.Coolant);
            BuildOne("Station", false, new Color(0.2f, 0.9f, 0.4f), 1.5f, PickupKind.Station);
        }

        static void BuildOne(string name, bool circle, Color color, float scale, PickupKind kind)
        {
            string path = $"Assets/Prefabs/{name}.prefab";
            var go = new GameObject(name);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.Ensure(circle ? "circle" : "square", circle);
            sr.color = color;
            go.transform.localScale = Vector3.one * scale;
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.6f;
            go.AddComponent<Pickup>().kind = kind;

            switch (kind)
            {
                case PickupKind.Coin: BuildCoinVisual(go); break;
                case PickupKind.Coolant: BuildCoolantVisual(go); break;
                case PickupKind.Station: BuildStationVisual(go, sr); break;
            }

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        static void BuildCoinVisual(GameObject go)
        {
            var circleSprite = SpriteFactory.Ensure("circle", true);
            AddChild(go, "Rim", circleSprite, new Color(0.75f, 0.55f, 0.05f), Vector3.zero, Vector3.one * 0.55f);
            var spin = go.AddComponent<CoinSpin>();
            spin.flip = true;
            spin.bob = false;
            spin.spinSpeed = 3f;
        }

        static void BuildCoolantVisual(GameObject go)
        {
            var squareSprite = SpriteFactory.Ensure("square", false);
            AddChild(go, "DropCap", squareSprite, new Color(0.45f, 0.75f, 1f), new Vector3(0f, 0.5f, 0f), Vector3.one * 0.45f, 45f);
            var spin = go.AddComponent<CoinSpin>();
            spin.flip = false;
            spin.bob = true;
            spin.spinSpeed = 2f;
        }

        static void BuildStationVisual(GameObject go, SpriteRenderer rootSprite)
        {
            rootSprite.enabled = false;
            var barSprite = SpriteFactory.Ensure("square", false);
            var barColor = new Color(0.1f, 0.45f, 0.2f);
            AddChild(go, "PillarLeft", barSprite, barColor, new Vector3(-1.0f, 0.15f, 0f), new Vector3(0.22f, 1.7f, 1f));
            AddChild(go, "PillarRight", barSprite, barColor, new Vector3(1.0f, 0.15f, 0f), new Vector3(0.22f, 1.7f, 1f));
            AddChild(go, "Beam", barSprite, barColor, new Vector3(0f, 1.1f, 0f), new Vector3(2.3f, 0.28f, 1f));
            AddChild(go, "Sign", barSprite, new Color(0.95f, 0.92f, 0.78f), new Vector3(0f, 1.1f, 0f), new Vector3(0.7f, 0.42f, 1f), sortingOrder: 2);
        }

        static void AddChild(GameObject parent, string name, Sprite sprite, Color color, Vector3 localPos, Vector3 localScale, float zRot = 0f, int sortingOrder = 1)
        {
            var child = new GameObject(name);
            child.transform.SetParent(parent.transform, false);
            child.transform.localPosition = localPos;
            child.transform.localScale = localScale;
            child.transform.localRotation = Quaternion.Euler(0f, 0f, zRot);
            var sr = child.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = sortingOrder;
        }

        [MenuItem("Downshift/Build/Run Scene")]
        public static void BuildRunScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
            var car = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            var config = AssetDatabase.LoadAssetAtPath<TerrainConfig>("Assets/Data/Terrain.asset");
            car.transform.position = new Vector3(4f, TerrainProfile.Height(4f, config) + 2f, 0f);

            var terrain = new GameObject("Terrain");
            var streamer = terrain.AddComponent<TerrainStreamer>();
            streamer.config = config;
            streamer.target = car.transform;

            BuildPickupPrefabs();
            streamer.coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Coin.prefab");
            streamer.coolantPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Coolant.prefab");
            streamer.stationPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Station.prefab");

            var input = new GameObject("Input");
            input.AddComponent<KeyboardInput>().vehicle = car.GetComponent<VehicleController>();

            var runGo = new GameObject("RunManager");
            var run = runGo.AddComponent<RunManager>();
            run.vehicle = car.GetComponent<VehicleController>();
            car.GetComponentInChildren<RoofCrashDetector>().runManager = run;

            var camGo = new GameObject("CM Follow");
            var cine = camGo.AddComponent<CinemachineCamera>();
            cine.Follow = car.transform;
            var composer = camGo.AddComponent<CinemachinePositionComposer>();
            var lens = cine.Lens;
            lens.OrthographicSize = 6f;
            cine.Lens = lens;
            var speedCam = camGo.AddComponent<SpeedCamera>();
            speedCam.vehicle = car.GetComponent<VehicleController>();
            Camera.main.gameObject.AddComponent<Unity.Cinemachine.CinemachineBrain>();

            BuildBackground(Camera.main.transform);

            HudBuilder.Build(car.GetComponent<VehicleController>(), run);

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Run.unity");

            var scenePath = "Assets/Scenes/Run.unity";
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (!scenes.Exists(s => s.path == scenePath))
            {
                scenes.Add(new EditorBuildSettingsScene(scenePath, true));
                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }

        static void BuildBackground(Transform cam)
        {
            var skyGo = new GameObject("Sky");
            var sky = skyGo.AddComponent<SkyGradient>();
            sky.cam = cam;
            sky.topColor = HexColor("2E3A59");
            sky.horizonColor = HexColor("B8886B");

            var far = new GameObject("FarHills").AddComponent<HillLayerBuilder>();
            far.cam = cam;
            far.color = HexColor("4A5578");
            far.parallaxFactor = 0.15f;
            far.baseline = 1f;
            far.amplitude = 2f;
            far.baseY = 1f;
            far.sortingOrder = -90;

            var near = new GameObject("NearHills").AddComponent<HillLayerBuilder>();
            near.cam = cam;
            near.color = HexColor("5D6B8C");
            near.parallaxFactor = 0.35f;
            near.baseline = -0.5f;
            near.amplitude = 1.5f;
            near.baseY = -0.5f;
            near.sortingOrder = -80;
        }

        static Color HexColor(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out var c);
            return c;
        }
    }
}
