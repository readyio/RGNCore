using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RGN.Impl.Firebase.Network;
using RGN.ImplDependencies.Assets;
using RGN.Network;

namespace RGN.Impl.Firebase.Assets
{
    public class HttpAssetDownloader : IAssetDownloader
    {
        public async Task<byte[]> DownloadAsync(AssetCategory category, string url, bool bypassCache, CancellationToken cancellationToken = default)
        {
            IAssetCache assetCache = RGNCore.I.Dependencies.AssetCache;
            if (!bypassCache && assetCache.TryReadFromCache(category, url, out byte[] cacheData))
            {
                return cacheData;
            }
            byte[] data = await DownloadNoCacheAsync(url, cancellationToken);
            string fileName = Path.GetFileName(new Uri(url).LocalPath);
            assetCache.WriteToCache(category, fileName, data);
            return data;
        }

        public async Task<byte[]> DownloadNoCacheAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                using IHttpClient httpClient = HttpClientFactory.Get("assets");
                using IHttpResponse result = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(url)), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                byte[] bytes = await result.ReadAsBytes();
                cancellationToken.ThrowIfCancellationRequested();
                return bytes;
            }
            catch (HttpRequestException ex)
            {
                RGNCore.I.Dependencies.Logger.Log($"AssetDownloader download exception, url: {url}, message: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                RGNCore.I.Dependencies.Logger.LogException(ex);
                return null;
            }
        }
    }
}
