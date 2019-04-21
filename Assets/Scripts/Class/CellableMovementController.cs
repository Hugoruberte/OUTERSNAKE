using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

[System.Serializable]
public abstract class CellableMovementController : MovementController
{
	private readonly int freePathLayerMask;

	protected struct StepOver {
		public Cell cell;
		public Vector3 up;
		public Quaternion rotation;

		public StepOver(Cell c, Vector3 u, Quaternion q) {
			this.cell = c;
			this.rotation = q;
			this.up = u;
		}
	}

	public CellableMovementController(LivingEntity e) : base(e)
	{
		this.freePathLayerMask = ~(1 << LayerMask.NameToLayer("Ground"));
	}







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
		start = entity.myTransform.position;
		dir = end - start;
		dist = dir.magnitude;
		dir.Normalize();

		hits = Physics.SphereCastAll(start, 0.475f, dir, dist, this.freePathLayerMask);
		foreach(RaycastHit hit in hits) {
			if(hit.transform != entity.myTransform) {
				return false;
			}
		}

		return true;
	}
	protected bool TargetIsTooFarAway(Cell c)
	{
		Vector3 relative;

		relative = this.entity.cellable.currentCell.local - c.local;

		for(int i = 0; i < 3; i++) {
			if(relative[i] > this.entity.maxStepDistance) {
				Debug.LogWarning($"Target is too far away ! Entity cannot walk it in only one step ! (max step distance = {this.entity.maxStepDistance} and relative = {relative[i]}", entity.myTransform);
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

		g = this.entity.cellable.currentCell.grill;
		res = null;
		count = 10;

		do {
			res = g.GetOneSurroundingCellOf(this.entity.cellable.currentCell, radius);
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

		g = this.entity.cellable.currentCell.grill;
		res = null;
		count = 10;

		do {
			res = g.GetOneCellInThisDirection(this.entity.cellable.currentCell, dir, this.entity.maxStepDistance);
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
