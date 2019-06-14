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
	private int previous = 0;

	[SerializeField] private Rigidbody snake = null;


	private void Awake()
	{
		this.turret = this.GetComponent<Turret>();
		
	}

	private void Start()
	{
		this.behaviour = this.turret.behaviour as TurretAI;
		
		// Debug.Log(this.behaviour, transform);
		// Debug.Log(this.turret, transform);
		this.ctr = this.behaviour.GetController(this.turret) as TurretController;

		this.ctr.onStartNewAction += this.RewardOnNewAction;
		this.turret.onHit += this.RewardOnHit;
	}

	public override void AgentReset()
	{
		this.snake.position = this.transform.parent.position + new Vector3(Random.Range(-11f, 11f), this.snake.transform.localPosition.y, Random.Range(-11f, 11f));
		this.transform.localPosition = new Vector3(Random.Range(-11f, 11f), this.transform.localPosition.y, Random.Range(-11f, 11f));

		if(this.ctr != null) {
			this.ctr.target = null;
			this.ctr.lastShootTime = 0f;
			this.ctr.aimStartTime = 0f;
		}
	}

	public override void CollectObservations()
	{
		bool isTargetAround, isTargetSight, couldShoot;

		isTargetAround = this.behaviour.IsThereTargetAround(this.ctr);
		isTargetSight = this.behaviour.IsTargetInSight(this.ctr);
		couldShoot = this.behaviour.CouldShoot(this.ctr);

		this.AddVectorObs(isTargetAround);
		this.AddVectorObs(isTargetSight);
		this.AddVectorObs(couldShoot);
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		int current = (int)vectorAction[0];

		// CHANGE OF STATE -> Turret should switch action the less possible
		if(current != this.previous) {
			this.AddReward(-0.001f);
		}
		this.previous = current;

		// Choose action and launch it with old UtilityAI
		UtilityAction selected = this.behaviour.actions[current];
		this.behaviour.UpdateUtilityActions(this.ctr, selected);


		// AIM -> Turret should aim the less possible
		if(this.behaviour.IsActionRunning(this.ctr, "Aim") && this.ctr.target == null)
		{
			this.AddReward(-0.1f);
		}
	}

	private void RewardOnNewAction(string actionName)
	{
		switch(actionName)
		{
			// SHOOT -> Turret should shoot the less possible
			case "Shoot":
				this.AddReward(-0.01f);
				break;
		}
	}

	public void RewardOnHit()
	{
		// HIT -> Turret receives huge reward when hiting something
		this.AddReward(5f);
	}
}
