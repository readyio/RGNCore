using RGN.Utility;
using UnityEditor;
using UnityEngine;

namespace RGN
{
    [CustomEditor(typeof(ApplicationStore))]
    public class ApplicationStoreEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ApplicationStore applicationStore = (ApplicationStore)target;

            GUI.enabled = false;
            
            EditorGUILayout.TextField(
                new GUIContent("Project Id / AppId", "The unique identifier for your project"),
                applicationStore.GetRGNProjectId);
            EditorGUILayout.TextField(
                new GUIContent("API Key", "The API key used for signing requests with hmac. Keep it secret."),
                applicationStore.GetRGNApiKey);

            EditorGUILayout.EnumPopup("Current Environment", applicationStore.RGNEnvironment);

            GUI.enabled = true;

            if (applicationStore.usingEmulator)
            {
                GUILayout.Label("Emulator Settings:");

                EditorGUI.indentLevel++;
                applicationStore.emulatorServerIp = EditorGUILayout.TextField(
                    new GUIContent("Emulator Server IP", "The IP address of the emulator server"),
                    applicationStore.emulatorServerIp);
                applicationStore.firestorePort = EditorGUILayout.TextField(
                    new GUIContent("Firestore Port", "The port number for Firestore in the emulator"),
                    applicationStore.firestorePort);
                applicationStore.functionsPort = EditorGUILayout.TextField(
                    new GUIContent("Functions Port", "The port number for Functions in the emulator"),
                    applicationStore.functionsPort);
                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button("Open Dashboard"))
            {
                OpenDashboard(applicationStore.RGNEnvironment);
            }
            if (GUILayout.Button("Open Documentation"))
            {
                Application.OpenURL("https://readygames.gitbook.io/readygg-sdk-documentation/");
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(applicationStore);
            }
        }

        private void OpenDashboard(EnumRGNEnvironment environment)
        {
            string url = "";
            switch (environment)
            {
                case EnumRGNEnvironment.Development:
                    url = "https://development.ready.gg/";
                    break;
                case EnumRGNEnvironment.Staging:
                    url = "https://staging.ready.gg/";
                    break;
                case EnumRGNEnvironment.Production:
                    url = "https://dev.ready.gg/";
                    break;
            }
            Application.OpenURL(url);
        }
    }
}
