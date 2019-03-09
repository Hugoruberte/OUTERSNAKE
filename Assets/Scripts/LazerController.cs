using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class LazerController : PoolableEntity
{
	private Transform myTransform;
	private Rigidbody trailRigidbody;

	private TrailRenderer trailRenderer;
	
	private Vector3 direction;

	private AnimationCurve curve = new AnimationCurve();

	private bool[] widthPoints;

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
	private bool dying;
	private bool hitSomething;

	[HideInInspector]
	public static int lazerLayerMask;
	private static int bounceLayerMask;

	private float startTime;

	public delegate void OnLazerHit(Collision other);
	private OnLazerHit onLazerHit = null;

	public LazerData lazerData;


	protected override void Awake()
	{
		base.Awake();

		this.myTransform = transform;
		this.trailRenderer = GetComponentInChildren<TrailRenderer>();
		this.trailRigidbody = trailRenderer.GetComponent<Rigidbody>();

		lazerLayerMask = (1 << LayerMask.NameToLayer("Ground"))
					   | (1 << LayerMask.NameToLayer("Snake"))
					   | (1 << LayerMask.NameToLayer("Snake Body"));

		bounceLayerMask = (1 << LayerMask.NameToLayer("Ground"))
						| (1 << LayerMask.NameToLayer("Snake Body"));
	}

	public void Initialize(Vector3 from, Vector3 towards, OnLazerHit callback)
	{
		this.myTransform.position = from;
		this.direction = towards;
		this.onLazerHit = callback;
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
		if((Time.time > this.startTime + this.lazerData.lifetime) && this.dying == false && this.hitSomething == false) {
			this.Death(true);
		}
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
		this.onLazerHit(other);

		// Bounce
		if(this.lazerData.bounce && bounceLayerMask.IsInLayerMask(other.gameObject.layer)) {
			
			this.direction = Vector3.Reflect(this.direction, other.contacts[0].normal);
			this.hitSomething = false;
			return;
		}

		this.move = false;
			
		this.Death(false);
	}

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
