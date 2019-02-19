using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactive.Engine;

namespace Snakes
{
	public enum SnakeMoveState
	{
		Run = 0,
		Stop,
		Idle
	};

	public enum SnakePartState
	{
		Alive = 0,
		Reduce,
		Reusable,
		Explode,
		Dead
	};

	public abstract class Snake : CellableEntity
	{
		public Transform _transform { get { return this.myTransform; } }

		protected override void Awake()
		{
			base.Awake();

			this.SetInteractiveState(ChemicalElementEntity.voidd, ChemicalMaterialEntity.flesh, PhysicalStateEntity.neutral);
		}
	}
}