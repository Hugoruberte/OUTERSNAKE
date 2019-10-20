using UnityEngine;

public abstract class PoolableEntity : MonoBehaviour
{
	[HideInInspector] public bool isActive = false;

	public virtual void Launch()
	{
		gameObject.SetActive(true);

		this.StopAllCoroutines();

		this.isActive = true;
	}

	public virtual void Reset()
	{
		this.isActive = false;

		this.StopAllCoroutines();

		gameObject.SetActive(false);
	}
}
