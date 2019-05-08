using UnityEngine;
using Interactive.Engine;

[RequireComponent(typeof(Rigidbody))]
public abstract class LivingEntity : InteractiveEntity
{
	[HideInInspector] public UtilityAIBehaviour behaviour;

	protected virtual void Death()
	{
		this.behaviour.Remove(this);
	}
}
