using System.Diagnostics;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Settings.Editor
{
    public abstract class BuildScript
    {
        private const string BuildPath = "Builds/";

        [MenuItem("Build/Build All", false, 1)]
        public static void BuildAll()
        {
            BuildClient();
            BuildServer();
        }

        [MenuItem("Build/Build Client", false, 2)]
        public static void BuildClient()
        {
            var defaultScenePaths = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = defaultScenePaths,
                locationPathName = $"{BuildPath}/Client/Client.exe",
                target = BuildTarget.StandaloneWindows64,
                subtarget = (int)StandaloneBuildSubtarget.NoSubtarget,
                extraScriptingDefines = new []{"UNITY_CLIENT"}
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("Build/Build Server", false, 3)]
        public static void BuildServer()
        {
            var defaultScenePaths = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = defaultScenePaths,
                locationPathName = $"{BuildPath}/Server/Server.exe",
                target = BuildTarget.StandaloneWindows64,
                subtarget = (int)StandaloneBuildSubtarget.Server,
                extraScriptingDefines = new []{"UNITY_SERVER"},
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);

            SwitchPlatform();
        }

        [MenuItem("Build/Build and Run All", false, 21)]
        public static void BuildAndRun()
        {
            BuildAll();
            RunAll();
        }

        [MenuItem("Build/Run Client", false, 22)]
        public static void RunClient()
        {
            var projectPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "../"));
            var clientProcess = new Process();
            clientProcess.StartInfo.FileName = $"{projectPath}{BuildPath}Client/Client.exe";
            var started = clientProcess.Start();
            
            if(!started)
            {
                Debug.LogError("Failed to start client process. Are you sure it's built?");
            }
        }

        [MenuItem("Build/Run Server", false, 23)]
        public static void RunServer()
        {
            var projectPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "../"));
            var serverProcess = new Process();
            serverProcess.StartInfo.FileName = $"{projectPath}{BuildPath}Server/Server.exe";
            var started = serverProcess.Start();
            
            if(!started)
            {
                Debug.LogError("Failed to start server process. Are you sure it's built?");
            }
        }
        [MenuItem("Build/Switch Target", false, 38)]
        public static void SwitchPlatform()
        {
            EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget.NoSubtarget;
            //EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            Debug.Log("Done Switching");
        }

        private static void RunAll()
        {
            RunServer();
            RunClient();
        }
    }
}
