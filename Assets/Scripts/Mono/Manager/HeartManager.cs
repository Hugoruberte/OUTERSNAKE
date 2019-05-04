using UnityEngine;
using UnityEngine.Events;
using Tools;

public class HeartManager : Singleton<HeartManager>
{
	public _Transform heart = new _Transform();
	private _Transform cache = new _Transform();
	private _Transform previous = new _Transform();

	public class OnRotateEvent : UnityEvent<_Transform, _Transform> {}
	public OnRotateEvent onRotate = new OnRotateEvent();

	protected override void Awake()
	{
		base.Awake();

		this.previous.Copy(this.cache);
		this.cache.Copy(this.heart);
		this.heart.onRotate.AddListener(this.OnRotateEventLauncher);
	}

	private void OnRotateEventLauncher()
	{
		this.previous.Copy(this.cache);
		this.cache.Copy(this.heart);

		this.onRotate.Invoke(this.previous, this.heart);
	}
}
