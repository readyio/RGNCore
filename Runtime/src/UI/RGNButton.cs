using UnityEngine;
using UnityEngine.UI;

namespace RGN.UI
{
    [RequireComponent(typeof(Button), typeof(RectTransform))]
    public sealed class RGNButton : MonoBehaviour
    {
        public RectTransform RectTransform { get; private set; }
        public Button Button { get; private set; }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            Button = GetComponent<Button>();
        }

        public float GetHeight()
        {
            return RectTransform.sizeDelta.y;
        }
    }
}
