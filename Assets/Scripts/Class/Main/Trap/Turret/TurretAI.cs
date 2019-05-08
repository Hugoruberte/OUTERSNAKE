using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Snakes;
using Utility.AI;

[System.Serializable]
[CreateAssetMenu(fileName = "TurretAI", menuName = "Scriptable Object/AI/TurretAI", order = 3)]
public class TurretAI : UtilityAIBehaviour<TurretAI>
{
	[System.NonSerialized] private Transform snake;
	[System.NonSerialized] private Collider[] colliderResults;
	[System.NonSerialized] private RaycastHit[] raycastResults;
	[System.NonSerialized] private TurretData turretData;


	public UtilityAIBehaviour Launch(Turret lazer)
	{
		MovementController ctr = new TurretController(lazer);
		this.AddController(ctr);

		if(this.turretData == null) {
			this.turretData = lazer.turretData;
		}
		
		return this;
	}

	public override void OnStart()
	{
		base.OnStart();
		
		this.snake = SnakeData.instance.snakeTransform;
		this.colliderResults = new Collider[10];
		this.raycastResults = new RaycastHit[10];
	}








	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------------ SCORERS FUNCTIONS -------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public bool IsThereTargetAround(MovementController ctr)
	{
		TurretController ltr;
		int count;
		float dist, min;
		Vector3 pos;
		Collider c;

		ltr = ctr as TurretController;
		min = float.MaxValue;
		pos = ctr.position;
		count = Physics.OverlapSphereNonAlloc(pos, this.turretData.rangeOfView, this.colliderResults, this.turretData.targetLayerMask);
		
		for(int i = 0; i < this.colliderResults.Length; i++) {
			c = this.colliderResults[i];

			if(c == null) {
				continue;
			}

			dist = Vector3.Distance(pos, c.transform.position);
			if(dist < min) {
				min = dist;
				ltr.target = c.transform;
			}

			this.colliderResults[i] = null;
		}

		return (min <= this.turretData.rangeOfView);
	}

	public bool IsTargetVisible(MovementController ctr)
	{
		return true; // ?
	}

	public bool IsTargetInSight(MovementController ctr)
	{
		TurretController ltr;
		Vector3 dir;
		int count;
		
		ltr = ctr as TurretController;
		dir = ltr.turret.muzzle.forward;
		
		count = this.SphereCastTowards(ltr, dir);

		return (count > 0);
	}

	public bool CouldShoot(MovementController ctr)
	{
		TurretController ltr = ctr as TurretController;

		return (Time.time > ltr.lastShootTime + this.turretData.shootDelay);
	}



	private int SphereCastTowards(TurretController ltr, Vector3 towards)
	{
		Vector3 origin;
		float radius, dist;
		int count;
		
		origin = ltr.turret.muzzle.position;
		dist = this.turretData.rangeOfView + 1f;
		radius = 0.457f;
		
		count = Physics.SphereCastNonAlloc(origin, radius, towards, this.raycastResults, dist, this.turretData.targetLayerMask);

		return count;
	}





	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------------ ACTION FUNCTIONS --------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public IEnumerator Wander(MovementController ctr)
	{
		yield return ctr.Wander();
	}

	public IEnumerator Aim(MovementController ctr)
	{
		TurretController ltr = ctr as TurretController;

		ltr.target = ltr.target ?? this.snake;

		yield return ltr.AimAt(ltr.target);
	}

	public IEnumerator Shoot(MovementController ctr)
	{
		TurretController ltr = ctr as TurretController;

		yield return ltr.Shoot();
	}
}
