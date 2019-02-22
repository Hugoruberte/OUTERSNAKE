using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public abstract class RabbitController : MovementController
{
	protected RabbitEntity rabbit;

	public float jumpHeight = 2f;

	protected const float ROTATION_SPEED = 1.75f;
	private const float ROTATION_OVER_FACE_SPEED = 1f;


	public RabbitController(RabbitEntity r)
	{
		this.entity = r;
		this.rabbit = r;
	}







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
		yield return entity.StartCoroutine(JumpTo(stepover.cell, from, stepover.rotation, stepover.up, ROTATION_OVER_FACE_SPEED));
	}

	protected virtual IEnumerator StepTowards(Cell c)
	{
		// declaration
		_Transform from;
		Quaternion look;

		// check
		if(TargetIsTooFarAway(c)) {
			yield break;
		}

		// initialization
		from = new _Transform(this.entity.myTransform);
		look = Quaternion.LookRotation(c.position - this.entity.cellable.currentCell.position, entity.myTransform.up).SetAbsoluteRotation();

		// jump
		yield return entity.StartCoroutine(JumpTo(c, from, look, this.entity.myTransform.up, ROTATION_SPEED));
	}

	public virtual IEnumerator Rest()
	{
		float duration;

		duration = Random.Range(this.rabbit.minAfterJumpTempo, this.rabbit.maxAfterJumpTempo);

		yield return new WaitForSeconds(duration);
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
		float step;

		// initialization
		to = c.position;
		step = 0f;

		// move to new cell
		this.entity.cellable.ReserveNextCell(c);
		while(step < 1f) {
			step += this.entity.speed * Time.deltaTime;

			// movement
			this.entity.myTransform.rotation = Quaternion.Slerp(from.rotation, look, step * omega);
			this.entity.myTransform.position = Vector3.Lerp(from.position, to, step) + up * Mathf.PingPong(step * this.jumpHeight * 2f, this.jumpHeight);

			yield return null;
		}

		// reach new cell
		this.entity.myTransform.SetPositionAndRotation(to, look);
		this.entity.cellable.UpdateCurrentCell();
	}
}
