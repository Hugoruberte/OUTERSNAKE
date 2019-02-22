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

	public abstract class SnakeEntity : CharacterEntity
	{
		// public Transform _transform { get { return this.myTransform; } }

		protected override void Awake()
		{
			base.Awake();

			this.SetInteractiveState(new Fire(), ChemicalMaterialEntity.flesh, PhysicalStateEntity.neutral);
		}
	}
}