using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Snakes;
using Utility.AI;

[System.Serializable]
[CreateAssetMenu(fileName = "WhiteRabbitAI", menuName = "Scriptable Object/AI/WhiteRabbitAI", order = 3)]
public class WhiteRabbitAI : UtilityAIBehaviour<WhiteRabbitAI>
{
	private Transform danger;
	private Transform snake;

	private IEnumerable<IDangerousEntity> dangers;

	public UtilityAIBehaviour Launch(WhiteRabbit rabbit)
	{
		MovementController ctr = new WhiteRabbitController(rabbit);
		this.AddController(ctr);
		return this;
	}

	public override void OnStart()
	{
		base.OnStart();

		snake = SnakeManager.instance.snakeTransform;

		dangers = FindObjectsOfType<MonoBehaviour>().OfType<IDangerousEntity>();
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
		float dist, min;
		WhiteRabbitController wrc;

		wrc = ctr as WhiteRabbitController;
		min = float.MaxValue;

		foreach(IDangerousEntity d in this.dangers) {
			dist = Vector3.Distance(d._transform.position, ctr.entity.myTransform.position);

			if(dist < min) {
				min = dist;
				this.danger = d._transform;
			}
		}

		return this.MapOnRangeOfView(ctr, min, wrc.rabbit.rabbitData.rangeOfView);
	}

	public bool RabbitIsScared(MovementController ctr)
	{
		// return ctr.entity.isScared;
		return false;
	}

	public bool RabbitIsHungry(MovementController ctr)
	{
		// return ctr.entity.isHungry;
		return false;
	}

	public bool ThereIsFood(MovementController ctr)
	{
		return false;
	}

	public bool CanReachFood(MovementController ctr)
	{
		return false;
	}

	public bool RabbitIsTired(MovementController ctr)
	{
		// return ctr.entity.isTired;
		return false;
	}




	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------------ ACTION FUNCTIONS --------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public void EatFood(MovementController ctr)
	{
		// to do
	}

	public IEnumerator GoToFood(MovementController ctr)
	{
		// yield return ctr.StepToFood();

		// do stuff with ctr...
		yield return null;
	}

	public IEnumerator Wander(MovementController ctr, UtilityAction act)
	{
		yield return ctr.Wander();

		act.isStoppable = true;
		yield return ctr.Rest();
	}

	public IEnumerator RunAway(MovementController ctr)
	{
		RabbitController rtr = ctr as RabbitController;

		Transform from = this.danger ?? this.snake;

		yield return rtr.RunAway(from);
	}
}
