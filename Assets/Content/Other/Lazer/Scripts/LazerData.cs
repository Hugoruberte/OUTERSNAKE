using UnityEngine;
using System.Collections.Generic;
using Lazers;

[CreateAssetMenu(fileName = "LazerData", menuName = "Scriptable Object/Data/LazerData", order = 3)]
public class LazerData : ScriptableObject
{
	public enum LazerMode {
		Shot = 0,
		Continuous
	}

	// Parameters
	[HideInInspector] public float speed;
	[HideInInspector] public float lifetime;
	[HideInInspector] public bool bounce;
	[HideInInspector] public bool flatten;
	[HideInInspector] public LazerMode mode = LazerMode.Shot;
	[HideInInspector] public GameObject lazerImpactPrefab;
	[HideInInspector] public GameObject groundImpactPrefab;

	// Layer Mask
	[HideInInspector] public LayerMask hitLayerMask;
	[HideInInspector] public LayerMask bounceLayerMask;

	// Bounce
	[HideInInspector] public int maxBounceCount;
	[HideInInspector] public LastBounceMode lastBounceMode;
	[HideInInspector] public float coneAngle;
	[HideInInspector] public float gravityForceMultiplier;
	[HideInInspector] public float forwardForceMultiplier;
	[HideInInspector] public float forceDampling;

	// Length
	[HideInInspector] public float length;
	
	// Width
	[HideInInspector] public float width;
	[HideInInspector] public float widthSpeed;

	// Width Point
	[HideInInspector] public float widthPointSpeed;
	[HideInInspector] public Vector2 distancePerPointMinMax;

	// Auto Aim
	[HideInInspector] public bool autoAim;
	[HideInInspector] public float autoAimRange;
	[HideInInspector] public float autoAimThreshold;
	[HideInInspector] public LayerMask autoAimLayerMask;
}