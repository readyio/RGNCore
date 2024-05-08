using System.IO;
using UnityEditor;
using UnityEngine;

namespace RGN.MyEditor
{
    [InitializeOnLoad]
    public class CredentialsSetup
    {
        private const string READY_MENU = "ReadyGamesNetwork/";

        private const string CREDENTIALS = READY_MENU + "Credentials";
        private const string OPEN_DEVELOPERS_DASHBOARD = READY_MENU + "Open Developers Dashboard";
        private const string SET_STAGING = READY_MENU + "Set Staging";
        private const string SET_PRODUCTION = READY_MENU + "Set Production";
#if READY_DEVELOPMENT
        private const string SET_DEVELOPMENT = READY_MENU + "Set Development";
        private const string SET_EMULATOR = READY_MENU + "Set Emulator";
#endif

        static CredentialsSetup()
        {
            EditorApplication.delayCall += UpdateUI;
        }
        private static void UpdateUI()
        {
            EditorApplication.delayCall -= UpdateUI;
            ApplicationStore.MoveToResourcesFolderOrCreateNewIfNeeded();
            ApplicationStore applicationStore = ApplicationStore.LoadFromResources();
            Menu.SetChecked(SET_STAGING, applicationStore.GetRGNEnvironment == EnumRGNEnvironment.Staging);
            Menu.SetChecked(SET_PRODUCTION, applicationStore.GetRGNEnvironment == EnumRGNEnvironment.Production);
#if READY_DEVELOPMENT
            Menu.SetChecked(SET_DEVELOPMENT, applicationStore.GetRGNEnvironment == EnumRGNEnvironment.Development);
            Menu.SetChecked(SET_EMULATOR, applicationStore.usingEmulator);
#endif
            CreateOrReplaceGitIgnoreFileInRGNFolder();
        }

        [MenuItem(CREDENTIALS, priority = 10)]
        public static void OpenApplicationStore()
        {
            Selection.activeObject = ApplicationStore.LoadFromResources();
        }
        [MenuItem(OPEN_DEVELOPERS_DASHBOARD, priority = 11)]
        public static void OpenDevelopersDashboard()
        {
            Application.OpenURL("https://dev.ready.gg/");
        }

        [MenuItem(SET_STAGING, priority = 1)]
        public static void SetStagingEnv()
        {
            BuildCredentials sourceCredentials = AssetDatabase.LoadAssetAtPath<BuildCredentials>(
                "Assets/ReadyGamesNetwork/Credentials/StagingBuildCredentials.asset");
            if (sourceCredentials == null)
            {
                Debug.LogError("Can not find source credentials for staging environment, please contact RGN team for help");
                return;
            }
            ApplicationStore.LoadFromResources().RGNEnvironment = EnumRGNEnvironment.Staging;
            SetEnvironment(sourceCredentials);
            UpdateUI();
        }
        [MenuItem(SET_PRODUCTION, priority = 2)]
        public static void SetProductionEnv()
        {
            BuildCredentials sourceCredentials = AssetDatabase.LoadAssetAtPath<BuildCredentials>(
                "Assets/ReadyGamesNetwork/Credentials/ProductionBuildCredentials.asset");
            if (sourceCredentials == null)
            {
                Debug.LogError("Can not find source credentials for production environment, please contact RGN team for help");
                return;
            }
            ApplicationStore.LoadFromResources().RGNEnvironment = EnumRGNEnvironment.Production;
            SetEnvironment(sourceCredentials);
            UpdateUI();
        }

#if READY_DEVELOPMENT
        [MenuItem(SET_DEVELOPMENT, priority = 0)]
        public static void SetDevelopmentEnv()
        {
            BuildCredentials sourceCredentials = AssetDatabase.LoadAssetAtPath<BuildCredentials>(
                "Assets/ReadyGamesNetwork/Credentials/DevelopmentBuildCredentials.asset");
            if (sourceCredentials == null)
            {
                Debug.LogError("Can not find source credentials for development environment, please contact RGN team for help");
                return;
            }
            ApplicationStore.LoadFromResources().RGNEnvironment = EnumRGNEnvironment.Development;
            SetEnvironment(sourceCredentials);
            UpdateUI();
        }
        [MenuItem(SET_EMULATOR, priority = 3)]
        public static void SetEmulator()
        {
            ApplicationStore applicationStore = ApplicationStore.LoadFromResources();
            applicationStore.usingEmulator = !applicationStore.usingEmulator;
            EditorUtility.SetDirty(applicationStore);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UpdateUI();
        }
#endif

        public static void SetEnvironment(BuildCredentials sourceCredentials)
        {
            ApplicationStore applicationStore = ApplicationStore.LoadFromResources();
            applicationStore.RGNProjectId = sourceCredentials.rgnProjectId;
            applicationStore.RGNApiKey = sourceCredentials.rgnApiKey;
            applicationStore.googleSignInWebClientIdAndroid = sourceCredentials.googleSignInWebClientIdAndroid;
            applicationStore.googleSignInReverseClientIdAndroid = sourceCredentials.googleSignInReverseClientIdAndroid;
            applicationStore.googleSignInWebClientIdiOS = sourceCredentials.googleSignInWebClientIdiOS;
            applicationStore.googleSignInReverseClientIdiOS = sourceCredentials.googleSignInReverseClientIdiOS;
            applicationStore.RGNMasterApiKey = sourceCredentials.firebaseMasterApiKey;
            applicationStore.RGNMasterAndroidAppID = sourceCredentials.firebaseMasterAndroidAppID;
            applicationStore.RGNMasterIOSAppID = sourceCredentials.firebaseMasterIOSAppID;
            applicationStore.RGNMasterProjectId = sourceCredentials.firebaseMasterProjectId;
            applicationStore.RGNStorageURL = sourceCredentials.firebaseStorageURL;
            applicationStore.firebaseAssociatedDomain = sourceCredentials.firebaseAssociatedDomain;
            applicationStore.RGNMasterMessageSenderId = sourceCredentials.firebaseMasterMessageSenderId;
            applicationStore.RGNMasterStorageBucket = sourceCredentials.firebaseMasterStorageBucket;
            applicationStore.RGNMasterDatabaseUrl = sourceCredentials.firebaseMasterDatabaseUrl;

            EditorUtility.SetDirty(applicationStore);
            AssetDatabase.SaveAssets();

            const string PLIST_PATH = "Assets/ReadyGamesNetwork/Credentials/GoogleService-Info.plist";
            const string JSON_PATH = "Assets/ReadyGamesNetwork/Credentials/google-services.json";
            if (string.IsNullOrEmpty(sourceCredentials.FirebaseCredentialsContentIOS))
            {
                File.Delete(PLIST_PATH);
            }
            else
            {
                File.WriteAllText(PLIST_PATH, sourceCredentials.FirebaseCredentialsContentIOS);
            }
            if (string.IsNullOrEmpty(sourceCredentials.FirebaseCredentialsContentAndroid))
            {
                File.Delete(JSON_PATH);
            }
            else
            {
                File.WriteAllText(JSON_PATH, sourceCredentials.FirebaseCredentialsContentAndroid);
            }
            AssetDatabase.Refresh();
        }

        private static void CreateOrReplaceGitIgnoreFileInRGNFolder()
        {
            string filePath = Path.Combine(Application.dataPath, "ReadyGamesNetwork", ".gitignore");
            File.WriteAllText(filePath, $"Linker/*\nLinker.meta");
        }
    }
}
