using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RGN.ImplDependencies.Core.Auth;
using RGN.ImplDependencies.Serialization;
using RGN.Jwt;
using RGN.Network;

namespace RGN.Impl.Firebase.Core.Auth
{
    internal sealed class User : IUser
    {
        private readonly Auth mAuth;
        private readonly IJson mJson;
        private UserIdTokenInfo _userIdTokenInfo;

        public string IdToken { get; private set; }
        public string RefreshToken { get; private set; }
        public string UserId => _userIdTokenInfo.UserId;
        public string Email => _userIdTokenInfo.Email;
        public bool EmailVerified => _userIdTokenInfo.EmailVerified ?? false;
        public string DisplayName => _userIdTokenInfo.Name;
        public bool IsAnonymous => _userIdTokenInfo.IsAnonymous;

        internal User(Auth auth, IJson json, string idToken, string refreshToken)
        {
            mAuth = auth;
            mJson = json;
            IdToken = idToken;
            RefreshToken = refreshToken;
            _userIdTokenInfo = CreateTokenInfo(idToken);
        }

        public async Task<string> TokenAsync(bool forceRefresh, CancellationToken cancellationToken = default)
        {
            if (forceRefresh || _userIdTokenInfo.ExpiredAt < DateTime.UtcNow)
            {
                try
                {
                    IUserTokensPair tokensPair = await mAuth.RefreshTokensAsync(RefreshToken, cancellationToken);
                    IdToken = tokensPair.IdToken;
                    RefreshToken = tokensPair.RefreshToken;
                    _userIdTokenInfo = CreateTokenInfo(tokensPair.IdToken);
                    mAuth.SaveUserTokens();
                }
                catch (HttpRequestException httpException)
                {
                    if (httpException.StatusCode == 400)
                    {
                        mAuth.SignOut();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return IdToken;
        }

        private UserIdTokenInfo CreateTokenInfo(string token)
        {
            JwtDecoder.JwtDecodeResult decodedToken = JwtDecoder.Decode(token);
            Dictionary<string, object> decodedPayload = mJson.FromJson<Dictionary<string, object>>(decodedToken.mPayload);
            return new UserIdTokenInfo(token, decodedPayload);
        }
    }
}
