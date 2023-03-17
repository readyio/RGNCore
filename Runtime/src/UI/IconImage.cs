using UnityEngine;
using UnityEngine.UI;

namespace RGN.UI
{
    [RequireComponent(typeof(Button), typeof(RectTransform))]
    public sealed class IconImage : MonoBehaviour
    {
        public Button.ButtonClickedEvent OnClick
        {
            get { return _uploadImageButton.onClick; }
            set { _uploadImageButton.onClick = value; }
        }

        [SerializeField] private RawImage _iconRawImage;
        [SerializeField] private Image _uploadIconImage;
        [SerializeField] private Button _uploadImageButton;
        [SerializeField] private LoadingIndicator _loadingIndicator;

        public void SetLoading(bool loading)
        {
            _loadingIndicator.SetEnabled(loading);
        }
        public void SetProfileTexture(Texture2D texture)
        {
            _iconRawImage.texture = texture;
            _uploadIconImage.gameObject.SetActive(texture == null);
        }
    }
}
