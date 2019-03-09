using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class LazerCollisionController : MonoBehaviour
{
	private LazerController lazer;

	private int lazerLayerMask;

	void Awake()
	{
		this.lazer = GetComponentInParent<LazerController>();
		this.lazerLayerMask = LazerController.lazerLayerMask;

		this.GetComponent<SphereCollider>().radius = Mathf.Max(this.lazer.lazerData.width / 2f, 0.15f);
	}

	void OnCollisionEnter(Collision other)
	{
		if(this.lazerLayerMask.IsInLayerMask(other.gameObject.layer)) {
			lazer.Hit(other);
		}
	}
}
