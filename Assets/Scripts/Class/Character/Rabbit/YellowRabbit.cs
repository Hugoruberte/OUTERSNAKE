using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeAI;

public class YellowRabbit : Rabbit
{
	protected override void Awake()
	{
		base.Awake();
		
		// this.behaviour = new YellowRabbitBehaviour(this);
		// this.controller = new YellowRabbitController(this);
	}
}
