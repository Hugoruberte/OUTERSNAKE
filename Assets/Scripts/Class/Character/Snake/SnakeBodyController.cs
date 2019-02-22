using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snakes;

public class SnakeBodyController : MonoBehaviour
{
	private Transform snake;
	[HideInInspector]
	public Transform snakeBody;
	[Header("Body Prefab")]
	public GameObject snakeBodyPrefab;

	private SnakeController snakeController;
	private Dictionary<Transform, SnakePartCharacter> snakePartInteracts = new Dictionary<Transform, SnakePartCharacter>();

	[HideInInspector]
	public int bodyLength = 0;
	private const int SNAKE_TAIL_MARGIN = 2;
	public const int SNAKE_MINIMAL_LENGTH = 2;
	private float reduceSpeed;


	void Start()
	{
		snake = SnakeManager.instance.snake.transform;
		snakeController = SnakeManager.instance.snakeController;

		snakeController.events.onStartStep.AddListener(UpdateSnakeTail);
		snakeController.events.onEndStep.AddListener(UpdateSnakeHead);

		InitializeSnakeBody();
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

		if(count <= bodyLength - 1) {
			return;
		}

		reduceSpeed = 0.333f * snakeController.speed + 1.667f;
		snakeBodyScript = snakePartInteracts[snakeBody.GetChild(bodyLength - 1)];
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
		if(count < bodyLength + SNAKE_TAIL_MARGIN)
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
				CreateNewSnakePart();
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
		GameObject part = Instantiate(snakeBodyPrefab, snake.position, snake.rotation);

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

		if(index >= bodyLength){
			snakePartInteracts[tr].Explosion();
			return;
		}

		bodyLength = index + 1;

		if(bodyLength > SNAKE_MINIMAL_LENGTH)
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


