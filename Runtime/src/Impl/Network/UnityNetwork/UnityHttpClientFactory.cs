using RGN.Network;

namespace RGN.Impl.Network.UnityNetwork
{
    public class UnityHttpClientFactory : IHttpClientFactory
    {
        public IHttpClient Get(string name) =>
            new UnityHttpClient();

        public IHttpClient Get() =>
            Get(string.Empty);
    }
}
