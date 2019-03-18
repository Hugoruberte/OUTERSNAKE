using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Lazers;

public class LazerTrap : TrapEntity
{
	private PoolingManager poolingManager;

	private Transform muzzle;

	public GameObject lazerPrefab;

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
		Lazer lazer = poolingManager.Get<Lazer>(lazerPrefab);

		if(lazer == null) {
			return;
		}

		lazer.Initialize(this.muzzle.position, this.muzzle.forward, this.OnHit);

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
