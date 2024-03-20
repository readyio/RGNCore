using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RGN.ImplDependencies.Core.Auth;
using RGN.ImplDependencies.Core.Functions;
using RGN.ImplDependencies.Engine;
using RGN.ImplDependencies.Serialization;

namespace RGN.Impl.Firebase.Core.Auth
{
    internal sealed class Auth : IAuth
    {
        private readonly Dictionary<string, UserTokensPair> mUserTokensCache = new Dictionary<string, UserTokensPair>();
        private readonly HashSet<EventHandler> mListeners = new HashSet<EventHandler>();
        
        private IFunctions _functions;
        private IPersistenceData _persistenceData;
        private IJson _json;
        private IUser _currentUser;

        public IUser CurrentUser => _currentUser;

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
            string idToken = _currentUser != null ? _currentUser.IdToken : string.Empty;
            string refreshToken = _currentUser != null ? _currentUser.RefreshToken : string.Empty;
            _persistenceData.SaveFile(AuthTokenKeys.IdToken.GetKeyName(), idToken);
            _persistenceData.SaveFile(AuthTokenKeys.RefreshToken.GetKeyName(), refreshToken);
        }

        public IUser SetUserTokens(string idToken, string refreshToken)
        {
            if (!string.IsNullOrEmpty(idToken))
            {
                _currentUser = new User(this, _json, idToken, refreshToken);
            }
            else
            {
                _currentUser = null;
            }
            
            SaveUserTokens();

            foreach (EventHandler listener in mListeners)
            {
                listener.Invoke(this, null);
            }

            return _currentUser;
        }

        public async Task<IUser> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            if (!mUserTokensCache.TryGetValue(email, out UserTokensPair userTokensPair))
            {
                userTokensPair = await SignInWithEmailAndPasswordHttpAsync(email, password);
                mUserTokensCache[email] = userTokensPair;
            }
            return SetUserTokens(userTokensPair.IdToken, userTokensPair.RefreshToken);
        }

        public async Task<IUser> SignInAnonymouslyAsync()
        {
            UserTokensPair userTokensPair = await SignInAnonymouslyHttpAsync();
            return SetUserTokens(userTokensPair.IdToken, userTokensPair.RefreshToken);
        }

        public async Task<IUserTokensPair> RefreshTokensAsync(string refreshToken)
        {
            return await RefreshTokensHttpAsync(refreshToken);
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            await SendPasswordResetEmailHttpAsync(email);
        }

        public void SignOut()
        {
            SetUserTokens(string.Empty, string.Empty);
        }

        private async Task<UserTokensPair> SignInWithEmailAndPasswordHttpAsync(string email, string password)
        {
            var function = _functions.GetHttpsCallable("user-signInWithEmailPassword");
            var response = await function.CallAsync<Dictionary<string, object>, Dictionary<string, object>>(
                new Dictionary<string, object> {
                    { "email", email },
                    { "password", password },
                    { "returnSecureToken", true }
                }
            );
            string idToken = (string)response["idToken"];
            string refreshToken = (string)response["refreshToken"];
            return new UserTokensPair(idToken, refreshToken);
        }
        
        private async Task<UserTokensPair> SignInAnonymouslyHttpAsync()
        {
            var function = _functions.GetHttpsCallable("user-signUpAnonymously");
            function.SetUnauthenticated(true);
            var response = await function.CallAsync<Dictionary<string, object>, Dictionary<string, object>>();
            string idToken = (string)response["idToken"];
            string refreshToken = (string)response["refreshToken"];
            return new UserTokensPair(idToken, refreshToken);
        }

        private async Task<UserTokensPair> RefreshTokensHttpAsync(string oldRefreshToken)
        {
            var function = _functions.GetHttpsCallable("user-refreshTokens");
            function.SetUnauthenticated(true);
            var response = await function.CallAsync<Dictionary<string, object>, Dictionary<string, object>>(
                new Dictionary<string, object> {
                    { "refreshToken", oldRefreshToken }
                }
            );
            string idToken = (string)response["idToken"];
            string refreshToken = (string)response["refreshToken"];
            return new UserTokensPair(idToken, refreshToken);
        }

        private async Task SendPasswordResetEmailHttpAsync(string email)
        {
            var function = _functions.GetHttpsCallable("user-resetAccountPassword");
            function.SetUnauthenticated(true);
            await function.CallAsync<Dictionary<string, object>, Dictionary<string, object>>(
                new Dictionary<string, object> {
                    { "email", email }
                }
            );
        }
    }
}
