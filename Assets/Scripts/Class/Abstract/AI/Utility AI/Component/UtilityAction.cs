using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using My.Tools;
using My.Events;

namespace Utility.AI
{
	[System.Serializable]
	public class UtilityAction
	{
		// scorers
		public List<UtilityScorer> scorers = new List<UtilityScorer>();

		// inspector
		public int index = 0;
		public int score = 0;
		public string method = "";
		public bool active = true;
		public bool discarded_active = false;
		public bool discarded_max = false;
		[SerializeField] private int _max = int.MaxValue;

		// invocation
		private System.Action<MovementController> action = null;
		private System.Func<MovementController, IEnumerator> coroutineFactory_1 = null;
		private System.Func<MovementController, UtilityAction, IEnumerator> coroutineFactory_2 = null;
		private IEnumerator coroutine = null;

		// state
		[SerializeField] private bool _isParallelizable = false;
		public bool isParallelizable {
			get { return this._isParallelizable; }
			set {
				this._isParallelizable = value;
				if(value) {
					this._isForceAlone = false;
				}
			}
		}
		[SerializeField] private bool _isForceAlone = false;
		public bool isForceAlone {
			get { return this._isForceAlone; }
			set {
				this._isForceAlone = value;
				if(value) {
					this._isParallelizable = false;
				}
			}
		}
		public bool isStoppable = false;
		public bool isRunning { get; private set; } = false;

		// Event
		public ActionEvent onStart = null;
		public ActionEvent onEnd = null;
		



		public UtilityAction(string method, int index)
		{
			this.index = index;
			this.method = method;
		}

		public int Score(MovementController ctr)
		{
			this.score = 0;
			foreach(UtilityScorer s in this.scorers) {
				this.score += s.Score(ctr);
			}

			return this.score;
		}

		public void Start(MovementController ctr, MonoBehaviour handler)
		{
			this.onStart?.Invoke();

			if(this.action != null) {
				this.isRunning = false;
				this.action(ctr);
				this.onEnd?.Invoke();
			} else {
				this.coroutine = this.HandlerCoroutine(ctr);
				handler.StartCoroutine(this.coroutine);
			}
		}

		public void Stop(MonoBehaviour handler)
		{
			if(this.action != null) {
				Debug.LogWarning("Warning: Could not stop this UtilityAction because it is not a coroutine !");
				return;
			}

			handler.TryStopCoroutine(ref this.coroutine);

			this.onEnd?.Invoke();
		}







		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* -------------------------------------- UTIL FUNCTION ---------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		public void Initialize(UtilityAIBehaviour t)
		{
			MethodInfo methodInfo;

			this.isRunning = false;

			methodInfo = t.GetType().GetMethod(method);

			if(methodInfo.ReturnType == typeof(void)) {
				this.action = System.Action<MovementController>.CreateDelegate(typeof(System.Action<MovementController>), t, methodInfo) as System.Action<MovementController>;
			} else {
				try {
					this.coroutineFactory_1 = System.Func<MovementController, IEnumerator>.CreateDelegate(typeof(System.Func<MovementController, IEnumerator>), t, methodInfo) as System.Func<MovementController, IEnumerator>;
				} catch {
					try {
						this.coroutineFactory_2 = System.Func<MovementController, UtilityAction, IEnumerator>.CreateDelegate(typeof(System.Func<MovementController, UtilityAction, IEnumerator>), t, methodInfo) as System.Func<MovementController, UtilityAction, IEnumerator>;
					} catch {
						Debug.LogWarning($"WARNING : This method '{method}' is not suitable to be an UtilityAction. Check its arguments.");
					}
				}
			}

			foreach(UtilityScorer sc in this.scorers) {
				sc.Initialize(t);
			}
		}

		public int Max()
		{
			if(this._max < int.MaxValue) {
				return this._max;
			}

			int max, smax;

			max = 0;
			foreach(UtilityScorer s in this.scorers) {
				smax = s.Max();
				if(smax > 0) {
					max += smax;
				}
			}

			this._max = max;
			return max;
		}

		public void UpdateCacheMax()
		{
			foreach(UtilityScorer s in this.scorers) {
				s.UpdateCacheMax();
			}
			this._max = int.MaxValue;
			this.Max();
		}

		private IEnumerator HandlerCoroutine(MovementController ctr)
		{
			this.isRunning = true;

			IEnumerator co = (this.coroutineFactory_2 == null) ? this.coroutineFactory_1(ctr) : this.coroutineFactory_2(ctr, this);
			yield return co;

			this.onEnd?.Invoke();
			this.isRunning = false;
		}








		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* ----------------------------------- INSPECTOR FUNCTIONS ------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		public void AddScorer(string m, bool ic, int i)
		{
			UtilityScorer scorer = new UtilityScorer(ic, m, i);
			this.scorers.Add(scorer);
		}

		public void RemoveScorerAt(int index)
		{
			// only does that
			this.scorers.RemoveAt(index);
		}
	}
}
