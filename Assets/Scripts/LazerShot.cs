using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Lazers;

public class LazerShot : Lazer
{
	public override void Hit(Collision other)
	{
		this.hitSomething = true;

		// Callback
		this.onLazerHit(new LazerHit(other));

		// Bounce
		if(this.lazerData.bounce && lazerData.bounceLayerMask.IsInLayerMask(other.gameObject.layer)) {
			this.direction = Vector3.Reflect(this.direction, LazerHit.GetContactNormalSum(other));
			this.trailRigidbody.velocity = this.direction * this.lazerData.speed;
			this.hitSomething = false;
			return;
		}

		this.trailRigidbody.velocity = Vector3.zero;
			
		this.Death(false);
	}

	protected override IEnumerator DeathCoroutine(bool hitNothing)
	{
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
