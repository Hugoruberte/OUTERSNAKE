using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.AI;

public abstract class UtilityAIBehaviour : MonoBehaviour
{
	private class CAA {
		public readonly MovementController ctr;
		public UtilityAction act;

		public CAA(MovementController c, UtilityAction a) {
			this.ctr = c;
			this.act = a;
		}
	}

	// inspector cache
	[HideInInspector]
	public string[] actionCandidates = {};
	[HideInInspector]
	public string[] scorerConditionCandidates = {};
	[HideInInspector]
	public string[] scorerCurveCandidates = {};
	[HideInInspector]
	public List<bool> displayScorers = new List<bool>();
	[HideInInspector]
	public List<UtilityAction> actions = new List<UtilityAction>();



	private UtilityAI utilityAI;
	
	private float lastUpdate = 0f;
	public float updateRate = 0.02f;

	private List<CAA> controllers;


	protected virtual void Awake()
	{
		this.utilityAI = new UtilityAI(this.actions, this);
		this.controllers = new List<CAA>();
	}

	protected virtual void Start()
	{
		// empty
	}

	protected virtual void Update()
	{
		if(Time.time - lastUpdate < updateRate) {
			return;
		}

		lastUpdate = Time.time;

		this.UpdateUtilityActions();
	}

	private void UpdateUtilityActions()
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
						current.Stop(this);

						// start selected action
						act.Start(ctr, this);
						this.controllers[i].act = act;
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
			act?.Start(ctr, this);
			this.controllers[i].act = act;
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
	public void AddAction()
	{
		UtilityAction action = new UtilityAction();
		this.actions.Add(action);
	}

	public void RemoveActionAt(int index)
	{
		this.actions.RemoveAt(index);
	}

	public UtilityAction GetCurrentAction(LivingEntity ent)
	{
		CAA caa = this.controllers.Find(x => x.ctr.entity == ent);
		return caa?.act;
	}
}
