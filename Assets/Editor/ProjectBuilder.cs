using System;
using System.Linq;
using UnityEditor;

namespace NeonBlack.Editor
{
    public static class ProjectBuilder
    {
        public static void PerformBuild()
        {
            BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray(),
                target = EditorUserBuildSettings.activeBuildTarget,
                locationPathName = Environment.GetEnvironmentVariable("BUILD_PATH")
            });
        }
    }
}
