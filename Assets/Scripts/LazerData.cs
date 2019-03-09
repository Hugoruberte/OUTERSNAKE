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

	public float speed;
	public float lifetime;
	public bool bounce;
	public LazerMode mode = LazerMode.Shot;

	public bool easyAim;

	public float length;
	
	public float width;
	public float widthSpeed;

	public float widthPointSpeed;
	public Vector2 distancePerPointMinMax;
}