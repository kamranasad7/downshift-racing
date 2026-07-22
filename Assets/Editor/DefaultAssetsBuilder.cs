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
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (AssetDatabase.LoadAssetAtPath<VehicleStats>("Assets/Data/Hatchback.asset") == null)
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<VehicleStats>(), "Assets/Data/Hatchback.asset");
            if (AssetDatabase.LoadAssetAtPath<TerrainConfig>("Assets/Data/Terrain.asset") == null)
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TerrainConfig>(), "Assets/Data/Terrain.asset");

            if (AssetDatabase.LoadAssetAtPath<EconomyConfig>("Assets/Data/Economy.asset") != null)
                AssetDatabase.MoveAsset("Assets/Data/Economy.asset", "Assets/Resources/Economy.asset");
            if (AssetDatabase.LoadAssetAtPath<EconomyConfig>("Assets/Resources/Economy.asset") == null)
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<EconomyConfig>(), "Assets/Resources/Economy.asset");

            var hatchback = AssetDatabase.LoadAssetAtPath<VehicleStats>("Assets/Data/Hatchback.asset");
            EditorUtility.SetDirty(hatchback);
            var terrain = AssetDatabase.LoadAssetAtPath<TerrainConfig>("Assets/Data/Terrain.asset");
            EditorUtility.SetDirty(terrain);
            var economy = AssetDatabase.LoadAssetAtPath<EconomyConfig>("Assets/Resources/Economy.asset");
            EditorUtility.SetDirty(economy);

            AssetDatabase.SaveAssets();
        }
    }
}
