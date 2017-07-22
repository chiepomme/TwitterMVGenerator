using System.Linq;
using UnityEditor;

public static class Build
{
    [MenuItem("File/Build For All Platforms")]
    public static void BuildForAllPlatforms()
    {
        var scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();

        var winOptions = new BuildPlayerOptions()
        {
            locationPathName = "TwitterMVGenerator/win/TwitterMVGenerator.exe",
            target = BuildTarget.StandaloneWindows,
            options = BuildOptions.None,
            scenes = scenes,
        };
        BuildPipeline.BuildPlayer(winOptions);

        var osxOptions = new BuildPlayerOptions()
        {
            locationPathName = "TwitterMVGenerator/osx/TwitterMVGenerator.app",
            target = BuildTarget.StandaloneOSXIntel64,
            options = BuildOptions.None,
            scenes = scenes,
        };
        BuildPipeline.BuildPlayer(osxOptions);
    }
}
