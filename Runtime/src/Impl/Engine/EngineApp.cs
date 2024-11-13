using RGN.ImplDependencies.Engine;
using UnityEngine;

namespace RGN.Impl.Firebase.Engine
{
    public sealed class EngineApp : IEngineApp, IEngineAppOpenUrlPatcher
    {
        bool IEngineApp.IsStandalonePlatform =>
            Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.LinuxPlayer;

        bool IEngineApp.IsEditor =>
            Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.LinuxEditor;

        public ISystemInfo SystemInfo { get; }
        public IPlayerPrefs PlayerPrefs { get; }

        private System.Action<string> _openUrlPatch;

        internal EngineApp()
        {
            SystemInfo = new SystemInfo();
            PlayerPrefs = new PlayerPrefs();
        }

        IRGNUpdater IEngineApp.CreateGameObjectWithUpdater()
        {
            GameObject go = new GameObject("RGNModuleCore");
            Object.DontDestroyOnLoad(go);
            RGNUnityUpdater updater = go.AddComponent<RGNUnityUpdater>();
            return updater;
        }
        
        public void DestroyGameObjectWithUpdater(IRGNUpdater rgnUpdater)
        {
            var asComponent = rgnUpdater as RGNUnityUpdater;
            if ( asComponent != null && asComponent.gameObject)
            {
                Object.Destroy(asComponent.gameObject);
            }
        }
        
        public void OpenUrl(string url)
        {
            if (_openUrlPatch != null)
            {
                _openUrlPatch(url);
                return;
            }
            Application.OpenURL(url);
        }

        public void PatchOpenUrl(System.Action<string> openUrlAction) =>
            _openUrlPatch = openUrlAction;
    }
}
