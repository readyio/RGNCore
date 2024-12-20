using RGN.Network;
using RGN.Impl.Firebase.Network;
using RGN.ImplDependencies.Core.Auth;
using RGN.ImplDependencies.Core.Functions;
using RGN.ImplDependencies.Serialization;
using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
#if READY_DEVELOPMENT && EMULATE_COLDSTART
using System.Diagnostics;
#endif

namespace RGN.Impl.Firebase.Core.FunctionsHttpClient
{
    public sealed class HttpsReference : IHttpsCallableReference
    {
        private const string EMPTY_JSON = "{}";
#if READY_DEVELOPMENT && EMULATE_COLDSTART
        private const int COLD_START_EMULATE_DELAY = 10000;
#endif
        
        private readonly IJson mJson;
        private readonly IAuth mReadyMasterAuth;
        private readonly string mRngMasterProjectId;
        private readonly string mApiKey;
        private readonly string mFunctionName;
        private readonly Uri mCallAddress;
        private readonly bool mActAsACallable;
        private readonly bool mComputeHmac;

        private bool isUnauthenticated;
        private bool isRetryRequest;

        internal HttpsReference(
            IJson json,
            IAuth readyMasterAuth,
            string rngMasterProjectId,
            string apiKey,
            string baseAddress,
            string functionName,
            bool actAsACallable,
            bool computeHmac)
        {
            mJson = json;
            mReadyMasterAuth = readyMasterAuth;
            mRngMasterProjectId = rngMasterProjectId;
            mApiKey = apiKey;
            mFunctionName = functionName;
            mCallAddress = new Uri(new Uri(baseAddress), functionName);
            mActAsACallable = actAsACallable;
            mComputeHmac = computeHmac;
        }

        IHttpsCallableReference IHttpsCallableReference.SetUnauthenticated(bool value)
        {
            isUnauthenticated = value;
            return this;
        }
        Task IHttpsCallableReference.CallAsync(CancellationToken cancellationToken)
        {
            return CallInternalAsync(null, cancellationToken);
        }
        Task IHttpsCallableReference.CallAsync(object data, CancellationToken cancellationToken)
        {
            return CallInternalAsync(data, cancellationToken);
        }
        Task<TResult> IHttpsCallableReference.CallAsync<TPayload, TResult>(CancellationToken cancellationToken)
        {
            return CallInternalAsync<TPayload, TResult>(default, cancellationToken);
        }
        Task<TResult> IHttpsCallableReference.CallAsync<TPayload, TResult>(TPayload payload, CancellationToken cancellationToken)
        {
            return CallInternalAsync<TPayload, TResult>(payload, cancellationToken);
        }

        private async Task CallInternalAsync(object data, CancellationToken cancellationToken = default)
        {
#if READY_DEVELOPMENT
#if EMULATE_COLDSTART
            var callSw = new Stopwatch();
            callSw.Start();
#endif
            UnityEngine.Debug.Log(mCallAddress);   
#endif
            var request = new HttpRequestMessage(HttpMethod.Post, mCallAddress);
            string jsonContent = EMPTY_JSON;
            if (data != null)
            {
                if (data is string dataString)
                {
                    jsonContent = dataString;
                }
                else
                {
                    jsonContent = mJson.ToJson(data);
                }
            }
            string content = jsonContent;
            if (mActAsACallable)
            {
                content = $"{{\"data\": {jsonContent} }}";
            }
            request.SetStringBody(content);
            if (!isUnauthenticated && mReadyMasterAuth.CurrentUser != null)
            {
                string token = await mReadyMasterAuth.CurrentUser.TokenAsync(false, cancellationToken);
                request.AddHeader("Authorization", "Bearer " + token);
            }
            if (mComputeHmac)
            {
                string hmac = ComputeHmac(mApiKey, content);
                request.AddHeader("hmac", hmac);
            }
            if (!string.IsNullOrWhiteSpace(RGNCore.I.AppIDForRequests))
            {
                request.AddHeader("app-id", RGNCore.I.AppIDForRequests);
            }
            using IHttpClient httpClient = HttpClientFactory.Get();
            using IHttpResponse response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                string errorJsonStr = await response.ReadAsString(cancellationToken);
                var errorJsonDict = mJson.FromJson<Dictionary<string, string>>(errorJsonStr);
                string message = errorJsonDict["message"];
                errorJsonDict.TryGetValue("code", out string code);
                string errorMessage = GetErrorMessage(message);
                bool isNotAuthenticatedError = response.StatusCode == 401 || message.Contains("INVALID_ID_TOKEN");
                if (!isRetryRequest && !isUnauthenticated)
                {
                    if (isNotAuthenticatedError && mReadyMasterAuth.CurrentUser != null)
                    {
                        isRetryRequest = true;
                        await mReadyMasterAuth.CurrentUser.TokenAsync(true, cancellationToken);
                        await CallInternalAsync(data, cancellationToken);
                        isRetryRequest = false;
                        return;
                    }
                }
                throw new HttpRequestException(errorMessage, response.StatusCode, code);
            }
            await response.ReadAsString(cancellationToken);
#if READY_DEVELOPMENT && EMULATE_COLDSTART
            await EmulateColdStart(callSw, cancellationToken);
#endif
        }

        private async Task<TResult> CallInternalAsync<TPayload, TResult>(TPayload payload, CancellationToken cancellationToken = default)
        {
#if READY_DEVELOPMENT
#if EMULATE_COLDSTART
            var callSw = new Stopwatch();
            callSw.Start();
#endif
            UnityEngine.Debug.Log(mCallAddress);   
#endif
            var request = new HttpRequestMessage(HttpMethod.Post, mCallAddress);
            string jsonContent = EMPTY_JSON;
            if (payload != null)
            {
                if (payload is string payloadString)
                {
                    jsonContent = payloadString;
                }
                else
                {
                    jsonContent = mJson.ToJson(payload);
                }
            }
            string content = jsonContent;
            if (mActAsACallable)
            {
                content = $"{{\"data\": {jsonContent} }}";
            }
            request.SetStringBody(content);
            if (!isUnauthenticated && mReadyMasterAuth.CurrentUser != null)
            {
                string token = await mReadyMasterAuth.CurrentUser.TokenAsync(false, cancellationToken);
                request.AddHeader("Authorization", "Bearer " + token);
            }
            if (mComputeHmac)
            {
                string hmac = ComputeHmac(mApiKey, content);
                request.AddHeader("hmac", hmac);
            }
            if (!string.IsNullOrWhiteSpace(RGNCore.I.AppIDForRequests))
            {
                request.AddHeader("app-id", RGNCore.I.AppIDForRequests);
            }
            using IHttpClient httpClient = HttpClientFactory.Get();
            using IHttpResponse response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                string errorJsonStr = await response.ReadAsString(cancellationToken);
                var errorJsonDict = mJson.FromJson<Dictionary<string, string>>(errorJsonStr);
                string message = errorJsonDict["message"];
                errorJsonDict.TryGetValue("code", out string code);
                string errorMessage = GetErrorMessage(message);
                bool isNotAuthenticatedError = response.StatusCode == 401 || message.Contains("INVALID_ID_TOKEN");
                if (!isRetryRequest && !isUnauthenticated)
                {
                    if (isNotAuthenticatedError && mReadyMasterAuth.CurrentUser != null)
                    {
                        isRetryRequest = true;
                        await mReadyMasterAuth.CurrentUser.TokenAsync(true, cancellationToken);
                        TResult result = await CallInternalAsync<TPayload, TResult>(payload, cancellationToken);
                        isRetryRequest = false;
                        return result;
                    }
                }
                throw new HttpRequestException(errorMessage, response.StatusCode, code);
            }
            if (typeof(TResult) == typeof(string))
            {
                string result = await response.ReadAsString(cancellationToken);
#if READY_DEVELOPMENT && EMULATE_COLDSTART
                await EmulateColdStart(callSw, cancellationToken);
#endif
                return (TResult)(object)result;
            }
            var stream = await response.ReadAsStream(cancellationToken);
#if READY_DEVELOPMENT && EMULATE_COLDSTART
            await EmulateColdStart(callSw, cancellationToken);
#endif
            if (mActAsACallable)
            {
                var dict = mJson.FromJson<Dictionary<object, TResult>>(stream);
                var result = dict["result"];
                return result;
            }
            return mJson.FromJson<TResult>(stream);
        }

        private string GetErrorMessage(string message)
        {
#if READY_DEVELOPMENT
            string urlToFunctionLog =
                        @$"https://console.cloud.google.com/logs/query;query=resource.type%3D%22cloud_run_revision%22%20resource.labels.service_name%3D%22{mFunctionName}%22?project={mRngMasterProjectId}&authuser=0&hl=en";
            string errorMessage = mFunctionName + ": " + message + ", url: " + urlToFunctionLog;
            return errorMessage;
#else
            return mFunctionName + ": " + message;
#endif
        }

        private string ComputeHmac(string secret, string message)
        {
            var key = Encoding.UTF8.GetBytes(secret);
            using (var hasher = new HMACSHA256(key))
            {
                var messageBytes = Encoding.UTF8.GetBytes(message);
                var hash = hasher.ComputeHash(messageBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

#if READY_DEVELOPMENT && EMULATE_COLDSTART
        private async Task EmulateColdStart(Stopwatch functionStopwatch, CancellationToken cancellationToken = default)
        {
            functionStopwatch.Stop();
            int delayToReachColdStart = Math.Max(0, COLD_START_EMULATE_DELAY - (int)functionStopwatch.ElapsedMilliseconds);
            await Task.Delay(delayToReachColdStart, cancellationToken);
        }
#endif
    }
}
