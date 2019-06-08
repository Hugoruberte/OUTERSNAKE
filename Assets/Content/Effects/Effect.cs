using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

public abstract class Effect : PoolableEntity
{
	private Transform myTransform;
	private Transform followTarget;
	private Vector3 followOffset;


	protected virtual private void Awake()
	{
		this.myTransform = transform;
	}



	public void SetPosition(Vector3 position) => this.transform.position = position;

	public void SetDirection(Vector3 direction) => this.SetDirection(direction, Shared.vector3Up);
	public void SetDirection(Vector3 direction, Vector3 up)
	{
		// only does that
		this.transform.rotation = Quaternion.LookRotation(direction, up);
	}

	public void SetOrientation(Vector3 position, Vector3 direction) => this.SetOrientation(position, direction, Shared.vector3Up);
	public void SetOrientation(Vector3 position, Vector3 direction, Vector3 up)
	{
		this.SetPosition(position);
		this.SetDirection(direction, up);
	}

	public virtual void SetColor(Color c)
	{
		Renderer[] rends = this.GetComponentsInChildren<Renderer>();

		foreach(Renderer r in rends) {
			r.material.color = c;
		}
	}

	public void SetFollow(Transform target, Vector3 offset = default(Vector3))
	{
		this.followTarget = target;
		this.followOffset = offset;
	}

	public override void Launch()
	{
		base.Launch();

		if(this.followTarget) {
			this.StartCoroutine(this.FollowCoroutine(this.followTarget, this.followOffset));
		}
	}

	public override void Reset()
	{
		this.followTarget = null;

		base.Reset();
	}




	private IEnumerator FollowCoroutine(Transform target, Vector3 offset)
	{
		while(true)
		{
			this.myTransform.position = target.position + offset;

			yield return null;
		}
	}
}
