using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Lazers;

namespace Lazers
{
	public delegate void OnLazerHit(LazerHit other);

	public struct LazerImpact {
		public readonly GameObject other;
		public readonly Vector3 point;
		public readonly Vector3 normal;
		
		public LazerImpact(GameObject o, Vector3 p, Vector3 n) {
			this.other = o;
			this.normal = n;
			this.point = p;
		}
	}

	public struct LazerHit {
		public readonly LazerImpact[] impacts;
		public readonly Vector3 bounceNormal;

		public LazerHit(Collider[] colliders, Vector3 pos, LayerMask bounceLayerMask) {
			this.impacts = new LazerImpact[colliders.Length];
			this.bounceNormal = Vector3.zero;

			GameObject other;
			Vector3 point;
			Vector3 normal;
			for(int i = 0; i < colliders.Length; i++) {
				other = colliders[i].transform.gameObject;
				point = colliders[i].ClosestPoint(pos);
				normal = (pos - point).normalized;
				
				this.impacts[i] = new LazerImpact(other, point, normal);
				
				if(bounceLayerMask.IsInLayerMask(other.layer)) {
					this.bounceNormal += normal;
				}
			}

			this.bounceNormal.Normalize();
		}
	}

	public enum LastBounceMode {
		Curve = 0,
		Up,
		Random
	};
}

public abstract class Lazer : PoolableEntity
{
	protected Collider trailCollider;
	[HideInInspector] public Rigidbody trailRigidbody;

	protected List<Collider> ignoredCollider = new List<Collider>();

	protected TrailRenderer trailRenderer;
		
	public Vector3 direction;

	protected AnimationCurve curve = new AnimationCurve();

	protected IEnumerator behaviourCoroutine = null;

	protected bool dying;
	protected bool hiting;

	protected LazerCollisionController lazerCollisionController;

	protected int bounceCount;

	protected float startTime;

	protected OnLazerHit onLazerHit = null;

	public LazerData lazerData;


	protected override void Awake()
	{
		base.Awake();

		this.trailRenderer = GetComponentInChildren<TrailRenderer>();
		this.trailCollider = trailRenderer.GetComponent<Collider>();
		this.trailRigidbody = trailRenderer.GetComponent<Rigidbody>();
		this.lazerCollisionController = trailRenderer.GetComponent<LazerCollisionController>();
	}

	public virtual void Initialize(Transform from, Vector3 towards, OnLazerHit callback)
	{
		this.transform.SetPositionAndRotation(from.position, from.rotation);
		this.direction = towards.normalized;
		this.onLazerHit = callback;
	}

	public override void Launch()
	{
		base.Launch();

		this.startTime = Time.time;
		this.trailRigidbody.velocity = this.direction * this.lazerData.speed;

		this.StartAndStopCoroutine(ref this.behaviourCoroutine, this.LifeCoroutine());
	}

	private IEnumerator LifeCoroutine()
	{
		float clock = 0f;

		while((clock >= this.lazerData.lifetime && !this.dying && !this.hiting) == false) {
			clock += Time.deltaTime;
			yield return null;
		}

		yield return this.DeathCoroutine(true);
	}

	public abstract void Hit(Collider[] colliders, Vector3 pos);

	protected void Death()
	{
		this.lazerCollisionController.enabled = false;
		this.trailCollider.enabled = false;
		this.trailRigidbody.constraints = RigidbodyConstraints.FreezeAll;

		this.StartAndStopCoroutine(ref this.behaviourCoroutine, this.DeathCoroutine(false));
	}

	protected abstract IEnumerator DeathCoroutine(bool hitNothing);

	protected IEnumerator WidthCoroutine()
	{
		float step = 0f;

		while(step < 1f) {
			step += this.lazerData.widthSpeed * Time.deltaTime;
			this.trailRenderer.widthMultiplier = Mathf.Lerp(lazerData.width, 0f, step);
			yield return null;
		}

		this.trailRenderer.widthMultiplier = 0f;
	}

	protected Vector3 LastBounceUp(Vector3 forward)
	{
		// declaration
		Vector3 up, cross;

		// initialize
		up = this.trailRigidbody.transform.up;
		cross = Vector3.Cross(up, forward);

		// randomize 'up' with cone angle
		up = (Quaternion.Euler(forward * Random.Range(-this.lazerData.coneAngle, this.lazerData.coneAngle)) * up).normalized;
		up = (Quaternion.Euler(cross * Random.Range(-this.lazerData.coneAngle, this.lazerData.coneAngle)) * up).normalized;
		
		// movement
		this.trailRigidbody.velocity = up * this.lazerData.speed;

		return up;	
	}

	protected IEnumerator LastBounceCurveCoroutine(Vector3 forward)
	{
		// declaration
		Vector3 up, fd, project;
		WaitForFixedUpdate waitForFixedUpdate;
		float forwardForce, gravityForce;

		// initialize
		up = this.LastBounceUp(forward);
		fd = forward;
		forwardForce = this.lazerData.forwardForceMultiplier * this.lazerData.speed * Random.Range(0.25f + this.lazerData.forceDampling * 0.75f, 1f);
		gravityForce = this.lazerData.gravityForceMultiplier * this.lazerData.speed * Random.Range(-0.5f * this.lazerData.forceDampling + 1.5f, 1f);

		// compute 'up' and 'forward' forces
		project = Vector3.ProjectOnPlane(up, this.trailRigidbody.transform.up).normalized;
		forward = Vector3.Cross(project, up).normalized * (2 * Random.Range(0, 2) - 1);
		if(forward.magnitude == 0) { forward = fd; }
		waitForFixedUpdate = new WaitForFixedUpdate();

		// movement
		this.trailRigidbody.AddForce(forward * forwardForce, ForceMode.VelocityChange);
		while(this.trailCollider.enabled) {
			this.trailRigidbody.AddForce(-up * gravityForce, ForceMode.Acceleration);
			yield return waitForFixedUpdate;
		}
	}

	public override void Reset()
	{
		// coroutine
		this.StopAllCoroutines();

		// width
		this.ClearWidthPoint();
		this.curve.AddKey(0f, 1f);
		this.curve.AddKey(1f, 1f);
		this.trailRenderer.widthCurve = curve;
		this.trailRenderer.widthMultiplier = this.lazerData.width;

		// parameter
		this.bounceCount = 0;
		this.direction = Vector3.zero;

		// callback
		this.onLazerHit = null;

		// boolean
		this.hiting = false;
		this.dying = false;
		
		// rigidbody
		this.trailRigidbody.velocity = Vector3.zero;
		this.trailRigidbody.transform.localPosition = Vector3.zero;
		this.trailRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

		// collider
		this.trailCollider.enabled = true;
		foreach(Collider c in this.ignoredCollider) { Physics.IgnoreCollision(this.trailCollider, c, false); }
		this.ignoredCollider.Clear();

		// trail
		this.trailRenderer.Clear();
		this.lazerCollisionController.enabled = true;

		base.Reset();
	}

	protected void ClearWidthPoint()
	{
		int length;

		length = this.curve.length;

		for(int i = length - 1; i >= 0; i--) {
			this.curve.RemoveKey(i);
		}

		this.trailRenderer.widthCurve = curve;
	}
}