using System;
using UnityEngine;

namespace RGN.Impl.Firebase.Core.Auth
{
    public enum AuthTokenKeys
    {
        IdToken = 0,
        RefreshToken = 1
    }
    
    public static class AuthTokenKeysExtensions
    {
        private static ApplicationStore sApplicationStore;
        
        public static string GetKeyName(this AuthTokenKeys tokenKey)
        {
            string productName = Application.productName;
            string environment = "unknown";
            if (sApplicationStore == null)
            {
                sApplicationStore = ApplicationStore.LoadFromResources();
            }
            if (sApplicationStore != null)
            {
                environment = sApplicationStore.GetRGNEnvironment.ToString().ToLower();
            }
            switch (tokenKey)
            {
                case AuthTokenKeys.IdToken: return $"readygg_idtoken_{productName}_{environment}";
                case AuthTokenKeys.RefreshToken: return $"readygg_refreshtoken_{productName}_{environment}";
                default: throw new ArgumentOutOfRangeException(nameof(AuthTokenKeys));
            }
        }
    }
}
