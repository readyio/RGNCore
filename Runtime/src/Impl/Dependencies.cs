using RGN.DeepLink;
using RGN.Impl.Firebase.Engine;
using RGN.ImplDependencies.Assets;
using RGN.ImplDependencies.Core;
using RGN.ImplDependencies.Core.Auth;
using RGN.ImplDependencies.Core.Functions;
using RGN.ImplDependencies.Core.Messaging;
using RGN.ImplDependencies.DeepLink;
using RGN.ImplDependencies.Engine;
using RGN.ImplDependencies.Serialization;
using RGN.ImplDependencies.WebForm;
using RGN.ModuleDependencies;
using RGN.WebForm;

namespace RGN.Impl.Firebase
{
    public sealed class Dependencies : IDependencies
    {
        public IRGNAnalytics RGNAnalytics { get; }
        public IRGNMessaging RGNMessaging { get; }
        public IRGNGuestSignIn RGNGuestSignIn { get; }
        public IApplicationStore ApplicationStore { get; }
        public IApp App { get; }
        public IAnalytics Analytics { get; }
        public IAuth ReadyMasterAuth { get; }
        public IFunctions ReadyMasterFunction { get; }
        public IMessaging Messaging { get; }
        public IJson Json { get; }
        public IPersistenceData PersistenceData { get; }
        public IEngineApp EngineApp { get; }
        public ITime Time { get; }
        public ILogger Logger { get; }
        public IAssetCache AssetCache { get; }
        public IAssetDownloader AssetDownloader { get; }
        public IDeepLink DeepLink { get; }
        public IWebForm WebForm { get; }

        public Dependencies()
            : this(RGN.ApplicationStore.LoadFromResources())
        {
        }
        public Dependencies(IApplicationStore applicationStore)
        {
            ApplicationStore = applicationStore;
            App = new Core.AppStub();
            Json = new Serialization.Json();
            PersistenceData = new PersistenceData();
            
            var readyMasterAuth = new Core.Auth.Auth();
            ReadyMasterAuth = readyMasterAuth;
            ReadyMasterFunction = new Core.FunctionsHttpClient.Functions(Json, ReadyMasterAuth, ApplicationStore.GetRGNMasterProjectId, applicationStore.GetRGNApiKey);
            
            readyMasterAuth.SetDependencies(ReadyMasterFunction, PersistenceData, Json);
            readyMasterAuth.LoadUserTokens();

            Messaging = new Core.MessagingStub();

            EngineApp = new Engine.EngineApp();
            Time = new Engine.Time();
            Logger = new Engine.Logger();
            Analytics = new Core.AnalyticsStub();
            AssetCache = new Assets.FileAssetsCache();
            AssetDownloader = new Assets.HttpAssetDownloader();
            DeepLink = new RGNDeepLink();
            WebForm = new RGNWebForm();
        }

        public void Init(RGNCore rgnCore)
        {
            DeepLink.Init(rgnCore);
        }

        public void Dispose()
        {
            Analytics.Dispose();
            DeepLink.Dispose();
        }
    }
}
