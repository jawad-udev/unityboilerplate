using System.Collections.Generic;
using Zenject;

public class UIService : IUIService
{
    private readonly Dictionary<Screens, GameMonoBehaviour> _screens;
    private readonly Dictionary<Popups, GameMonoBehaviour> _popups;

    [Inject] private SplashScreen _splashScreen;
    [Inject] private HomeScreen _homeScreen;
    [Inject] private GamePlayScreen _gamePlayScreen;
    [Inject] private ProfilePopup _profilePopup;
    [Inject] private SettingsPopup _settingsPopup;
    [Inject] private CommonPopup _commonPopup;
    [Inject] private PausePopup _pausePopup;
    [Inject] private GameWinPopup _gameWinPopup;
    [Inject] private GameFailPopup _gameFailPopup;
    [Inject] private GameLosePopup _gameLosePopup;

    // Lazy initialization of dictionaries after injection
    private bool _initialized;

    private void EnsureInitialized()
    {
        if (_initialized) return;
        _initialized = true;
    }

    #region Public API - Screen/Popup Routing

    public void ActivateUIScreen(Screens screenType)
    {
        switch (screenType)
        {
            case Screens.SPLASH: _splashScreen.Show(); break;
            case Screens.HOME: _homeScreen.Show(); break;
            case Screens.PLAY: _gamePlayScreen.Show(); break;
        }
    }

    public void ActivateUIPopups(Popups popupType)
    {
        switch (popupType)
        {
            case Popups.PROFILE: _profilePopup.Show(BacklogType.KeepPreviousScreen); break;
            case Popups.SETTINGS: _settingsPopup.Show(BacklogType.KeepPreviousScreen); break;
            case Popups.PAUSE: _pausePopup.Show(BacklogType.KeepPreviousScreen); break;
            case Popups.WIN: _gameWinPopup.Show(BacklogType.KeepPreviousScreen); break;
            case Popups.LOSE: _gameLosePopup.Show(BacklogType.KeepPreviousScreen); break;
            case Popups.FAIL: _gameFailPopup.Show(BacklogType.KeepPreviousScreen); break;
        }
    }

    #endregion

    #region Screen Accessors

    public SplashScreen SplashScreen => _splashScreen;
    public HomeScreen HomeScreen => _homeScreen;
    public GamePlayScreen GamePlayScreen => _gamePlayScreen;

    #endregion

    #region Popup Accessors

    public ProfilePopup ProfilePopup => _profilePopup;
    public SettingsPopup SettingsPopup => _settingsPopup;
    public CommonPopup CommonPopup => _commonPopup;
    public PausePopup PausePopup => _pausePopup;
    public GameWinPopup GameWinPopup => _gameWinPopup;
    public GameFailPopup GameFailPopup => _gameFailPopup;
    public GameLosePopup GameLosePopup => _gameLosePopup;

    #endregion
}