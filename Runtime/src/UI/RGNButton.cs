using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RGN.UI
{
    [RequireComponent(typeof(Button), typeof(RectTransform))]
    public sealed class RGNButton : MonoBehaviour, System.IDisposable
    {
        public RectTransform RectTransform { get => _rectTransform; }
        public Button Button { get => _button; }
        public TextMeshProUGUI ButtonText { get => _buttonText; }

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _buttonText;

        public void Init()
        {

        }
        public void Dispose()
        {
            Destroy(gameObject);
        }
        public float GetHeight()
        {
            return RectTransform.sizeDelta.y;
        }
        public void SetHeight(float height)
        {
            RectTransform.sizeDelta = new Vector2(
                RectTransform.sizeDelta.x,
                height);
        }
    }
}
