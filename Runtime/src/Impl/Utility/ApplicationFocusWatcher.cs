using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RGN
{
    internal class ApplicationFocusWatcher : MonoBehaviour
    {
        public event Action<ApplicationFocusWatcher, bool> OnFocusChanged;

        private float delay;
        
        public static ApplicationFocusWatcher Create(float delay = 0)
        {
            var watcherObj = new GameObject(nameof(ApplicationFocusWatcher));
            var watcher = watcherObj.AddComponent<ApplicationFocusWatcher>();
            watcher.delay = delay;
            return watcher;
        }

        public void Destroy() => Object.Destroy(gameObject);

        private void OnApplicationFocus(bool hasFocus) => StartCoroutine(TriggerFocusChangeEvent(hasFocus));

        private IEnumerator TriggerFocusChangeEvent(bool hasFocus)
        {
            yield return new WaitForSeconds(delay);
            OnFocusChanged?.Invoke(this, hasFocus);
        }
    }
}
