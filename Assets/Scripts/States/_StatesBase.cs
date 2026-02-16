using UnityEngine;

/// <summary>
/// Base class for all game states. Attach as children of the GameService prefab.
/// </summary>
public abstract class StateBase : MonoBehaviour
{
	public abstract void OnActivate();
	public abstract void OnDeactivate();
	public abstract void OnUpdate();

	public override string ToString()
	{
		return this.GetType().ToString();
	}
}