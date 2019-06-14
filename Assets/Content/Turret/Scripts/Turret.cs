using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;
using My.Events;
using Lazers;

public class Turret : TrapEntity
{
	public Transform muzzle { get; private set; }
	public TurretData turretData = null;
	public ActionEvent onHit = null;

	protected override void Awake()
	{
		base.Awake();

		this.muzzle = transform.DeepFind("Muzzle");

		// Initialize AI behaviour (this will launch the AI)
		this.behaviour = UtilityAIManager.instance.Get<TurretAI>().Launch(this);
	}

	public void Shoot()
	{
		Lazer lazer = PoolingManager.instance.Get<Lazer>(this.turretData.lazerPrefab);

		lazer?.Launch(this.muzzle, this.muzzle.forward, this.OnHit);
	}

	private void OnHit(LazerHit other)
	{
		// Debug.Log("TO DO");
		this.onHit?.Invoke();
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
