using System.IO;
using System.Threading.Tasks;
using RGN.Network;

namespace RGN.Impl.Network.UnityNetwork
{
    public class UnityHttpResponse : IHttpResponse
    {
        private readonly int mStatusCode;
        private readonly byte[] mData;
        private readonly string mText;

        public int StatusCode => mStatusCode;
        public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode < 300;

        public UnityHttpResponse(int statusCode, byte[] data, string text)
        {
            mStatusCode = statusCode;
            mData = data;
            mText = text;
        }
        
        public void EnsureSuccessStatusCode()
        {
            if (!IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Http request exception, statusCode: {StatusCode}", StatusCode);
            }
        }

        public Task<string> ReadAsString() =>
            Task.FromResult(mText);

        public Task<byte[]> ReadAsBytes() =>
            Task.FromResult(mData);

        public Task<Stream> ReadAsStream()
        {
            Stream stream = new MemoryStream(mData);
            return Task.FromResult(stream);
        }

        public void Dispose()
        {
        }
    }
}
