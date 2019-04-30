using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Snakes;

[System.Serializable]
[CreateAssetMenu(fileName = "LazerTrapAI", menuName = "Scriptable Object/AI/LazerTrapAI", order = 3)]
public class LazerTrapAI : UtilityAIBehaviour<LazerTrapAI>
{
	[System.NonSerialized] private Transform snake;
	[System.NonSerialized] private Transform target;
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
		
		this.snake = SnakeManager.instance.snake.transform;
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
	public float DistanceToNearestTarget(MovementController ctr)
	{
		int count;
		float dist, min;
		Vector3 pos;

		min = float.MaxValue;
		pos = ctr.position;
		count = Physics.OverlapSphereNonAlloc(pos, this.lazerTrapData.rangeOfView, this.colliderResults, this.lazerTrapData.targetLayerMask);
		
		foreach(Collider c in this.colliderResults) {
			if(c == null) {
				continue;
			}

			dist = Vector3.Distance(pos, c.transform.position);
			if(dist < min) {
				min = dist;
				this.target = c.transform;
			}
		}

		return this.MapOnRangeOfView(ctr, min, this.lazerTrapData.rangeOfView);
	}

	public bool IsTargetInSight(MovementController ctr)
	{
		LazerTrapController ltr;
		Vector3 origin, dir;
		float radius, dist;
		int count;
		
		ltr = ctr as LazerTrapController;		
		origin = ltr.lazerTrap.muzzle.position;
		dir = ltr.lazerTrap.muzzle.forward;
		dist = this.lazerTrapData.rangeOfView + 1f;
		radius = 0.457f;
		
		count = Physics.SphereCastNonAlloc(origin, radius, dir, this.raycastResults, dist, this.lazerTrapData.targetLayerMask);

		return (count > 0);
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

		this.target = this.target ?? this.snake;

		yield return ltr.AimAt(this.target);
	}

	public IEnumerator Shoot(MovementController ctr)
	{
		LazerTrapController ltr = ctr as LazerTrapController;

		yield return ltr.Shoot();
	}
}
