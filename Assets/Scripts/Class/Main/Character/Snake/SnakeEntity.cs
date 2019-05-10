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
		protected override void Awake()
		{
			base.Awake();

			this.SetInteractiveState(new Void(), ChemicalMaterialEntity.flesh, PhysicalStateEntity.neutral);
		}

		protected override void Start()
		{
			base.Start();

			// initialize cell
			this.cellable.Initialize(this.myTransform);
		}
	}
}