using RGN.ImplDependencies.Core;
using System.Threading.Tasks;
using DependencyStatus = RGN.ImplDependencies.Core.DependencyStatus;

namespace RGN.Impl.Firebase.Core
{
    public sealed class AppStub : IApp
    {
        async Task<DependencyStatus> IApp.CheckAndFixDependenciesAsync() => await Task.FromResult(DependencyStatus.Available);
        public string GetFirebaseSdkVersion() => "unknown";
    }
}
