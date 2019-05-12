using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

public class LazerCollisionController : MonoBehaviour
{
	private Transform myTransform;
	private Collider[] cache = new Collider[1];
	private Lazer lazer;

	void Awake()
	{
		this.myTransform = transform;
		this.lazer = GetComponentInParent<Lazer>();
		this.GetComponent<SphereCollider>().radius = Mathf.Max(this.lazer.lazerData.width / 2f, 0.1f);
	}

	void OnCollisionEnter(Collision other)
	{
		if(this.lazer.lazerData.hitLayerMask.IsInLayerMask(other.gameObject.layer)) {

			this.cache[0] = other.collider;
			this.lazer.Hit(this.cache, this.myTransform.position);
		}
	}
}
