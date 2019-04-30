using static System.Array;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Lazers;

public class LazerShot : Lazer
{
	private Rigidbody explosionRigidbody;
	private ParticleSystem explosion;

	protected override void Awake()
	{
		base.Awake();

		this.explosion = GetComponentInChildren<ParticleSystem>();
		this.explosionRigidbody = explosion.GetComponent<Rigidbody>();
	}

	public override void Initialize(Transform from, Vector3 towards, OnLazerHit callback)
	{
		base.Initialize(from, towards, callback);

		// tail width
		this.curve.MoveKey(1, new Keyframe(1f, 0f));
		this.headRenderer.widthCurve = curve;
		// this.SetTailEnd();
	}

	/*private void SetTailEnd()
	{
		if(this.lazerData.tailWidthPointLength == 0f) {
			this.curve.MoveKey(1, new Keyframe(1f, 0f));
			this.headRenderer.widthCurve = curve;
			return;
		}

		Keyframe key;
		AnimationCurve cacheCurve;
		float time;
		float point, widthPointValue;

		key = new Keyframe();
		point = 0f;

		this.ClearWidthPoint();

		key.time = 0f;
		key.value = 1f;
		key.weightedMode = WeightedMode.Both;
		key.outWeight = 0f;
		key.inWeight = 0f;
		this.curve.AddKey(key);

		key.time = 1f;
		key.value = 0f;
		key.weightedMode = WeightedMode.Both;
		key.outWeight = 0f;
		key.inWeight = 0f;
		this.curve.AddKey(key);

		cacheCurve = new AnimationCurve(this.curve.keys);

		while(point < this.lazerData.tailWidthPointLength) {
			point += this.lazerData.tailWidthPointSpacing + Random.value * 0.1f;

			time = 1f - (point / this.lazerData.length);
			widthPointValue = (Random.Range(0, 100) < 75) ? 0f : cacheCurve.Evaluate(time);

			key.time = time;
			key.value = widthPointValue;
			key.weightedMode = WeightedMode.Both;
			key.outWeight = 0f;
			key.inWeight = 0f;
			this.curve.AddKey(key);
		}

		point += (this.lazerData.tailWidthPointSpacing + Random.value * 0.1f) / 2f;
		time = 1f - (point / this.lazerData.length);
		key.time = time;
		key.value = cacheCurve.Evaluate(time);
		key.weightedMode = WeightedMode.Both;
		key.outWeight = 0f;
		key.inWeight = 0f;
		this.curve.AddKey(key);

		this.headRenderer.widthCurve = this.curve;
	}*/

	public override void Hit(Collider[] colliders, Vector3 pos)
	{
		this.hiting = true;

		LazerHit hit = new LazerHit(colliders, pos, this.lazerData.bounceLayerMask);

		// Callback
		this.onLazerHit(hit);

		// Impact
		this.SetGroundImpact(hit, pos);

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
			this.Kill();
		}
	}

	private void Bounce(ref LazerHit hit, Collider[] colliders)
	{
		Vector3 impactDirection = -direction;

		// Basic bounce
		if(this.bounceCount < this.lazerData.maxBounceCount - 1)
		{
			this.direction = this.ComputeReflectDirection(this.headRigidbody.position, this.direction, hit.bounceNormal, colliders);
			this.headRigidbody.velocity = this.direction * this.lazerData.speed;

			impactDirection += this.direction;
		}
		// Last bounce
		else
		{
			// Physics
			foreach(Collider c in colliders) {
				this.ignoredCollider.Add(c);
				Physics.IgnoreCollision(this.headCollider, c);
			}

			this.SetLazerImpact(this.headRigidbody.position, -this.direction);
			
			if(this.lazerData.lastBounceMode == LastBounceMode.Curve)
			{
				this.StartCoroutine(this.LastBounceCurveCoroutine(this.direction));
			}
			else if(this.lazerData.lastBounceMode == LastBounceMode.Up)
			{
				this.LastBounceUp(this.direction);
			}
			else
			{
				if(Random.Range(0, 2) == 0) {
					this.StartCoroutine(this.LastBounceCurveCoroutine(this.direction));
				} else {
					this.LastBounceUp(this.direction);
				}
			}
		}

		this.SetLazerImpact(this.headRigidbody.position, impactDirection);
		this.bounceCount ++;
		this.hiting = false;
	}

	private Vector3 ComputeReflectDirection(Vector3 point, Vector3 dir, Vector3 normal, Collider[] colliders)
	{
		Vector3 reflect;

		reflect = Vector3.Reflect(dir, normal).normalized;

		if(this.lazerData.autoAim) {
			reflect = this.ComputeAutoAimDirection(point, reflect, colliders);
		}

		return reflect;
	}

	private Vector3 ComputeAutoAimDirection(Vector3 point, Vector3 dir, Collider[] colliders)
	{
		int count;
		Collider c;
		float dot, max;
		RaycastHit hit;
		Vector3 result, link;

		max = float.MinValue;
		count = Physics.OverlapSphereNonAlloc(point, this.lazerData.autoAimRange, this.lazerData.autoAimResults, this.lazerData.autoAimLayerMask);
		result = dir;

		if(count > 0)
		{
			for(int i = 0; i < this.lazerData.autoAimResults.Length; i++) {
				c = this.lazerData.autoAimResults[i];
				if(c == null || IndexOf(colliders, c) > -1) {
					continue;
				}

				// could use "c.ClosestPoint" instead of "c.transform.position" here...
				link = (c.transform.position - point).normalized;

				// check reachability
				if(!Physics.Raycast(point, link, out hit, this.lazerData.autoAimRange + 10f, this.lazerData.autoAimLayerMask) || hit.collider != c) {
					continue;
				}

				// alignment
				dot = Vector3.Dot(dir, link);

				if(dot >= this.lazerData.autoAimThreshold && dot > max) {
					max = dot;
					result = link;
				}

				// clear
				this.lazerData.autoAimResults[i] = null;
			}
		}

		return result;
	}

	private void SetGroundImpact(LazerHit hit, Vector3 position)
	{
		bool found = false;

		for(int i = 0; i < hit.impacts.Length; i++) {
			if(hit.impacts[i].other.layer == LayerMask.NameToLayer("Ground")) {
				found = true;
				break;
			}
		}

		if(!found) {
			return;
		}

		GroundImpact impact = poolingManager.Get<GroundImpact>(this.lazerData.groundImpactPrefab);

		if(impact == null) {
			return;
		}

		impact.Initialize(position, -hit.bounceNormal);

		impact.Launch();
	}

	private void SetLazerImpact(Vector3 position, Vector3 direction)
	{
		ParticleInstantController impact = poolingManager.Get<ParticleInstantController>(this.lazerData.lazerImpactPrefab);

		if(impact == null) {
			return;
		}

		impact.Initialize(position, direction, Color.red);

		impact.Launch();
	}

	protected override void Kill()
	{
		this.explosionRigidbody.isKinematic = false;
		this.explosionRigidbody.velocity = this.direction * this.lazerData.speed;
		this.explosion.Play();

		base.Kill();
	}

	protected override IEnumerator DeathCoroutine(bool deathOfOldAge)
	{
		this.dying = true;

		if(deathOfOldAge) {
			yield return this.WidthCoroutine();
		} else {
			ParticleSystem.MainModule main = this.explosion.main;
			float delay = Mathf.Max(this.headRenderer.time + 0.2f, main.duration);
			yield return Yielders.Wait(delay);
		}
		
		// stow
		this.poolingManager.Stow(this);
	}

	public override void Reset()
	{
		// time
		this.headRenderer.time = this.lazerData.length / this.lazerData.speed;

		// explosion
		this.explosionRigidbody.isKinematic = true;
		this.explosionRigidbody.velocity = Vector3.zero;
		this.explosionRigidbody.transform.localPosition = Vector3.zero;

		// reset
		base.Reset();
	}
}