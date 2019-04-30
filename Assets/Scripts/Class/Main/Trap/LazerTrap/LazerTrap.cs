using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Lazers;

public class LazerTrap : TrapEntity
{
	public Transform muzzle { get; private set; }
	public LazerTrapData lazerTrapData = null;


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
	}

	public void Shoot()
	{
		Lazer lazer = PoolingManager.instance.Get<Lazer>(this.lazerTrapData.lazerPrefab);

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
			this.Shoot();
		}
	}
}
