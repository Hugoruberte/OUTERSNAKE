using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactive.Engine;
using Snakes;


public class SnakeCharacter : SnakeEntity, IDangerousEntity
{
	[Header("Snake Data")]
	public SnakeData data;

	public Transform _transform { get { return this.myTransform; } }

	protected override void Awake()
	{
		base.Awake();

		this.data.snakeObject = gameObject;
		this.data.snakeTransform = this.myTransform;
		this.data.snakeCollider = this.GetComponent<Collider>();
	}

	protected override void Start()
	{
		base.Start();

		this.data.snakeMovementEvents.onStartStepTo += this.cellable.ReserveNextCell;
		this.data.snakeMovementEvents.onEndStep += this.cellable.UpdateCurrentCell;
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
