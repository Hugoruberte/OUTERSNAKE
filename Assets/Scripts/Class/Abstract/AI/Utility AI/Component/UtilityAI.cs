using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.AI
{
	public static class UtilityAI
	{
		private static List<UtilityAction> _selected = new List<UtilityAction>();

		public static UtilityAction Select(MovementController ctr, List<UtilityAction> actions)
		{
			UtilityAction result;
			int max, score;
			int count;

			result = null;
			max = int.MinValue;

			foreach(UtilityAction act in actions) {
				// discard this action, it is not active
				if(!act.active) {
					act.discarded_active = true;
					continue;
				}
				act.discarded_active = false;

				// discard this action, best it can do won't be greater than the current max
				if(act.Max() < max) {
					act.discarded_max = true;
					continue;
				}
				act.discarded_max = false;

				score = act.Score(ctr);
				
				if(score > max) {
					max = score;
					_selected.Clear();
					_selected.Add(act);
				} else if(score == max) {
					_selected.Add(act);
				}
			}

			count = _selected.Count;

			if(count > 0) {
				result = _selected[UnityEngine.Random.Range(0, count)];
			} else {
				Debug.LogWarning("WARNING : This Utility AI does not have any action.");
				result = null;
			}

			_selected.Clear();

			return result;
		}
	}
}

