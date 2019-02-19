using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProjectorScript : MonoBehaviour
{
	private Transform myTransform;
	public Transform Target;

	void Awake()
	{
		myTransform = transform;
	}

	void Update()
	{
		if(Target)
		{
			myTransform.LookAt(Target);
		}
	}
}
