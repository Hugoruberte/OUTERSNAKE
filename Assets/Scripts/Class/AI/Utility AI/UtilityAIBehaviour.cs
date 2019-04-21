﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.AI;

public abstract class UtilityAIBehaviour : ScriptableObject
{
	private UtilityAI utilityAI = new UtilityAI();

	[HideInInspector] public List<UtilityAction> actions = new List<UtilityAction>();

	[HideInInspector] public float lastUpdate = 0f;
	[HideInInspector] public float updateRate = 0.02f;

	[System.NonSerialized] private List<CAA> controllers = new List<CAA>();


	public void UpdateUtilityActions() {
		UtilityAction act;
		UtilityAction current;
		MovementController ctr;

		// foreach controller
		for(int i = 0; i < this.controllers.Count; i++) {

			ctr = this.controllers[i].ctr;
			current = this.controllers[i].act;

			// if current is running (i.e. it is a coroutine)
			if(current != null && current.isRunning)
			{
				// if current is stoppable
				if(current.isStoppable)
				{
					// select best action by score
					act = utilityAI.Select(ctr, this.actions);

					// if best action is not the current action
					if(current != act) {
						// stop current
						current.Stop(ctr.entity);

						// start selected action
						act.Start(ctr, ctr.entity);
						this.controllers[i].act = act;
					}
				}

				// either current is unstoppable or
				// a new best action has been launch or
				// current action is the best action.
			}
			// if current is not running
			else
			{
				// select best action by score
				act = utilityAI.Select(ctr, this.actions);

				// start selected action
				act?.Start(ctr, ctr.entity);
				this.controllers[i].act = act;
			}
		}
	}

	public virtual void OnAwake() {
		this.lastUpdate = 0f;
		foreach(UtilityAction act in this.actions) {
			act.Initialize(this);
		}
	}
	public virtual void OnStart() {
		// empty
	}
	public virtual void OnUpdate() {
		// empty
	}

	protected void AddController(MovementController ctr) {
		CAA caa = new CAA(ctr, null);
		this.controllers.Add(caa);
	}

	public void Remove(LivingEntity ent) {
		CAA caa = this.controllers.Find(x => x.ctr.entity == ent);

		// Remove from registred controllers
		this.controllers.Remove(caa);

		// Stop current action
		caa.act.Stop(caa.ctr.entity);
	}









	protected float MapOnRangeOfView(float value, MovementController ctr)
	{
		// mapping :
		// min = 0 --> res = 1
		// min >= rangeOfView --> res <= 0
		return (-1 / ctr.entity.rangeOfView) * value + 1f;
	}








	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* --------------------------------------- INSPECTORS ----------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	// inspector cache //
	[HideInInspector] public string[] actionCandidates = {};
	[HideInInspector] public string[] scorerCandidates = {};
	[HideInInspector] public string[] scorerConditionCandidates = {};
	[HideInInspector] public string[] scorerCurveCandidates = {};
	[HideInInspector] public List<bool> displayScorers = new List<bool>();
	// inspector cache //

	// inspector function //
	public void AddAction(string method, int index) {
		UtilityAction act = new UtilityAction(method, index);
		// act.Initialize(this);
		this.actions.Add(act);
	}
	public void RemoveActionAt(int index) {
		this.actions.RemoveAt(index);
	}
	public UtilityAction GetCurrentAction(LivingEntity ent) => this.controllers.Find(x => x.ctr.entity == ent).act;
	// inspector function //





	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------ USEFUL STRUCT BECAUSE DICTIONARY ARE NOT SERIALIZABLE -------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	[System.Serializable]
	private class CAA {
		[SerializeField] public MovementController ctr;
		[SerializeField] public UtilityAction act;

		public CAA(MovementController c, UtilityAction a) {
			this.ctr = c;
			this.act = a;
		}
	}
}

public abstract class UtilityAIBehaviour<T> : UtilityAIBehaviour where T : class
{
	public static T instance;

	public override void OnAwake()
	{
		base.OnAwake();

		instance = this as T;
	}
}