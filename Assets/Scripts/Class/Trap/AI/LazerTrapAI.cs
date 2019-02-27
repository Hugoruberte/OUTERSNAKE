﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Snakes;

public class LazerTrapAI : UtilityAIBehaviour<LazerTrapAI>
{
	private Transform snake;

	public UtilityAIBehaviour Launch(LazerTrap lazer)
	{
		MovementController ctr = new LazerTrapController(lazer);
		this.AddController(ctr);
		return this;
	}

	public override void OnStart()
	{
		base.OnStart();
		
		snake = SnakeManager.instance.snake.transform;
	}
}
