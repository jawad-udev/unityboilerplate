using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackLogService : MonoBehaviour, IBackLogService
{
    public List<GameObject> pileOfScreen = new List<GameObject>();

    public void OnScreenOpen(GameObject screen, BacklogType backlogType = BacklogType.DisablePreviousScreen)
    {
        if (backlogType == BacklogType.Ignore)
            return;

        if (backlogType == BacklogType.DisablePreviousScreen)
            DisableLastScreen();

        if (backlogType == BacklogType.ClearTop)
            ClearTop();

        pileOfScreen.Add(screen);
        RemoveRecentDuplication();
    }

    public void OnScreenOpen(GameObject screen, params BacklogType[] backlogTypes)
    {
        foreach (BacklogType backlogType in backlogTypes)
        {
            if (backlogType == BacklogType.Ignore)
                return;

            if (backlogType == BacklogType.DisablePreviousScreen)
                DisableLastScreen();

            if (backlogType == BacklogType.ClearTop)
                ClearTop();
        }

        pileOfScreen.Add(screen);
        RemoveRecentDuplication();
    }

    private void RemoveRecentDuplication()
    {
        if (pileOfScreen.Count > 1)
        {
            if (pileOfScreen[pileOfScreen.Count - 1] == pileOfScreen[pileOfScreen.Count - 2])
            {
                RemoveLastScreens(1);
            }
        }
    }

    public void CloseLastUI()
    {
        if (pileOfScreen.Count > 1)
        {
            pileOfScreen[pileOfScreen.Count - 1].SetActive(false);
            pileOfScreen.RemoveAt(pileOfScreen.Count - 1);
            pileOfScreen[pileOfScreen.Count - 1].SetActive(true);
        }
    }

    public void OnClickBack()
    {
        CloseLastUI();
    }

    public void ClearTop(GameObject current)
    {
        pileOfScreen = new List<GameObject>();
        pileOfScreen.Add(current);
    }

    public void ClearTop()
    {
        pileOfScreen = new List<GameObject>();
    }

    public void DisableLastScreen()
    {
        if (pileOfScreen.Count > 0)
        {
            pileOfScreen[pileOfScreen.Count - 1].SetActive(false);
        }
    }

    public void DisableAndremoveAllScreens()
    {
        foreach (GameObject screen in pileOfScreen)
        {
            if (screen != null)
                screen.SetActive(false);
        }

        ClearTop();
    }

    public void RemoveLastScreens(int count)
    {
        if (count > 0 && pileOfScreen.Count >= count)
            pileOfScreen.RemoveRange(pileOfScreen.Count - count, count);
    }

    public void RemoveLastScreens(GameObject gameObject)
    {
        pileOfScreen.Remove(gameObject);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseLastUI();
        }
    }
}