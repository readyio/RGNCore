using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RGN.MyEditor
{
    public sealed class UsefulMenuItems
    {
        public const string READY_MENU = "ReadyGamesNetwork/Developer/";

#if READY_DEVELOPMENT
        [MenuItem(READY_MENU + "Open Persistent Data Path", priority = 10)]
        public static void SetEmulator()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
        [MenuItem(READY_MENU + "Get User Token")]
        public static async void GetUserToken()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Please start the application by pressing the play button in Unity editor");
                return;
            }
            if (RGNCore.I.MasterAppUser == null)
            {
                Debug.LogError("The user is not logged in, please login and try again");
                return;
            }
            string token = await RGNCore.I.MasterAppUser.TokenAsync(false);
            Debug.Log(token);
            Clipboard.SetText(token);
        }
#endif
    }
}
