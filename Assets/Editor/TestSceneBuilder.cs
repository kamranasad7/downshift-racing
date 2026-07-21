using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Downshift.EditorTools
{
    public static class TestSceneBuilder
    {
        [MenuItem("Downshift/Build/Slope Test Scene")]
        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var ground = new GameObject("Slope");
            var col = ground.AddComponent<EdgeCollider2D>();
            var pts = new Vector2[50];
            for (int i = 0; i < pts.Length; i++)
                pts[i] = new Vector2(i * 4f, -i * 4f * 0.15f);
            col.points = pts;

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hatchback.prefab");
            var car = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            car.transform.position = new Vector3(2f, 2f, 0f);

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/SlopeTest.unity");
        }
    }
}
