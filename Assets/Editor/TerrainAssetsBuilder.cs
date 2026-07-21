using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.U2D;
using Downshift;

namespace Downshift.EditorTools
{
    public static class TerrainAssetsBuilder
    {
        [MenuItem("Downshift/Build/Terrain Shape Profile")]
        public static void BuildShapeProfile()
        {
            if (AssetDatabase.LoadAssetAtPath<SpriteShape>("Assets/Art/TerrainShapeProfile.asset") != null) return;
            var shape = ScriptableObject.CreateInstance<SpriteShape>();
            shape.fillTexture = null;
            var angleRange = new AngleRange { start = -180f, end = 180f, order = 0 };
            angleRange.sprites.Add(SpriteFactory.Ensure("square", false));
            shape.angleRanges.Add(angleRange);
            AssetDatabase.CreateAsset(shape, "Assets/Art/TerrainShapeProfile.asset");
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Downshift/Build/Run Scene")]
        public static void BuildRunScene()
        {
            BuildShapeProfile();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
            var car = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            var config = AssetDatabase.LoadAssetAtPath<TerrainConfig>("Assets/Data/Terrain.asset");
            car.transform.position = new Vector3(4f, TerrainProfile.Height(4f, config) + 2f, 0f);

            var terrain = new GameObject("Terrain");
            var streamer = terrain.AddComponent<TerrainStreamer>();
            streamer.config = config;
            streamer.target = car.transform;
            streamer.shapeProfile = AssetDatabase.LoadAssetAtPath<SpriteShape>("Assets/Art/TerrainShapeProfile.asset");

            var input = new GameObject("Input");
            input.AddComponent<KeyboardInput>().vehicle = car.GetComponent<VehicleController>();

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Run.unity");
        }
    }
}
