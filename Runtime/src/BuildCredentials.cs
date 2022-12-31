using UnityEngine;

namespace RGN
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BuildCredentials", order = 2)]
    public class BuildCredentials : ScriptableObject
    {
        [Header("Google SignIn Credentials")]
        public string googleSignInWebClientID = "";
        public string googleSignInReverseClientID = "";

        [Header("Game Credentials")]
        public string firebaseAssociatedDomain = "";
        [TextArea(50, 100)]
        [Tooltip("Paste GoogleService-Info.plist file")]
        public string FirebaseCredentialsContentIOS = "";
        [TextArea(50, 100)]
        [Tooltip("Paste google-services.json file")]
        public string FirebaseCredentialsContentAndroid = "";

        [Header("Master Credentials")]
        public string firebaseMasterApiKey = "";
        public string firebaseMasterAndroidAppID = "";
        public string firebaseMasterIOSAppID = "";
        public string firebaseMasterProjectId = "";
        public string firebaseStorageURL = "";
        public string firebaseMasterMessageSenderId = "";
        public string firebaseMasterStorageBucket = "";
        public string firebaseMasterDatabaseUrl = "";
    }
}
