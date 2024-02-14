using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace RGN.Impl.Firebase
{
    public class RGNUnityInitializer : MonoSingleton<RGNUnityInitializer>
    {
        [SerializeField] private bool _autoGuestLogin = true;

        protected override async void OnAwakeInternal()
        {
            await InitializeAsync();
        }
        protected override void OnDestroyInternal()
        {
            Dispose(true);
        }

        protected virtual async Task InitializeAsync()
        {
            if (RGNCoreBuilder.Initialized)
            {
                return;
            }
            RGNCoreBuilder.CreateInstance(new Dependencies());
            RGNCore.I.AuthenticationChanged += OnAuthenticationChanged;
            await RGNCoreBuilder.BuildAsync();
        }
        protected virtual void Dispose(bool disposing)
        {
            RGNCoreBuilder.Dispose();
        }

        private void OnAuthenticationChanged(AuthState authState)
        {
            if (_autoGuestLogin && authState.LoginState == EnumLoginState.NotLoggedIn)
            {
                StartCoroutine(CallTryToLoginAfterAFrame());
            }
        }
        private IEnumerator CallTryToLoginAfterAFrame()
        {
            yield return null;
            if (RGNCoreBuilder.I.Dependencies.RGNGuestSignIn == null)
            {
                RGNCoreBuilder.I.Dependencies.Logger.Log("The RGNGuestSignIn is not installed, skipping auto guest login.");
                yield break;
            }
            Debug.Log("Automatically logging in as a guest");
            RGNCoreBuilder.I.Dependencies.RGNGuestSignIn.TryToSignInAsync();
        }
    }
}
