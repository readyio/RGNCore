using UnityEngine;

namespace RGN.Impl.Firebase
{
    internal sealed class ScreenAnimation
    {
        internal bool IsDone { get; private set; }

        private readonly IUIScreen mOutScreen;
        private readonly IUIScreen mInScreen;
        private readonly bool mIsOpening;
        private readonly float mHorizontalScreenSize;

        internal ScreenAnimation(IUIScreen outScreen, IUIScreen inScreen, bool isOpening)
        {
            mOutScreen = outScreen;
            mInScreen = inScreen;
            mIsOpening = isOpening;
            Vector2 inScreenSize = mInScreen.RectTransform.rect.size;
            mHorizontalScreenSize = inScreenSize.x;
            mInScreen.RectTransform.anchoredPosition = new Vector3(
                mIsOpening ? mHorizontalScreenSize : -mHorizontalScreenSize,
                0,
                0);

            IsDone = false;
        }

        internal void Tick()
        {
            Move(mInScreen.RectTransform, mIsOpening ? -1 : 1);
            Move(mOutScreen.RectTransform, mIsOpening ? -1 : 1);
            if ((mInScreen.RectTransform.anchoredPosition.x <= 0 && mIsOpening) ||
                (mInScreen.RectTransform.anchoredPosition.x >= 0 && !mIsOpening))
            {
                mOutScreen.SetVisible(false, false);
                mInScreen.RectTransform.anchoredPosition = Vector3.zero;
                mOutScreen.RectTransform.anchoredPosition = new Vector3(-mHorizontalScreenSize, 0, 0);
                IsDone = true;
            }
        }

        private void Move(RectTransform rectTransform, int direction)
        {
            Vector3 pos = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector3(
                pos.x + direction * Time.deltaTime * mHorizontalScreenSize * 3,
                0,
                0);
        }
    }
}
