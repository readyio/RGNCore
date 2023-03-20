using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RGN.UI
{
    public class NestedScrollView : ScrollRect
    {
        [SerializeField] private ScrollRect _parentScroll;

        // Made this field static because the LateUpdate() method.
        // If there are multiple nested scroll views under one parent scroll view
        // The late update is executed on all nested scroll rects and the m_OnValueChanged.Invoke(normalizedPosition);
        // even if just the parent scroll rect is dragged
        private static bool sDraggingParent = false;

        protected override void Awake()
        {
            base.Awake();
            _parentScroll = _parentScroll ?? GetScrollParent(transform);
            sDraggingParent = false;
        }

        private ScrollRect GetScrollParent(Transform t)
        {
            if (t.parent != null)
            {
                ScrollRect scroll = t.parent.GetComponent<ScrollRect>();
                return scroll != null ? scroll : GetScrollParent(t.parent);
            }
            return null;
        }

        private bool IsPotentialParentDrag(Vector2 inputDelta)
        {
            if (_parentScroll != null)
            {
                if (_parentScroll.horizontal && !_parentScroll.vertical)
                {
                    return Mathf.Abs(inputDelta.x) > Mathf.Abs(inputDelta.y);
                }
                if (!_parentScroll.horizontal && _parentScroll.vertical)
                {
                    return Mathf.Abs(inputDelta.x) < Mathf.Abs(inputDelta.y);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);
            _parentScroll?.OnInitializePotentialDrag(eventData);
        }

        protected override void LateUpdate()
        {
            if (sDraggingParent)
            {
                return;
            }
            base.LateUpdate();
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (IsPotentialParentDrag(eventData.delta))
            {
                _parentScroll.OnBeginDrag(eventData);
                sDraggingParent = true;
            }
            else
            {
                base.OnBeginDrag(eventData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (sDraggingParent)
            {
                _parentScroll.OnDrag(eventData);
            }
            else
            {
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (_parentScroll != null && sDraggingParent)
            {
                sDraggingParent = false;
                _parentScroll.OnEndDrag(eventData);
            }
        }
    }
}
