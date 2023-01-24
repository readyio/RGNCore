using UnityEditor;

namespace RGN.MyEditor
{
    internal sealed class CredentialsPostprocessor : AssetPostprocessor
    {
#if UNITY_2021_2_OR_NEWER
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload)
#else
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
#endif
        {
            for (int i = 0; i < importedAssets.Length; i++)
            {
                string asset = importedAssets[i];
                if (asset.Contains("StagingBuildCredentials"))
                {
                    UnityEngine.Debug.Log("Trying to set the staging credentials automatically...");
                    CredentialsSetup.SetStagingEnv();
                    return;
                }
                if (asset.Contains("ProductionBuildCredentials"))
                {
                    UnityEngine.Debug.Log("Trying to set the production credentials automatically...");
                    CredentialsSetup.SetProductionEnv();
                    return;
                }
            }
        }
    }
}
