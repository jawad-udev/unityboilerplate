using UnityEngine;
using Zenject;

public interface IGameMonoBehaviour
{
    void Show();
    void Hide(bool destroy = false);
    void Hide(BacklogType backlogType, bool destroy = false);
    void Show(params BacklogType[] backlogTypes);
    void Show(BacklogType backlogType = BacklogType.DisablePreviousScreen);
}

public class GameMonoBehaviour : MonoBehaviour, IGameMonoBehaviour
{
    [Inject] protected IBackLogService _backLogService;

    public void Show()
    {
        _backLogService.OnScreenOpen(gameObject, BacklogType.DisablePreviousScreen);
        View();
    }

    public void Show(BacklogType backlogType = BacklogType.DisablePreviousScreen)
    {
        _backLogService.OnScreenOpen(gameObject, backlogType);
        View();
    }

    public void Show(params BacklogType[] backlogTypes)
    {
        _backLogService.OnScreenOpen(gameObject, backlogTypes);
        View();
    }

    private void View()
    {
        gameObject.transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    public void Hide(bool destroy = false)
    {
        gameObject.SetActive(false);

        if (destroy)
            Destroy(gameObject);
    }

    public void Hide(BacklogType backlogType, bool destroy = false)
    {
        if (backlogType == BacklogType.RemovePreviousScreen)
        {
            _backLogService.RemoveLastScreens(gameObject);
        }
        gameObject.SetActive(false);

        if (destroy)
            Destroy(gameObject);
    }
}
