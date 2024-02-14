using RGN.ImplDependencies.Engine;
using System;

namespace RGN.Impl.Firebase.Engine
{
    public sealed class Logger : ILogger
    {
        void ILogger.Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        void ILogger.LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        void ILogger.LogException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }

        void ILogger.LogFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(format, args);
        }

        void ILogger.LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
    }
}
