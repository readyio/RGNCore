using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RGN.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public sealed class PullToRefresh : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public System.Func<Task> RefreshRequested;

        [SerializeField] private Image _reloadImagePrefab;
        [SerializeField] private float _refreshDistance = 100f;

        private ScrollRect _scrollRect;
        private Image _reloadImage;
        private RectTransform _reloadImageRectTransform;
        private bool _needToRotate;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }
        private void OnDestroy()
        {
            RefreshRequested = null;
        }
        private void Update()
        {
            if (_needToRotate == false)
            {
                return;
            }
            _reloadImageRectTransform.Rotate(Vector3.back, 1);
        }
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 contentAnchoredPosition = _scrollRect.content.anchoredPosition;
            if (contentAnchoredPosition.y > -1)
            {
                return;
            }
            if (_reloadImage == null)
            {
                _reloadImage = Instantiate(_reloadImagePrefab, _scrollRect.viewport);
                _reloadImageRectTransform = _reloadImage.GetComponent<RectTransform>();
            }
            _reloadImage.gameObject.SetActive(true);
            _needToRotate = false;
            _reloadImageRectTransform.localRotation = Quaternion.Euler(0, 0, contentAnchoredPosition.y);
        }
        public async void OnEndDrag(PointerEventData eventData)
        {
            if (_reloadImage == null)
            {
                return;
            }
            Vector2 contentAnchoredPosition = _scrollRect.content.anchoredPosition;
            if (contentAnchoredPosition.y > -_refreshDistance)
            {
                _reloadImage.gameObject.SetActive(false);
                return;
            }
            _needToRotate = true;
            try
            {
                if (RefreshRequested != null)
                {
                    await RefreshRequested.Invoke();
                }
            }
            finally
            {
                _reloadImage.gameObject.SetActive(false);
                _needToRotate = false;
            }
        }
    }
}
