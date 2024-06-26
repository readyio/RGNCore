using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using RGN.ImplDependencies.Engine;
using UnityEngine;

namespace RGN.Impl.Firebase.Engine
{
    public class PersistenceData : IPersistenceData
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void JsFileSystemSync();
#endif
        
        private readonly Regex InvalidNameCharactersRegex;

        public PersistenceData()
        {
            string invalidCharsRegStr = $"[{Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()))}]+";
            InvalidNameCharactersRegex = new Regex(invalidCharsRegStr);
        }

        public string LoadFile(string name)
        {
            string formattedName = FormatFileName(name);
            try
            {
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, formattedName);
                return System.IO.File.Exists(filePath) ? System.IO.File.ReadAllText(filePath) : string.Empty;
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"Exception thrown while loading file: {exception}");
                return UnityEngine.PlayerPrefs.GetString(formattedName);
            }
        }

        public void SaveFile(string name, string content)
        {
            string formattedName = FormatFileName(name);
            try
            {
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, formattedName);
                System.IO.File.WriteAllText(filePath, content);
#if UNITY_WEBGL && !UNITY_EDITOR
                JsFileSystemSync();
#endif
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"Exception thrown while saving file: {exception}");
                UnityEngine.PlayerPrefs.SetString(formattedName, content);
                UnityEngine.PlayerPrefs.Save();
            }
        }
        
        public string FormatFileName(string name) =>
            InvalidNameCharactersRegex.Replace(name, "_");
    }
}
