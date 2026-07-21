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
            AssetDatabase.SaveAssets();
        }
    }
}
