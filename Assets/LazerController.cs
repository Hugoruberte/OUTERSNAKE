using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerController : PoolableEntity
{
	private Transform myTransform;
	private Transform trailTransform;

	private TrailRenderer trailRenderer;

	private float startTime;
	
	private Vector3 direction;

	private AnimationCurve curve = new AnimationCurve();

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
	}

	public void Initialize(Vector3 from, Vector3 towards)
	{
		this.myTransform.position = from;
		this.direction = towards;
	}

	public override void Launch()
	{
		this.startTime = Time.time;

		base.Launch();
	}

	void Update()
	{
		this.trailTransform.Translate(this.direction * this.lazerData.speed * Time.deltaTime);

		if(Time.time - this.startTime > this.lazerData.lifetime) {
			this.poolingManager.Stow(this);
		}
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

	private void InitializeWidthPoint(float instantLength)
	{
		float time;
		int numberOfPoint, pointPerDistance;

		pointPerDistance = Random.Range((int)this.lazerData.pointPerDistanceMinMax[0], (int)this.lazerData.pointPerDistanceMinMax[1] + 1);
		numberOfPoint = Mathf.CeilToInt(instantLength / pointPerDistance) - 1;

		for(int i = 1; i <= numberOfPoint; i++) {
			time = i * pointPerDistance / instantLength;
			this.curve.AddKey(time, 1f);
		}

		this.trailRenderer.widthCurve = curve;
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
				time = this.curve[i].time;
				this.curve.MoveKey(i, new Keyframe(time, value));
			}

			this.trailRenderer.widthCurve = curve;
			yield return null;
		}
	}
}
