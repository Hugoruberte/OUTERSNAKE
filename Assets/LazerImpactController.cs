using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

[RequireComponent(typeof(ParticleInstantController))]
public class LazerImpactController : MonoBehaviour
{
	[Header("Parameters")]
	[SerializeField, Range(0f, 5f)] private float margin = 1f;
	[SerializeField, Range(0.1f, 5f)] private float depth = 1f;
	[SerializeField] private LayerMask impactLayerMask = 0;
	[SerializeField] private GameObject voidImpactPrefab = null;

	private ParticleInstantController controller;
	private IEnumerator checkCoroutine = null;
	private bool shouldBeCancelled;


	private void Awake()
	{
		this.controller = GetComponent<ParticleInstantController>();

		this.controller.onAwake.AddListener(this.OnAwakeImpact);
		this.controller.onPlay.AddListener(this.OnPlayImpact);
		this.controller.onEnd.AddListener(this.OnEndImpact);
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
		if(!controller) {
			return;
		}

		if(this.shouldBeCancelled) {
			PoolingManager.instance.Stow(controller);
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

		while(this.controller.main.IsAlive(true))
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
		if(!controller) {
			PoolingManager.instance.Stow(controller);
		}

		ParticleInstantController impact = PoolingManager.instance.Get<ParticleInstantController>(this.voidImpactPrefab);

		if(impact == null) {
			return;
		}

		impact.Initialize(pos, Color.red);

		impact.Launch();
	}
}
