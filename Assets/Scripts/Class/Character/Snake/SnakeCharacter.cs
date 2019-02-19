using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactive.Engine;
using Snakes;


public class SnakeCharacter : Snake, IDangerousEntity, IFoodChainEntity
{
	private SnakeController snakeController;

	protected override void Start()
	{
		base.Start();

		snakeController = SnakeManager.GetController();
		snakeController.events.onStartStep.AddListener(this.ReserveNextCell);
		snakeController.events.onEndStep.AddListener(this.UpdateCurrentCell);
	}







	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ----------------------------------- FOOD CHAIN VARIABLES ------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public float foodChainValue { get; private set; } = 1000f;
	public int foodChainRank { get; private set; } = 100;








	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ------------------------------------ INTERACT FUNCTIONS -------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public override void InteractivelyReactWith(InteractiveStatus s, PhysicalInteractionEntity i)
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
