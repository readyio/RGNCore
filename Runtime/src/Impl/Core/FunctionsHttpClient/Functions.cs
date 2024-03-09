using RGN.ImplDependencies.Core.Auth;
using RGN.ImplDependencies.Core.Functions;
using RGN.ImplDependencies.Serialization;

namespace RGN.Impl.Firebase.Core.FunctionsHttpClient
{
    public sealed class Functions : IFunctions
    {
        private const string REGION = "us-central1";

        private readonly IJson mJson;
        private readonly IAuth mReadyMasterAuth;
        private readonly string mRngMasterProjectId;
        private readonly string mApiKey;
        private string _baseCloudAddress;

        internal Functions(
            IJson json,
            IAuth readyMasterAuth,
            string rngMasterProjectId,
            string apiKey)
        {
            mJson = json;
            mReadyMasterAuth = readyMasterAuth;
            mRngMasterProjectId = rngMasterProjectId;
            mApiKey = apiKey;
            _baseCloudAddress = $"https://{REGION}-{mRngMasterProjectId}.cloudfunctions.net/";
        }

        IHttpsCallableReference IFunctions.GetHttpsCallable(string name, bool computeHmac)
        {
            return new HttpsReference(
                mJson,
                mReadyMasterAuth,
                mRngMasterProjectId,
                mApiKey,
                _baseCloudAddress,
                name,
                true,
                computeHmac);
        }
        IHttpsCallableReference IFunctions.GetHttpsRequest(string name, bool computeHmac)
        {
            return new HttpsReference(
                mJson,
                mReadyMasterAuth,
                mRngMasterProjectId,
                mApiKey,
                _baseCloudAddress,
                name,
                false,
                computeHmac);
        }

        void IFunctions.UseFunctionsEmulator(string hostAndPort)
        {
            _baseCloudAddress = $"http://{hostAndPort}/{mRngMasterProjectId}/{REGION}/";
        }
    }
}
