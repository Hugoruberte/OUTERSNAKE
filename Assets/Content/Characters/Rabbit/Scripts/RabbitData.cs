using UnityEngine;

[CreateAssetMenu(fileName = "RabbitData", menuName = "Scriptable Object/Data/RabbitData", order = 3)]
public class RabbitData : ScriptableObject
{
	// Parameters
	[HideInInspector] public float speed = 2f;
	[HideInInspector] public float rangeOfView;
	[HideInInspector] public int maxStepDistance;

	// Jump
	[HideInInspector] public float jumpHeight = 2f;
	[HideInInspector] public float[] afterJumpTempoInterval = new float[2];
}
