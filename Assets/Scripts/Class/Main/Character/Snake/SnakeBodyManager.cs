using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snakes;

public class SnakeBodyManager : Singleton<SnakeBodyManager>
{
	[SerializeField] private GameObject snakeBodyPrefab = null;
	[HideInInspector] public int bodyLength = 10;
	public Transform snakeBody { get; private set; }

	private Transform snake;
	private Collider snakeColl;
	private Collider currentIgnoredCollider;
	private Collider previousIgnoredCollider;
	private Dictionary<int, SnakePartCharacter> snakePartCharacters = new Dictionary<int, SnakePartCharacter>();
	private SnakeController snakeController;
	private const int SNAKE_TAIL_MARGIN = 2;
	public const int SNAKE_MINIMAL_LENGTH = 2;

	
	void Start()
	{
		this.snake = SnakeManager.instance.snakeTransform;
		this.snakeColl = SnakeManager.instance.snakeCollider;
		this.snakeController = SnakeManager.instance.snakeController;

		SnakeManager.instance.snakeEvents.onStartStepTo += this.UpdateSnakeTail;
		SnakeManager.instance.snakeEvents.onEndStepTo += this.UpdateSnakeHead;

		this.InitializeSnakeBody();
	}



	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------------- EVENT FUNCTIONS --------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	// Event called 'onStartStep' from SnakeController
	// -> Start moving to destination : reduce last snakepart
	private void UpdateSnakeTail(Vector3 target, Vector3 normal)
	{
		float reduceSpeed;
		int count;

		count = snakeBody.childCount;
		SnakePartCharacter snakePartScript;

		if(count <= this.bodyLength - 1) {
			return;
		}

		reduceSpeed = 0.333f * this.snakeController.speed + 1.667f;
		snakePartScript = this.snakePartCharacters[snakeBody.GetChild(this.bodyLength - 1).GetInstanceID()];
		snakePartScript.ReduceSnakePart(reduceSpeed);
	}

	// Event called 'onEndStep' from SnakeController
	// -> Reach destination : spawn a snakepart
	private void UpdateSnakeHead(Vector3 target)
	{
		int count;
		Transform last;
		SnakePartCharacter snakePartScript;

		// How many snake body we currently have
		count = this.snakeBody.childCount;

		// If we do not have enough snake body
		if(count < this.bodyLength + SNAKE_TAIL_MARGIN)
		{
			// Spawn a new one
			this.CreateNewSnakePart(target);
		}
		// If we have enough, try to move the last one
		else
		{
			last = this.snakeBody.GetChild(count - 1);
			snakePartScript = this.snakePartCharacters[last.GetInstanceID()];

			// If the last one can be moved
			if(snakePartScript.snakePartState == SnakePartState.Reusable)
			{
				// Ignore collision with snake
				this.SetIgnoredCollider(last.gameObject);

				// Move it the snake position
				last.position = target;
				last.rotation = this.snake.rotation;
				snakePartScript.SetupSnakePart();
			}
			// If last one cannot be moved
			else
			{
				// Spawn a new one
				this.CreateNewSnakePart(target);
			}
		}
	}


	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ----------------------------- SNAKE BODY MANAGEMENT FUNCTIONS ------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	private void InitializeSnakeBody()
	{
		GameObject newSnakeBody = new GameObject();
		newSnakeBody.name = "Snake Body";
		this.snakeBody = newSnakeBody.transform;

		this.snakeBody.SetSiblingIndex(snake.GetSiblingIndex() + 1);
	}

	private void CreateNewSnakePart(Vector3 target)
	{
		// Create
		GameObject part = Instantiate(this.snakeBodyPrefab, target, snake.rotation);
		part.name = "Snake Part";
		part.transform.parent = snakeBody;

		// Initialize
		SnakePartCharacter controller = part.GetComponent<SnakePartCharacter>();
		controller.InitializeSnakePart();
		this.snakePartCharacters.Add(part.transform.GetInstanceID(), controller);

		// Ignore collision with snake
		this.SetIgnoredCollider(part);
	}

	private void SetIgnoredCollider(GameObject part)
	{
		this.previousIgnoredCollider = this.currentIgnoredCollider;
		this.currentIgnoredCollider = part.GetComponent<Collider>();

		// Ignore collision with current
		Physics.IgnoreCollision(this.currentIgnoredCollider, this.snakeColl, true);

		// Recheck collision with previous ignored as it is not touching snake anymore (theorically)
		if(this.previousIgnoredCollider != null) {
			Physics.IgnoreCollision(this.previousIgnoredCollider, this.snakeColl, false);
		}
	}

	public void ExplodeAllSnakeBody()
	{
		foreach(SnakePartCharacter controller in this.snakePartCharacters.Values)
		{
			controller.Explosion();
		}
	}

	public void ExplodeFromSnakePart(Transform tr)
	{
		int index = tr.GetSiblingIndex();

		if(index >= this.bodyLength) {
			this.snakePartCharacters[tr.GetInstanceID()].Explosion();
			return;
		}

		this.bodyLength = index + 1;

		if(this.bodyLength > SNAKE_MINIMAL_LENGTH)
		{
			for(int i = index; i < this.snakeBody.childCount; i++)
			{
				this.snakePartCharacters[this.snakeBody.GetChild(i).GetInstanceID()].Explosion();
			}
		}
		else
		{
			this.ExplodeAllSnakeBody();

			Debug.Log("Snake is dead");
		}
	}
}
