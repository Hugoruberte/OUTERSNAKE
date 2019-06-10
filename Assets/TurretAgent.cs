using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.AI;
using MLAgents;
using static System.Array;


[RequireComponent(typeof(Turret))]
public class TurretAgent : Agent
{
	private Turret turret;
	private TurretAI behaviour;
	private MovementController ctr;

	[SerializeField] private Rigidbody snake = null;


	private void Awake()
	{
		this.turret = this.GetComponent<Turret>();
		this.behaviour = this.turret.behaviour as TurretAI;
	}

	private void Start()
	{
		this.ctr = this.behaviour.GetController(this.turret);
		this.ctr.onChooseAction += this.OnChooseUtilityAction;
	}

	public override void AgentReset()
	{
		this.snake.position = new Vector3(Random.Range(-14f, 9f), this.snake.position.y, Random.Range(-45f, -22f));
		this.transform.position = new Vector3(Random.Range(-14f, 9f), this.transform.position.y, Random.Range(-45f, -22f));

		this.RequestDecision();
	}

	public override void CollectObservations()
	{
		bool isTargetAround, isTargetSight, couldShoot, aimIsRunning;

		isTargetAround = this.behaviour.IsThereTargetAround(this.ctr);
		isTargetSight = this.behaviour.IsTargetInSight(this.ctr);
		couldShoot = this.behaviour.CouldShoot(this.ctr);
		aimIsRunning = this.behaviour.IsActionRunning(this.ctr, "Aim");

		this.AddVectorObs(isTargetAround);
		this.AddVectorObs(isTargetSight);
		this.AddVectorObs(couldShoot);
		this.AddVectorObs(aimIsRunning);
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		UtilityAction action;
		UtilityScorer scorer;
		int index;

		index = 0;
		for(int i = 0; i < this.behaviour.actions.Count; i++)
		{
			action = this.behaviour.actions[i];

			for(int j = 0; j < action.scorers.Count; j++)
			{
				scorer = action.scorers[j];

				if(!scorer.isCondition) {
					continue;
				}

				scorer.score = Mathf.RoundToInt(vectorAction[index++] * 10);
				scorer.not = (vectorAction[index++] > 0f);
			}
		}
	}

	private void OnChooseUtilityAction(string[] chosens)
	{
		bool isTargetAround, isTargetSight, couldShoot, aimIsRunning;

		isTargetAround = this.behaviour.IsThereTargetAround(this.ctr);
		isTargetSight = this.behaviour.IsTargetInSight(this.ctr);
		couldShoot = this.behaviour.CouldShoot(this.ctr);
		aimIsRunning = this.behaviour.IsActionRunning(this.ctr, "Aim");

		this.SetReward(0f);

		// WANDER
		if(!isTargetAround)
		{
			if(IndexOf(chosens, "Wander") > -1) {
				this.AddReward(1.0f);
			} else {
				this.Done();
			}
		}
		
		// AIM
		if(isTargetAround)
		{
			if(IndexOf(chosens, "Aim") > -1) {
				this.AddReward(1.0f);
			} else {
				this.Done();
			}
		}

		// SHOOT
		if(aimIsRunning && couldShoot && isTargetSight)
		{
			if(IndexOf(chosens, "Shoot") > -1) {
				this.AddReward(1.0f);
			} else {
				this.Done();
			}
		}
	}
}
