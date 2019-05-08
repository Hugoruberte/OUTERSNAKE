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

		SnakeManager.instance.snakeEvents.onStartStepTo += this.cellable.ReserveNextCell;
		SnakeManager.instance.snakeEvents.onEndStep += this.cellable.UpdateCurrentCell;
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
		// use a death layer instead !
		if(other.CompareTag("Snake Part")) {

			// snake percuted a snake part...
			Debug.Log("Snake death");
		}
	}
}
