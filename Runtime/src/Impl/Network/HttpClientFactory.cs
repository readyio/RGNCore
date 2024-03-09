using RGN.Network;
#if UNITY_WEBGL && !UNITY_EDITOR
using RGN.Impl.Network.UnityNetwork;
#else
using RGN.Impl.Network.DotNetNetwork;
#endif

namespace RGN.Impl.Firebase.Network
{
    public static class HttpClientFactory
    {
        private static readonly IHttpClientFactory sImplHttpClientFactory =
#if UNITY_WEBGL && !UNITY_EDITOR
            new UnityHttpClientFactory();
#else
            new DotNetHttpClientFactory();
#endif
        
        public static IHttpClient Get(string name) =>
            sImplHttpClientFactory.Get(name);

        public static IHttpClient Get() =>
            Get(string.Empty);
    }
}
