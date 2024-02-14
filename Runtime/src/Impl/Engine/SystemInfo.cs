using RGN.ImplDependencies.Engine;

namespace RGN.Impl.Firebase.Engine
{
    public sealed class SystemInfo : ISystemInfo
    {
        public string ProcessorType => UnityEngine.SystemInfo.processorType;
        public int ProcessorCount => UnityEngine.SystemInfo.processorCount;
        public int SystemMemorySize => UnityEngine.SystemInfo.systemMemorySize;
        public string GraphicsDeviceName => UnityEngine.SystemInfo.graphicsDeviceName;
        public int GraphicsMemorySize => UnityEngine.SystemInfo.graphicsMemorySize;
        public string OperatingSystem => UnityEngine.SystemInfo.operatingSystem;
        public string OperatingSystemFamily => UnityEngine.SystemInfo.operatingSystemFamily.ToString();
        public string UnityVersion => UnityEngine.Application.unityVersion;
        public string FirebaseSDKVersion => FirebaseVersionRetriever.GetFirebaseSdkVersion();
        public string RGNSDKVersion => SDKVersion.Version;
    }
}
