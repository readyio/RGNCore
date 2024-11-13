using System;
using System.Collections.Specialized;
using RGN.DeepLink;
using RGN.ImplDependencies.WebForm;
using RGN.Utility;
using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using RGN.DeepLink.iOS;
#endif

namespace RGN.WebForm
{
    public sealed class RGNWebForm : IWebForm
    {
        private WebFormSignInRedirectDelegate _onWebFormSignInRedirect;
        private WebFormCreateWalletRedirectDelegate _onWebFormCreateWalletRedirect;
        private WebFormOpenMarketplaceRedirectDelegate _onWebFormOpenMarketplaceRedirect;

        public void SignIn(WebFormSignInRedirectDelegate redirectCallback, string idToken)
        {
            _onWebFormSignInRedirect = redirectCallback;
            string redirectUrl = RGNDeepLinkHttpUtility.GetDeepLinkRedirectScheme();
            string url = GetWebFormUrl(redirectUrl) +
                         "&returnSecureToken=true" +
                         "&returnRefreshToken=true" +
                         "&idToken=" + idToken +
                         "&platform=" + GetCurrentPlatform();
            OpenWebForm(url, redirectUrl);
        }

        public void SignInWithDeviceCode(string deviceCode, string idToken)
        {
            string url = GetWebFormDeviceFlowUrl() +
                         "&returnSecureToken=true" +
                         "&returnRefreshToken=true" +
                         "&device_id=" + deviceCode +
                         "&idToken=" + idToken +
                         "&platform=" + GetCurrentPlatform();
            RGNCore.I.Dependencies.EngineApp.OpenUrl(url);
        }

        public void CreateWallet(WebFormCreateWalletRedirectDelegate redirectCallback, string idToken)
        {
            _onWebFormCreateWalletRedirect = redirectCallback;
            string redirectUrl = RGNDeepLinkHttpUtility.GetDeepLinkRedirectScheme();
            string url = GetWebFormUrl(redirectUrl) +
                         "&returnSecureToken=true" +
                         "&returnRefreshToken=true" +
                         "&idToken=" + idToken +
                         "&view=createwallet" +
                         "&platform=" + GetCurrentPlatform();
            OpenWebForm(url, redirectUrl);
        }

        public void OpenMarketplace(WebFormOpenMarketplaceRedirectDelegate redirectCallback, string idToken, string inventoryItemId = null)
        {
            _onWebFormOpenMarketplaceRedirect = redirectCallback;
            string redirectUrl = RGNDeepLinkHttpUtility.GetDeepLinkRedirectScheme();
            string url = GetMarketplaceUrl(redirectUrl) +
                         "&idToken=" + idToken +
                         "&platform=" + GetCurrentPlatform();
            if (!string.IsNullOrEmpty(inventoryItemId))
            {
                url += "&inventoryItemId=" + inventoryItemId;
            }
            OpenWebForm(url, redirectUrl);
        }

        private void OpenWebForm(string url, string redirectUrl)
        {
#if UNITY_EDITOR
            RGNCore.I.Dependencies.DeepLink.StartEmulatorWatcher(redirectUrl);
#endif
            RGNCore.I.Dependencies.DeepLink.OnDeepLinkEvent += OnDeepLink;

            ApplicationFocusWatcher appFocusWatcher = ApplicationFocusWatcher.Create(delay: 1f);
            appFocusWatcher.OnFocusChanged += OnAppFocusChanged;

#if UNITY_IOS && !UNITY_EDITOR
            WebViewPlugin.ChangeURLScheme(redirectUrl);
            WebViewPlugin.OpenURL(url);
#else
            Application.OpenURL(url);
#endif
        }

        private void OnAppFocusChanged(ApplicationFocusWatcher appFocusWatcher, bool hasFocus)
        {
            if (hasFocus)
            {
                appFocusWatcher.OnFocusChanged -= OnAppFocusChanged;
                appFocusWatcher.Destroy();

                _onWebFormSignInRedirect?.Invoke(true, "");
                _onWebFormCreateWalletRedirect?.Invoke(true, "");
                _onWebFormOpenMarketplaceRedirect?.Invoke(true, "");
            }
        }

        private void OnDeepLink(string url)
        {
            string token = "";
            string[] urlParts = url.Split('?');
            if (urlParts.Length > 1)
            {
                string parameters = urlParts[1];
                NameValueCollection parsedParameters = HttpUtility.ParseQueryArgs(parameters);
                if (!string.IsNullOrEmpty(parsedParameters["token"]))
                {
                    token = parsedParameters["token"];
                }
            }

            if (_onWebFormSignInRedirect != null)
            {
                _onWebFormSignInRedirect.Invoke(false, token);
                _onWebFormSignInRedirect = null;
            }

            if (_onWebFormCreateWalletRedirect != null)
            {
                _onWebFormCreateWalletRedirect.Invoke(false, token);
                _onWebFormCreateWalletRedirect = null;
            }

            if (_onWebFormOpenMarketplaceRedirect != null)
            {
                _onWebFormOpenMarketplaceRedirect.Invoke(false, token);
                _onWebFormOpenMarketplaceRedirect = null;
            }
        }

        private string GetWebFormUrl(string redirectUrl) =>
            GetBaseWebFormUrl() +
            redirectUrl +
            "&appId=" + RGNCore.I.AppIDForRequests +
            "&lang=" + Utility.LanguageUtility.GetISO631Dash1CodeFromSystemLanguage();

        private string GetWebFormDeviceFlowUrl() =>
            GetBaseWebFormDeviceFlowUrl() +
            "?appId=" + RGNCore.I.AppIDForRequests +
            "&lang=" + Utility.LanguageUtility.GetISO631Dash1CodeFromSystemLanguage();

        private string GetBaseWebFormUrl()
        {
            ApplicationStore applicationStore = ApplicationStore.LoadFromResources();
            return applicationStore.GetRGNEnvironment switch {
                EnumRGNEnvironment.Development => applicationStore.GetRGNDevelopmentEmailSignInURL,
                EnumRGNEnvironment.Staging => applicationStore.GetRGNStagingEmailSignInURL,
                EnumRGNEnvironment.Production => applicationStore.GetRGNProductionEmailSignInURL,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetBaseWebFormDeviceFlowUrl()
        {
            ApplicationStore applicationStore = ApplicationStore.LoadFromResources();
            return applicationStore.GetRGNEnvironment switch {
                EnumRGNEnvironment.Development => ApplicationStore.DEVELOPMENT_DEVICE_FLOW_SIGN_IN_URL,
                EnumRGNEnvironment.Staging => ApplicationStore.STAGING_DEVICE_FLOW_SIGN_IN_URL,
                EnumRGNEnvironment.Production => ApplicationStore.PRODUCTION_DEVICE_FLOW_SIGN_IN_URL,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetMarketplaceUrl(string redirectUrl) =>
            GetBaseMarketplaceUrl() +
            redirectUrl +
            "&appId=" + RGNCore.I.AppIDForRequests +
            "&lang=" + Utility.LanguageUtility.GetISO631Dash1CodeFromSystemLanguage();

        private string GetBaseMarketplaceUrl()
        {
            ApplicationStore applicationStore = ApplicationStore.LoadFromResources();
            return applicationStore.GetRGNEnvironment switch {
                EnumRGNEnvironment.Development => ApplicationStore.DEVELOPMENT_MARKETPLACE_URL,
                EnumRGNEnvironment.Staging => ApplicationStore.STAGING_MARKETPLACE_URL,
                EnumRGNEnvironment.Production => ApplicationStore.PRODUCTION_MARKETPLACE_URL,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetCurrentPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "android";
                case RuntimePlatform.IPhonePlayer:
                    return "ios";
                case RuntimePlatform.WindowsPlayer:
                    return "windows";
                case RuntimePlatform.OSXPlayer:
                    return "macos";
                case RuntimePlatform.LinuxPlayer:
                    return "linux";
                case RuntimePlatform.LinuxEditor:
                    return "linux_editor";
                case RuntimePlatform.OSXEditor:
                    return "macos_editor";
                case RuntimePlatform.WindowsEditor:
                    return "windows_editor";
                case RuntimePlatform.WebGLPlayer:
                    return "webgl";
                case RuntimePlatform.WSAPlayerX86:
                    return "wsa_x86";
                case RuntimePlatform.WSAPlayerX64:
                    return "wsa_x64";
                case RuntimePlatform.WSAPlayerARM:
                    return "wsa_arm";
                case RuntimePlatform.PS4:
                    return "ps4";
                case RuntimePlatform.XboxOne:
                    return "xbox_one";
                case RuntimePlatform.tvOS:
                    return "tvos";
                case RuntimePlatform.Switch:
                    return "nintendo_switch";
                case RuntimePlatform.Stadia:
                    return "stadia";
                case RuntimePlatform.PS5:
                    return "ps5";
                default:
                    return "unknown";
            }
        }

    }
}
