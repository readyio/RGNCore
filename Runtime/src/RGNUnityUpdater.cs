using UnityEngine;

namespace RGN
{
    public class RGNUnityUpdater : MonoBehaviour, IRGNUpdater
    {
        private RGNCore moduleCore = null;

        public void BindModuleCore(RGNCore moduleCore)
        {
            this.moduleCore = moduleCore;
        }

        private void Update()
        {
            moduleCore?.OnUnityUpdate();
        }
    }
}