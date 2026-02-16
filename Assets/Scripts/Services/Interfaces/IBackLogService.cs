using UnityEngine;

public interface IBackLogService
{
    void OnScreenOpen(GameObject screen, BacklogType backlogType = BacklogType.DisablePreviousScreen);
    void OnScreenOpen(GameObject screen, params BacklogType[] backlogTypes);
    void CloseLastUI();
    void OnClickBack();
    void ClearTop(GameObject current);
    void ClearTop();
    void DisableLastScreen();
    void DisableAndremoveAllScreens();
    void RemoveLastScreens(int count);
    void RemoveLastScreens(GameObject gameObject);
}
