using System.Threading;
using RGN.ImplDependencies.Core;
using System.Threading.Tasks;
using DependencyStatus = RGN.ImplDependencies.Core.DependencyStatus;

namespace RGN.Impl.Firebase.Core
{
    public sealed class AppStub : IApp, IImplStub
    {
        Task<DependencyStatus> IApp.CheckAndFixDependenciesAsync(CancellationToken cancellationToken) => 
            Task.FromResult(DependencyStatus.Available);
        public string GetFirebaseSdkVersion() => "unknown";
    }
}
