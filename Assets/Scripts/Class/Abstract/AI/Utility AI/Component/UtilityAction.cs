using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using Tools;

[System.Serializable]
public class UtilityAction
{
	// scorers
	public List<UtilityScorer> scorers = new List<UtilityScorer>();

	private int _max = int.MinValue;
	public int max {
		get {
			if(_max == int.MinValue) { _max = this.Max(); }
			return _max;
		}
	}

	// inspector
	public int index = 0;
	public int score = 0;
	public string method = "";
	public bool active = true;

	// invocation
	private System.Action<MovementController> action = null;
	private System.Func<MovementController, IEnumerator> coroutineFactory_1 = null;
	private System.Func<MovementController, UtilityAction, IEnumerator> coroutineFactory_2 = null;
	private IEnumerator coroutine = null;

	// state
	public bool isStoppable = false;
	public bool isParallelizable = false;
	public bool isRunning { get; private set; } = false;
	



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
		if(this.action != null) {
			this.isRunning = false;
			this.action(ctr);
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

		this.isStoppable = false;
		this.isRunning = false;

		methodInfo = t.GetType().GetMethod(method);

		if(methodInfo.ReturnType == typeof(void)) {
			this.action = System.Action<MovementController>.CreateDelegate(typeof(System.Action<MovementController>), t, methodInfo) as System.Action<MovementController>;
		} else {
			if(methodInfo.GetGenericArguments().Length == 2) {
				this.coroutineFactory_1 = System.Func<MovementController, IEnumerator>.CreateDelegate(typeof(System.Func<MovementController, IEnumerator>), t, methodInfo) as System.Func<MovementController, IEnumerator>;
			} else {
				this.coroutineFactory_2 = System.Func<MovementController, UtilityAction, IEnumerator>.CreateDelegate(typeof(System.Func<MovementController, UtilityAction, IEnumerator>), t, methodInfo) as System.Func<MovementController, UtilityAction, IEnumerator>;
			}
		}

		foreach(UtilityScorer sc in this.scorers) {
			sc.Initialize(t);
		}
	}

	private int Max()
	{
		int max;

		max = 0;
		foreach(UtilityScorer s in this.scorers) {
			max += s.Max();
		}

		return max;
	}

	private IEnumerator HandlerCoroutine(MovementController ctr)
	{
		this.isRunning = true;

		IEnumerator co = (this.coroutineFactory_2 == null) ? this.coroutineFactory_1(ctr) : this.coroutineFactory_2(ctr, this);
		yield return co;

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