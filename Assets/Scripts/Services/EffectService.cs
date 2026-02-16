using UnityEngine;
using System;

public class EffectService : MonoBehaviour, IEffectService
{
    public void PlayEffect(Effects effect, Vector3 position, Action callback = null)
    {
        // TODO: Implement effect spawning
        callback?.Invoke();
    }

    public void PlayEffect(Effects effect, Vector3 position, Color color, Action callback = null)
    {
        // TODO: Implement effect spawning with color
        callback?.Invoke();
    }
}
