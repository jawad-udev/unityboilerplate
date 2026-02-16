public interface IUIService
{
    void ActivateUIScreen(Screens screenType);
    void ActivateUIPopups(Popups popupType);

    SplashScreen SplashScreen { get; }
    HomeScreen HomeScreen { get; }
    GamePlayScreen GamePlayScreen { get; }

    ProfilePopup ProfilePopup { get; }
    SettingsPopup SettingsPopup { get; }
    CommonPopup CommonPopup { get; }
    PausePopup PausePopup { get; }
    GameWinPopup GameWinPopup { get; }
    GameFailPopup GameFailPopup { get; }
    GameLosePopup GameLosePopup { get; }
}
