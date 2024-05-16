using System.Runtime.InteropServices;
using UnityEngine;

public static class AppInfoPluginWrapper
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern System.IntPtr getInstallerNameForRGN();
#endif
    
    public static string GetInstallerName()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return GetIOSInstallerNameNative();
#elif UNITY_ANDROID && !UNITY_EDITOR
        return GetAndroidInstallerNameNative();
#endif
        return "";
    }
    
#if UNITY_IOS && !UNITY_EDITOR
    private static string GetIOSInstallerNameNative()
    {
        try
        {
            return Marshal.PtrToStringAuto(getInstallerNameForRGN());
        }
        catch (System.Exception exception)
        {
            Debug.LogError("Failed to get iOS installer, exception: " + exception);
            return Application.installMode == ApplicationInstallMode.Store ? "Apple App Store" : "";
        }
    }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
    private static string GetAndroidInstallerNameNative()
    {
        string installerPackageName;
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass appInfoClass = new AndroidJavaClass("io.getready.appinfo.plugin.AppInfo");
            installerPackageName = appInfoClass.CallStatic<string>("getInstallerName", currentActivity);
        }
        catch (System.Exception exception)
        {
            Debug.LogError("Failed to get android installer, exception: " + exception);
            installerPackageName = Application.installerName;
        }
        return GetAndroidStoreName(installerPackageName);
    }

    /**
     * Converts a android package name to a human-readable store name.
     * @param packageName The android package name of the installer.
     * @return The name of the store.
     */
    private static string GetAndroidStoreName(string packageName)
    {
        switch (packageName)
        {
            case "com.android.vending": return "Google Play Store";
            case "com.sec.android.app.samsungapps": return "Samsung Galaxy Store";
            case "com.amazon.venezia": return "Amazon Appstore";
            case "com.huawei.appmarket": return "Huawei AppGallery";
            case "com.xiaomi.market": return "Xiaomi GetApps";
            case "com.oppo.market": return "Oppo App Market";
            case "com.bbk.appstore": return "Vivo App Store";
            case "com.oneplus.store": return "OnePlus Store";
            default: return "";
        }
    }
#endif
}
