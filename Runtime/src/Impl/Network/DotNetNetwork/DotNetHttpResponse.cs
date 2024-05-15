using RGN.Network;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RGN.Impl.Network.DotNetNetwork
{
    public class DotNetHttpResponse : IHttpResponse
    {
        private readonly System.Net.Http.HttpResponseMessage mHttpResponseMessage;

        public int StatusCode => (int)mHttpResponseMessage.StatusCode;
        public bool IsSuccessStatusCode => mHttpResponseMessage.IsSuccessStatusCode;

        public DotNetHttpResponse(System.Net.Http.HttpResponseMessage httpResponseMessage)
        {
            mHttpResponseMessage = httpResponseMessage;
        }

        public void EnsureSuccessStatusCode()
        {
            if (!IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Http request exception, statusCode: {StatusCode}", StatusCode);
            }
        }

        public async Task<string> ReadAsString(CancellationToken cancellationToken = default)
        {
            string readAsString = await mHttpResponseMessage.Content.ReadAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();
            return readAsString;
        }

        public async Task<byte[]> ReadAsBytes(CancellationToken cancellationToken = default)
        {
            byte[] readAsBytes = await mHttpResponseMessage.Content.ReadAsByteArrayAsync();
            cancellationToken.ThrowIfCancellationRequested();
            return readAsBytes;
        }

        public async Task<Stream> ReadAsStream(CancellationToken cancellationToken = default)
        {
            Stream readAsStream = await mHttpResponseMessage.Content.ReadAsStreamAsync();
            cancellationToken.ThrowIfCancellationRequested();
            return readAsStream;
        }

        public void Dispose() =>
            mHttpResponseMessage?.Dispose();
    }
}
