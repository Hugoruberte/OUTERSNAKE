using UnityEngine;
using System;
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

	// Layer Mask
	[HideInInspector] public LayerMask hitLayerMask;
	[HideInInspector] public LayerMask bounceLayerMask;

	// Miscellaneous
	[HideInInspector] public bool autoAim;

	// Bounce
	[HideInInspector] public int maxBounceCount;
	[HideInInspector] public LastBounceMode lastBounceMode;
	[HideInInspector] public float coneAngle = 45f;
	[HideInInspector] public float gravityForceMultiplier = 10f;
	[HideInInspector] public float forwardForceMultiplier = 10f;
	[HideInInspector] public float forceDampling = 0.5f;

	// Length
	[HideInInspector] public float length;
	
	// Width
	[HideInInspector] public float width;
	[HideInInspector] public float widthSpeed;

	// Width Point
	[HideInInspector] public float widthPointSpeed;
	[HideInInspector] public Vector2 distancePerPointMinMax;
}