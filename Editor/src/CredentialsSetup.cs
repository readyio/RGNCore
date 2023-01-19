using System.IO;
using UnityEditor;
using UnityEngine;

namespace RGN.MyEditor
{
    [InitializeOnLoad]
    public class CredentialsSetup
    {
        private const string READY_MENU = "ReadyGamesNetwork/";
        private const string APPLICATION_STORE = READY_MENU + "Application Store";
        private const string SET_STAGING = READY_MENU + "Set Staging";
        private const string SET_PRODUCTION = READY_MENU + "Set Production";
        private const string SET_EMULATOR = READY_MENU + "Set Emulator";
        private const string EXPORT_CREDENTIALS = READY_MENU + "Export Credentials";

        static CredentialsSetup()
        {
            EditorApplication.delayCall += UpdateUI;
        }
        private static void UpdateUI()
        {
            EditorApplication.delayCall -= UpdateUI;
            ApplicationStore.MoveToResourcesFolderOrCreateNewIfNeeded();
            ApplicationStore applicationStore = ApplicationStore.LoadFromResources();
            Menu.SetChecked(SET_STAGING, !applicationStore.isProduction);
            Menu.SetChecked(SET_PRODUCTION, applicationStore.isProduction);
            Menu.SetChecked(SET_EMULATOR, applicationStore.usingEmulator);
        }

        [MenuItem(APPLICATION_STORE)]
        public static void OpenApplicationStore()
        {
            Selection.activeObject = ApplicationStore.LoadFromResources();
        }


        [MenuItem(SET_STAGING)]
        public static void SetStagingEnv()
        {
            BuildCredentials sourceCredentials = AssetDatabase.LoadAssetAtPath<BuildCredentials>(
                "Assets/ReadyGamesNetwork/Credentials/StagingBuildCredentials.asset");
            if (sourceCredentials == null)
            {
                Debug.LogError("Can not find source credentials for staging environment, please contact RGN team for help");
                return;
            }
            ApplicationStore.LoadFromResources().isProduction = false;
            SetEnvironment(sourceCredentials);
            UpdateUI();
        }
        [MenuItem(SET_PRODUCTION)]
        public static void SetProductionEnv()
        {
            BuildCredentials sourceCredentials = AssetDatabase.LoadAssetAtPath<BuildCredentials>(
                "Assets/ReadyGamesNetwork/Credentials/ProductionBuildCredentials.asset");
            if (sourceCredentials == null)
            {
                Debug.LogError("Can not find source credentials for production environment, please contact RGN team for help");
                return;
            }
            ApplicationStore.LoadFromResources().isProduction = true;
            SetEnvironment(sourceCredentials);
            UpdateUI();
        }
        [MenuItem(SET_EMULATOR)]
        public static void SetEmulator()
        {
            ApplicationStore applicationStore = ApplicationStore.LoadFromResources();
            applicationStore.usingEmulator = !applicationStore.usingEmulator;
            EditorUtility.SetDirty(applicationStore);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UpdateUI();
        }

        public static void SetEnvironment(BuildCredentials sourceCredentials)
        {
            ApplicationStore applicationStore = ApplicationStore.LoadFromResources();
            applicationStore.RGNProjectId = sourceCredentials.rgnProjectId;
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

        [MenuItem(EXPORT_CREDENTIALS)]
        public static void ExportCredentials()
        {
            string[] assetPathNames = {
                "Assets/ReadyGamesNetwork",
            };
            string fileName = "credentials.unitypackage";
            AssetDatabase.ExportPackage(assetPathNames, fileName, ExportPackageOptions.Recurse);
        }
    }
}
