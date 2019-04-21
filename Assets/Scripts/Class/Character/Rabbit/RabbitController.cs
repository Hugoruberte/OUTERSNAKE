using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public abstract class RabbitController : CellableMovementController
{
	protected RabbitEntity rabbit;

	public float jumpHeight = 2f;

	protected const float ROTATION_SPEED = 1.75f;
	private const float ROTATION_OVER_FACE_SPEED = 1f;


	public RabbitController(RabbitEntity r) : base(r)
	{
		this.rabbit = r;
	}







	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------------- BASIC MOVEMENT COROUTINE ----------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public override IEnumerator Wander()
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
			yield return this.StepTowards(c);
		}
	}

	public override IEnumerator Rest()
	{
		float duration;

		duration = Random.Range(this.rabbit.minAfterJumpTempo, this.rabbit.maxAfterJumpTempo);

		yield return Yielders.Wait(duration);
	}

	public virtual IEnumerator RunAway(Transform from)
	{
		// declaration
		Cell c, current;
		Vector3 dir;
		
		// initialization
		current = this.entity.cellable.currentCell;
		dir = Vector3.ProjectOnPlane(current.position - from.position, current.normal).normalized;

		// search new cell
		c = this.GetOneCellWithDirection(dir);

		// cannot move
		if(c == null)
		{
			// search for other random cell
			c = this.GetOneSurroundingCell(this.entity.maxStepDistance);
			if(c != null) {
				yield return this.StepTowards(c);
			}
		}
		// can move
		else
		{
			yield return this.StepTowards(c);
		}
	}








	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------- BASIC MOVEMENT CONTROLLER OVERRIDINGS ---------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	protected virtual IEnumerator StepOverFace(StepOver stepover)
	{
		// declaration
		_Transform from;

		// check
		if(this.entity.cellable.currentCell.isInner) {
			yield break;
		}

		// initialization
		from = new _Transform(this.entity.myTransform);

		// jump
		yield return this.JumpTo(stepover.cell, from, stepover.rotation, stepover.up, ROTATION_OVER_FACE_SPEED);
	}

	protected virtual IEnumerator StepTowards(Cell c)
	{
		// declaration
		_Transform from;
		Quaternion look;

		// check
		if(this.TargetIsTooFarAway(c)) {
			yield break;
		}

		// initialization
		from = new _Transform(this.entity.myTransform);
		look = Quaternion.LookRotation(c.position - this.entity.cellable.currentCell.position, entity.myTransform.up).SetAbsoluteRotation();

		// jump
		yield return this.JumpTo(c, from, look, this.entity.myTransform.up, ROTATION_SPEED);
	}

	





	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ----------------------------- SPECIAL RABBIT ACTION FUNCTIONS ------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	protected IEnumerator JumpTo(Cell c, _Transform from, Quaternion look, Vector3 up, float omega)
	{
		// declaration
		Vector3 to;
		float step, height;

		// initialization
		to = c.position;
		step = 0f;

		// reserve new cell
		this.entity.cellable.ReserveNextCell(c);

		// move to new cell
		while(step < 1f) {
			step += this.entity.speed * Time.deltaTime;
			height = Mathf.PingPong(Mathf.Min(step * this.jumpHeight * 2f, this.jumpHeight * 2f), this.jumpHeight);

			// movement
			this.entity.myTransform.rotation = Quaternion.Slerp(from.rotation, look, step * omega);
			this.entity.myTransform.position = Vector3.Lerp(from.position, to, step) + up * height;

			yield return null;
		}

		// reach new cell
		this.entity.myTransform.SetPositionAndRotation(to, look);
		this.entity.cellable.UpdateCurrentCell();
	}
}
