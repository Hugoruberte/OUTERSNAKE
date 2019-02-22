using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteRabbit : RabbitEntity
{
	protected override void Start()
	{
		base.Start();

		// Initialize AI behaviour (this will launch the AI)
		this.behaviour = WhiteRabbitAI.Initialize(this);
	}
}
