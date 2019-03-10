using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class LazerCollisionController : MonoBehaviour
{
	private LazerController lazer;

	void Awake()
	{
		this.lazer = GetComponentInParent<LazerController>();

		this.GetComponent<SphereCollider>().radius = Mathf.Max(this.lazer.lazerData.width / 2f, 0.15f);
	}

	void OnCollisionEnter(Collision other)
	{
		if(this.lazer.lazerData.hitLayerMask.IsInLayerMask(other.gameObject.layer)) {

			lazer.Hit(other);
		}
	}
}
