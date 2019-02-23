using UnityEngine;
using Snakes;

public class SnakeManager : Singleton<SnakeManager>
{
	public SnakeData snakeData;

	public GameObject snake { get; private set; }

	public readonly SnakeMovementEvents events = new SnakeMovementEvents();

	void Awake()
	{
		instance = this;

		snake = GameObject.FindWithTag("Player");
	}
}
