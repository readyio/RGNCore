using System.Net;
using System.Net.Sockets;

namespace RGN.DeepLink
{
    public static class RGNDeepLinkHttpUtility
    {
        public static string GetDeepLinkRedirectScheme()
        {
#if UNITY_EDITOR
            return GetDeepLinkRedirectSchemeForEditor();
#else
            return GetDeepLinkRedirectSchemeForBuild();
#endif
        }
        
        public static string GetDeepLinkRedirectSchemeForEditor()
        {
            return $"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";
        }
        
        public static string GetDeepLinkRedirectSchemeForBuild()
        {
            ApplicationStore applicationStore = ApplicationStore.LoadFromResources();
            string projectId = "rgn" + applicationStore.GetRGNProjectId;
            return projectId.
                ToLower().
                Replace(".", string.Empty).
                Replace("-", string.Empty).
                Replace("_", string.Empty);
        }
        
        private static int GetRandomUnusedPort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
