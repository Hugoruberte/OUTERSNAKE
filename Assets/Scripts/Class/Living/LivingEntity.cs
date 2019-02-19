using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class LivingEntity : CellableEntity
{
	[HideInInspector] public float speed = 2;
	[HideInInspector] public int maxStepDistance = 2;
	[HideInInspector] public float rangeOfView = 5f;

	// AI
	[HideInInspector] public UtilityBehaviourAI behaviour;
}
