using RGN;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ApplicationStore", order = 1)]
public class ApplicationStore : ScriptableObject
{
#if UNITY_EDITOR
    private const string APPLICATION_STORE_DIR = "Assets/ReadyGamesNetwork";

    private const string APPLICATION_STORE_FILE = "Assets/ReadyGamesNetwork/ApplicationStore.asset";

    private static ApplicationStore instance;

    public static ApplicationStore Instance
    {
        get
        {
            if (instance == null) {
                if (!AssetDatabase.IsValidFolder(APPLICATION_STORE_DIR)) {
                    AssetDatabase.CreateFolder("Assets", "ReadyGamesNetwork");
                }

                instance = (ApplicationStore)AssetDatabase.LoadAssetAtPath(APPLICATION_STORE_FILE, typeof(ApplicationStore));

                if (instance == null) {
                    instance = CreateInstance<ApplicationStore>();
                    AssetDatabase.CreateAsset(instance, APPLICATION_STORE_FILE);
                }
            }
            return instance;
        }
    }

    public static ApplicationStore I
    {
        get
        {
            return Instance;
        }
    }
#endif

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

}