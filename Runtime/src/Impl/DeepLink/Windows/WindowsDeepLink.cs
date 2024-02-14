#if UNITY_STANDALONE_WIN
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace RGN.DeepLink.Windows
{
    internal sealed class WindowsDeepLink
    {
        private static string ApplicationName;
        
        private static Queue<string> _events;
        private static Mutex _mutex;
        private static Thread _thread;
        
        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private static readonly CancellationToken CancellationToken = CancellationTokenSource.Token;

        public delegate void DeepLinkActivatedDelegate(string url);
        public static DeepLinkActivatedDelegate DeepLinkActivated;

        public static void StartHandling()
        {
            ApplicationName = Application.productName;
            
            if (!IsCustomUrlRegistered())
            {
                using (Process appRunasProcess = new Process())
                {
                    appRunasProcess.StartInfo.FileName = GetAppReflectorExecutablePath();
                    appRunasProcess.StartInfo.Verb = "runas";
                    appRunasProcess.Start();
                }
                SetIsCustomUrlRegistered(true);
            }

            _events = new Queue<string>();
            if (!string.IsNullOrEmpty(Environment.CommandLine))
            {
                _events.Enqueue(Environment.CommandLine);
            }
            
            _mutex = new Mutex(false, ApplicationName);
            try
            {
                _mutex.WaitOne();
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError($"Error while to try open mutex: {exception}");
            }
            
            _thread = new Thread(ListenPipe);
            _thread.Start();
        }

        public static void Tick()
        {
            if (_events.Count > 0)
            {
                string message = _events.Dequeue();
                try
                {
                    string[] messageParts = message.Split(' ');
                    message = messageParts[messageParts.Length - 1].Replace("\"", "");
                    Debug.LogError(message);
                    DeepLinkActivated?.Invoke(message);
                }
                catch
                {
                    // ignore
                }
            }
        }

        public static void Dispose()
        {
            CancellationTokenSource.Cancel();
            _mutex?.Close();
            _thread?.Abort();
        }

        private static void ListenPipe()
        {
#pragma warning disable CS4014
            Task.Run(ClosePipeHack);
#pragma warning restore CS4014
        
            while (!CancellationToken.IsCancellationRequested)
            {
                var pipeServer = new NamedPipeServerStream(ApplicationName, PipeDirection.In, 1);
                pipeServer.WaitForConnection();
            
                using var sr = new StreamReader(pipeServer);
                string message = sr.ReadLine();
                _events.Enqueue(message);
            
                pipeServer.Dispose();
            }
        }
        
        private static async Task ClosePipeHack()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                Thread.Yield();
            }

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", ApplicationName, PipeDirection.Out))
            {
                await pipeClient.ConnectAsync(100);
            }
        }

        private static string GetAppExecutablePath()
        {
            string dataPath = Application.dataPath;
            string appPath = Path.Combine(dataPath, "../");
            string appExecutablePath = Path.GetFullPath(Path.Combine(appPath, ApplicationName + ".exe"));
            return appExecutablePath;
        }

        private static string GetAppReflectorExecutablePath()
        {
            string dataPath = Application.dataPath;
            string appPath = Path.Combine(dataPath, "../");
            string appExecutablePath = Path.GetFullPath(Path.Combine(appPath, $"{ApplicationName}DL.exe"));
            return appExecutablePath;
        }

        public static bool IsCustomUrlRegistered()
        {
            string deepLinkRedirectScheme = RGNDeepLinkHttpUtility.GetDeepLinkRedirectScheme();
            return PlayerPrefs.GetInt(deepLinkRedirectScheme, 0) == 1;
        }
        
        private static void SetIsCustomUrlRegistered(bool value)
        {
            string deepLinkRedirectScheme = RGNDeepLinkHttpUtility.GetDeepLinkRedirectScheme();
            PlayerPrefs.SetInt(deepLinkRedirectScheme, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
#endif
