using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RGN.UI
{
    public sealed class ToastMessage : MonoSingleton<ToastMessage>, IPointerClickHandler
    {
        [Header("Settings")]
        [SerializeField] private Color _errorColor = Color.red;
        [SerializeField] private Color _normalColor = Color.gray;
        [SerializeField] private Color _successColor = Color.green;
        [SerializeField] private float _showTimeInSeconds = 5f;
        [Range(0.01f, 1)] [SerializeField] private float _alphaAnimationInTimeSec = 0.15f;
        [Range(0.01f, 1)] [SerializeField] private float _alphaAnimationOutTimeSec = 0.3f;
        [Header("Internal")]
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Image _bgImage;
        [SerializeField] private CanvasGroup _canvasGroup;

        private float _hideMessageTime;

        protected override void OnAwakeInternal()
        {
            _canvasGroup.alpha = 0;
        }
        private void Update()
        {
            if (Time.time > _hideMessageTime)
            {
                _canvasGroup.blocksRaycasts = false;
            }
            if (!_canvasGroup.blocksRaycasts && _canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= Time.deltaTime / _alphaAnimationOutTimeSec;
            }
            if (_canvasGroup.blocksRaycasts && _canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += Time.deltaTime / _alphaAnimationInTimeSec;
            }
        }

        public void ShowError(string message)
        {
            ShowInternal(message, _errorColor);
        }
        public void ShowSuccess(string message)
        {
            ShowInternal(message, _successColor);
        }
        public void Show(string message)
        {
            ShowInternal(message, _normalColor);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
        }

        private void ShowInternal(string message, Color bgColor)
        {
            _bgImage.color = bgColor;
            _messageText.text = message;

            _hideMessageTime = Time.time + _showTimeInSeconds;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}
