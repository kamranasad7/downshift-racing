using UnityEditor;
using UnityEngine;
using Downshift;

namespace Downshift.EditorTools
{
    public static class DefaultAssetsBuilder
    {
        [MenuItem("Downshift/Build/Default Data Assets")]
        public static void BuildDataAssets()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
                AssetDatabase.CreateFolder("Assets", "Data");
            if (AssetDatabase.LoadAssetAtPath<VehicleStats>("Assets/Data/Hatchback.asset") == null)
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<VehicleStats>(), "Assets/Data/Hatchback.asset");
            if (AssetDatabase.LoadAssetAtPath<TerrainConfig>("Assets/Data/Terrain.asset") == null)
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TerrainConfig>(), "Assets/Data/Terrain.asset");

            var hatchback = AssetDatabase.LoadAssetAtPath<VehicleStats>("Assets/Data/Hatchback.asset");
            EditorUtility.SetDirty(hatchback);
            var terrain = AssetDatabase.LoadAssetAtPath<TerrainConfig>("Assets/Data/Terrain.asset");
            EditorUtility.SetDirty(terrain);

            AssetDatabase.SaveAssets();
        }
    }
}
