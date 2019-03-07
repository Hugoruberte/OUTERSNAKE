using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerTrap : TrapEntity
{
	private PoolingManager poolingManager;

	protected override void Start()
	{
		base.Start();

		// Initialize AI behaviour (this will launch the AI)
		// this.behaviour = LazerTrapAI.instance.Launch(this);

		poolingManager = PoolingManager.instance;

		this.Attack();
	}

	private void Attack()
	{
		LazerController lazer = poolingManager.Get<LazerController>();

		if(lazer == null) {
			return;
		}

		lazer.Initialize(myTransform.position, new Vector3(10, 15, 10));

		lazer.Launch();
	}
}
