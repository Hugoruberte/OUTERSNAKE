using UnityEngine;
using Interactive.Engine;

[RequireComponent(typeof(Rigidbody))]
public abstract class LivingEntity : InteractiveEntity
{
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
