using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Lazers;

namespace Lazers
{
	public delegate void OnLazerHit(LazerHit other);

	public struct LazerHit {
		public Vector3 normal;
		public Vector3 point;
		public GameObject[] others;

		public LazerHit(RaycastHit[] hs) {
			Vector3 n = Vector3.zero;
			Vector3 p = Vector3.zero;
			this.others = new GameObject[hs.Length];

			for(int i = 0; i < hs.Length; i++) {
				n += hs[i].normal;
				p += hs[i].point;
				this.others[i] = hs[i].transform.gameObject;
			}

			this.normal = n.normalized;
			this.point = p / hs.Length;
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
	protected Rigidbody trailRigidbody;

	protected TrailRenderer trailRenderer;
		
	[HideInInspector]
	public Vector3 direction;

	protected AnimationCurve curve = new AnimationCurve();

	protected IEnumerator behaviourCoroutine = null;

	protected bool dying;
	protected bool hiting;

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
	}

	public virtual void Initialize(Vector3 from, Vector3 towards, OnLazerHit callback)
	{
		this.transform.position = from;
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

	// public abstract void Hit(Collision other);
	public abstract void Hit(RaycastHit[] others);

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
		forward = Vector3.Cross(project, up).normalized;
		if(forward.magnitude == 0) { forward = fd; }
		waitForFixedUpdate = new WaitForFixedUpdate();

		// movement
		this.trailRigidbody.AddForce(forward * forwardForce, ForceMode.VelocityChange);
		while(true) {
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

		// callback
		this.onLazerHit = null;

		// boolean
		this.hiting = false;
		this.dying = false;
		
		// rigidbody + collider
		this.direction = Vector3.zero;
		this.trailRigidbody.velocity = Vector3.zero;
		this.trailRigidbody.transform.localPosition = Vector3.zero;
		this.trailCollider.enabled = true;

		// trail
		this.trailRenderer.Clear();

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