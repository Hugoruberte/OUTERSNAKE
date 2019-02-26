using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class UtilityAction
{
	// scorers
	public List<UtilityScorer> scorers;

	// inspector
	public int index = 0;
	public int score = 0;
	public string method = "";
	public bool active = true;

	// invocation
	public System.Action<MovementController> action;
	public System.Func<MovementController, UtilityAction, IEnumerator> coroutineFactory;
	private IEnumerator coroutine;

	// state
	public bool isStoppable = false; // need to be change by the coroutine launched by this UtilityAction !
	public bool isRunning = false; // need to be change by the coroutine launched by this UtilityAction !
	



	public UtilityAction()
	{
		this.action = null;
		this.coroutineFactory = null;
		this.scorers = new List<UtilityScorer>();
	}

	public int Score(MovementController ctr)
	{
		score = 0;
		foreach(UtilityScorer s in this.scorers) {
			score += s.Score(ctr);
		}

		return score;
	}

	public int Max()
	{
		int max;

		max = 0;
		foreach(UtilityScorer s in this.scorers) {
			max += s.Max();
		}

		return max;
	}

	public void Start(MovementController ctr, UtilityAIBehaviour main)
	{
		if(this.action != null) {
			this.isRunning = false;
			this.action(ctr);
		}
		else {
			this.coroutine = this.coroutineFactory(ctr, this);
			this.isRunning = true;
			main.StartCoroutine(this.coroutine);
		}
	}

	public void Stop(UtilityAIBehaviour main)
	{
		if(this.action != null) {
			Debug.LogError("ERROR: Could not stop this UtilityAction because it is not a coroutine !");
			return;
		}

		main.StopCoroutine(this.coroutine);
	}







	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------------------- UTIL FUNCTION ---------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public void Initialize(string a, UtilityAIBehaviour target)
	{
		MethodInfo methodInfo;

		methodInfo = target.GetType().GetMethod(method);

		if(methodInfo.ReturnType == typeof(void)) {
			this.action = System.Action<MovementController>.CreateDelegate(typeof(System.Action<MovementController>), target, methodInfo) as System.Action<MovementController>;
		} else {
			this.coroutineFactory = System.Func<MovementController, UtilityAction, IEnumerator>.CreateDelegate(typeof(System.Func<MovementController, UtilityAction, IEnumerator>), target, methodInfo) as System.Func<MovementController, UtilityAction, IEnumerator>;
		}

		foreach(UtilityScorer scorer in this.scorers) {
			scorer.Initialize(target);
		}

		this.Check(a, target);
	}

	private void Check(string a, UtilityAIBehaviour target)
	{
		UtilityScorer scorer;
		string[] methods = new string[this.scorers.Count];
		for(int i = 0; i < this.scorers.Count; i++) {
			scorer = this.scorers[i];
			if(Array.IndexOf(methods, scorer.method) >= 0) {
				Debug.LogWarning($"WARNING : The scorer '{scorer.method}' is defined multiples times for the same action '{a}' !", target.transform);
			} else {
				methods[i] = scorer.method;
			}
		}
	}







	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ----------------------------------- INSPECTOR FUNCTIONS ------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public void AddCondition()
	{
		UtilityScorer scorer = new UtilityScorer(true);
		this.scorers.Add(scorer);
	}

	public void AddCurve()
	{
		UtilityScorer scorer = new UtilityScorer(false);
		this.scorers.Add(scorer);
	}

	public void RemoveScorerAt(int index)
	{
		this.scorers.RemoveAt(index);
	}
}