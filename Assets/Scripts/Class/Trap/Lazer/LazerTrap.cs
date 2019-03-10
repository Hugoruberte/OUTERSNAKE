using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Lazers;

public class LazerTrap : TrapEntity
{
	private PoolingManager poolingManager;

	private Transform muzzle;

	protected override void Start()
	{
		base.Start();

		// Initialize AI behaviour (this will launch the AI)
		// this.behaviour = LazerTrapAI.instance.Launch(this);

		poolingManager = PoolingManager.instance;

		this.muzzle = transform.DeepFind("Muzzle");
	}

	private void Attack()
	{
		LazerController lazer = poolingManager.Get<LazerController>();

		if(lazer == null) {
			return;
		}

		lazer.Initialize(muzzle.position, muzzle.forward, OnHit);

		lazer.Launch();
	}

	private void OnHit(LazerHit other)
	{
		Debug.Log("TO DO");
	}

	void Update()
	{
		if(Input.GetButtonDown("Space")) {
			this.Attack();
		}
	}
}
