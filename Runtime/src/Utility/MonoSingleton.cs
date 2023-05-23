using UnityEngine;

namespace RGN
{
    /// <summary>
    /// Base class for MonoBehaviour singletons.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour
        where T : MonoSingleton<T>
    {
        private static T sInstance;
        private static readonly object sLock = new object();
        private static bool sObjectIsDestroyed = false;

        [SerializeField] private bool _dontDestroyOnLoadNewScene = true;

        public static T I
        {
            get
            {
                if (sObjectIsDestroyed)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                     "' already destroyed on application quit. Won't create again - returning null.");
                    return null;
                }

                lock (sLock)
                {
                    if (sInstance == null)
                    {
                        sInstance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                           " - there should never be more than 1 singleton!" +
                                           " Reopenning the scene might fix it.");
                            return sInstance;
                        }

                        if (sInstance == null)
                        {
                            GameObject singleton = new GameObject();
                            sInstance = singleton.AddComponent<T>();
                            sInstance._dontDestroyOnLoadNewScene = true;
                            singleton.name = typeof(T).ToString();
                            DontDestroyOnLoad(singleton);
                        }
                    }
                    return sInstance;
                }
            }
        }

        protected void Awake()
        {
            if (_dontDestroyOnLoadNewScene)
            {
                var root = gameObject.transform.root.gameObject;
                DontDestroyOnLoad(root);
                if (gameObject.transform.parent != null)
                {
                    Debug.LogWarning(
                        "[RGNUnityInitializer]: The game object will be not destroyed between scenes: " + root.name);
                }
            }
            if (sInstance == null)
            {
                sInstance = this as T;
                OnAwakeInternal();
            }
            else if (sInstance != this)
            {
                var root = gameObject.transform.root.gameObject;
                Debug.Log(typeof(T).Name + " instance already exist, destroying object!");
                Destroy(root);
            }
        }

        protected void OnDestroy()
        {
            if (sInstance == this)
            {
                OnDestroyInternal();
                sObjectIsDestroyed = true;
            }
        }

        protected virtual void OnAwakeInternal() { }
        protected virtual void OnDestroyInternal() { }
    }
}
