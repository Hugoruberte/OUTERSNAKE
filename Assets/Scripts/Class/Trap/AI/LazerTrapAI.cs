using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Snakes;

public class LazerTrapAI : UtilityAIBehaviour
{
	private Transform snake;

	private static LazerTrapAI instance = null;

	public static UtilityAIBehaviour Initialize(LazerTrap lazer)
	{
		MovementController ctr = new LazerTrapController(lazer);
		instance.AddController(ctr);

		return instance;
	}

	public override void Initialize()
	{
		instance = this;
	}

	public override void Start()
	{
		base.Start();
		
		snake = SnakeManager.instance.snake.transform;
	}
}
