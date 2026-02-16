using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService : MonoBehaviour, ISceneService
{
    [SerializeField] private SceneReference gameScene;
    [SerializeField] private SceneReference mainScene;

    public void LoadGameScene()
    {
        SceneManager.LoadSceneAsync(gameScene);
    }

    public void LoadMainScene()
    {
        SceneManager.LoadSceneAsync(mainScene);
    }
}
