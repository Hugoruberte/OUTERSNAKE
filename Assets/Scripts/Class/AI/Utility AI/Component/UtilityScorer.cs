using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I am obligated to use an 'isCondition' variable instead of create
// two subclass like it is done below because of Unity serialization
// which do not manage polymorphism and inheritance...
// I am as sad as you.

[System.Serializable]
public class UtilityScorer
{
	public bool isCondition = true;
	public bool not = false;
	private bool initialized = false;

	// score
	public int score = 0;
	public AnimationCurve curve = null;

	// inspector
	public string method;
	public int index = 0;

	// action
	private System.Func<MovementController, bool> condition;
	private System.Func<MovementController, float> mapper;


	public UtilityScorer(bool c)
	{
		this.isCondition = c;
	}

	public int Score(MovementController ctr)
	{
		int res;
		bool cond;

		if(!this.initialized) {
			return 0;
		}

		if(this.isCondition) {
			cond = this.condition(ctr);
			res = ((!cond && this.not) || (cond && !this.not)) ? score : 0;
		} else {
			res = Mathf.RoundToInt(this.curve.Evaluate(this.mapper(ctr)));
		}

		return res;
	}

	public int Max()
	{
		int max;

		max = int.MinValue;

		if(isCondition) {
			max = score;
		} else {
			int val;
			for(float step = 0f; step <= 1f; step += 0.1f) {
				val = Mathf.RoundToInt(this.curve.Evaluate(step));
				if(val > max) {
					max = val;
				}
			}
		}

		return max;
	}

	public void Initialize(UtilityAIBehaviour target)
	{
		if(this.isCondition)
		{
			this.condition = System.Func<MovementController, bool>.CreateDelegate(typeof(System.Func<MovementController, bool>), target, target.GetType().GetMethod(method)) as System.Func<MovementController, bool>;
		}
		else
		{
			this.mapper = System.Func<MovementController, float>.CreateDelegate(typeof(System.Func<MovementController, float>), target, target.GetType().GetMethod(method)) as System.Func<MovementController, float>;
		}

		this.initialized = true;
	}
}











/*[System.Serializable]
public abstract class UtilityScorer
{
	// score
	public int score = 0;
	public AnimationCurve curve = null;

	// inspector
	protected UtilityAIBehaviour target;
	public string method;
	public int index = 0;


	public UtilityScorer(UtilityAIBehaviour t)
	{
		this.target = t;
	}

	public abstract int Check();
	public abstract void Initialize();
}

[System.Serializable]
public class UtilityScorerCondition : UtilityScorer
{
	private System.Func<bool> act;

	public UtilityScorerCondition(UtilityAIBehaviour t) : base(t){}

	public override int Check()
	{
		return (this.act()) ? score : 0;
	}

	public override void Initialize()
	{
		this.act = System.Func<bool>.CreateDelegate(typeof(System.Func<bool>), target, target.GetType().GetMethod(method)) as System.Func<bool>;
	}
}

[System.Serializable]
public class UtilityScorerCurve : UtilityScorer
{
	private System.Func<float> act;

	public UtilityScorerCurve(UtilityAIBehaviour t) : base(t)
	{
		this.curve = new AnimationCurve();
	}

	public override int Check()
	{
		return Mathf.RoundToInt(this.curve.Evaluate(this.act()));
	}

	public override void Initialize()
	{
		this.act = System.Func<float>.CreateDelegate(typeof(System.Func<float>), target, target.GetType().GetMethod(method)) as System.Func<float>;
	}
}*/