using UnityEngine;

[CreateAssetMenu(fileName = "LazerTrapData", menuName = "Scriptable Object/Data/LazerTrapData", order = 3)]
public class LazerTrapData : ScriptableObject
{
	// Prefab
	[HideInInspector] public GameObject lazerPrefab;
	
	// Parameters
	[HideInInspector] public float rangeOfView;
	[HideInInspector] public LayerMask targetLayerMask;

	// Aim
	[HideInInspector] public float aimOmega;

	// Wander
	[HideInInspector] public float wanderOmega;
	[HideInInspector] public float[] wanderRotationDurationInterval = new float[2];
	[HideInInspector] public int wanderMaxPauseQuarter;

	// Shoot
	[HideInInspector] public float shootDelay;
}