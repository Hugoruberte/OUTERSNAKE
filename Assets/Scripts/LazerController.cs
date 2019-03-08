using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class LazerController : PoolableEntity
{
	private Transform myTransform;
	private Transform trailTransform;

	private TrailRenderer trailRenderer;
	
	private Vector3 direction;

	private IEnumerator behaviourCoroutine = null;

	private AnimationCurve curve = new AnimationCurve();

	private bool[] widthPoints;
	private bool move = true;

	private int lazerLayerMask;

	private const float CHECK_COLLISION_ACCURACY = 0.075f;

	// public delegate void OnLazerHit();


	public LazerData lazerData;


	protected override void Awake()
	{
		base.Awake();

		this.myTransform = transform;
		this.trailRenderer = GetComponentInChildren<TrailRenderer>();
		this.trailTransform = trailRenderer.transform;

		// set
		this.trailRenderer.time = lazerData.lifetime;
		this.trailRenderer.widthMultiplier = lazerData.initialWidth;

		this.lazerLayerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Player"));
	}

	public void Initialize(Vector3 from, Vector3 towards)
	{
		this.myTransform.position = from;
		this.direction = towards;
	}

	public override void Launch()
	{
		base.Launch();

		this.StopAndStartCoroutine(this.behaviourCoroutine, LifetimeCoroutine);
	}

	void Update()
	{
		if(move) {
			this.trailTransform.Translate(this.direction * this.lazerData.speed * Time.deltaTime);
		}
	}

	private IEnumerator LifetimeCoroutine()
	{
		// declaration
		float clock, check, radius;
		bool hitSomething;

		// initialization
		clock = check = 0f;
		hitSomething = false;
		radius = (this.lazerData.initialWidth / 2f) - 0.1f;

		while(clock < this.lazerData.lifetime) {
			clock += Time.deltaTime;
			check += Time.deltaTime;

			if(check > CHECK_COLLISION_ACCURACY) {
				check = 0f;

				// check collision
				if(Physics.CheckSphere(this.trailTransform.position, radius, this.lazerLayerMask, QueryTriggerInteraction.Collide)) {
					Debug.Log("Hit !!");
					hitSomething = true;
					break;
				}
			}

			yield return null;
		}

		if(hitSomething) {
			// width point
		} else {
			// global width
			yield return this.WidthCoroutine();
		}

		// stow
		this.poolingManager.Stow(this);
	}

	public override void Reset()
	{
		// width
		this.ClearWidthPoint();
		this.curve.AddKey(0f, 1f);
		this.curve.AddKey(1f, 1f);
		this.trailRenderer.widthCurve = curve;

		this.trailTransform.localPosition = Vector3.zero;

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

		instantLength = Vector3.Distance(this.myTransform.position, this.trailTransform.position);
		distancePerPoint = Random.Range(this.lazerData.distancePerPointMinMax[0], this.lazerData.distancePerPointMinMax[1]);
		nbOfPoint = Mathf.CeilToInt(instantLength / distancePerPoint) + 1;
		this.widthPoints = new bool[nbOfPoint];

		for(int i = 0; i < nbOfPoint; i++) {
			time = Mathf.Min(i * distancePerPoint / instantLength, 1f);

			this.widthPoints[i] = (Random.Range(0, 2) == 0);
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

	private IEnumerator WidthCoroutine()
	{
		float step = 0f;

		while(step < 1f) {

			step += this.lazerData.widthSpeed * Time.deltaTime;
			this.trailRenderer.widthMultiplier = Mathf.Lerp(lazerData.initialWidth, 0f, step);

			yield return null;
		}

		this.trailRenderer.widthMultiplier = 0f;
	}
}
