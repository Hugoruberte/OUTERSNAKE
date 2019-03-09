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
	public float DistanceToDanger(MovementController ctr)
	{
		return 0f;
	}







	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------------ ACTION FUNCTIONS --------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public IEnumerator Patrol(MovementController ctr, UtilityAction act)
	{
		act.isStoppable = false;

		yield return null;
	}
}
