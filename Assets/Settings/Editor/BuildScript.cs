using UnityEditor;

namespace Settings
{
    public abstract class BuildScript
    {
        [MenuItem("Build/Build All !!Broken - do not use!!")]
        public static void BuildAll()
        {
            return;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);

            BuildClient();
            BuildServer();
        }

        private const string BuildPath = "Builds-2/";

        private static void BuildClient()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "UNITY_CLIENT");

            var defaultScenePaths = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
            
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = defaultScenePaths,
                locationPathName = $"{BuildPath}/Client/Client.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.CompressWithLz4HC,
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        private static void BuildServer()
        {
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
                locationPathName = $"{BuildPath}/Server/Server.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.CompressWithLz4HC,
                subtarget = (int)StandaloneBuildSubtarget.Server
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}