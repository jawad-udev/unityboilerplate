using System;
using UnityEngine;

public interface IEffectService
{
    void PlayEffect(Effects effect, Vector3 position, Action callback = null);
    void PlayEffect(Effects effect, Vector3 position, Color color, Action callback = null);
}
