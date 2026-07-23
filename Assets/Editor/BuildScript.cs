using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Downshift.EditorTools
{
    public static class BuildScript
    {
        [MenuItem("Downshift/Build/Android APK")]
        public static void PerformAndroidBuild()
        {
            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = "Builds/downshift-debug.apk",
                target = BuildTarget.Android,
                options = BuildOptions.Development,
            };

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;
            Debug.Log(string.Format(
                "Android build finished: result={0} size={1} bytes time={2}",
                summary.result, summary.totalSize, summary.totalTime));
        }
    }
}
