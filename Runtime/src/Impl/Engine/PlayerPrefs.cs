using RGN.ImplDependencies.Engine;

namespace RGN.Impl.Firebase.Engine
{
    public sealed class PlayerPrefs : IPlayerPrefs
    {
        public void DeleteKey(string key) => UnityEngine.PlayerPrefs.DeleteKey(key);
        public float GetFloat(string key) => UnityEngine.PlayerPrefs.GetFloat(key);
        public float GetFloat(string key, float defaultValue) => UnityEngine.PlayerPrefs.GetFloat(key, defaultValue);
        public int GetInt(string key) => UnityEngine.PlayerPrefs.GetInt(key);
        public int GetInt(string key, int defaultValue) => UnityEngine.PlayerPrefs.GetInt(key, defaultValue);
        public string GetString(string key) => UnityEngine.PlayerPrefs.GetString(key);
        public string GetString(string key, string defaultValue) => UnityEngine.PlayerPrefs.GetString(key, defaultValue);
        public bool HasKey(string key) => UnityEngine.PlayerPrefs.HasKey(key);
        public void SetFloat(string key, float value) => UnityEngine.PlayerPrefs.SetFloat(key, value);
        public void SetInt(string key, int value) => UnityEngine.PlayerPrefs.SetInt(key, value);
        public void SetString(string key, string value) => UnityEngine.PlayerPrefs.SetString(key, value);
    }
}
