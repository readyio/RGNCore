using UnityEngine;

namespace RGN
{
    public class RGNUnityUpdater : MonoBehaviour, IRGNUpdater
    {
        private RGNCore moduleCore = null;

        public void BindModuleCore(RGNCore moduleCore)
        {
            this.moduleCore = moduleCore;
        }

        private void Update()
        {
            moduleCore?.OnUnityUpdate();
        }

        private async void OnApplicationFocus(bool focus)
        {
            if (moduleCore == null ||
                moduleCore.Dependencies.RGNAnalytics == null)
            {
                return;
            }
            await moduleCore.Dependencies.RGNAnalytics.LogEventAsync(
                "on_app_focus",
                "{\"focus\":" + (focus ? "true" : "false") + "}");
        }
        private async void OnApplicationPause(bool pause)
        {
            if (moduleCore == null ||
                moduleCore.Dependencies.RGNAnalytics == null)
            {
                return;
            }
            await moduleCore.Dependencies.RGNAnalytics.LogEventAsync(
                "on_app_pause",
                "{\"pause\":" + (pause ? "true" : "false") + "}");
        }
        private async void OnApplicationQuit()
        {
            if (moduleCore == null ||
                moduleCore.Dependencies.RGNAnalytics == null)
            {
                return;
            }
            await moduleCore.Dependencies.RGNAnalytics.LogEventAsync("on_app_quit");
        }
    }
}
