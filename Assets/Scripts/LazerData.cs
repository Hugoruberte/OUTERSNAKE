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

	[Header("Parameters")]
	public float speed;
	public float lifetime;
	public LazerMode mode = LazerMode.Shot;
	
	[Header("Min Max Distance per Point")]
	public Vector2 distancePerPointMinMax;

	[Header("Width")]
	public float initialWidth;
	public float widthPointSpeed;
	public float widthSpeed;
}