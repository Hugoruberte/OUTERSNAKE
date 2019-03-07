using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LazerData", menuName = "Scriptable Object/Data/LazerData", order = 3)]
public class LazerData : ScriptableObject
{
	[Header("Paramaters")]
	public float speed;
	public float lifetime;
	
	[Header("Min Max Point per Distance")]
	public Vector2 pointPerDistanceMinMax;

	[Header("Width")]
	public float initialWidth;
	public float widthPointSpeed;
	public float widthSpeed;
}