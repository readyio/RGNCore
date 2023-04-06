using System;
using System.Reflection;
using UnityEngine;

namespace RGN
{
    /// <summary>
    /// Helper class to copy-paste string values.
    /// Is supported on standalone and mobile platforms
    /// </summary>
    public static class Clipboard
    {
        private static IBoard sBoard;

        private static IBoard Board
        {
            get
            {
                if (sBoard == null)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    sBoard = new AndroidBoard();
#elif UNITY_IOS && !UNITY_TVOS && !UNITY_EDITOR
                    sBoard = new IOSBoard ();
#else
                    sBoard = new StandardBoard();
#endif
                }
                return sBoard;
            }
        }

        public static void SetText(string str)
        {
            Board.SetText(str);
        }

        public static string GetText()
        {
            return Board.GetText();
        }
    }

    internal interface IBoard
    {
        void SetText(string str);
        string GetText();
    }

    internal sealed class StandardBoard : IBoard
    {
        private static PropertyInfo sSystemCopyBufferProperty = null;
        private static PropertyInfo GetSystemCopyBufferProperty()
        {
            if (sSystemCopyBufferProperty == null)
            {
                Type T = typeof(GUIUtility);
                sSystemCopyBufferProperty = T.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.Public);
                if (sSystemCopyBufferProperty == null)
                {
                    sSystemCopyBufferProperty =
                        T.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
                }

                if (sSystemCopyBufferProperty == null)
                {
                    throw new Exception(
                        "Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
                }
            }
            return sSystemCopyBufferProperty;
        }
        public void SetText(string str)
        {
            PropertyInfo P = GetSystemCopyBufferProperty();
            P.SetValue(null, str, null);
        }
        public string GetText()
        {
            PropertyInfo P = GetSystemCopyBufferProperty();
            return (string)P.GetValue(null, null);
        }
    }

#if UNITY_IOS && !UNITY_TVOS
    internal sealed class IOSBoard : IBoard
    {
        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void Clipboard_SetText_(string str);
        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern string Clipboard_GetText_();

        public void SetText(string str)
        {
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                Clipboard_SetText_(str);
            }
        }
        public string GetText()
        {
            return Clipboard_GetText_();
        }
    }
#endif

#if UNITY_ANDROID
    internal sealed class AndroidBoard : IBoard
    {
        private AndroidJavaObject _currentActivity;
        private AndroidJavaObject _clipboardService;
        public void SetText(string str)
        {
            GetClipboardManager().Call("setText", str);
        }

        public string GetText()
        {
            return GetClipboardManager().Call<string>("getText");
        }

        private AndroidJavaObject GetClipboardManager()
        {
            if (_currentActivity == null || _clipboardService == null)
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                var staticContext = new AndroidJavaClass("android.content.Context");
                _clipboardService = staticContext.GetStatic<AndroidJavaObject>("CLIPBOARD_SERVICE");
            }
            return _currentActivity.Call<AndroidJavaObject>("getSystemService", _clipboardService);
        }
    }
#endif
}
