using RGN.Network;
using UnityEngine.Networking;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RGN.Impl.Network.UnityNetwork
{
    public class UnityHttpClient : IHttpClient
    {
        public async Task<IHttpResponse> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
        {
            using UnityWebRequest unityRequest = new UnityWebRequest(requestMessage.RequestUri, requestMessage.Method.ToString().ToUpper());

            if (requestMessage.Method == HttpMethod.Post ||
                requestMessage.Method == HttpMethod.Put ||
                requestMessage.Method == HttpMethod.Patch)
            {
                UploadHandlerRaw unityRequestUploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestMessage.StringBody));
                unityRequestUploadHandler.contentType = "application/json";
                unityRequest.uploadHandler = unityRequestUploadHandler;
            }

            DownloadHandlerBuffer unityRequestDownloadHandlerBuffer = new DownloadHandlerBuffer();
            unityRequest.downloadHandler = unityRequestDownloadHandlerBuffer;
            
            foreach (string key in requestMessage.Headers.Keys!)
            {
                unityRequest.SetRequestHeader(key, requestMessage.Headers[key]);
            }
            
            UnityWebRequestAsyncOperation operation = unityRequest.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
                cancellationToken.ThrowIfCancellationRequested();
            }

            DownloadHandler downloadHandler = unityRequest.downloadHandler;
            return new UnityHttpResponse((int)unityRequest.responseCode, downloadHandler.data, downloadHandler.text);
        }

        public void Dispose()
        {
        }
    }
}
