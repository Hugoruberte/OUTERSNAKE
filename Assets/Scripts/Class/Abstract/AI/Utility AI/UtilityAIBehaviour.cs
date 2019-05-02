using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.AI;

public abstract class UtilityAIBehaviour : ScriptableObject
{
	[System.NonSerialized]
	private List<CAA> controllers = new List<CAA>();
	private UtilityAI utilityAI = new UtilityAI();

	[System.NonSerialized]
	private List<UtilityAction> all = new List<UtilityAction>();
	
	[HideInInspector] public List<UtilityAction> actions = new List<UtilityAction>();
	[HideInInspector] public float lastUpdate = 0f;
	[HideInInspector] public float updateRate = 0.02f;

	

	public void UpdateUtilityActions()
	{
		UtilityAction selected;
		UtilityAction current;
		MovementController ctr;

		// foreach controller
		for(int i = 0; i < this.controllers.Count; i++) {

			ctr = this.controllers[i].ctr;
			current = this.controllers[i].main;

			// if current is running (i.e. it is a coroutine)
			if(current != null && current.isRunning)
			{
				// if current is parallelizable
				if(current.isParallelizable)
				{
					// select best action by score
					selected = utilityAI.Select(ctr, this.actions);

					// start selected action
					selected?.Start(ctr, ctr.entity);
					this.controllers[i].AddAction(selected);
				}
				// if current is stoppable
				else if(current.isStoppable)
				{
					// select best action by score
					selected = utilityAI.Select(ctr, this.actions);

					// if best action is not the current action
					if(current != selected) {

						// if selected action is not parallelizable
						if(!selected.isParallelizable) {
							// stop current
							current.Stop(ctr.entity);
						}

						// start selected action
						selected.Start(ctr, ctr.entity);
						this.controllers[i].AddAction(selected);
					}
				}

				// here either :
				// - current is unstoppable or
				// - current is not parallelizable or
				// - a new best action has been launch or
				// - current action is the best action.
			}
			// if current is not running
			else
			{
				// select best action by score
				selected = utilityAI.Select(ctr, this.actions);

				// start selected action
				selected?.Start(ctr, ctr.entity);
				this.controllers[i].AddAction(selected);
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



	protected void AddController(MovementController ctr)
	{
		CAA caa = new CAA(ctr);
		this.controllers.Add(caa);
	}
	protected float MapOnRangeOfView(MovementController ctr, float value, float range)
	{
		// mapping :
		// min = 0 --> res = 1
		// min >= rangeOfView --> res <= 0
		return (-1 / range) * value + 1f;
	}



	public void Remove(LivingEntity ent)
	{
		CAA caa = this.controllers.Find(x => x.ctr.entity == ent);

		// Remove from registred controllers
		this.controllers.Remove(caa);

		// Stop all actions
		caa.main?.Stop(caa.ctr.entity);
		foreach(UtilityAction a in caa.parallelizables) {
			a?.Stop(caa.ctr.entity);
		}
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
	[HideInInspector] public List<bool> displayParameters = new List<bool>();
	// inspector cache //

	// inspector function //
	public void AddAction(string method, int index) {
		UtilityAction act = new UtilityAction(method, index);
		this.actions.Add(act);
	}
	public void RemoveActionAt(int index) {
		this.actions.RemoveAt(index);
	}
	public UtilityAction[] GetCurrentActions(LivingEntity ent) {
		CAA caa = this.controllers.Find(x => x.ctr.entity == ent);
		this.all.AddRange(caa.parallelizables);
		this.all.Add(caa.main);
		UtilityAction[] result = this.all.ToArray();
		this.all.Clear();
		return result;

	}
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
		[SerializeField] public UtilityAction main { get; private set; }
		[SerializeField] public List<UtilityAction> parallelizables { get; private set; }

		public CAA(MovementController c) {
			this.ctr = c;
			this.parallelizables = new List<UtilityAction>();
		}

		public void AddAction(UtilityAction act) {
			if(act == null) {
				return;
			}

			if(act.isParallelizable) {
				this.parallelizables.Add(act);
			} else {
				this.main = act;
			}
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