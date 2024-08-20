using System;
using System.Collections.Specialized;
using RGN.DeepLink;
using RGN.ImplDependencies.WebForm;
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
                         "&idToken=" + idToken;
            OpenWebForm(url, redirectUrl);
        }

        public void CreateWallet(WebFormCreateWalletRedirectDelegate redirectCallback, string idToken)
        {
            _onWebFormCreateWalletRedirect = redirectCallback;
            string redirectUrl = RGNDeepLinkHttpUtility.GetDeepLinkRedirectScheme();
            string url = GetWebFormUrl(redirectUrl) +
                         "&returnSecureToken=true" +
                         "&returnRefreshToken=true" +
                         "&idToken=" + idToken +
                         "&view=createwallet";
            OpenWebForm(url, redirectUrl);
        }

        public void OpenMarketplace(WebFormOpenMarketplaceRedirectDelegate redirectCallback, string idToken, string inventoryItemId = null)
        {
            _onWebFormOpenMarketplaceRedirect = redirectCallback;
            string redirectUrl = RGNDeepLinkHttpUtility.GetDeepLinkRedirectScheme();
            string url = GetMarketplaceUrl(redirectUrl) +
                         "&idToken=" + idToken;
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
                _onWebFormOpenMarketplaceRedirect?.Invoke(true);
            }
        }

        private void OnDeepLink(string url)
        {
            string token = "";
            string[] urlParts = url.Split('?');
            if (urlParts.Length > 1)
            {
                string parameters = urlParts[1];
                NameValueCollection parsedParameters = RGNDeepLinkHttpUtility.ParseQueryString(parameters);
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
                _onWebFormOpenMarketplaceRedirect.Invoke(false);
                _onWebFormOpenMarketplaceRedirect = null;
            }
        }

        private string GetWebFormUrl(string redirectUrl) =>
            GetBaseWebFormUrl() +
            redirectUrl +
            "&appId=" + RGNCore.I.AppIDForRequests +
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
    }
}
