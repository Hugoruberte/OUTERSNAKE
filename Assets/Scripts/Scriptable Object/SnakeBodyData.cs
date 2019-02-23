using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snakes;

[CreateAssetMenu(fileName = "SnakeBodyData", menuName = "Scriptable Object/Snake/SnakeBodyData", order = 3)]
public class SnakeBodyData : ScriptableObject
{
	[Header("Body Prefab")]
	public GameObject snakeBodyPrefab;

	[Range(SnakeBodyManager.SNAKE_MINIMAL_LENGTH, 100)]
	public int bodyLength = 10;
}


