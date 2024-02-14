using System.Net;
using System.Net.Http;

namespace RGN.Impl.Firebase.Core.FunctionsHttpClient
{
    public class HttpRequestExceptionWithStatusCode : HttpRequestException
    {
        public HttpStatusCode StatusCode { get; }

        public HttpRequestExceptionWithStatusCode(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
