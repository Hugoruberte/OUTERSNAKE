using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.AI;

public abstract class UtilityAIBehaviour : ScriptableObject
{
	[System.NonSerialized] private List<CAA> caas = new List<CAA>();

	private static List<UtilityAction> _all = new List<UtilityAction>();
	
	[System.NonSerialized] public float lastUpdate;
	[HideInInspector] public List<UtilityAction> actions = new List<UtilityAction>();
	[HideInInspector] public float updateRate = 0.02f;

	

	public void UpdateUtilityActions()
	{
		CAA caa;
		UtilityAction selected;

		// foreach controller
		for(int i = 0; i < this.caas.Count; i++) {

			// caa links a controller to its currently running actions
			// -> its 'ctr' field is the controller (i.e. a bunny)
			// -> its 'main' field contains a running action which cannot be parallelizable
			caa = this.caas[i];

			// select best action by score
			selected = UtilityAI.Select(caa.ctr, this.actions);
			
			// if there is already an action running (i.e. it is a coroutine)
			if(caa.main != null && caa.main.isRunning)
			{
				// if best action is not one of the current actions
				if(!caa.IsRunning(selected)) {

					// if selected is parallelizable && current allows it
					if(selected.isParallelizable && !caa.main.isForceAlone) {
						// start selected action
						caa.StartAction(selected);
					}

					// if current is stoppable
					else if(caa.main.isStoppable) {
						// stop current action
						caa.StopMainAction();

						// start selected action
						caa.StartAction(selected);
					}
				}

				// here either :
				// - current is unstoppable or
				// - current does not allow parallelization or
				// - selected is already running or
				// - selected has been launch.
			}
			// if current is not running (i.e. there is no 'current' or it is not a coroutine)
			else
			{
				// start selected action
				caa.StartAction(selected);
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
		this.caas.Add(caa);
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
		CAA caa = this.caas.Find(x => x.ctr.entity == ent);

		// Remove from registred caas
		this.caas.Remove(caa);

		// Stop all actions
		caa.StopAllActions();
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
		CAA caa = this.caas.Find(x => x.ctr.entity == ent);
		_all.AddRange(caa.parallelizables);
		_all.Add(caa.main);
		UtilityAction[] result = _all.ToArray();
		_all.Clear();
		return result;
	}
	public void UpdateAllMaxCache() {
		foreach(UtilityAction a in this.actions) {
			a.UpdateCacheMax();
		}
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

			this.CleanActions();

			if(act.isParallelizable) {
				this.parallelizables.Add(act);
			} else {
				this.main = act;
			}
		}

		public void StartAction(UtilityAction act)
		{
			// if action need to run alone
			if(act.isForceAlone) {
				// stop all other running actions
				this.StopAllActions();
			}

			// start action
			act?.Start(this.ctr, this.ctr.entity);
			this.AddAction(act);
		}

		public void StopMainAction() => this.main?.Stop(this.ctr.entity);

		public void StopAllActions() {
			this.main?.Stop(this.ctr.entity);
			foreach(UtilityAction a in this.parallelizables) {
				a?.Stop(this.ctr.entity);
			}
			this.parallelizables.Clear();
		}

		public bool IsRunning(UtilityAction act) {
			this.CleanActions();

			if(this.main.method.Equals(act.method)) {
				return true;
			}

			foreach(UtilityAction a in this.parallelizables) {
				if(a.method.Equals(act.method)) {
					return true;
				}
			}

			return false;
		}

		private void CleanActions() {
			if(this.main != null && !this.main.isRunning) {
				this.main = null;
			}

			for(int i = this.parallelizables.Count - 1; i >= 0; i--) {
				if(this.parallelizables[i] == null || !this.parallelizables[i].isRunning) {
					this.parallelizables.RemoveAt(i);
				}
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