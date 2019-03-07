using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snakes;

[System.Serializable]
[CreateAssetMenu(fileName = "SnakeData", menuName = "Scriptable Object/Snake/SnakeData", order = 3)]
public class SnakeData : ScriptableObject
{
	[Header("Motion")]
	[Range(0.0f, 100.0f)]
	public float speed = 10f;
	[Range(0.01f, 0.2f)]
	public float positionAccuracy = 0.1f;
	
	[System.NonSerialized]
	public bool cancelInput = false;

	[Header("States")]
	public SnakeMoveState state = SnakeMoveState.Idle;
}


