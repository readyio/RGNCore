using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RGN.Impl.Firebase
{
    public interface IRGNFrame
    {
        TScreen GetScreen<TScreen>() where TScreen : class;
        void OpenScreen<TScreen>(bool animate = true);
        void OpenScreen<TScreen>(object parameters, bool animate = true);
        void CloseScreen<TScreen>(bool animate = true);
        void CloseScreen(System.Type type, bool animate = true);
        void CloseTopScreen();
    }

    public class RGNFrame : RGNUnityInitializer, IRGNFrame
    {
        [SerializeField] private IUIScreen[] _initializables;

        private readonly Dictionary<System.Type, IUIScreen> mRegisteredScreens =
            new Dictionary<System.Type, IUIScreen>();
        private readonly Stack<IUIScreen> mScreensStack = new Stack<IUIScreen>();

        private IUIScreen _currentVisibleScreen;
        private ScreenAnimation _screenAnimation;

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            RGNCore.I.UpdateEvent += OnUpdate;

            for (int i = 0; i < _initializables.Length; ++i)
            {
                var screen = _initializables[i];
                screen.PreInit(this);
                if (i == 0)
                {
                    screen.SetVisible(true, false);
                    _currentVisibleScreen = screen;
                }
                else
                {
                    screen.SetVisible(false, false);
                }
                mRegisteredScreens.Add(screen.GetType(), screen);
            }
            for (int i = 0; i < _initializables.Length; ++i)
            {
                var screen = _initializables[i];
                await screen.InitAsync();
            }
        }
        protected override void Dispose(bool disposing)
        {
            for (int i = 0; i < _initializables.Length; ++i)
            {
                _initializables[i].Dispose();
            }
            RGNCore.I.UpdateEvent -= OnUpdate;
            base.Dispose(disposing);
        }
        private void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape) && _currentVisibleScreen != null)
            {
                CloseTopScreen();
            }
            if (_screenAnimation != null && !_screenAnimation.IsDone)
            {
                _screenAnimation.Tick();
            }
            if (_screenAnimation != null && _screenAnimation.IsDone)
            {
                _screenAnimation = null;
            }
        }

        public TScreen GetScreen<TScreen>()
            where TScreen : class
        {
            var screenTypeToOpen = typeof(TScreen);
            if (mRegisteredScreens.TryGetValue(screenTypeToOpen, out var screen))
            {
                return screen as TScreen;
            }
            Debug.LogError("Can not find screen: " + screenTypeToOpen);
            return null;
        }
        public void OpenScreen<TScreen>(bool animate = true)
        {
            var screenTypeToOpen = typeof(TScreen);
            if (mRegisteredScreens.TryGetValue(screenTypeToOpen, out var screen))
            {
                _screenAnimation = new ScreenAnimation(_currentVisibleScreen, screen, true);
                screen.SetVisible(true, true);
                mScreensStack.Push(_currentVisibleScreen);
                _currentVisibleScreen = screen;
                return;
            }
            Debug.LogError("Can not find screen to open: " + screenTypeToOpen);
        }
        public void OpenScreen<TScreen>(object parameters, bool animate = true)
        {
            OpenScreen<TScreen>(animate);
            _currentVisibleScreen.OnWillAppearNow(parameters);
        }
        public void CloseScreen<TScreen>(bool animate = true)
        {
            var screenTypeToClose = typeof(TScreen);
            CloseScreen(screenTypeToClose);
        }
        public void CloseScreen(System.Type screenTypeToClose, bool animate = true)
        {
            if (mRegisteredScreens.TryGetValue(screenTypeToClose, out var screen))
            {
                _currentVisibleScreen = null;
                if (mScreensStack.Count > 0)
                {
                    _currentVisibleScreen = mScreensStack.Pop();
                    _screenAnimation = new ScreenAnimation(screen, _currentVisibleScreen, false);
                    _currentVisibleScreen.SetVisible(true, mScreensStack.Count > 0);
                }
                return;
            }
            Debug.LogError("Can not find screen to close: " + screenTypeToClose);
        }
        public void CloseTopScreen()
        {
            if (_currentVisibleScreen == null)
            {
                Debug.LogError("There is no current opened screen, nothing to close");
                return;
            }
            CloseScreen(_currentVisibleScreen.GetType(), true);
        }
    }
}
