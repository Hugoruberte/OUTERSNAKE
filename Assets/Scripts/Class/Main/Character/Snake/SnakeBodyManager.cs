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
	private Dictionary<Transform, SnakePartCharacter> snakePartInteracts = new Dictionary<Transform, SnakePartCharacter>();
	private SnakeController snakeController;
	private const int SNAKE_TAIL_MARGIN = 2;
	public const int SNAKE_MINIMAL_LENGTH = 2;
	private float reduceSpeed;

	
	void Start()
	{
		snake = SnakeManager.instance.snake.transform;

		snakeController = SnakeManager.instance.snakeController;

		// SnakeManager.instance.events.onStartStepTo.AddListener(UpdateSnakeTail);
		// SnakeManager.instance.events.onEndStepTo.AddListener(UpdateSnakeHead);

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
	private void UpdateSnakeTail(Vector3 target, Vector3 normal)
	{
		int count = snakeBody.childCount;
		SnakePartCharacter snakeBodyScript;

		if(count <= this.bodyLength - 1) {
			return;
		}

		reduceSpeed = 0.333f * this.snakeController.speed + 1.667f;
		snakeBodyScript = snakePartInteracts[snakeBody.GetChild(this.bodyLength - 1)];
		snakeBodyScript.ReduceSnakePart(reduceSpeed);
	}

	// Event called 'onEndStep' from SnakeController
	private void UpdateSnakeHead(Vector3 target)
	{
		int count;
		Transform last;
		SnakePartCharacter snakeBodyScript;

		// How many snake body we currently have
		count = snakeBody.childCount;

		// We do not have enough snake body
		if(count < this.bodyLength + SNAKE_TAIL_MARGIN)
		{
			this.CreateNewSnakePart(target);
		}
		// We have enough, move the last one
		else
		{
			last = snakeBody.GetChild(count - 1);
			snakeBodyScript = snakePartInteracts[last];

			if(snakeBodyScript.snakePartState == SnakePartState.Reusable)
			{
				last.position = target;
				last.rotation = snake.rotation;
				snakeBodyScript.SetupSnakePart();
			}
			else
			{
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
		newSnakeBody.name = "SnakeBody";
		snakeBody = newSnakeBody.transform;

		snakeBody.SetSiblingIndex(snake.GetSiblingIndex() + 1);
	}

	private void CreateNewSnakePart(Vector3 target)
	{
		GameObject part = Instantiate(this.snakeBodyPrefab, target, snake.rotation);

		part.name = "SnakePart";
		part.transform.parent = snakeBody;

		SnakePartCharacter controller = part.GetComponent<SnakePartCharacter>();
		controller.InitializeSnakePart();
		
		snakePartInteracts.Add(part.transform, controller);
	}

	public void ExplodeSnakeBody()
	{
		foreach(SnakePartCharacter controller in snakePartInteracts.Values)
		{
			controller.Explosion();
		}
	}

	public void ExplodeFromSnakePart(Transform tr)
	{
		int index = tr.GetSiblingIndex();

		if(index >= this.bodyLength){
			snakePartInteracts[tr].Explosion();
			return;
		}

		this.bodyLength = index + 1;

		if(this.bodyLength > SNAKE_MINIMAL_LENGTH)
		{
			for(int i = index; i < snakeBody.childCount; i++)
			{
				snakePartInteracts[snakeBody.GetChild(i)].Explosion();
			}
		}
		else
		{
			ExplodeSnakeBody();

			Debug.Log("Snake is dead");
		}
	}
}
