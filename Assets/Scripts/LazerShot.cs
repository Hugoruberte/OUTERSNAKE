using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Lazers;

public class LazerShot : Lazer
{
	public override void Initialize(Transform from, Vector3 towards, OnLazerHit callback)
	{
		base.Initialize(from, towards, callback);

		// tail width
		this.curve.MoveKey(1, new Keyframe(1f, 0f));
		this.trailRenderer.widthCurve = curve;
	}

	public override void Hit(Collider[] colliders, Vector3 pos)
	{
		this.hiting = true;

		LazerHit hit = new LazerHit(colliders, pos, this.lazerData.bounceLayerMask);

		// Callback
		this.onLazerHit(hit);

		// Bounce
		if(this.lazerData.bounce && this.bounceCount < this.lazerData.maxBounceCount)
		{
			if(hit.bounceNormal.magnitude > 0)
			{
				this.Bounce(ref hit, colliders);
			}
		}
		else
		{			
			this.Death();
		}
	}

	private void Bounce(ref LazerHit hit, Collider[] colliders)
	{
		if(this.bounceCount < this.lazerData.maxBounceCount - 1)
		{
			this.direction = Vector3.Reflect(this.direction, hit.bounceNormal).normalized;
			this.trailRigidbody.velocity = this.direction * this.lazerData.speed;
		}
		else
		{
			// Physics
			foreach(Collider c in colliders) {
				this.ignoredCollider.Add(c);
				Physics.IgnoreCollision(this.trailCollider, c);
			}
			
			if(this.lazerData.lastBounceMode == LastBounceMode.Curve) {
				this.StartCoroutine(this.LastBounceCurveCoroutine(this.direction));
			} else if(this.lazerData.lastBounceMode == LastBounceMode.Up) {
				this.LastBounceUp(this.direction);
			} else {
				if(Random.Range(0, 2) == 0) {
					this.StartCoroutine(this.LastBounceCurveCoroutine(this.direction));
				} else {
					this.LastBounceUp(this.direction);
				}
			}
		}

		this.bounceCount ++;
		this.hiting = false;
	}

	protected override IEnumerator DeathCoroutine(bool hitNothing)
	{
		this.dying = true;

		if(hitNothing) {
			yield return this.WidthCoroutine();
		} else {
			yield return new WaitForSeconds(this.trailRenderer.time + 0.2f);
		}
		
		// stow
		this.poolingManager.Stow(this);
	}

	public override void Reset()
	{
		// time
		this.trailRenderer.time = this.lazerData.length / this.lazerData.speed;

		base.Reset();
	}
}
