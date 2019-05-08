using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeAI;
using My.Tools;

public class WhiteRabbitController : RabbitController
{
	public WhiteRabbitController(WhiteRabbit r) : base(r) {}


	public override IEnumerator Wander()
	{
		// declaration
		Cell c;
		StepOver stepover;
		int radius;
		bool random;

		// jump over face
		random = (Random.Range(0, 100) < 33);
		if(this.entity.cellable.currentCell.isBound && random)
		{
			stepover = this.GetOneStepOverFrom(this.entity.cellable.currentCell);
			yield return this.StepOverFace(stepover);
		}
		else
		{
			// search new cell
			radius = Random.Range(1, this.rabbit.rabbitData.maxStepDistance + 1);
			c = this.GetOneSurroundingCell(radius);
			if(c != null) {
				yield return this.StepTowards(c);
			}
		}
	}

	public override IEnumerator RunAway(Transform from)
	{
		// declaration
		Cell c, current;
		Vector3 dir;
		StepOver stepover;
		
		// initialization
		current = this.entity.cellable.currentCell;
		dir = Vector3.ProjectOnPlane(current.position - from.position, current.normal).normalized;

		// search new cell
		c = this.GetOneCellWithDirection(dir, this.rabbit.rabbitData.maxStepDistance);

		// cannot move
		if(c == null)
		{
			// jump over to the new face !
			if(current.isBound)
			{
				stepover = this.GetOneStepOverFrom(current);

				// is this cell better ?
				if(Vector3.Distance(from.position, stepover.cell.position) > Vector3.Distance(from.position, current.position))
				{
					yield return this.StepOverFace(stepover);
				}
			}
			else
			{
				// search for other random cell
				c = this.GetOneSurroundingCell(this.rabbit.rabbitData.maxStepDistance);

				if(c != null) {
					yield return this.StepTowards(c);
				}
			}
		}
		// can move
		else
		{
			yield return this.StepTowards(c);
		}
	}
}
