using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Snakes;

[System.Serializable]
[CreateAssetMenu(fileName = "LazerTrapAI", menuName = "Scriptable Object/AI/LazerTrapAI", order = 3)]
public class LazerTrapAI : UtilityAIBehaviour<LazerTrapAI>
{
	private Transform snake;

	public UtilityAIBehaviour Launch(LazerTrap lazer)
	{
		MovementController ctr = new LazerTrapController(lazer);
		this.AddController(ctr);
		return this;
	}

	public override void OnStart()
	{
		base.OnStart();
		
		snake = SnakeManager.instance.snake.transform;
	}








	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------------ SCORERS FUNCTIONS -------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public float DistanceToNearestTarget(MovementController ctr)
	{
		Debug.Log("TO DO");
		return 0f;
	}







	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------------ ACTION FUNCTIONS --------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public IEnumerator Wander(MovementController ctr, UtilityAction act)
	{
		Debug.Log("TO DO");
		act.isStoppable = true;
		yield return ctr.Wander();
	}

	public IEnumerator Aim(MovementController ctr, UtilityAction act)
	{
		Debug.Log("TO DO");
		act.isStoppable = true;
		yield return null;
	}
}
