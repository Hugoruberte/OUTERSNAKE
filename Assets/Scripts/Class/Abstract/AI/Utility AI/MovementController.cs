using UnityEngine;

[System.Serializable]
public abstract class MovementController
{
	public readonly LivingEntity entity;
	public Vector3 position { get { return this.entity.myTransform.position; } }
	public Quaternion rotation { get { return this.entity.myTransform.rotation; } }

	public MovementController(LivingEntity e)
	{
		this.entity = e;
	}
}
