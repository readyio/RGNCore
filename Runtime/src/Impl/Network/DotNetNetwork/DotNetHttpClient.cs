using RGN.Network;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttpMethod = RGN.Network.HttpMethod;
using HttpRequestMessage = RGN.Network.HttpRequestMessage;

namespace RGN.Impl.Network.DotNetNetwork
{
    public class DotNetHttpClient : IHttpClient
    {
        private readonly System.Net.Http.HttpClient mDotNetHttpClient;
        
        public DotNetHttpClient(System.Net.Http.HttpClient dotNetHttpClient) =>
            mDotNetHttpClient = dotNetHttpClient;

        public async Task<IHttpResponse> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
        {
            System.Net.Http.HttpRequestMessage dotNetHttpRequestMessage =
                new System.Net.Http.HttpRequestMessage(ConvertHttpMethod2DotNetHttpMethod(requestMessage.Method), requestMessage.RequestUri);
            foreach (string headersKey in requestMessage.Headers.Keys!)
            {
                dotNetHttpRequestMessage.Headers.TryAddWithoutValidation(headersKey, requestMessage.Headers[headersKey]);
            }
            if (requestMessage.Method == HttpMethod.Post ||
                requestMessage.Method == HttpMethod.Put ||
                requestMessage.Method == HttpMethod.Patch)
            {
                dotNetHttpRequestMessage.Content = new System.Net.Http.StringContent(requestMessage.StringBody, Encoding.UTF8, "application/json");
            }
            System.Net.Http.HttpResponseMessage dotNetHttpResponse =
                await mDotNetHttpClient.SendAsync(dotNetHttpRequestMessage, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            return new DotNetHttpResponse(dotNetHttpResponse);
        }

        private System.Net.Http.HttpMethod ConvertHttpMethod2DotNetHttpMethod(HttpMethod httpMethod) =>
            httpMethod switch {
                HttpMethod.Get => System.Net.Http.HttpMethod.Get,
                HttpMethod.Head => System.Net.Http.HttpMethod.Head,
                HttpMethod.Post => System.Net.Http.HttpMethod.Post,
                HttpMethod.Put => System.Net.Http.HttpMethod.Put,
                HttpMethod.Delete => System.Net.Http.HttpMethod.Delete,
                HttpMethod.Connect => new System.Net.Http.HttpMethod("CONNECT"),
                HttpMethod.Options => System.Net.Http.HttpMethod.Options,
                HttpMethod.Trace => System.Net.Http.HttpMethod.Trace,
                HttpMethod.Patch => new System.Net.Http.HttpMethod("PATCH"),
                _ => throw new NotImplementedException(nameof(httpMethod))
            };

        public void Dispose() =>
            mDotNetHttpClient?.Dispose();
    }
}
