using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Snakes;
using Utility.AI;

[System.Serializable]
[CreateAssetMenu(fileName = "LazerTrapAI", menuName = "Scriptable Object/AI/LazerTrapAI", order = 3)]
public class LazerTrapAI : UtilityAIBehaviour<LazerTrapAI>
{
	[System.NonSerialized] private Transform snake;
	[System.NonSerialized] private Collider[] colliderResults;
	[System.NonSerialized] private RaycastHit[] raycastResults;
	[System.NonSerialized] private LazerTrapData lazerTrapData;


	public UtilityAIBehaviour Launch(LazerTrap lazer)
	{
		MovementController ctr = new LazerTrapController(lazer);
		this.AddController(ctr);

		if(this.lazerTrapData == null) {
			this.lazerTrapData = lazer.lazerTrapData;
		}
		
		return this;
	}

	public override void OnStart()
	{
		base.OnStart();
		
		this.snake = SnakeManager.instance.snakeTransform;
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
		LazerTrapController ltr;
		int count;
		float dist, min;
		Vector3 pos;
		Collider c;

		ltr = ctr as LazerTrapController;
		min = float.MaxValue;
		pos = ctr.position;
		count = Physics.OverlapSphereNonAlloc(pos, this.lazerTrapData.rangeOfView, this.colliderResults, this.lazerTrapData.targetLayerMask);
		
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

		return (min <= this.lazerTrapData.rangeOfView);
	}

	public bool IsTargetVisible(MovementController ctr)
	{
		return true; // ?
	}

	public bool IsTargetInSight(MovementController ctr)
	{
		LazerTrapController ltr;
		Vector3 dir;
		int count;
		
		ltr = ctr as LazerTrapController;
		dir = ltr.lazerTrap.muzzle.forward;
		
		count = this.SphereCastTowards(ltr, dir);

		return (count > 0);
	}

	public bool CouldShoot(MovementController ctr)
	{
		LazerTrapController ltr = ctr as LazerTrapController;

		return (Time.time > ltr.lastShootTime + this.lazerTrapData.shootDelay);
	}



	private int SphereCastTowards(LazerTrapController ltr, Vector3 towards)
	{
		Vector3 origin;
		float radius, dist;
		int count;
		
		origin = ltr.lazerTrap.muzzle.position;
		dist = this.lazerTrapData.rangeOfView + 1f;
		radius = 0.457f;
		
		count = Physics.SphereCastNonAlloc(origin, radius, towards, this.raycastResults, dist, this.lazerTrapData.targetLayerMask);

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
		LazerTrapController ltr = ctr as LazerTrapController;

		ltr.target = ltr.target ?? this.snake;

		yield return ltr.AimAt(ltr.target);
	}

	public IEnumerator Shoot(MovementController ctr)
	{
		LazerTrapController ltr = ctr as LazerTrapController;

		yield return ltr.Shoot();
	}
}
