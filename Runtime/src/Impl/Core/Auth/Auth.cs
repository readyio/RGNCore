using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RGN.ImplDependencies.Core.Auth;
using RGN.ImplDependencies.Core.Functions;
using RGN.ImplDependencies.Serialization;
using UnityEngine;

namespace RGN.Impl.Firebase.Core.Auth
{
    internal sealed class Auth : IAuth
    {
        private readonly Dictionary<string, UserTokensPair> mUserTokensCache = new Dictionary<string, UserTokensPair>();
        private readonly HashSet<EventHandler> mListeners = new HashSet<EventHandler>();
        
        private IFunctions _functions;
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
        
        internal void SetJson(IJson json)
        {
            _json = json;
        }
        
        internal void SetFunctions(IFunctions functions)
        {
            _functions = functions;
        }

        internal void LoadUserTokensFromPlayerPrefs()
        {
            string idToken = PlayerPrefs.GetString(AuthTokenKeys.IdToken.GetKeyName());
            string refreshToken = PlayerPrefs.GetString(AuthTokenKeys.RefreshToken.GetKeyName());

            if (!string.IsNullOrEmpty(idToken) && !string.IsNullOrEmpty(refreshToken))
            {
                SetUserTokens(idToken, refreshToken);
            }
        }

        internal void SaveUserTokensToPlayerPrefs()
        {
            if (_currentUser != null)
            {
                PlayerPrefs.SetString(AuthTokenKeys.IdToken.GetKeyName(), _currentUser.IdToken);
                PlayerPrefs.SetString(AuthTokenKeys.RefreshToken.GetKeyName(), _currentUser.RefreshToken);
                PlayerPrefs.Save();
            }
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
            
            SaveUserTokensToPlayerPrefs();

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
                mUserTokensCache.Add(email, userTokensPair);
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
