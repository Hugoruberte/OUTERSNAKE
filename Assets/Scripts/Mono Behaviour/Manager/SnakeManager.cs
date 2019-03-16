using UnityEngine;
using Snakes;

public class SnakeManager : Singleton<SnakeManager>
{
	public GameObject snake { get; private set; }

	public readonly SnakeMovementEvents events = new SnakeMovementEvents();

	public SnakeController snakeController { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		
		snake = GameObject.FindWithTag("Player");

		snakeController = snake.GetComponent<SnakeController>();
	}
}
