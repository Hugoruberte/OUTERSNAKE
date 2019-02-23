using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snakes;

public class SnakeBodyManager : Singleton<SnakeBodyManager>
{
	public SnakeBodyData snakeBodyData;

	private SnakeData snakeData;

	[HideInInspector]
	public Transform snakeBody { get; private set; }

	private Transform snake;

	private Dictionary<Transform, SnakePartCharacter> snakePartInteracts = new Dictionary<Transform, SnakePartCharacter>();

	private const int SNAKE_TAIL_MARGIN = 2;
	public const int SNAKE_MINIMAL_LENGTH = 2;
	private float reduceSpeed;


	void Awake()
	{
		instance = this;
	}

	
	void Start()
	{
		snakeData = SnakeManager.instance.snakeData;

		snake = SnakeManager.instance.snake.transform;

		SnakeManager.instance.events.onStartStep.AddListener(UpdateSnakeTail);
		SnakeManager.instance.events.onEndStep.AddListener(UpdateSnakeHead);

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

		if(count <= snakeBodyData.bodyLength - 1) {
			return;
		}

		reduceSpeed = 0.333f * snakeData.speed + 1.667f;
		snakeBodyScript = snakePartInteracts[snakeBody.GetChild(snakeBodyData.bodyLength - 1)];
		snakeBodyScript.ReduceSnakePart(reduceSpeed);
	}

	// Event called 'onEndStep' from SnakeController
	private void UpdateSnakeHead()
	{
		int count;
		Transform last;
		SnakePartCharacter snakeBodyScript;

		// How many snake body we currently have
		count = snakeBody.childCount;

		// We do not have enough snake body
		if(count < snakeBodyData.bodyLength + SNAKE_TAIL_MARGIN)
		{
			CreateNewSnakePart();
		}
		// We have enough, move the last one
		else
		{
			last = snakeBody.GetChild(count - 1);
			snakeBodyScript = snakePartInteracts[last];

			if(snakeBodyScript.snakePartState == SnakePartState.Reusable)
			{
				last.position = snake.position;
				last.rotation = snake.rotation;
				snakeBodyScript.SetupSnakePart();
			}
			else
			{
				this.CreateNewSnakePart();
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

	private void CreateNewSnakePart()
	{
		GameObject part = Instantiate(snakeBodyData.snakeBodyPrefab, snake.position, snake.rotation);

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

		if(index >= snakeBodyData.bodyLength){
			snakePartInteracts[tr].Explosion();
			return;
		}

		snakeBodyData.bodyLength = index + 1;

		if(snakeBodyData.bodyLength > SNAKE_MINIMAL_LENGTH)
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
