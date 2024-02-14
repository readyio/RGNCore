using RGN.ImplDependencies.Core.Auth;

namespace RGN.Impl.Firebase.Core.Auth
{
    public struct UserTokensPair : IUserTokensPair
    {
        public string IdToken { get; }
        public string RefreshToken { get; }

        public UserTokensPair(string idToken, string refreshToken)
        {
            IdToken = idToken;
            RefreshToken = refreshToken;
        }
    }
}
