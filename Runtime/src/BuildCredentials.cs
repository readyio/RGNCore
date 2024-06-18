using UnityEngine;

namespace RGN
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BuildCredentials", order = 2)]
    public class BuildCredentials : ScriptableObject
    {
        [Header("Backend connection settings for the ReadyGG platform")]
        public string rgnProjectId;
        public string rgnApiKey;

        [HideInInspector] public string googleSignInWebClientIdAndroid = "";
        [HideInInspector] public string googleSignInReverseClientIdAndroid = "";
        [HideInInspector] public string googleSignInWebClientIdiOS = "";
        [HideInInspector] public string googleSignInReverseClientIdiOS = "";

        [HideInInspector] public string firebaseAssociatedDomain = "";
        [HideInInspector] public string FirebaseCredentialsContentIOS = "";
        [HideInInspector] public string FirebaseCredentialsContentAndroid = "";

        [HideInInspector] public string firebaseMasterApiKey = "";
        [HideInInspector] public string firebaseMasterAndroidAppID = "";
        [HideInInspector] public string firebaseMasterIOSAppID = "";
        [HideInInspector] public string firebaseMasterProjectId = "";
        [HideInInspector] public string firebaseStorageURL = "";
        [HideInInspector] public string firebaseMasterMessageSenderId = "";
        [HideInInspector] public string firebaseMasterStorageBucket = "";
        [HideInInspector] public string firebaseMasterDatabaseUrl = "";
    }
}
