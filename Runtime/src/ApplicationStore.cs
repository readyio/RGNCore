using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RGN
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ApplicationStore", order = 1)]
    public class ApplicationStore : ScriptableObject, IApplicationStore
    {
        private const string APPLICATION_STORE_FILE_NAME_WITHOUT_EXTENSION = "ApplicationStore";
        private const string APPLICATION_STORE_FILE_NAME_WITH_EXTENSION = APPLICATION_STORE_FILE_NAME_WITHOUT_EXTENSION + ".asset";
        
        private const string APPLICATION_STORE_DIR_OLD = "ReadyGamesNetwork";
        private const string APPLICATION_STORE_FILE_OLD_LOCATION = "Assets/" + APPLICATION_STORE_DIR_OLD + "/" +
            APPLICATION_STORE_FILE_NAME_WITH_EXTENSION;

        private const string APPLICATION_STORE_DIR_NEW = "ReadyGamesNetwork/Resources";
        private const string APPLICATION_STORE_FILE_NEW_LOCATION = "Assets/" + APPLICATION_STORE_DIR_NEW + "/" +
            APPLICATION_STORE_FILE_NAME_WITH_EXTENSION;

#if UNITY_EDITOR
        public static void MoveToResourcesFolderOrCreateNewIfNeeded()
        {
            bool oldLocationFileExists = File.Exists(
                Path.Combine(Application.dataPath, APPLICATION_STORE_DIR_OLD, APPLICATION_STORE_FILE_NAME_WITH_EXTENSION));
            bool newLocationFileExists = File.Exists(
                Path.Combine(Application.dataPath, APPLICATION_STORE_DIR_NEW, APPLICATION_STORE_FILE_NAME_WITH_EXTENSION));
            if (!oldLocationFileExists && !newLocationFileExists)
            {
                CreateFoldersIfNeeded();
                var instance = CreateInstance<ApplicationStore>();
                AssetDatabase.CreateAsset(instance, APPLICATION_STORE_FILE_NEW_LOCATION);
                Debug.LogError(
                    $"The application store file was missing, created automatically new one at location: {APPLICATION_STORE_FILE_NEW_LOCATION}");
                Debug.LogError(
                    $"Please contact RGN team to provide you the right settings for the new {APPLICATION_STORE_FILE_NAME_WITH_EXTENSION} file");
            }
            else if (oldLocationFileExists && !newLocationFileExists)
            {
                CreateFoldersIfNeeded();
                string error = AssetDatabase.MoveAsset(APPLICATION_STORE_FILE_OLD_LOCATION, APPLICATION_STORE_FILE_NEW_LOCATION);
                if (string.IsNullOrEmpty(error))
                {
                    Debug.LogWarning(
                        "RGN: Successfully moved the Application Store file from old to new location: " + APPLICATION_STORE_FILE_NEW_LOCATION);
                }
                else
                {
                    Debug.LogError(error);
                }
            }
        }

        private static void CreateFoldersIfNeeded()
        {
            if (AssetDatabase.IsValidFolder("Assets/" + APPLICATION_STORE_DIR_OLD) == false)
            {
                AssetDatabase.CreateFolder("Assets", APPLICATION_STORE_DIR_OLD);
            }
            if (AssetDatabase.IsValidFolder("Assets/" + APPLICATION_STORE_DIR_NEW) == false)
            {
                AssetDatabase.CreateFolder("Assets/" + APPLICATION_STORE_DIR_OLD, "Resources");
            }
        }
#endif
        public static ApplicationStore LoadFromResources()
        {
            var toReturn = Resources.Load<ApplicationStore>(APPLICATION_STORE_FILE_NAME_WITHOUT_EXTENSION);
            if (toReturn == null)
            {
                Debug.LogError($"Can not find Application Store file in {APPLICATION_STORE_DIR_NEW} folder");
            }
            return toReturn;
        }

        [Header("RGN APP")]
        public string RGNAppId = "";

        [Header("Supported Platform")]
        public bool android = false;
        public bool iOS = false;

        [Header("Base")]
        public string iosAppID = "";
        public string appLinkPrefix = "";

        [ReadOnly] public bool isProduction = false;

        [Header("Emulator Setup")]
        public bool usingEmulator = false;
        public string emulatorServerIp = "127.0.0.1";
        public string firestorePort = ":8080";
        public string functionsPort = ":5001";

        [HideInInspector] public string googleSignInWebClientID = "";
        [HideInInspector] public string googleSignInReverseClientID = "";
        [HideInInspector] public string firebaseAssociatedDomain = "";
        [HideInInspector] public string RGNMasterApiKey = "";
        [HideInInspector] public string RGNMasterAndroidAppID = "";
        [HideInInspector] public string RGNMasterIOSAppID = "";
        [HideInInspector] public string RGNMasterProjectId = "";
        [HideInInspector] public string RGNStorageURL = "";
        [HideInInspector] public string RGNFriendInviteMessage = "";
        [HideInInspector] public string RGNMasterMessageSenderId = "";
        [HideInInspector] public string RGNMasterStorageBucket = "";
        [HideInInspector] public string RGNMasterDatabaseUrl = "";

        public string RGNMasterAppID
        {
            get
            {
#if UNITY_ANDROID
                return RGNMasterAndroidAppID;
#elif UNITY_IOS
            return RGNMasterIOSAppID;
#else
            return RGNMasterAndroidAppID;
#endif
            }
        }

        public string GetRGNMasterAppID => RGNMasterAppID;
        public string GetRGNAppID => RGNAppId;
        public bool IsAndroidSupported => android;
        public bool IsiOSSupported => iOS;
        public string GetiOSAppId => iosAppID;
        public string GetAppLinkPrefix => appLinkPrefix;
        public bool GetIsProduction => isProduction;
        public bool IsUsingEmulator => usingEmulator;
        public string GetEmulatorServerIp => emulatorServerIp;
        public string GetFirestorePort => firestorePort;
        public string GetFunctionsPort => functionsPort;
        public string GetGoogleSignInWebClientID => googleSignInWebClientID;
        public string GetGoogleSignInReverseClientID => googleSignInReverseClientID;
        public string GetFirebaseAssociatedDomain => firebaseAssociatedDomain;
        public string GetRGNMasterApiKey => RGNMasterApiKey;
        public string GetRGNMasterAndroidAppID => RGNMasterAndroidAppID;
        public string GetRGNMasterIOSAppID => RGNMasterIOSAppID;
        public string GetRGNMasterProjectId => RGNMasterProjectId;
        public string GetRGNStorageURL => RGNStorageURL;
        public string GetRGNMasterMessageSenderId => RGNMasterMessageSenderId;
        public string GetRGNMasterStorageBucket => RGNMasterStorageBucket;
        public Uri GetRGNMasterDatabaseUrl => new Uri(RGNMasterDatabaseUrl);
        public string GetRGNFriendInviteMessage => RGNFriendInviteMessage;
    }
}
