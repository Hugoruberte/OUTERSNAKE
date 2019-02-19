using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
	private static SnakeManager _instance = null;
	public static SnakeManager instance {
		get {
			if(_instance == null) {
				Debug.LogError("Instance is null, either you tried to access it from the Awake function or it has not been initialized in its own Awake function");
			}
			return _instance;
		}
		set {
			_instance = value;
		}
	}

	private static GameObject _snake = null;

	private static SnakeController _snakeController = null;

	private static SnakeBodyController _snakeBodyController = null;


	void Awake()
	{
		instance = this;

		_snake = GameObject.FindWithTag("Player");
		_snakeController = FindObjectOfType<SnakeController>() as SnakeController;
		_snakeBodyController = FindObjectOfType<SnakeBodyController>() as SnakeBodyController;
	}



	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------------- STATIC FUNCTION --------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public static GameObject GetSnake()
	{
		return _snake;
	}

	public static SnakeController GetController()
	{
		return _snakeController;
	}

	public static SnakeBodyController GetBodyController()
	{
		return _snakeBodyController;
	}

	public static Transform GetSnakeBody()
	{
		return _snakeBodyController.snakeBody;
	}
}


