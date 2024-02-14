using System.Threading.Tasks;
using RGN.ImplDependencies.Core;

namespace RGN.Impl.Firebase.Core
{
    public sealed class AnalyticsStub : IAnalytics
    {
        public Task<string> GetAnalyticsInstanceIdAsync()
        {
            return Task.FromResult(string.Empty);
        }
        public void Init()
        {
        }
        public void Dispose()
        {
        }
        public void DisableUserTracking()
        {
        }
        public void LogEvent(string name)
        {
        }
        public void LogEvent(string name, string parameterName, long parameterValue)
        {
        }
        public void LogEvent(string name, string parameterName, string parameterValue)
        {
        }
        public void LogEvent(string name, string parameterName, float parameterValue)
        {
        }
        public void LogEvent(
            string name,
            string parameterNameOne,
            string parameterValueOne,

            string parameterNameTwo,
            long parameterValueTwo)
        {
        }
        public void LogEvent(
            string name,
            string parameterNameOne,
            string parameterValueOne,

            string parameterNameTwo,
            string parameterValueTwo)
        {
        }
        public void LogEvent(
            string name,
            string parameterNameOne,
            string parameterValueOne,

            string parameterNameTwo,
            float parameterValueTwo)
        {
        }
        public void LogEvent(
            string name,
            string parameterNameOne,
            string parameterValueOne,

            string parameterNameTwo,
            double parameterValueTwo)
        {
        }
        public void LogEvent(
            string name,
            params AnalyticsParameter[] analyticParameters)
        {
        }
        public void LogExceptionEvent(string exceptionMessage)
        {
        }

        public void SetCurrentScreen(string screenName, string screenClass)
        {
        }
        public void SetUserId(string userId)
        {
        }
        public void TutorialBegin()
        {
        }
        public void TutorialComplete()
        {
        }
        public void Login()
        {
        }

        private void AddNewParameterToParametersArguments(AnalyticsParameter param)
        {
        }
    }
}
