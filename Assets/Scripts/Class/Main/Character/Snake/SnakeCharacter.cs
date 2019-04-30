using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactive.Engine;
using Snakes;


public class SnakeCharacter : SnakeEntity, IDangerousEntity
{
	public Transform _transform { get { return this.myTransform; } }

	protected override void Start()
	{
		base.Start();

		SnakeManager.instance.events.onStartStep.AddListener(this.cellable.ReserveNextCell);
		SnakeManager.instance.events.onEndStep.AddListener(this.cellable.UpdateCurrentCell);
	}








	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ------------------------------------ INTERACT FUNCTIONS -------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public override void InteractWith(InteractiveStatus s, PhysicalInteractionEntity i)
	{
		// to do
	}



	private void ManageContactWith(GameObject other)
	{
		if(other.CompareTag("Snake Part")) {

			float dist = Vector3.Distance(myTransform.position, other.transform.position);

			// it is the collision when the snake part is created
			if(dist < 0.5f) {
				return;
			}

			// snake percuted a snake part...
			Debug.Log("Snake death");
		}
	}
}
