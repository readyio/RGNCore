using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RGN.Impl.Firebase
{
    public abstract class IUIScreen : MonoBehaviour, System.IDisposable
    {
        [SerializeField] private Button _backButton;

        public RectTransform RectTransform { get; private set; }

        protected IRGNFrame _rgnFrame;

        public virtual void PreInit(IRGNFrame rgnFrame)
        {
            RectTransform = GetComponent<RectTransform>();
            _rgnFrame = rgnFrame;
            if (_backButton != null)
            {
                _backButton.gameObject.SetActive(false);
                _backButton.onClick.AddListener(OnBackButtonClick);
            }
        }
        public virtual Task InitAsync()
        {
            return Task.CompletedTask;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_backButton != null)
            {
                _backButton.onClick.RemoveListener(OnBackButtonClick);
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
        }
        public virtual void OnWillAppearNow(object parameters) { }

        internal void SetVisible(bool visible, bool showBackButton)
        {
            if (_backButton != null)
            {
                _backButton.gameObject.SetActive(showBackButton);
            }
            gameObject.SetActive(visible);
            if (visible)
            {
                OnShow();
            }
            else
            {
                OnHide();
            }
        }
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
        protected void OnBackButtonClick()
        {
            _rgnFrame.CloseTopScreen();
        }
    }
}
