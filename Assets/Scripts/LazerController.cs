using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Lazers;

namespace Lazers
{
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

	public delegate void OnLazerHit(LazerHit other);
}

public class LazerController : PoolableEntity
{
	private Transform myTransform;
	private Rigidbody trailRigidbody;

	private TrailRenderer trailRenderer;
	
	private List<Vector3> hitPoints;
	// private Vector3 surfaceNormal;
	private Vector3 _direction;
	private Vector3 direction {
		get { return this._direction; }
		set {
			if(this.lazerData.flatten) {
				Debug.Log("TO DO");
				// value = Vector3.ProjectOnPlane(value, this.surfaceNormal);
			}
			this._direction = value;
		}
	}

	private AnimationCurve curve = new AnimationCurve();

	private bool[] widthPoints;

	private bool shouldCheckRaycast;
	private bool dying;
	private bool hitSomething;
	private bool _move;
	private bool move {
		get { return this._move; }
		set {
			this._move = value;
			if(!value) {
				this.trailRigidbody.velocity = Vector3.zero;
			}
		}
	}

	private float startTime;

	private OnLazerHit onLazerHit = null;

	public LazerData lazerData;


	protected override void Awake()
	{
		base.Awake();

		this.myTransform = transform;
		this.trailRenderer = GetComponentInChildren<TrailRenderer>();
		this.trailRigidbody = trailRenderer.GetComponent<Rigidbody>();

		this.shouldCheckRaycast = (this.lazerData.bounce && this.lazerData.mode == LazerData.LazerMode.Continuous);
		if(this.shouldCheckRaycast) {
			this.hitPoints = new List<Vector3>();
		}
	}

	public void Initialize(Vector3 from, Vector3 towards, OnLazerHit callback)
	{
		this.myTransform.position = from;
		this.direction = towards;
		this.onLazerHit = callback;

		if(this.shouldCheckRaycast) {
			this.hitPoints.Add(from + towards);
		}
	}

	public override void Launch()
	{
		base.Launch();

		this.startTime = Time.time;
		this.move = true;
	}

	void Update()
	{
		// Lifetime
		if((Time.time > this.startTime + this.lazerData.lifetime) && this.dying == false && this.hitSomething == false)
		{
			this.Death(true);
		}
		// else if(this.shouldCheckRaycast)
		// {
		// 	RaycastHit hit;
		// 	for(int i = 0; i < this.hitPoints.Count - 1; i++) {
		// 		Vector3 dir = (this.hitPoints[i+1] - this.hitPoints[i]).normalized;

		// 		if(Physics.Linecast(this.hitPoints[i] + dir, this.hitPoints[i+1] - dir, out hit, this.lazerData.hitLayerMask, QueryTriggerInteraction.Ignore)) {
		// 			this.Intersect(hit);
		// 		}

		// 		Debug.DrawLine(this.hitPoints[i] + dir, this.hitPoints[i+1] - dir, Color.white);
		// 	}

		// 	Vector3 last = this.hitPoints[this.hitPoints.Count - 1] + direction;
		// 	if(Physics.Raycast(last, this.direction, out hit, Vector3.Distance(last, this.trailRigidbody.position - this.direction), this.lazerData.hitLayerMask, QueryTriggerInteraction.Ignore)) {

		// 		this.Intersect(hit);
		// 	}

		// 	Debug.DrawRay(last, this.direction * Vector3.Distance(last, this.trailRigidbody.position - this.direction), Color.white);
		// }
	}

	void FixedUpdate()
	{
		// Movement
		if(this.move) {
			this.trailRigidbody.velocity = this.direction * this.lazerData.speed;
		}
	}

	public void Hit(Collision other)
	{
		this.hitSomething = true;

		// Callback
		this.onLazerHit(new LazerHit(other));

		// Bounce
		if(this.lazerData.bounce && lazerData.bounceLayerMask.IsInLayerMask(other.gameObject.layer)) {
			this.direction = Vector3.Reflect(this.direction, LazerHit.GetContactNormalSum(other));
			this.hitSomething = false;

			if(this.shouldCheckRaycast) {
				this.hitPoints.Add(other.contacts[0].point);
			}

			return;
		}

		this.move = false;
			
		this.Death(false);
	}

	// private void Intersect(RaycastHit hit)
	// {
	// 	Debug.Log("Intersect with " + hit.transform.name, hit.transform);

	// 	this.hitSomething = true;

	// 	// Callback
	// 	this.onLazerHit(new LazerHit(hit));

	// 	// Bounce
	// 	if(lazerData.bounceLayerMask.IsInLayerMask(hit.transform.gameObject.layer)) {
	// 		this.BounceOnIntersect(hit);
	// 		return;
	// 	}

	// 	this.move = false;
			
	// 	this.Death(false);
	// }

	// private void BounceOnIntersect(RaycastHit hit)
	// {
	// 	Debug.Log("Bounce on intersect with " + hit.transform.name);

	// 	// declaration
	// 	int index, hitpointindex;
	// 	Vector3[] pos;
	// 	Vector3 hitpoint;

	// 	// initialization
	// 	index = 0;
	// 	pos = new Vector3[this.trailRenderer.positionCount];
	// 	this.trailRenderer.GetPositions(pos);
	// 	hitpointindex = 0;
	// 	hitpoint = this.hitPoints[hitpointindex];

	// 	// bounce junk work
	// 	this.direction = Vector3.Reflect(this.direction, hit.normal);
	// 	this.hitSomething = false;

	// 	// check bounce recalculation
	// 	for(int i = 0; i < pos.Length; i++) {

	// 		// check still valid hitpoints
	// 		if(Vector3.Distance(pos[i], hitpoint) <= this.trailRenderer.minVertexDistance) {
	// 			hitpoint = this.hitPoints[++ hitpointindex];
	// 		}

	// 		// check intersect point on trail
	// 		if(Vector3.Distance(pos[i], hit.point) <= this.trailRenderer.minVertexDistance) {
	// 			index = i;
	// 			break;
	// 		}
	// 	}

	// 	// hit points recalculation
	// 	this.hitPoints.RemoveRange(hitpointindex, this.hitPoints.Count - hitpointindex);
	// 	this.hitPoints.Add(hit.point);

	// 	// trail recalculation
	// 	this.trailRenderer.Clear();
	// 	pos = pos.Take(index).ToArray();
	// 	this.trailRenderer.SetPositions(pos);
	// 	this.trailRigidbody.position = hit.point;
	// }

	private void Death(bool hitNothing)
	{
		this.dying = true;

		if(this.lazerData.mode == LazerData.LazerMode.Shot) {
			StartCoroutine(this.DeathShotCoroutine(hitNothing));
		} else {
			StartCoroutine(this.DeathContinuousCoroutine());
		}
	}

	private IEnumerator DeathShotCoroutine(bool hitNothing)
	{
		if(hitNothing) {
			yield return this.WidthCoroutine();
		} else {
			yield return new WaitForSeconds(this.trailRenderer.time + 0.2f);
		}
		
		// stow
		this.poolingManager.Stow(this);
	}

	private IEnumerator DeathContinuousCoroutine()
	{
		this.move = false;

		// width point
		this.InitializeWidthPoint();
		this.StartCoroutine(this.WidthPointCoroutine());
		yield return this.WidthCoroutine();

		// stow
		this.poolingManager.Stow(this);
	}

	private IEnumerator WidthCoroutine()
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

		// time
		if(this.lazerData.mode == LazerData.LazerMode.Shot) {
			this.trailRenderer.time = this.lazerData.length / this.lazerData.speed;
		} else {
			this.trailRenderer.time = float.MaxValue;
		}

		// boolean
		this.hitSomething = false;
		this.dying = false;
		this.move = false;
		
		// position
		this.trailRigidbody.transform.localPosition = Vector3.zero;

		// trail
		this.trailRenderer.Clear();

		base.Reset();
	}




	

	







	



	










	private void ClearWidthPoint()
	{
		int length;

		length = this.curve.length;

		for(int i = length - 1; i >= 0; i--) {
			this.curve.RemoveKey(i);
		}

		this.trailRenderer.widthCurve = curve;
	}

	private void InitializeWidthPoint()
	{
		float time, distancePerPoint, instantLength;
		int nbOfPoint;

		this.ClearWidthPoint();

		instantLength = (Time.time - this.startTime) * this.lazerData.speed;
		distancePerPoint = Random.Range(this.lazerData.distancePerPointMinMax[0], this.lazerData.distancePerPointMinMax[1]);
		nbOfPoint = Mathf.CeilToInt(instantLength / distancePerPoint) + 1;
		this.widthPoints = new bool[nbOfPoint];

		for(int i = 0; i < nbOfPoint; i++) {
			time = Mathf.Min(i * distancePerPoint / instantLength, 1f);

			this.widthPoints[i] = (Random.Range(0, 100) < 75);
			this.curve.AddKey(time, 1f);
		}
	}

	private IEnumerator WidthPointCoroutine()
	{
		float time, value;
		float step = 0f;
		int length = this.curve.length;

		while(step < 1f) {
			step += this.lazerData.widthPointSpeed * Time.deltaTime;
			value = Mathf.Lerp(1f, 0f, step);

			for(int i = 0; i < length; i++) {
				if(i >= this.widthPoints.Length) {
					break;
				}
				if(!this.widthPoints[i]) {
					continue;
				}
				time = this.curve[i].time;
				this.curve.MoveKey(i, new Keyframe(time, value));
			}

			this.trailRenderer.widthCurve = curve;
			yield return null;
		}
	}
}
