using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Snakes;

public class LazerTrapAI : UtilityBehaviourAI
{
	private Transform snake;

	private static LazerTrapAI instance = null;

	public static UtilityBehaviourAI Initialize(LazerTrap lazer)
	{
		MovementController ctr = new LazerTrapController(lazer);
		instance.AddController(ctr);

		return instance;
	}

	protected override void Awake()
	{
		base.Awake();

		instance = this;
	}

	void Start()
	{
		snake = SnakeManager.instance.snake.transform;
	}
}
