using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class LazerCollisionController : MonoBehaviour
{
	private Lazer lazer;

	void Awake()
	{
		this.lazer = GetComponentInParent<Lazer>();

		this.GetComponent<SphereCollider>().radius = Mathf.Max(this.lazer.lazerData.width / 2f, 0.1f);
	}

	void OnCollisionEnter(Collision other)
	{
		if(this.lazer.lazerData.hitLayerMask.IsInLayerMask(other.gameObject.layer)) {

			this.lazer.Hit(other);
		}
	}
}
