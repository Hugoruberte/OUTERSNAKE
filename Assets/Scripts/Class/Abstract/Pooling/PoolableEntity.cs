using UnityEngine;
using UnityEngine.Events;

public abstract class PoolableEntity : MonoBehaviour
{
	protected GameObject myGameObject;
	protected PoolingManager poolingManager { get; private set; }

	[HideInInspector] public bool isActive = false;


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

		this.StopAllCoroutines();

		this.isActive = true;
	}

	public virtual void Reset()
	{
		this.isActive = false;

		this.StopAllCoroutines();

		this.myGameObject.SetActive(false);
	}
}
