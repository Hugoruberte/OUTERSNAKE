using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Utility.AI
{
	public class UtilityAI
	{
		private List<UtilityAction> actions = null;
		private List<UtilityAction> selected = null;

		public UtilityAI(List<UtilityAction> acts, UtilityBehaviourAI target)
		{
			this.actions = acts;
			this.selected = new List<UtilityAction>();

			this.Initialize(target);
		}

		public UtilityAction Select(MovementController ctr)
		{
			UtilityAction result;
			int max;
			int score;
			int nb;

			score = int.MinValue;
			max = int.MinValue;
			nb = 0;

			foreach(UtilityAction act in this.actions) {
				// discard this action, it is not active
				if(!act.active) {
					continue;
				}

				// discard this action, best it can do won't be greater that the max
				if(act.Max() < max) {
					continue;
				}

				score = act.Score(ctr);
				// Debug.Log(" -> " + act.method + " score = " + score);
				
				if(score > max) {
					max = score;
					this.selected.Clear();
					this.selected.Add(act);
					nb = 1;
				} else if(score == max) {
					this.selected.Add(act);
					nb ++;
				}
			}

			// Debug.Log(" \n ");

			if(nb == 1) {
				result = this.selected[0];
			} else if(nb > 1) {
				result = this.selected[UnityEngine.Random.Range(0, nb)];
			} else {
				Debug.LogError("ERROR : Could not find a UtilityAction, check it out dude.");
				result = null;
			}

			this.selected.Clear();

			return result;
		}

		private void Initialize(UtilityBehaviourAI target)
		{
			if(this.actions == null) {
				Debug.LogError("ERROR: Could not initialize AI, maybe the initialization was called in an Awake function instead of a Start function.");
				return;
			}

			foreach(UtilityAction action in this.actions) {
				action.Initialize(action.method, target);
			}

			this.Check(target);
		}

		private void Check(UtilityBehaviourAI target)
		{
			UtilityAction action;
			string[] methods = new string[this.actions.Count];
			for(int i = 0; i < this.actions.Count; i++) {
				action = this.actions[i];
				if(Array.IndexOf(methods, action.method) >= 0) {
					Debug.LogWarning($"WARNING : The action '{action.method}' is defined multiples times in inspector !", target.transform);
				} else {
					methods[i] = action.method;
				}
			}
		}
	}
}

