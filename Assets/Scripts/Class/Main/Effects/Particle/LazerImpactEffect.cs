using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

public class LazerImpactEffect : InstantParticleEffect
{
	[Header("Parameters")]
	[SerializeField, Range(0f, 5f)] private float margin = 1f;
	[SerializeField, Range(0.1f, 5f)] private float depth = 1f;
	[SerializeField] private LayerMask impactLayerMask = 0;
	[SerializeField] private GameObject voidImpactPrefab = null;

	private IEnumerator checkCoroutine = null;
	private bool shouldBeCancelled;


	protected override void Awake()
	{
		base.Awake();
		
		this.onAwake += this.OnAwakeImpact;
		this.onPlay += this.OnPlayImpact;
		this.onEnd += this.OnEndImpact;
	}

	private void OnAwakeImpact()
	{
		Vector3 origin, pos;
		Vector3 dir;
		RaycastHit result;
		float dist;

		dir = -transform.forward;
		origin = transform.position;
		pos = origin - dir * this.margin;
		dist = this.margin + this.depth;

		this.shouldBeCancelled = false;

		if(Physics.Raycast(pos, dir, out result, dist, this.impactLayerMask))
		{
			// replace impact
			transform.position = result.point;
		}
		else
		{
			// cancel impact
			this.shouldBeCancelled = true;
		}
	}

	private void OnPlayImpact()
	{
		if(this.shouldBeCancelled) {
			PoolingManager.instance.Stow(this);
			return;
		}

		// only does that
		this.StartAndStopCoroutine(ref this.checkCoroutine, this.OnPlayImpactCoroutine());
	}

	private IEnumerator OnPlayImpactCoroutine()
	{
		Vector3 origin, pos;
		Vector3 dir;
		RaycastHit result;
		float dist;

		dir = -transform.forward;
		dist = this.margin + this.depth;

		while(this.main.IsAlive(true))
		{
			origin = transform.position;
			pos = origin - dir * this.margin;
			Debug.DrawRay(pos, dir * dist, Color.yellow);

			if(!Physics.Raycast(pos, dir, out result, dist, this.impactLayerMask)) {

				// cancel impact
				this.VoidImpact(origin);
				break;
			}
			else
			{
				// replace impact
				transform.position = result.point;
			}

			yield return null;
		}
	}

	private void OnEndImpact()
	{
		// only does that
		this.TryStopCoroutine(ref this.checkCoroutine);
	}

	private void VoidImpact(Vector3 pos)
	{
		PoolingManager.instance.Stow(this);

		InstantParticleEffect impact = PoolingManager.instance.Get<InstantParticleEffect>(this.voidImpactPrefab);

		if(impact == null) {
			return;
		}

		impact.SetPosition(pos);
		impact.SetColor(Color.red);

		impact.Launch();
	}
}
