using UnityEngine;

namespace RGN.UI
{
    public sealed class RGNUISettings : ScriptableObject
    {
        private static RGNUISettings sInstance;
        public static RGNUISettings I
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = LoadFromResources();
                }
                return sInstance;
            }
        }

        [SerializeField] private Color _activeColor;
        public Color ActiveColor { get => _activeColor; }


        private static RGNUISettings LoadFromResources()
        {
            var toReturn = Resources.Load<RGNUISettings>("RGNUISettings");
            if (toReturn == null)
            {
                Debug.LogError($"Can not find RGNUISettings file");
            }
            return toReturn;
        }
    }
}
