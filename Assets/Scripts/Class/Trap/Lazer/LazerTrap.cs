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
	public LazerTrapData lazerTrapData;


	protected override void Awake()
	{
		base.Awake();

		this.muzzle = transform.DeepFind("Muzzle");
	}

	protected override void Start()
	{
		base.Start();

		// Initialize AI behaviour (this will launch the AI)
		this.behaviour = LazerTrapAI.instance.Launch(this);

		poolingManager = PoolingManager.instance;
	}

	private void Attack()
	{
		Lazer lazer = poolingManager.Get<Lazer>(lazerPrefab);

		if(lazer == null) {
			return;
		}

		lazer.Initialize(this.muzzle, this.muzzle.forward, this.OnHit);

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
