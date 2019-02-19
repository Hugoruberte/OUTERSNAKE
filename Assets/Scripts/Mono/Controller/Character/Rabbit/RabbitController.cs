using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public abstract class RabbitController : MovementController
{
	protected Rabbit rabbit;

	public float jumpHeight = 2f;
	private const float ROTATION_OVER_FACE_SPEED = 1f;


	public RabbitController(Rabbit r)
	{
		this.entity = r;
		this.rabbit = r;
		this.myTransform = entity.transform;
	}





	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------- BASIC MOVEMENT CONTROLLER OVERRIDINGS ---------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	protected override IEnumerator StepOverFace(StepOver stepover)
	{
		// declaration
		_Transform from;

		// check
		if(this.entity.currentCell.isInner) {
			yield break;
		}

		// initialization
		from = new _Transform(myTransform);

		// jump
		yield return entity.StartCoroutine(JumpTo(stepover.cell, from, stepover.rotation, stepover.up, ROTATION_OVER_FACE_SPEED));
	}

	protected override IEnumerator StepTowards(Cell c)
	{
		// declaration
		_Transform from;
		Quaternion look;

		// check
		if(TargetIsTooFarAway(c)) {
			yield break;
		}

		// initialization
		from = new _Transform(myTransform);
		look = Quaternion.LookRotation(c.position - this.entity.currentCell.position, myTransform.up).SetAbsoluteRotation();

		// jump
		yield return entity.StartCoroutine(JumpTo(c, from, look, myTransform.up, ROTATION_SPEED));
	}

	public override IEnumerator Rest()
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
		this.entity.ReserveNextCell(c);
		while(step < 1f) {
			step += this.walkSpeed * Time.deltaTime;

			// movement
			myTransform.rotation = Quaternion.Slerp(from.rotation, look, step * omega);
			myTransform.position = Vector3.Lerp(from.position, to, step) + up * Mathf.PingPong(step * this.jumpHeight * 2f, this.jumpHeight);

			yield return null;
		}

		// reach new cell
		myTransform.SetPositionAndRotation(to, look);
		this.entity.UpdateCurrentCell();
	}
}
