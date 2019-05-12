using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;
using Lazers;

public class Turret : TrapEntity
{
	public Transform muzzle { get; private set; }
	public TurretData turretData = null;

	protected override void Awake()
	{
		base.Awake();

		this.muzzle = transform.DeepFind("Muzzle");
	}

	protected override void Start()
	{
		base.Start();

		// Initialize AI behaviour (this will launch the AI)
		this.behaviour = TurretAI.instance.Launch(this);
	}

	public void Shoot()
	{
		Lazer lazer = PoolingManager.instance.Get<Lazer>(this.turretData.lazerPrefab);

		lazer?.Launch(this.muzzle, this.muzzle.forward, this.OnHit);
	}

	private void OnHit(LazerHit other)
	{
		Debug.Log("TO DO");
	}






	private void OnDrawGizmosSelected()
	{
		if(this.turretData == null) {
			return;
		}

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, this.turretData.rangeOfView);
	}
}
