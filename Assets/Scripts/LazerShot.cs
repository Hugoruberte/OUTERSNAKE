using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Lazers;

public class LazerShot : Lazer
{
	public override void Initialize(Vector3 from, Vector3 towards, OnLazerHit callback)
	{
		base.Initialize(from, towards, callback);

		// tail width
		this.curve.MoveKey(1, new Keyframe(1f, 0f));
		this.trailRenderer.widthCurve = curve;
	}

	public override void Hit(RaycastHit[] others)
	{
		this.hiting = true;

		// Callback
		LazerHit hit = new LazerHit(others);
		this.onLazerHit(hit);

		// Bounce
		if(this.lazerData.bounce)
		{
			Vector3 normal = Vector3.zero;
			foreach(RaycastHit other in others) {
				if(lazerData.bounceLayerMask.IsInLayerMask(other.transform.gameObject.layer)) {
					normal += other.normal;
				}
			}

			normal.Normalize();

			if(normal.magnitude > 0) {

				this.bounceCount ++;

				if(this.bounceCount < this.lazerData.maxBounceCount - 1)
				{
					this.direction = Vector3.Reflect(this.direction, normal).normalized;
					this.trailRigidbody.velocity = this.direction * this.lazerData.speed;
				}
				else
				{
					this.trailCollider.enabled = false;

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

				this.hiting = false;
			}
		}
		else
		{
			this.trailRigidbody.velocity = Vector3.zero;
			this.trailCollider.enabled = false;

			this.StartAndStopCoroutine(ref this.behaviourCoroutine, this.DeathCoroutine(false));
		}
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
