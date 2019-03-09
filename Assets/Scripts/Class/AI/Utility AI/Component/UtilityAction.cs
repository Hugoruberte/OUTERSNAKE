using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

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
	public System.Action<MovementController> action = null;
	public System.Func<MovementController, UtilityAction, IEnumerator> coroutineFactory = null;
	private IEnumerator coroutine = null;

	// state
	[System.NonSerialized]
	public bool isStoppable = false; // to be changed by the coroutine launched by this UtilityAction !
	public bool isRunning { get; private set; } = false;
	



	public UtilityAction(string method, int index)
	{
		this.index = index;
		this.method = method;
	}

	public int Score(MovementController ctr)
	{
		score = 0;
		foreach(UtilityScorer s in this.scorers) {
			score += s.Score(ctr);
		}

		return score;
	}

	public void Start(MovementController ctr, UtilityAIManager main)
	{
		if(this.action != null) {
			this.isRunning = false;
			this.action(ctr);
		} else {
			if(this.coroutine != null) {
				Debug.Log("Ddhdhzdqnioufsdjjjjjjjjjjjjjjjjjjjj");
			} else {
				Debug.Log("Need to remove this...");
			}
			this.coroutine = this.CoroutineManager(ctr);
			main.StartCoroutine(this.coroutine);
		}
	}

	public void Stop(UtilityAIManager main)
	{
		if(this.action != null) {
			Debug.LogError("ERROR: Could not stop this UtilityAction because it is not a coroutine !");
			return;
		}

		main.StopCoroutine(this.coroutine);
		this.coroutine = null;
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
			this.coroutineFactory = System.Func<MovementController, UtilityAction, IEnumerator>.CreateDelegate(typeof(System.Func<MovementController, UtilityAction, IEnumerator>), t, methodInfo) as System.Func<MovementController, UtilityAction, IEnumerator>;
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

	private IEnumerator CoroutineManager(MovementController ctr)
	{
		this.isRunning = true;

		yield return this.coroutineFactory(ctr, this);

		this.isRunning = false;
	}








	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ----------------------------------- INSPECTOR FUNCTIONS ------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public void AddCondition(string method)
	{
		UtilityScorer scorer = new UtilityScorer(true, method);
		this.scorers.Add(scorer);
	}

	public void AddCurve(string method)
	{
		UtilityScorer scorer = new UtilityScorer(false, method);
		this.scorers.Add(scorer);
	}

	public void RemoveScorerAt(int index)
	{
		this.scorers.RemoveAt(index);
	}
}