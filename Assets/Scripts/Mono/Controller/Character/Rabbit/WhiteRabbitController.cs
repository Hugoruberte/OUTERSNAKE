using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeAI;
using Tools;

public class WhiteRabbitController : RabbitController
{
	public WhiteRabbitController(WhiteRabbit r) : base(r) { }



	public override IEnumerator StepToWander()
	{
		// declaration
		Cell c;
		StepOver stepover;
		int radius, random;

		// jump over face
		random = Random.Range(0, 100);
		if(this.entity.currentCell.isBound && (random < 33))
		{
			stepover = this.GetOneStepOverFrom(this.entity.currentCell);
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
		Cell c;
		Vector3 dir;
		StepOver stepover;
		
		// initialization
		dir = Vector3.ProjectOnPlane(this.entity.currentCell.position - from.position, this.entity.currentCell.normal).normalized;

		// search new cell
		c = this.GetOneCellWithDirection(dir);

		// cannot move
		if(c == null)
		{
			// jump over to the new face !
			if(this.entity.currentCell.isBound)
			{
				stepover = this.GetOneStepOverFrom(this.entity.currentCell);

				// is this cell better ?
				if(Vector3.Distance(from.position, stepover.cell.position) > Vector3.Distance(from.position, this.entity.currentCell.position))
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
