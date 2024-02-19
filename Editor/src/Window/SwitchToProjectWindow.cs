using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RGN.Impl.Firebase.Core.Auth;
using RGN.Impl.Firebase.Network;
using RGN.ImplDependencies.Core.Auth;
using RGN.Network;
using UnityEditor;
using UnityEngine;
using HttpMethod = RGN.Network.HttpMethod;
using HttpRequestMessage = RGN.Network.HttpRequestMessage;

namespace RGN.MyEditor
{
    public class SwitchToProjectWindow : EditorWindow
    {
        [System.Serializable]
        public class ProjectResultData
        {
            public string owner;
            public ProjectData project;
        }
        [System.Serializable]
        public class ProjectData
        {
            public string name;
            public string id;
            public string projectId;
            public List<object> firebase_apps;
        }

        private const string REGION = "us-central1";

        private string searchField = "";
        private List<ProjectData> projectsList = new List<ProjectData>();
        private List<ProjectData> filteredProjectsList = new List<ProjectData>();
        private Vector2 scrollPosition;
        private bool uiEnabled = true;
        private string _errorMessage;
        private string _baseCloudAddress;
        private int _functionsPort = -1;

#if READY_DEVELOPMENT
        [MenuItem(UsefulMenuItems.READY_MENU + "Window/Switch Project Credentials")]
        public static void ShowWindow()
        {
            GetWindow<SwitchToProjectWindow>("Project Switch");
        }
#endif

        private async void OnEnable()
        {
            var appStore = ApplicationStore.LoadFromResources();
            if (appStore.IsUsingEmulator)
            {
                _functionsPort = int.Parse(appStore.GetFunctionsPort.Substring(1));
                _baseCloudAddress = $"http://{appStore.GetEmulatorServerIp + appStore.GetFunctionsPort}/{appStore.GetRGNMasterProjectId}/{REGION}/";
            }
            else
            {
                _baseCloudAddress = $"https://{REGION}-{appStore.GetRGNMasterProjectId}.cloudfunctions.net/";
            }
            await ReloadProjectsListAsync();
        }

        private async void OnGUI()
        {
            if (!uiEnabled)
            {
                var style = GUI.skin.GetStyle("label");
                style.fontSize = 42;
                GUILayout.Label("Loading...", style);
                return;
            }
            if (!string.IsNullOrWhiteSpace(_errorMessage))
            {
                var style = GUI.skin.GetStyle("label");
                style.fontSize = 42;
                GUILayout.Label(_errorMessage, style);
                return;
            }
            GUI.enabled = uiEnabled;

            GUILayout.BeginHorizontal();
            string oldSearch = searchField;
            searchField = EditorGUILayout.TextField("Search Projects", searchField);
            if (oldSearch != searchField)
            {
                FilterProjects();
            }
            if (GUILayout.Button("Update", GUILayout.MaxWidth(200)))
            {
                await ReloadProjectsListAsync();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
            foreach (ProjectData project in filteredProjectsList)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(project.name))
                {
                    Application.OpenURL("https://console.firebase.google.com/u/0/project/" + project.projectId);
                }

                GUI.enabled = project.firebase_apps != null && project.firebase_apps.Count > 0;
                if (GUILayout.Button("Apply Credentials", GUILayout.MaxWidth(200)))
                {
                    await ApplyCredentialsAsync(project.id);
                }
                GUI.enabled = uiEnabled;

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUI.enabled = uiEnabled;
        }

        private async Task ApplyCredentialsAsync(string projectId)
        {
            try
            {
                _errorMessage = null;
                uiEnabled = false;
                string token = PlayerPrefs.GetString(AuthTokenKeys.IdToken.GetKeyName());
                if (string.IsNullOrEmpty(token))
                {
                    return;
                }
                Dictionary<string, string> queryParameters = new Dictionary<string, string>() {
                    { "idToken", token },
                    { "projectId", projectId }
                };
                var result = await CallHttpRequestFunctionAsync<dynamic>(
                    "projectManagement-generateUnityPackageRequest",
                    null,
                    queryParameters);

                byte[] data = await result.ReadAsBytes();

                // Use data as needed, e.g., save to a file
                string filePath = Path.Combine(Application.persistentDataPath, "credentials.unitypackage");
                File.WriteAllBytes(filePath, data);
                if (Application.isPlaying)
                {
                    EditorApplication.isPlaying = false;
                }
                await Task.Delay(1000);
                AssetDatabase.ImportPackage(filePath, true);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                _errorMessage = "Exception: " + ex.Message;
            }
            Repaint();
            uiEnabled = true;
        }

        private async Task ReloadProjectsListAsync()
        {
            try
            {
                _errorMessage = null;
                uiEnabled = false;

                var result = await CallHttpRequestFunctionJsonAsync<dynamic, Dictionary<string, List<ProjectResultData>>>(
                    "projectsCollection-getUserProjects");
                projectsList = new List<ProjectData>();
                foreach (var resultData in result["projects"])
                {
                    projectsList.Add(resultData.project);
                }

                FilterProjects();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                _errorMessage = "Exception: " + ex.Message;
            }
            Repaint();
            uiEnabled = true;
        }

        private async Task<TReturn> CallHttpRequestFunctionJsonAsync<TParams, TReturn>(
            string functionName,
            TParams parameters = null,
            Dictionary<string, string> queryParameters = null)
            where TParams : class
        {
            IHttpResponse httpResult = await CallHttpRequestFunctionAsync(
                functionName,
                parameters,
                queryParameters);
            string resposeJson = await httpResult.ReadAsString();
            var result = JsonConvert.DeserializeObject<TReturn>(resposeJson);
            return result;
        }

        private async Task<IHttpResponse> CallHttpRequestFunctionAsync<TParams>(
            string functionName,
            TParams parameters,
            Dictionary<string, string> queryParameters) where TParams : class
        {
            using IHttpClient httpClient = HttpClientFactory.Get(typeof(SwitchToProjectWindow).ToString());
            string functionUrl = _baseCloudAddress + functionName;
            if (queryParameters != null)
            {
                var builder = new UriBuilder(functionUrl) {
                    Port = _functionsPort,
                    Query = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"))
                };
                functionUrl = builder.ToString();
            }
            Debug.Log("Calling function with url: " + functionUrl);
            HttpRequestMessage request = BuildHttpRequest(functionUrl);
            if (parameters != null)
            {
                string jsonRequest = JsonConvert.SerializeObject(parameters);
                request.SetStringBody(jsonRequest);
            }
            IHttpResponse httpResult = await httpClient.SendAsync(request);
            httpResult.EnsureSuccessStatusCode();
            return httpResult;
        }

        private void FilterProjects()
        {
            if (string.IsNullOrEmpty(searchField))
            {
                filteredProjectsList = new List<ProjectData>(projectsList);
            }
            else
            {
                filteredProjectsList = projectsList.Where(p => p.name.ToLower().Contains(searchField.ToLower())).ToList();
            }
        }

        private HttpRequestMessage BuildHttpRequest(string functionUrl)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(functionUrl));
            string token = PlayerPrefs.GetString(AuthTokenKeys.IdToken.GetKeyName());
            if (!string.IsNullOrEmpty(token))
            {
                request.AddHeader("Authorization", "Bearer " + token);
            }
            return request;
        }
    }
}
