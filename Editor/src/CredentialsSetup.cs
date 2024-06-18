using System.IO;
using RGN.Utility;
using UnityEditor;
using UnityEngine;

namespace RGN.MyEditor
{
    [InitializeOnLoad]
    public class CredentialsSetup
    {
        private const string READY_MENU = "ReadyGG/";

        private const string CREDENTIALS = READY_MENU + "Credentials";
        private const string OPEN_DEVELOPERS_DASHBOARD = READY_MENU + "Open Developers Dashboard";
        private const string OPEN_DOCUMENTATION = READY_MENU + "Open Documentation";
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
        [MenuItem(OPEN_DOCUMENTATION, priority = 12)]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://readygames.gitbook.io/readygg-sdk-documentation/");
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
            int randomSeed = GetRandomSecretSeedNumberForObfuscation();
            applicationStore.RGNProjectId = Obfuscator.Obfuscate(sourceCredentials.rgnProjectId, randomSeed);
            applicationStore.RGNApiKey = Obfuscator.Obfuscate(sourceCredentials.rgnApiKey, randomSeed);
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
        private static int GetRandomSecretSeedNumberForObfuscation()
        {
            string scriptsFolder = Path.Combine(Application.dataPath, "ReadyGamesNetwork", "Source");
            string randomSecretNumberFile = Path.Combine(scriptsFolder, "NumberHolder.cs");
            string assemblyDefinitionFile = Path.Combine(scriptsFolder, "GetReady.Developer.Runtime.asmdef");

            if (!Directory.Exists(scriptsFolder))
            {
                Directory.CreateDirectory(scriptsFolder);
            }

            if (!File.Exists(assemblyDefinitionFile))
            {
                string asmdefContent = "{\n\t\"name\": \"GetReady.Developer.Runtime\"\n}";
                File.WriteAllText(assemblyDefinitionFile, asmdefContent);
            }

            if (!File.Exists(randomSecretNumberFile))
            {
                int randomSecretNumber = new System.Random().Next(int.MinValue + 1, int.MaxValue);
                string numberHolderContent = $"namespace RGN\n{{\n\tpublic static class NumberHolder\n\t{{\n\t\tpublic static readonly int s = {randomSecretNumber};\n\t}}\n}}";
                File.WriteAllText(randomSecretNumberFile, numberHolderContent);
                return randomSecretNumber;
            }
            else
            {
                string[] lines = File.ReadAllLines(randomSecretNumberFile);
                foreach (string line in lines)
                {
                    if (line.Contains("public static readonly int s ="))
                    {
                        string numberString = line.Split('=')[1].Trim().TrimEnd(';');
                        if (int.TryParse(numberString, out int existingNumber))
                        {
                            return existingNumber;
                        }
                    }
                }
            }
            // Fallback in case of unexpected file content
            Debug.LogError("Error in getting or generating random number for the obfuscation.");
            return int.MinValue;
        }
    }
}
