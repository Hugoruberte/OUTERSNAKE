using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerTrap : TrapEntity
{
	protected override void Start()
	{
		base.Start();

		// Initialize AI behaviour (this will launch the AI)
		this.behaviour = LazerTrapAI.Initialize(this);
	}

	private void Attack()
	{
		
	}
}
