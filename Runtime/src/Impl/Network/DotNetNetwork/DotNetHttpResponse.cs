using RGN.Network;
using System.IO;
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

        public Task<string> ReadAsString() =>
            mHttpResponseMessage.Content.ReadAsStringAsync();

        public Task<byte[]> ReadAsBytes() =>
            mHttpResponseMessage.Content.ReadAsByteArrayAsync();

        public Task<Stream> ReadAsStream() =>
            mHttpResponseMessage.Content.ReadAsStreamAsync();

        public void Dispose() =>
            mHttpResponseMessage?.Dispose();
    }
}
