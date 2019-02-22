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

	public GameObject snake { get; private set; }

	public Transform snakeBody { get { return snakeBodyController.snakeBody; } }

	public SnakeController snakeController { get; private set; }

	public SnakeBodyController snakeBodyController { get; private set; }


	void Awake()
	{
		instance = this;

		snake = GameObject.FindWithTag("Player");
		snakeController = FindObjectOfType<SnakeController>() as SnakeController;
		snakeBodyController = FindObjectOfType<SnakeBodyController>() as SnakeBodyController;
	}
}


