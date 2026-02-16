using UnityEngine;

/// <summary>
/// Minimal app-level initializer. The old god-object Services singleton has been removed.
/// All service access is now through Zenject dependency injection via interfaces.
/// Use [Inject] private IPlayerService _playerService; etc. in your classes.
/// </summary>
public class AppInitializer : MonoBehaviour
{
    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

#if UNITY_EDITOR
    public bool clearPrefs;

    private void Update()
    {
        if (clearPrefs)
        {
            clearPrefs = false;
            PlayerPrefs.DeleteAll();
            Debug.Log("Prefs Cleared");
        }
    }
#endif
}