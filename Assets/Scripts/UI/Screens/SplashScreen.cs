using UnityEngine;
using Zenject;

public class SplashScreen : GameMonoBehaviour
{
    [Inject] private IGameService _gameService;

    private void Start()
    {
        Extensions.PerformActionWithDelay(this, 2f, () =>
        {
            this.Hide(destroy: true);
            _gameService.SetState<MenuState>();
        });
    }
}