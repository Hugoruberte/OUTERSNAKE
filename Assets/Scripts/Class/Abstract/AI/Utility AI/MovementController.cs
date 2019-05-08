using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

[System.Serializable]
public abstract class MovementController
{
	public readonly LivingEntity entity;

	public Vector3 position { get { return this.entity.myTransform.position; } }
	public Quaternion rotation { get { return this.entity.myTransform.rotation; } }

	public MovementController(LivingEntity e)
	{
		this.entity = e;
	}




	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------------- BASIC BEHAVIOUR FUNCTIONS ---------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public virtual IEnumerator Wander() {
		// empty
		yield break;
	}

	public virtual IEnumerator Rest() {
		// empty
		yield break;
	}
}
