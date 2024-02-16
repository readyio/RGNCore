using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace RGN.Impl.Network.DotNetNetwork
{
    public class BaseDotNetHttpClientFactory
    {
        private const int DNS_REFRESH_TIMEOUT = 120;
        private const int CONNECTION_LIMIT = 30;

        private static readonly Dictionary<string, BaseDotNetHttpClientFactory> sFactories =
            new Dictionary<string, BaseDotNetHttpClientFactory> {
                { string.Empty, new BaseDotNetHttpClientFactory() }
            };
        
        private static readonly object sFactoryLock = new object();

        public static HttpClient Get(string name)
        {
            BaseDotNetHttpClientFactory factory;
            lock (sFactoryLock)
            {
                if (!sFactories.TryGetValue(name, out factory))
                {
                    factory = new BaseDotNetHttpClientFactory();
                    sFactories.Add(name, factory);
                }
            }
            return factory.GetNewHttpClient();
        }

        public static HttpClient Get() => Get(string.Empty);

        static BaseDotNetHttpClientFactory() => Configure();

        private static void Configure()
        {
            ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
        }
        
        private readonly Stopwatch mHandlerTimer = new Stopwatch();
        private readonly object mHandlerLock = new object();
        private HttpClientHandler _handler = new HttpClientHandler();
        
        private BaseDotNetHttpClientFactory() { }
        
        private HttpClient GetNewHttpClient() =>
            new HttpClient(GetHandler(), disposeHandler: false);

        private HttpClientHandler GetHandler()
        {
            lock (mHandlerLock)
            {
                if (mHandlerTimer.Elapsed.TotalSeconds > DNS_REFRESH_TIMEOUT)
                {
                    _handler = new HttpClientHandler();
                    mHandlerTimer.Restart();
                }
                return _handler;
            }
        }
    }
}
