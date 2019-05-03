using UnityEngine;
using Snakes;

public class SnakeManager : Singleton<SnakeManager>
{
	public GameObject snakeObject { get; private set; }
	public Transform snakeTransform { get; private set; }
	public Collider snakeCollider { get; private set; }
	public SnakeController snakeController { get; private set; }
	public SnakeMovementEvents snakeEvents { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		
		snakeObject = GameObject.FindWithTag("Player");
		snakeTransform = snakeObject.transform;
		snakeCollider = snakeObject.GetComponent<Collider>();
		snakeController = snakeObject.GetComponent<SnakeController>();
		snakeEvents = snakeController.events;
	}
}
