using UnityEngine;
using Interactive.Engine;

[RequireComponent(typeof(Rigidbody))]
public abstract class LivingEntity : InteractiveEntity
{
	// standard variable, not necessarily used
	[HideInInspector] public float speed = 2;
	[HideInInspector] public int maxStepDistance = 2;
	[HideInInspector] public float rangeOfView = 5f;

	[HideInInspector] public bool isTired = false;
	[HideInInspector] public bool isHungry = false;
	[HideInInspector] public bool isScared = false;

	// AI
	[HideInInspector] public UtilityAIBehaviour behaviour;

	



	/* ---------------------------------------------------------------------------------------------*/
	/* ------------------------------------------- LIFE --------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	protected override void OnUpdateLife() {
		base.OnUpdateLife();

		if(this.life <= 0) {
			this.Death();
		}
	}

	protected virtual void Death() {
		// empty
	}
}
