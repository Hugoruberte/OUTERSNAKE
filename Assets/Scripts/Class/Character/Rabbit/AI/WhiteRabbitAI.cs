using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Snakes;

[CreateAssetMenu(fileName = "WhiteRabbitAI", menuName = "Scriptable Object/AI/WhiteRabbitAI", order = 3)]
public class WhiteRabbitAI : UtilityAIBehaviour
{
	private Transform danger;
	private Transform snake;

	private IEnumerable<IDangerousEntity> dangers;


	private static WhiteRabbitAI instance = null;

	public static UtilityAIBehaviour Launch(WhiteRabbit rabbit)
	{
		MovementController ctr = new WhiteRabbitController(rabbit);
		instance.AddController(ctr);

		return instance;
	}

	public override void Initialize()
	{
		instance = this;
	}

	public override void Start()
	{
		snake = SnakeManager.instance.snake.transform;

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
		float dist, min, res;

		min = float.MaxValue;

		foreach(IDangerousEntity d in this.dangers) {
			dist = Vector3.Distance(d._transform.position, ctr.entity.myTransform.position);

			if(dist < min) {
				min = dist;
				this.danger = d._transform;
			}
		}

		// mapping :
		// min = 0 --> res = 1
		// min >= rangeOfView --> res <= 0
		res = (-1 / ctr.entity.rangeOfView) * min + 1f;

		return res;
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

	public IEnumerator GoToFood(MovementController ctr, UtilityAction act)
	{
		act.isStoppable = false;
		// yield return ctr.StepToFood();

		// do stuff with ctr...
		yield return null;
	}

	public IEnumerator Wander(MovementController ctr, UtilityAction act)
	{
		RabbitController rtr = ctr as RabbitController;

		act.isStoppable = false;
		yield return rtr.StepToWander();

		act.isStoppable = true;
		yield return rtr.Rest();

		act.isRunning = false;
	}

	public IEnumerator RunAway(MovementController ctr, UtilityAction act)
	{
		RabbitController rtr = ctr as RabbitController;
		
		act.isStoppable = false;

		Transform from = (this.danger != null) ? this.danger : this.snake;
		yield return rtr.StepToRunAway(from);
		
		act.isRunning = false;
	}
}
