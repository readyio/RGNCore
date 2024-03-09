using RGN.Network;

namespace RGN.Impl.Network.DotNetNetwork
{
    public class DotNetHttpClientFactory : IHttpClientFactory
    {
        public IHttpClient Get(string name) =>
            new DotNetHttpClient(BaseDotNetHttpClientFactory.Get(name));

        public IHttpClient Get() =>
            Get(string.Empty);
    }
}
