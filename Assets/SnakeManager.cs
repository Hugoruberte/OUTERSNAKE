using UnityEngine;
using Snakes;

public class SnakeManager : Singleton<SnakeManager>
{
	public SnakeData snakeData;

	public GameObject snake { get; private set; }

	public readonly SnakeMovementEvents events = new SnakeMovementEvents();

	protected override void Awake()
	{
		base.Awake();
		
		snake = GameObject.FindWithTag("Player");
	}
}
