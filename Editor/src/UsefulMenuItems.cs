using UnityEditor;
using UnityEngine;

namespace RGN.MyEditor
{
    public sealed class UsefulMenuItems
    {
        public const string READY_MENU = "ReadyGamesNetwork/";


#if READY_DEVELOPMENT
        [MenuItem(READY_MENU + "Open Persistent Data Path")]
        public static void SetEmulator()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
#endif
    }
}
