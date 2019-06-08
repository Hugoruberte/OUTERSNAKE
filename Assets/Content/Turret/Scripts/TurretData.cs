using UnityEngine;

[CreateAssetMenu(fileName = "TurretData", menuName = "Scriptable Object/Data/TurretData", order = 3)]
public class TurretData : ScriptableObject
{
	// Prefabs
	[HideInInspector] public GameObject lazerPrefab;
	[HideInInspector] public GameObject loadPrefab;
	[HideInInspector] public GameObject shootPrefab;
	[HideInInspector] public GameObject haloPrefab;
	
	// Parameters
	[HideInInspector] public float rangeOfView;
	[HideInInspector] public LayerMask targetLayerMask;

	// Aim
	[HideInInspector] public float aimOmega;
	[HideInInspector] public float aimDuration;
	[HideInInspector] public float aimTargetSmooth;
	[HideInInspector] public GameObject targetPrefab;

	// Wander
	[HideInInspector] public float wanderOmega;
	[HideInInspector] public float[] wanderRotationDurationInterval = new float[2];
	[HideInInspector] public int wanderMaxPauseQuarter;

	// Shoot
	[HideInInspector] public float loadDuration;
	[HideInInspector] public float shootDelay;
}