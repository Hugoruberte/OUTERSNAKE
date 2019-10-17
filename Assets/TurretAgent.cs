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
	private TurretController ctr;

	private bool isTargetAround;
	private bool isTargetSight;
	private bool couldShoot;

	private bool[] previous = new bool[3];
	private bool[] currents = new bool[3];

	[SerializeField] private Rigidbody snake = null;


	private void Awake()
	{
		this.turret = this.GetComponent<Turret>();
	}

	private void Start()
	{
		this.behaviour = this.turret.behaviour as TurretAI;
		this.ctr = this.behaviour.GetController(this.turret) as TurretController;

		// EVENTS
		this.ctr.onStartNewAction += this.RewardOnNewAction;
		this.turret.onHit += this.RewardOnHit;
	}

	private void Update()
	{
		this.isTargetAround = this.behaviour.IsThereTargetAround(this.ctr);
		this.isTargetSight = this.behaviour.IsTargetInSight(this.ctr);
		this.couldShoot = this.behaviour.CouldShoot(this.ctr);

		this.currents[0] = this.isTargetAround;
		this.currents[1] = this.isTargetSight;
		this.currents[2] = this.couldShoot;

		if(this.CheckDecisionRequired()) {
			this.RequestDecision();
		}
	}

	private bool CheckDecisionRequired()
	{
		bool result = false;

		for(int i = 0; i < this.currents.Length; i++)
		{
			if(this.currents[i] != this.previous[i]) {
				result = true;
			}

			this.previous[i] = this.currents[i];
		}

		return result;
	}

	public override void AgentReset()
	{
		// this.transform.localPosition = new Vector3(Random.Range(-11f, 11f), this.transform.localPosition.y, Random.Range(-11f, 11f));
		// this.snake.position = this.transform.parent.position + new Vector3(Random.Range(-11f, 11f), this.snake.transform.localPosition.y, Random.Range(-11f, 11f));
		
		// if(this.ctr != null) {
		// 	this.ctr.target = null;
		// 	this.ctr.lastShootTime = 0f;
		// 	this.ctr.aimStartTime = 0f;
		// }
	}

	public override void CollectObservations()
	{
		this.AddVectorObs(this.isTargetAround);
		this.AddVectorObs(this.isTargetSight);
		this.AddVectorObs(this.couldShoot);
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		// Choose action and launch it with old UtilityAI
		UtilityAction selected = this.behaviour.actions[(int)vectorAction[0]];
		this.behaviour.UpdateUtilityActions(this.ctr, selected);

		// AIM -> Turret should not aim if there is no target
		if(this.behaviour.IsActionRunning(this.ctr, "Aim"))
		{
			if(this.ctr.target == null) {
				this.AddReward(-0.25f);
			} else {
				this.AddReward(0.75f);
			}
		}

		// AIM -> Turret should not aim if there is no target
		if(this.behaviour.IsActionRunning(this.ctr, "Wander"))
		{
			if(this.ctr.target == null) {
				this.AddReward(0.75f);
			} else {
				this.AddReward(-0.25f);
			}
		}
	}

	private void RewardOnNewAction(string actionName)
	{
		switch(actionName)
		{
			// SHOOT -> Turret should shoot the less possible
			case "Shoot":
				if(this.couldShoot) {
					this.AddReward(0.5f);
				}
				if(this.isTargetSight) {
					this.AddReward(0.5f);
				}
				break;
		}
	}

	public void RewardOnHit()
	{
		// HIT -> Turret receives huge reward when hiting something
		this.AddReward(1f);
	}
}
