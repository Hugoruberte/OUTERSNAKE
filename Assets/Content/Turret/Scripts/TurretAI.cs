using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Snakes;
using Utility.AI;
using My.Tools;

[System.Serializable]
[CreateAssetMenu(fileName = "TurretAI", menuName = "Scriptable Object/AI/TurretAI", order = 3)]
public class TurretAI : UtilityAIBehaviour<TurretAI>
{
	[System.NonSerialized] private Transform snake;
	[System.NonSerialized] private RaycastHit[] raycastResults;
	[System.NonSerialized] private TurretData turretData;


	public UtilityAIBehaviour Launch(Turret lazer)
	{
		TurretController ltr = new TurretController(lazer);
		this.AddController(ltr);

		this.AddStartListener("Aim", ltr.OnStartAimAt);
		this.AddEndListener("Aim", ltr.OnEndAimAt);
		
		if(this.turretData == null) {
			this.turretData = lazer.turretData;
		}

		return this;
	}

	public override void OnStart()
	{
		base.OnStart();
		
		this.snake = SnakeData.instance.snakeTransform;
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
		float dist, min;
		int count;
		Vector3 pos, dir, up;
		Collider c;
		Transform tr;
		LayerMask mask;
		Collider[] colliderResults;

		ltr = ctr as TurretController;
		min = float.MaxValue;
		up = ltr.turret.transform.up;
		pos = ctr.position;
		tr = null;
		mask = 1 << LayerMask.NameToLayer("Ground");
		colliderResults = Shared.colliderArray;
		count = Physics.OverlapSphereNonAlloc(pos, this.turretData.rangeOfView, colliderResults, this.turretData.targetLayerMask);
		
		for(int i = 0; i < colliderResults.Length && count > 0; i++)
		{
			c = colliderResults[i];

			if(c == null) {
				continue;
			}

			// check if well-axed
			if(Mathf.Abs(ltr.turret.muzzle.InverseTransformPoint(c.transform.position).y) > 1f) {
				// clean cache
				colliderResults[i] = null;
				count --;
				continue;
			}

			// check if visible -> search wall/ground
			dir = c.transform.position - ltr.turret.muzzle.position;
			dir = Vector3.ProjectOnPlane(dir, up);
			if(Physics.Raycast(ltr.turret.muzzle.position, dir, dir.magnitude, mask)) {
				// clean cache
				colliderResults[i] = null;
				count --;
				continue;
			}

			// check nearest target
			dist = Vector3.Distance(pos, c.transform.position);
			if(dist < min) {
				min = dist;
				tr = c.transform;
			}

			// clean cache
			colliderResults[i] = null;
			count --;
		}

		ltr.target = tr;
		return (min <= this.turretData.rangeOfView);
	}

	public bool IsTargetInSight(MovementController ctr)
	{
		TurretController ltr;
		Vector3 dir;
		int count;
		
		ltr = ctr as TurretController;
		dir = ltr.turret.muzzle.forward;
		
		count = this.SphereCastTowards(ltr, dir, this.turretData.targetLayerMask, 0.45f);

		return (count > 0);
	}

	public bool CouldShoot(MovementController ctr)
	{
		TurretController ltr = ctr as TurretController;

		bool shootDelayOK = (Time.time > ltr.lastShootTime + this.turretData.shootDelay);
		bool aimDelayOK = (Time.time > ltr.aimStartTime + this.turretData.aimDuration);

		return (shootDelayOK && aimDelayOK);
	}



	private int SphereCastTowards(TurretController ltr, Vector3 towards, LayerMask mask, float radius)
	{
		Vector3 origin;
		float dist;
		int count;
		
		origin = ltr.turret.muzzle.position;
		dist = this.turretData.rangeOfView + 1f;
		
		count = Physics.SphereCastNonAlloc(origin, radius, towards, this.raycastResults, dist, mask);

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

		ltr.aimStartTime = Time.time;
		if(ltr.target == null) {
			ltr.target = this.snake;
		}
		
		yield return ltr.AimAtTarget();
	}

	public IEnumerator Shoot(MovementController ctr)
	{
		TurretController ltr = ctr as TurretController;

		yield return ltr.Shoot();
	}
}
