using UnityEngine;

/// <summary>
/// Deprecated — service parenting is now handled by Zenject container.
/// Kept to avoid missing-script errors on existing scene GameObjects.
/// </summary>
public class AutoParentToServices : MonoBehaviour
{
    public int siblingIndexToUse = 99;
}
