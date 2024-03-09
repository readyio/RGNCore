using System;
using System.Reflection;

namespace RGN.Impl.Firebase.Engine
{
    public class FirebaseVersionRetriever
    {
        private static string sCachedValue = null;
        public static string GetFirebaseSdkVersion()
        {
            if (sCachedValue != null)
            {
                return sCachedValue;
            }
            Type versionInfoType = Type.GetType("Firebase.VersionInfo, Firebase.App");

            if (versionInfoType != null)
            {
                PropertyInfo sdkVersionProperty =
                    versionInfoType.GetProperty("SdkVersion", BindingFlags.NonPublic | BindingFlags.Static);
                if (sdkVersionProperty != null)
                {
                    sCachedValue = sdkVersionProperty.GetValue(null) as string;
                    return sCachedValue;
                }
            }
            return "unknown";
        }
    }
}
