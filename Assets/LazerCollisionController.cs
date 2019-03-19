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

	private List<Collider> cache = new List<Collider>();
	private Collider[] results = new Collider[5];

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

		this.count =  Physics.OverlapSphereNonAlloc(this.myTransform.position, this.radius + 0.1f, this.results, this.lazer.lazerData.hitLayerMask);

		if(this.count > 0) {
			this.lastImpactTime = Time.time;

			foreach(Collider c in this.results) {
				if(c != null) {
					this.cache.Add(c);
				}
			}

			this.lazer.Hit(this.cache.ToArray(), this.myTransform.position);
			this.cache.Clear();
		}
	}
}
