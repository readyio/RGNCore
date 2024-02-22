using System;
using System.Runtime.InteropServices;
using RGN.ImplDependencies.Engine;
using UnityEngine;

namespace RGN.Impl.Firebase.Engine
{
    public class PersistenceData : IPersistenceData
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void JsFileSystemSync();
#endif
        
        public string LoadFile(string name)
        {
            try
            {
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, name);
                return System.IO.File.Exists(filePath) ? System.IO.File.ReadAllText(filePath) : string.Empty;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Exception thrown while loading file: {exception}");
                return UnityEngine.PlayerPrefs.GetString(name);
            }
        }

        public void SaveFile(string name, string content)
        {
            try
            {
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, name);
                System.IO.File.WriteAllText(filePath, content);
#if UNITY_WEBGL
                JsFileSystemSync();
#endif
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Exception thrown while saving file: {exception}");
                UnityEngine.PlayerPrefs.SetString(name, content);
                UnityEngine.PlayerPrefs.Save();
            }
        }
    }
}
