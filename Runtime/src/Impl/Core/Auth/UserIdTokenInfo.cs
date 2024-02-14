using System;
using System.Collections.Generic;
using RGN.ImplDependencies.Core.Auth;
using RGN.Utility;

namespace RGN.Impl.Firebase.Core.Auth
{
    internal struct UserIdTokenInfo : IUserIdTokenInfo
    {
        private readonly Dictionary<string, object> mDecodedPayload;
        
        public string RawIdToken { get; set; }

        public string Name => GetTokenProperty<string>(mDecodedPayload, "name");
        public string UserId => GetTokenProperty<string>(mDecodedPayload, "user_id");
        public string Email => GetTokenProperty<string>(mDecodedPayload, "email");
        public bool? EmailVerified => GetTokenProperty<bool>(mDecodedPayload, "email_verified");
        public bool IsAnonymous => string.IsNullOrEmpty(Email);
        public DateTime? ExpiredAt
        {
            get
            {
                long? exp = GetTokenProperty<long?>(mDecodedPayload, "exp");
                if (exp == null)
                {
                    return null;
                }

                DateTime unixEpochDateTime = DateTimeUtility.GetUnixEpochDateTimeUtc();
                DateTime expiredAtDateTime = unixEpochDateTime.AddSeconds((double)exp);
                return expiredAtDateTime;
            }
        }

        public UserIdTokenInfo(string idToken, Dictionary<string, object> decodedPayload)
        {
            RawIdToken = idToken;
            mDecodedPayload = decodedPayload;
        }

        private T GetTokenProperty<T>(Dictionary<string, object> tokenDictionary, string key, T defaultValue = default)
        {
            if (tokenDictionary.TryGetValue(key, out object value))
            {
                return (T)value;
            }
            return defaultValue;
        }
    }
}
