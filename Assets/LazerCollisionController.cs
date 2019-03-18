using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class LazerCollisionController : MonoBehaviour
{
	private Lazer lazer;

	private float lastImpactTime;
	private const float MAX_IMPACT_TIME_INTERVAL = 0.0325f;

	void Awake()
	{
		this.lazer = GetComponentInParent<Lazer>();
		this.GetComponent<SphereCollider>().radius = Mathf.Max(this.lazer.lazerData.width / 2f, 0.1f);
	}

	void OnCollisionEnter(Collision other)
	{
		if(Time.time > this.lastImpactTime + MAX_IMPACT_TIME_INTERVAL
			&& this.lazer.lazerData.hitLayerMask.IsInLayerMask(other.gameObject.layer)) {

			this.lastImpactTime = Time.time;
			this.lazer.Hit(other);
		}
	}
}
