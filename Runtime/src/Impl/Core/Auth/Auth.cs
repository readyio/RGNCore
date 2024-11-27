using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RGN.ImplDependencies.Core.Auth;
using RGN.ImplDependencies.Core.Functions;
using RGN.ImplDependencies.Engine;
using RGN.ImplDependencies.Serialization;
using RGN.Jwt;

namespace RGN.Impl.Firebase.Core.Auth
{
    internal sealed class Auth : IAuth
    {
        private readonly Dictionary<string, UserTokensPair> mUserTokensCache = new Dictionary<string, UserTokensPair>();
        private readonly HashSet<EventHandler> mListeners = new HashSet<EventHandler>();
        
        private IFunctions _functions;
        private IPersistenceData _persistenceData;
        private IJson _json;
        private TaskCompletionSource<UserTokensPair> _refreshTokensTask;

        public IUser CurrentUser { get; private set; }

        event EventHandler IAuth.StateChanged
        {
            add
            {
                mListeners.Add(value);
                value?.Invoke(this, null);
            }
            remove => mListeners.Remove(value);
        }
        
        internal void SetDependencies(IFunctions functions, IPersistenceData persistenceData, IJson json)
        {
            _functions = functions;
            _persistenceData = persistenceData;
            _json = json;
        }

        internal void LoadUserTokens()
        {
            string idToken = _persistenceData.LoadFile(AuthTokenKeys.IdToken.GetKeyName());
            string refreshToken = _persistenceData.LoadFile(AuthTokenKeys.RefreshToken.GetKeyName());
            SetUserTokens(idToken, refreshToken);
        }

        internal void SaveUserTokens()
        {
            string idToken = CurrentUser != null ? CurrentUser.IdToken : string.Empty;
            string refreshToken = CurrentUser != null ? CurrentUser.RefreshToken : string.Empty;
            _persistenceData.SaveFile(AuthTokenKeys.IdToken.GetKeyName(), idToken);
            _persistenceData.SaveFile(AuthTokenKeys.RefreshToken.GetKeyName(), refreshToken);
        }

        public IUser SetUserTokens(string idToken, string refreshToken)
        {
            if (!string.IsNullOrEmpty(idToken) && !string.IsNullOrEmpty(refreshToken))
            {
                CurrentUser = JwtDecoder.IsValid(idToken) ? new User(this, _json, idToken, refreshToken) : null;
            }
            else
            {
                CurrentUser = null;
            }
            
            SaveUserTokens();

            foreach (EventHandler listener in mListeners)
            {
                listener.Invoke(this, null);
            }

            return CurrentUser;
        }

        public async Task<IUser> SignInWithEmailAndPasswordAsync(string email, string password,
            CancellationToken cancellationToken = default)
        {
            if (mUserTokensCache.TryGetValue(email, out UserTokensPair userTokensInCache))
            {
                return SetUserTokens(userTokensInCache.IdToken, userTokensInCache.RefreshToken);
            }
            Dictionary<string, object> functionBody = new Dictionary<string, object> {
                { "email", email },
                { "password", password },
                { "returnSecureToken", true }
            };
            IHttpsCallableReference functionRef = _functions
                .GetHttpsCallable("user-signInWithEmailPassword")
                .SetUnauthenticated(true);
            Dictionary<string, object> functionResponse = await functionRef
                .CallAsync<Dictionary<string, object>, Dictionary<string, object>>(functionBody, cancellationToken);
            string idToken = (string)functionResponse["idToken"];
            string refreshToken = (string)functionResponse["refreshToken"];
            UserTokensPair userTokens = new UserTokensPair(idToken, refreshToken);
            mUserTokensCache[email] = userTokens;
            return SetUserTokens(userTokens.IdToken, userTokens.RefreshToken);
        }

        public async Task<IUser> SignInAnonymouslyAsync(CancellationToken cancellationToken = default)
        {
            IHttpsCallableReference functionRef = _functions
                .GetHttpsCallable("user-signUpAnonymously")
                .SetUnauthenticated(true);
            Dictionary<string, object> functionResponse = await functionRef
                .CallAsync<Dictionary<string, object>, Dictionary<string, object>>(cancellationToken);
            string idToken = (string)functionResponse["idToken"];
            string refreshToken = (string)functionResponse["refreshToken"];
            UserTokensPair userTokens = new UserTokensPair(idToken, refreshToken);
            return SetUserTokens(userTokens.IdToken, userTokens.RefreshToken);
        }

        public async Task<IUserTokensPair> RefreshTokensAsync(string refreshToken,
            CancellationToken cancellationToken = default)
        {
            if (_refreshTokensTask != null)
            {
                return await _refreshTokensTask.Task;
            }
            _refreshTokensTask = new TaskCompletionSource<UserTokensPair>();
            try
            {
                Dictionary<string, object> functionBody = new Dictionary<string, object> {
                    { "refreshToken", refreshToken },
                };
                IHttpsCallableReference functionRef = _functions
                    .GetHttpsCallable("user-refreshTokens")
                    .SetUnauthenticated(true);
                Dictionary<string, object> functionResponse = await functionRef
                    .CallAsync<Dictionary<string, object>, Dictionary<string, object>>(functionBody, cancellationToken);
                string newIdToken = (string)functionResponse["idToken"];
                string newRefreshToken = (string)functionResponse["refreshToken"];
                UserTokensPair tokens = new UserTokensPair(newIdToken, newRefreshToken);
                _refreshTokensTask.SetResult(tokens);
                return tokens;
            }
            catch (Exception ex)
            {
                _refreshTokensTask.SetException(ex);
                throw;
            }
            finally
            {
                _refreshTokensTask = null;
            }
        }

        public async Task SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            Dictionary<string, object> functionBody = new Dictionary<string, object> {
                { "email", email },
            };
            IHttpsCallableReference functionRef = _functions
                .GetHttpsCallable("user-resetAccountPassword")
                .SetUnauthenticated(true);
            await functionRef
                .CallAsync<Dictionary<string, object>, Dictionary<string, object>>(functionBody, cancellationToken);
        }

        public async Task<RequestDeviceCodeResponse> RequestOAuthDeviceCodeAsync(CancellationToken cancellationToken = default)
        {
            IHttpsCallableReference functionRef = _functions
                .GetHttpsCallable("user-device-requestAuthentication")
                .SetUnauthenticated(true);
            Dictionary<string, object> functionResponse = await functionRef
                .CallAsync<Dictionary<string, object>, Dictionary<string, object>>(cancellationToken);
            string deviceCode = (string)functionResponse["deviceId"];
            long expireAt = (long)functionResponse["expireAt"];
            return new RequestDeviceCodeResponse { deviceCode = deviceCode, expireAt = expireAt };
        }

        public async Task<PollTokenWithDeviceCodeResponse> PollTokenWithDeviceCodeAsync(string deviceCode,
            CancellationToken cancellationToken = default)
        {
            Dictionary<string, object> functionBody = new Dictionary<string, object> {
                { "deviceId", deviceCode },
            };
            IHttpsCallableReference functionRef = _functions
                .GetHttpsCallable("user-device-token")
                .SetUnauthenticated(true);
            return await functionRef
                .CallAsync<Dictionary<string, object>, PollTokenWithDeviceCodeResponse>(functionBody, cancellationToken);
        }

        public void SignOut() =>
            SetUserTokens(string.Empty, string.Empty);
    }
}
