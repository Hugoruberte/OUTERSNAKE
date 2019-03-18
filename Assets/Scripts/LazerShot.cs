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

	/*public override void Hit(Collision other)
	{
		this.hiting = true;

		// Callback
		this.onLazerHit(new LazerHit(other));

		// Bounce
		if(this.lazerData.bounce && lazerData.bounceLayerMask.IsInLayerMask(other.gameObject.layer)) {

			Vector3 normal = LazerHit.GetContactNormalSum(other);
			this.direction = Vector3.Reflect(this.direction, normal);

			Debug.Log(other.gameObject.name, other.gameObject.transform);
			Debug.DrawRay(other.contacts[0].point, normal * 5f, Color.yellow);
			Debug.DrawRay(other.contacts[0].point, direction * 5f, Color.green);
			Debug.Break();

			this.trailRigidbody.velocity = this.direction * this.lazerData.speed;
			this.hiting = false;
			return;
		}

		this.trailRigidbody.velocity = Vector3.zero;

		this.StartAndStopCoroutine(ref this.behaviourCoroutine, this.DeathCoroutine(false));
	}*/

	public override void Hit(RaycastHit[] others)
	{
		this.hiting = true;

		// Callback
		this.onLazerHit(new LazerHit(others));

		// Bounce
		if(this.lazerData.bounce && lazerData.bounceLayerMask.IsInLayerMask(other.gameObject.layer)) {

			Vector3 normal = LazerHit.GetContactNormalSum(other);
			this.direction = Vector3.Reflect(this.direction, normal);

			Debug.Log(other.gameObject.name, other.gameObject.transform);
			Debug.DrawRay(other.contacts[0].point, normal * 5f, Color.yellow);
			Debug.DrawRay(other.contacts[0].point, direction * 5f, Color.green);
			Debug.Break();

			this.trailRigidbody.velocity = this.direction * this.lazerData.speed;
			this.hiting = false;
			return;
		}

		this.trailRigidbody.velocity = Vector3.zero;

		this.StartAndStopCoroutine(ref this.behaviourCoroutine, this.DeathCoroutine(false));
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
