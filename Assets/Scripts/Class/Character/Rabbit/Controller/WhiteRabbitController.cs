using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeAI;
using Tools;

public class WhiteRabbitController : RabbitController
{
	public WhiteRabbitController(WhiteRabbit r) : base(r) {}



	public override IEnumerator StepToWander()
	{
		// declaration
		Cell c;
		StepOver stepover;
		int radius, random;

		// jump over face
		random = Random.Range(0, 100);
		if(this.entity.cellable.currentCell.isBound && (random < 33))
		{
			stepover = this.GetOneStepOverFrom(this.entity.cellable.currentCell);
			yield return entity.StartCoroutine(this.StepOverFace(stepover));
		}
		else
		{
			// search new cell
			radius = Random.Range(1, this.entity.maxStepDistance + 1);
			c = this.GetOneSurroundingCell(radius);
			if(c != null) {
				yield return entity.StartCoroutine(this.StepTowards(c));
			}
		}
	}

	public override IEnumerator StepToRunAway(Transform from)
	{
		// declaration
		Cell c, current;
		Vector3 dir;
		StepOver stepover;
		
		// initialization
		current = this.entity.cellable.currentCell;
		dir = Vector3.ProjectOnPlane(current.position - from.position, current.normal).normalized;

		// search new cell
		c = this.GetOneCellWithDirection(dir);

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
					yield return entity.StartCoroutine(this.StepOverFace(stepover));
				}
			}
			else
			{
				// search for other random cell
				c = this.GetOneSurroundingCell(this.entity.maxStepDistance);

				if(c != null) {
					yield return entity.StartCoroutine(this.StepTowards(c));
				}
			}
		}
		// can move
		else
		{
			yield return entity.StartCoroutine(this.StepTowards(c));
		}
	}
}
