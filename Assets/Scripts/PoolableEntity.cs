using UnityEngine;

public class PoolableEntity : MonoBehaviour
{
	protected GameObject myGameObject;

	[HideInInspector]
	public bool isActive = false;

	protected PoolingManager poolingManager { get; private set; }


	protected virtual void Awake()
	{
		this.myGameObject = gameObject;
	}

	protected virtual void Start()
	{
		this.poolingManager = PoolingManager.instance;
	}

	public virtual void Launch()
	{
		this.myGameObject.SetActive(true);

		this.isActive = true;
	}

	public virtual void Reset()
	{
		this.isActive = false;

		this.myGameObject.SetActive(false);
	}
}
