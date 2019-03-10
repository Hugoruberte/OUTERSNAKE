using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LazerData", menuName = "Scriptable Object/Data/LazerData", order = 3)]
public class LazerData : ScriptableObject
{
	public enum LazerMode {
		Shot = 0,
		Continuous
	}

	// Parameters
	public float speed;
	public float lifetime;
	public bool bounce;
	public bool flatten;
	public LazerMode mode = LazerMode.Shot;

	// Layer Mask
	public LayerMask hitLayerMask;
	public LayerMask bounceLayerMask;

	// Cheat
	public bool easyAim;

	// Length
	public float length;
	
	// Width
	public float width;
	public float widthSpeed;

	// Width Point
	public float widthPointSpeed;
	public Vector2 distancePerPointMinMax;
}