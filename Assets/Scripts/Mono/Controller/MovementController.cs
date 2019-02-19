using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public abstract class MovementController
{
	protected struct StepOver
	{
		public Cell cell;
		public Vector3 up;
		public Quaternion rotation;

		public StepOver(Cell c, Vector3 u, Quaternion q)
		{
			this.cell = c;
			this.rotation = q;
			this.up = u;
		}
	}


	public LivingEntity entity {get; protected set;}
	public Transform myTransform {get; protected set;}

	public float walkSpeed = 5f;
	protected const float ROTATION_SPEED = 1.75f;



	


	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------------- BASIC MOVEMENT COROUTINE ----------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public virtual IEnumerator StepToWander()
	{
		// declaration
		Cell c;
		int radius;
		
		// search new cell
		radius = Random.Range(1, this.entity.maxStepDistance + 1);
		c = this.GetOneSurroundingCell(radius);

		if(c == null) {
			yield break;
		} else {
			yield return entity.StartCoroutine(this.StepTowards(c));
		}
	}

	public virtual IEnumerator StepToRunAway(Transform from)
	{
		// declaration
		Cell c;
		Vector3 dir;
		
		// initialization
		dir = Vector3.ProjectOnPlane(this.entity.currentCell.position - from.position, this.entity.currentCell.normal).normalized;

		// search new cell
		c = this.GetOneCellWithDirection(dir);

		// cannot move
		if(c == null)
		{
			// search for other random cell
			c = this.GetOneSurroundingCell(this.entity.maxStepDistance);
			if(c != null) {
				yield return entity.StartCoroutine(this.StepTowards(c));
			}
		}
		// can move
		else
		{
			yield return entity.StartCoroutine(this.StepTowards(c));
		}
	}








	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------------- ACTION FUNCTION --------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	protected abstract IEnumerator StepTowards(Cell c);

	protected abstract IEnumerator StepOverFace(StepOver stepover);

	public abstract IEnumerator Rest();








	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------------- CHECK FUNCTIONS --------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	protected bool PathIsFreeTo(Cell c)
	{
		RaycastHit[] hits;
		float dist;
		Vector3 dir, start, end;

		end = c.position;
		start = myTransform.position;
		dir = end - start;
		dist = dir.magnitude;
		dir.Normalize();

        hits = Physics.SphereCastAll(start, 0.49f, dir, dist);
        foreach(RaycastHit hit in hits) {
        	if(hit.transform != myTransform && hit.transform.GetComponent<CellableEntity>() != null) {
        		return false;
        	}
        }

        return true;
	}

	protected bool TargetIsTooFarAway(Cell c)
	{
		Vector3 relative;

		relative = this.entity.currentCell.local - c.local;

		for(int i = 0; i < 3; i++) {
			if(relative[i] > this.entity.maxStepDistance) {
				Debug.LogWarning("Target is too far away ! Entity cannot walk it in only one step !", myTransform);
				return true;
			}
		}

		return false;
	}









	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------------------- UTIL FUNCTION ---------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	protected Cell GetOneSurroundingCell(int radius)
	{
		Grill g;
		Cell res;
		int count;

		g = this.entity.currentCell.grill;
		res = null;
		count = 10;

		do {
			res = g.GetOneSurroundingCellOf(this.entity.currentCell, radius);
			count --;
		}
		while((res == null || !res.isWalkable || !this.PathIsFreeTo(res)) && count > 0);

		if(count == 0) {
			return null;
		}

		return res;
	}

	protected Cell GetOneCellWithDirection(Vector3 dir)
	{
		Grill g;
		Cell res;
		int count;

		g = this.entity.currentCell.grill;
		res = null;
		count = 10;

		do {
			res = g.GetOneCellInThisDirection(this.entity.currentCell, dir, this.entity.maxStepDistance);
			count --;
		}
		while((res == null || !res.isWalkable || !this.PathIsFreeTo(res)) && count > 0);

		if(count == 0) {
			return null;
		}

		return res;
	}

	protected StepOver GetOneStepOverFrom(Cell bound)
	{
		Cell cell;
		Cell.Ledge ledge;
		Vector3 dir, normal, pos, transition, up;
		Quaternion look, rot;
		int sign;

		ledge = bound.ledges[Random.Range(0, bound.ledges.Count)];
		dir = ledge.direction;
		sign = (ledge.isLedge) ? 1 : -1;

		normal = sign * dir;
		pos = bound.position + dir * 0.5f - sign * 0.5f * bound.normal;
		cell = bound.surface.GetCellWithPositionAndFaceNormal(pos, normal);

		look = Quaternion.LookRotation(dir, bound.normal).SetAbsoluteRotation();
		rot = look * Quaternion.Euler(sign * 90, 0, 0);

		transition = (pos - bound.position).normalized;
		up = Vector3.Cross(transition, look * Vector3.right).normalized;

		return new StepOver(cell, up, rot);
	}
}
