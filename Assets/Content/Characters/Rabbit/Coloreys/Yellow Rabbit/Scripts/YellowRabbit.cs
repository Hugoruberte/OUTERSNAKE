using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeAI;

public class YellowRabbit : RabbitEntity
{
	protected override private void Awake()
	{
		base.Awake();
		
		// this.behaviour = new YellowRabbitBehaviour(this);
		// this.controller = new YellowRabbitController(this);
	}
}
