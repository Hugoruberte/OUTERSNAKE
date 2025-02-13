﻿using UnityEngine;


namespace Interactive.Engine
{
	public enum PhysicalInteraction
	{
		Contact = 0,
		Explosion,
		Lazer
	}

	public abstract class PhysicalInteractionEntity
	{
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ----------------------------------- PHYSICAL INTERACTION ------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		public static PhysicalInteractionEntity contact = new Contact();
		private class Contact : PhysicalInteractionEntity {
			public Contact() : base(PhysicalInteraction.Contact) {}
		}

		public static PhysicalInteractionEntity explosion = new Explosion();
		private class Explosion : PhysicalInteractionEntity {
			public Explosion() : base(PhysicalInteraction.Explosion) {}
		}

		public static PhysicalInteractionEntity lazer = new Lazer();
		private class Lazer : PhysicalInteractionEntity {
			public Lazer() : base(PhysicalInteraction.Lazer) {}
		}
















		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ------------------------------------------- CODE --------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		public readonly PhysicalInteraction type;
		
		private const float MIN_INTENSITY = 0f;
		private const float MAX_INTENSITY = 100f;
		private float _intensity = MIN_INTENSITY;
		public float intensity {
			get { return this._intensity; }
			set { this._intensity = (value > MAX_INTENSITY) ? MAX_INTENSITY : ((value < MIN_INTENSITY) ? MIN_INTENSITY : value); }
		}

		public GameObject other = null;

		public PhysicalInteractionEntity(PhysicalInteraction r)
		{
			this.type = r;
		}

		public override string ToString() => $"{this.type}";
	}
}

