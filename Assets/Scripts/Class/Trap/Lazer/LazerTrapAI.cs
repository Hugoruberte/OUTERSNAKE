using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Snakes;

[System.Serializable]
[CreateAssetMenu(fileName = "LazerTrapAI", menuName = "Scriptable Object/AI/LazerTrapAI", order = 3)]
public class LazerTrapAI : UtilityAIBehaviour<LazerTrapAI>
{
	[System.NonSerialized] private Transform snake;
	[System.NonSerialized] private Transform target;
	[System.NonSerialized] private Collider[] results;

	public LayerMask targetLayerMask;


	public UtilityAIBehaviour Launch(LazerTrap lazer)
	{
		MovementController ctr = new LazerTrapController(lazer);
		this.AddController(ctr);
		return this;
	}

	public override void OnStart()
	{
		base.OnStart();
		
		this.snake = SnakeManager.instance.snake.transform;
		this.results = new Collider[5];
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
		int count;
		float dist, min;
		Vector3 pos;

		min = float.MaxValue;
		pos = ctr.position;
		count = Physics.OverlapSphereNonAlloc(pos, ctr.entity.rangeOfView, this.results, this.targetLayerMask);
		
		foreach(Collider c in this.results) {
			if(c == null) {
				continue;
			}

			dist = Vector3.Distance(pos, c.transform.position);
			if(dist < min) {
				min = dist;
				this.target = c.transform;
			}
		}

		return this.MapOnRangeOfView(min, ctr);
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
		act.isStoppable = true;

		LazerTrapController ltr = ctr as LazerTrapController;

		this.target = this.target ?? this.snake;

		yield return ltr.AimAt(this.target);
	}
}
