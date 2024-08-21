using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace RGN
{
    public class RGNUnityUpdater : MonoBehaviour, IRGNUpdater
    {
        private RGNCore moduleCore;
        
        private DateTime appPauseTime;

        private IEnumerator firstSessionCounterCoroutine;
        private bool firstSessionEventIsCommitted;
        private int firstSessionDuration;

        private const long FIRST_SESSION_PAUSE_THRESHOLD = 60 * 5;
        private const string FIRST_SESSION_PREFS_KEY = "rgn_first_session";
        private const string FIRST_SESSION_EVENT_COMMITTED_PREFS_KEY = "rgn_first_session_event_committed";
        
        public void BindModuleCore(RGNCore moduleCore)
        {
            this.moduleCore = moduleCore;
        }

        private void Start() 
            => StartCoroutine(CoWaitForModuleCore());

        private async void OnModuleCoreReady()
        {
            firstSessionEventIsCommitted = moduleCore.Dependencies.EngineApp.PlayerPrefs.HasKey(FIRST_SESSION_EVENT_COMMITTED_PREFS_KEY);
            firstSessionDuration = moduleCore.Dependencies.EngineApp.PlayerPrefs.GetInt(FIRST_SESSION_PREFS_KEY, 0);

            if (firstSessionEventIsCommitted)
            {
                return;
            }
            
            if (firstSessionDuration > 0)
            {
                await CommitFirstSessionEventAsync(firstSessionDuration);
            }
            else
            {
                firstSessionCounterCoroutine = CoFirstSessionCounter();
                StartCoroutine(firstSessionCounterCoroutine);
            }
        }

        private void Update()
        {
            moduleCore?.OnUnityUpdate();
        }

        private async void OnApplicationFocus(bool focus)
        {
            if (moduleCore?.Dependencies.RGNAnalytics == null)
            {
                return;
            }
            await moduleCore.Dependencies.RGNAnalytics.LogEventAsync(
                "on_app_focus",
                "{\"focus\":" + (focus ? "true" : "false") + "}");
        }
        
        private async void OnApplicationPause(bool pause)
        {
            if (moduleCore?.Dependencies.RGNAnalytics == null)
            {
                return;
            }

            if (pause)
            {
                appPauseTime = DateTime.UtcNow;
            }
            else
            {
                if (firstSessionCounterCoroutine != null &&
                    (DateTime.UtcNow - appPauseTime).TotalSeconds > FIRST_SESSION_PAUSE_THRESHOLD)
                {
                    StopCoroutine(firstSessionCounterCoroutine);
                    firstSessionCounterCoroutine = null;
                    await CommitFirstSessionEventAsync(firstSessionDuration);
                }
            }
            
            await moduleCore.Dependencies.RGNAnalytics.LogEventAsync(
                "on_app_pause",
                "{\"pause\":" + (pause ? "true" : "false") + "}");
        }
        
        private async void OnApplicationQuit()
        {
            if (moduleCore?.Dependencies.RGNAnalytics == null)
            {
                return;
            }

            if (firstSessionCounterCoroutine != null)
            {
                StopCoroutine(firstSessionCounterCoroutine);
                firstSessionCounterCoroutine = null;
                await CommitFirstSessionEventAsync(firstSessionDuration);
            }
            
            await moduleCore.Dependencies.RGNAnalytics.LogEventAsync("on_app_quit");
        }

        private async Task CommitFirstSessionEventAsync(int duration)
        {
            if (moduleCore?.Dependencies.RGNAnalytics == null)
            {
                return;
            }
            
            await moduleCore.Dependencies.RGNAnalytics.LogEventAsync(
                "first_session_play_time",
                "{\"duration\":" + duration + "}");
            
            moduleCore.Dependencies.EngineApp.PlayerPrefs.SetInt(FIRST_SESSION_EVENT_COMMITTED_PREFS_KEY, 1);
            moduleCore.Dependencies.EngineApp.PlayerPrefs.Save();
        }

        private IEnumerator CoFirstSessionCounter()
        {
            firstSessionDuration = 0;
            moduleCore.Dependencies.EngineApp.PlayerPrefs.SetInt(FIRST_SESSION_PREFS_KEY, firstSessionDuration);
            
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);
                
                firstSessionDuration++;
                moduleCore.Dependencies.EngineApp.PlayerPrefs.SetInt(FIRST_SESSION_PREFS_KEY, firstSessionDuration);
                
                if (firstSessionDuration % 60 == 0)
                {
                    moduleCore.Dependencies.EngineApp.PlayerPrefs.Save();
                }
            }
        }
        
        private IEnumerator CoWaitForModuleCore()
        {
            while (moduleCore == null)
            {
                yield return null;
            }
            OnModuleCoreReady();
        }
    }
}
