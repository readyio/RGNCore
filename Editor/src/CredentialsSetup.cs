using System.IO;
using UnityEditor;

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

        static CredentialsSetup()
        {
            EditorApplication.delayCall += UpdateUI;
        }
        private static void UpdateUI()
        {
            EditorApplication.delayCall -= UpdateUI;

            ApplicationStore applicationStore = ApplicationStore.I;
            Menu.SetChecked(SET_STAGING, !applicationStore.isProduction);
            Menu.SetChecked(SET_PRODUCTION, applicationStore.isProduction);
            Menu.SetChecked(SET_EMULATOR, applicationStore.usingEmulator);
        }

        [MenuItem(APPLICATION_STORE)]
        public static void OpenApplicationStore()
        {
            Selection.activeObject = ApplicationStore.I;
        }


        [MenuItem(SET_STAGING)]
        public static void SetStagingEnv()
        {
            BuildCredentials sourceCredentials = AssetDatabase.LoadAssetAtPath<BuildCredentials>("Assets/ReadyGamesNetwork/Credentials/StagingBuildCredentials.asset");
            ApplicationStore.I.isProduction = false;
            SetEnvironment(sourceCredentials);
            UpdateUI();
        }
        [MenuItem(SET_PRODUCTION)]
        public static void SetProductionEnv()
        {
            BuildCredentials sourceCredentials = AssetDatabase.LoadAssetAtPath<BuildCredentials>("Assets/ReadyGamesNetwork/Credentials/ProductionBuildCredentials.asset");
            ApplicationStore.I.isProduction = true;
            SetEnvironment(sourceCredentials);
            UpdateUI();
        }
        [MenuItem(SET_EMULATOR)]
        public static void SetEmulator()
        {
            ApplicationStore applicationStore = ApplicationStore.I;
            applicationStore.usingEmulator = !applicationStore.usingEmulator;
            EditorUtility.SetDirty(applicationStore);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UpdateUI();
        }

        public static void SetEnvironment(BuildCredentials sourceCredentials)
        {
            ApplicationStore applicationStore = ApplicationStore.I;
            applicationStore.googleSignInWebClientID = sourceCredentials.googleSignInWebClientID;
            applicationStore.googleSignInReverseClientID = sourceCredentials.googleSignInReverseClientID;
            applicationStore.RGNMasterApiKey = sourceCredentials.firebaseMasterApiKey;
            applicationStore.RGNMasterAndroidAppID = sourceCredentials.firebaseMasterAndroidAppID;
            applicationStore.RGNMasterIOSAppID = sourceCredentials.firebaseMasterIOSAppID;
            applicationStore.RGNMasterProjectId = sourceCredentials.firebaseMasterProjectId;
            applicationStore.RGNStorageURL = sourceCredentials.firebaseStorageURL;
            applicationStore.firebaseAssociatedDomain = sourceCredentials.firebaseAssociatedDomain;
            EditorUtility.SetDirty(applicationStore);

            if (applicationStore.iOS)
                AssetDatabase.SaveAssets();
            File.WriteAllText("Assets/ReadyGamesNetwork/Credentials/GoogleService-Info.plist", sourceCredentials.FirebaseCredentialsContentIOS);
            if (applicationStore.android)
                File.WriteAllText("Assets/ReadyGamesNetwork/Credentials/google-services.json", sourceCredentials.FirebaseCredentialsContentAndroid);

            AssetDatabase.Refresh();
        }
    }
}