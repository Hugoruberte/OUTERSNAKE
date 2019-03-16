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
		public GameObject other;

		public LazerHit(Collision c) {
			this.normal = GetContactNormalSum(c);
			this.point = c.contacts[0].point;
			this.other = c.gameObject;
		}

		public LazerHit(RaycastHit h) {
			this.normal = h.normal;
			this.point = h.point;
			this.other = h.transform.gameObject;
		}

		public static Vector3 GetContactNormalSum(Collision c)
		{
			Vector3 sum = Vector3.zero;

			foreach(ContactPoint p in c.contacts) {
				sum.Set(sum.x + p.normal.x, sum.y + p.normal.y, sum.z + p.normal.z);
			}

			return sum.normalized;
		}
	}
}

public abstract class Lazer : PoolableEntity
{
	protected Rigidbody trailRigidbody;

	protected TrailRenderer trailRenderer;
	
	protected Vector3 direction;

	protected AnimationCurve curve = new AnimationCurve();

	private bool dying;
	protected bool hitSomething;

	protected float startTime;

	protected OnLazerHit onLazerHit = null;

	public LazerData lazerData;


	protected override void Awake()
	{
		base.Awake();

		this.trailRenderer = GetComponentInChildren<TrailRenderer>();
		this.trailRigidbody = trailRenderer.GetComponent<Rigidbody>();
	}

	public virtual void Initialize(Vector3 from, Vector3 towards, OnLazerHit callback)
	{
		this.transform.position = from;
		this.direction = towards;
		this.onLazerHit = callback;
	}

	public override void Launch()
	{
		base.Launch();

		this.startTime = Time.time;
		this.trailRigidbody.velocity = this.direction * this.lazerData.speed;
	}

	protected virtual void Update()
	{
		// Lifetime
		if((Time.time > this.startTime + this.lazerData.lifetime) && this.dying == false && this.hitSomething == false) {
			this.Death(true);
		}
	}

	public abstract void Hit(Collision other);

	protected void Death(bool hitNothing)
	{
		this.dying = true;

		this.StartCoroutine(this.DeathCoroutine(hitNothing));
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

		// callback
		this.onLazerHit = null;

		// boolean
		this.hitSomething = false;
		this.dying = false;
		
		// rigidbody
		this.trailRigidbody.velocity = Vector3.zero;
		this.trailRigidbody.transform.localPosition = Vector3.zero;

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