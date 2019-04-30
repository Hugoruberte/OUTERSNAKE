using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Lazers;

public class LazerRay : Lazer
{
	private List<Vector3> hitPoints;

	private bool[] widthPoints;

	protected override void Awake()
	{
		base.Awake();

		if(this.lazerData.bounce) {
			this.hitPoints = new List<Vector3>();
		}
	}

	public override void Initialize(Transform from, Vector3 towards, OnLazerHit callback)
	{
		base.Initialize(from, towards, callback);

		if(this.lazerData.bounce) {
			this.hitPoints.Add(from.position + towards);
		}
	}

	private void Update()
	{
		// Bounce
		if(this.lazerData.bounce)
		{
			// RaycastHit hit;
			// for(int i = 0; i < this.hitPoints.Count - 1; i++) {
			// 	Vector3 dir = (this.hitPoints[i+1] - this.hitPoints[i]).normalized;

			// 	if(Physics.Linecast(this.hitPoints[i] + dir, this.hitPoints[i+1] - dir, out hit, this.lazerData.hitLayerMask, QueryTriggerInteraction.Ignore)) {
			// 		this.Intersect(hit);
			// 	}

			// 	Debug.DrawLine(this.hitPoints[i] + dir, this.hitPoints[i+1] - dir, Color.white);
			// }

			// Vector3 last = this.hitPoints[this.hitPoints.Count - 1] + direction;
			// if(Physics.Raycast(last, this.direction, out hit, Vector3.Distance(last, this.headRigidbody.position - this.direction), this.lazerData.hitLayerMask, QueryTriggerInteraction.Ignore)) {

			// 	this.Intersect(hit);
			// }

			// Debug.DrawRay(last, this.direction * Vector3.Distance(last, this.headRigidbody.position - this.direction), Color.white);
		}
	}

	/*public override void Hit(Collision other)
	{
		this.hiting = true;

		// Callback
		this.onLazerHit(new LazerHit(other));

		// Bounce
		if(this.lazerData.bounce && lazerData.bounceLayerMask.IsInLayerMask(other.gameObject.layer)) {
			this.direction = Vector3.Reflect(this.direction, LazerHit.GetContactNormalSum(other));
			this.headRigidbody.velocity = this.direction * this.lazerData.speed;
			this.hiting = false;
			this.hitPoints.Add(other.contacts[0].point);

			return;
		}

		this.headRigidbody.velocity = Vector3.zero;
			
		this.StartAndStopCoroutine(ref this.behaviourCoroutine, this.DeathCoroutine(false));
	}*/

	public override void Hit(Collider[] colliders, Vector3 pos)
	{
		
	}

	private void Intersect(RaycastHit hit)
	{
		// Debug.Log("Intersect with " + hit.transform.name, hit.transform);

		// this.hiting = true;

		// // Callback
		// this.onLazerHit(new LazerHit(hit));

		// // Bounce
		// if(lazerData.bounceLayerMask.IsInLayerMask(hit.transform.gameObject.layer)) {
		// 	this.BounceOnIntersect(hit);
		// 	return;
		// }

		// this.headRigidbody.velocity = Vector3.zero;
			
		// this.Death(false);
	}

	private void BounceOnIntersect(RaycastHit hit)
	{
		// Debug.Log("Bounce on intersect with " + hit.transform.name);

		// // declaration
		// int index, hitpointindex;
		// Vector3[] pos;
		// Vector3 hitpoint;

		// // initialization
		// index = 0;
		// pos = new Vector3[this.headRenderer.positionCount];
		// this.headRenderer.GetPositions(pos);
		// hitpointindex = 0;
		// hitpoint = this.hitPoints[hitpointindex];

		// // bounce junk work
		// this.direction = Vector3.Reflect(this.direction, hit.normal);
		// this.hiting = false;

		// // check bounce recalculation
		// for(int i = 0; i < pos.Length; i++) {

		// 	// check still valid hitpoints
		// 	if(Vector3.Distance(pos[i], hitpoint) <= this.headRenderer.minVertexDistance) {
		// 		hitpoint = this.hitPoints[++ hitpointindex];
		// 	}

		// 	// check intersect point on head
		// 	if(Vector3.Distance(pos[i], hit.point) <= this.headRenderer.minVertexDistance) {
		// 		index = i;
		// 		break;
		// 	}
		// }

		// // hit points recalculation
		// this.hitPoints.RemoveRange(hitpointindex, this.hitPoints.Count - hitpointindex);
		// this.hitPoints.Add(hit.point);

		// // head recalculation
		// this.headRenderer.Clear();
		// pos = pos.Take(index).ToArray();
		// this.headRenderer.SetPositions(pos);
		// this.headRigidbody.position = hit.point;
	}

	protected override IEnumerator DeathCoroutine(bool deathOfOldAge)
	{
		this.headRigidbody.velocity = Vector3.zero;

		// width point
		this.InitializeWidthPoint();
		this.StartCoroutine(this.WidthPointCoroutine());
		yield return this.WidthCoroutine();

		// stow
		this.poolingManager.Stow(this);
	}

	public override void Reset()
	{
		// time
		this.headRenderer.time = float.MaxValue;

		base.Reset();
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

			this.headRenderer.widthCurve = curve;
			yield return null;
		}
	}
}
