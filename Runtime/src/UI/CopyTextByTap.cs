using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RGN.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class CopyTextByTap : MonoBehaviour, IPointerClickHandler
    {
        private TMP_Text _textComponent;
        private void Awake()
        {
            _textComponent = GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            string text = _textComponent.text;
            Clipboard.SetText(text);
            ToastMessage.I.Show("Copied to clipboard: " + text);
        }
    }
}
