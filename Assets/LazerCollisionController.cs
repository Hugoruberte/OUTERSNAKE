using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class LazerCollisionController : MonoBehaviour
{
	private Transform myTransform;

	private Lazer lazer;

	private int count;

	private float radius;
	private float lastImpactTime;
	private const float MAX_IMPACT_TIME_INTERVAL = 0.0325f;

	private List<RaycastHit> cache = new List<RaycastHit>();
	private RaycastHit[] results = new RaycastHit[5];

	private Vector3 dir;

	void Awake()
	{
		this.myTransform = transform;
		this.lazer = GetComponentInParent<Lazer>();
		this.radius = Mathf.Max(this.lazer.lazerData.width / 2f, 0.1f);
		this.GetComponent<SphereCollider>().radius = this.radius;
	}

	void Update()
	{
		if(Time.time < this.lastImpactTime + MAX_IMPACT_TIME_INTERVAL) {
			return;
		}

		this.dir = this.lazer.direction.normalized;
		this.count = Physics.RaycastNonAlloc(this.myTransform.position,
											 this.dir,
											 this.results,
											 this.radius + 0.1f,
											 this.lazer.lazerData.hitLayerMask);

		if(this.count > 0) {
			this.lastImpactTime = Time.time;

			foreach(RaycastHit hit in this.results) {
				if(hit.collider != null) {
					this.cache.Add(hit);
				}
			}
			this.lazer.Hit(this.cache.ToArray());
			this.cache.Clear();
		}
	}

	/*void OnCollisionEnter(Collision other)
	{
		if(Time.time > this.lastImpactTime + MAX_IMPACT_TIME_INTERVAL
			&& this.lazer.lazerData.hitLayerMask.IsInLayerMask(other.gameObject.layer)) {

			this.lastImpactTime = Time.time;
			this.lazer.Hit(other);
		}
	}*/
}
