using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.AI;

public abstract class UtilityAIBehaviour : ScriptableObject
{
	[System.Serializable]
	private struct CAA {
		public readonly MovementController ctr;
		public UtilityAction act;

		public CAA(MovementController c, UtilityAction a) {
			this.ctr = c;
			this.act = a;
		}

		public void UpdateAct(UtilityAction a) => this.act = a;
	}

	// inspector cache
	[HideInInspector] public string[] actionCandidates = {};
	[HideInInspector] public string[] scorerConditionCandidates = {};
	[HideInInspector] public string[] scorerCurveCandidates = {};
	[HideInInspector] public List<bool> displayScorers = new List<bool>();
	[HideInInspector] public List<UtilityAction> actions = new List<UtilityAction>();



	private UtilityAI utilityAI;
	
	[HideInInspector]
	public float lastUpdate = 0f;
	public float updateRate = 0.02f;

	private List<CAA> controllers = new List<CAA>();


	private void Awake()
	{
		this.utilityAI = new UtilityAI(this.actions, this);
	}

	public virtual void Initialize() {}
	public virtual void Start() {}

	public void UpdateUtilityActions(UtilityAIManager manager)
	{
		UtilityAction act;
		UtilityAction current;
		MovementController ctr;

		// foreach controller
		for(int i = 0; i < this.controllers.Count; i++) {

			ctr = this.controllers[i].ctr;
			current = this.controllers[i].act;

			// if current is running
			if(current != null && current.isRunning)
			{
				// if current is stoppable
				if(current.isStoppable)
				{
					// select best action by score
					act = utilityAI.Select(ctr);

					// if best action is not the current action
					if(current != act) {
						// stop current
						current.Stop(manager);

						// start selected action
						act.Start(ctr, manager);
						this.controllers[i].UpdateAct(act);
					}
				}

				// either current is unstoppable or a
				// new best action has been launch or
				// current action is the best action.
				continue;
			}

			// select best action by score
			act = utilityAI.Select(ctr);

			// start selected action
			act?.Start(ctr, manager);
			this.controllers[i].UpdateAct(act);
		}
	}

	protected void AddController(MovementController ctr)
	{
		CAA caa = new CAA(ctr, null);
		this.controllers.Add(caa);
	}

	public void Remove(LivingEntity ent)
	{
		// only does that
		this.controllers.RemoveAll(caa => caa.ctr.entity == ent);
	}







	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* ----------------------------------- INSPECTOR FUNCTION ------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	public void AddAction() => this.actions.Add(new UtilityAction());
	public void RemoveActionAt(int index) => this.actions.RemoveAt(index);
	public UtilityAction GetCurrentAction(LivingEntity ent) => this.controllers.Find(x => x.ctr.entity == ent).act;
}
