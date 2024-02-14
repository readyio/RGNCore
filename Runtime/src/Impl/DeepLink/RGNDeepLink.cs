using System;
using System.Net;
using RGN.ImplDependencies.DeepLink;
using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using RGN.DeepLink.iOS;
#endif
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
using RGN.DeepLink.Windows;
#endif

namespace RGN.DeepLink
{
    public sealed class RGNDeepLink : IDeepLink
    {
        private RGNCore _rgnCore;
        private HttpListener _editorHttpListener;
        private bool _initialized;
        private bool _disposed;
        
        public event DeepLinkEventDelegate OnDeepLinkEvent;
        
        public void Init(RGNCore rgnCore)
        {
            if (_initialized)
            {
                return;
            }

            _rgnCore = rgnCore;
            
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
            WindowsDeepLink.StartHandling();
            WindowsDeepLink.DeepLinkActivated += OnDeepLinkActivated;
            _rgnCore.UpdateEvent += WindowsDeepLink.Tick;
#endif
            
#if UNITY_IOS && !UNITY_EDITOR
            WebViewPlugin.SetBackButtonText("Back");
#endif
            
            Application.deepLinkActivated += OnDeepLinkActivated;

            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                // Cold start and Application.absoluteURL not null so process Deep Link.
                OnDeepLinkActivated(Application.absoluteURL);
            }
            
            _initialized = true;
        }
        
        public void Dispose()
        {
            if (!_initialized || _disposed)
            {
                return;
            }
            
            _disposed = true;
            
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
            _rgnCore.UpdateEvent -= WindowsDeepLink.Tick;
#endif
            
            Application.deepLinkActivated -= OnDeepLinkActivated;
            
#if UNITY_EDITOR
            _editorHttpListener?.Stop();
            _editorHttpListener?.Close();
#endif
            
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
            WindowsDeepLink.DeepLinkActivated -= OnDeepLinkActivated;
            WindowsDeepLink.Dispose();
#endif
            
            OnDeepLinkEvent = null;
        }
        
        public async void StartEmulatorWatcher(string redirectUrl)
        {
#if UNITY_EDITOR
            if (_editorHttpListener == null)
            {
                _editorHttpListener = new HttpListener();
            }
            _editorHttpListener.Prefixes.Clear();
            _editorHttpListener.Prefixes.Add(redirectUrl);
            _editorHttpListener.Start();

            HttpListenerContext context;
            try { context = await _editorHttpListener.GetContextAsync(); }
            catch (ObjectDisposedException) { return; }
            HttpListenerResponse response = context.Response;
            
            string url = context.Request.Url.ToString();
            
            response.Close();
            _editorHttpListener.Stop();
            
            OnDeepLinkActivated(url);
#endif
        }
        
        private void OnDeepLinkActivated(string url)
        {
            Debug.Log("OnDeepLinkActivated with url: " + url);
            OnDeepLinkEvent?.Invoke(url);
        }
    }
}
