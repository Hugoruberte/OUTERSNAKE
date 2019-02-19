﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactive.Engine
{
	[RequireComponent(typeof(Collider))]
	public abstract class InteractiveEntity : MonoBehaviour
	{
		private InteractiveStatus status = new InteractiveStatus(PhysicalStateEntity.neutral, ChemicalElementEntity.voidd);

		public PhysicalStateEntity physical {
			get { return this.status.state; }
			set { this.status.state = value; }
		}
		public ChemicalElementEntity chemical {
			get { return this.status.element; }
			set { this.status.element = value; }
		}
		public ChemicalMaterialEntity material { get; protected set; } = ChemicalMaterialEntity.flesh;
		
		private float _life = 100f;
		public float life {
			get { return this._life; }
			set {
				this._life = value;
				this.OnUpdateLife();
			}
		}

		public bool isAlive { get { return (this.life > 0f); }}

		protected Collider myCollider;




		protected virtual void Awake()
		{
			// initialize variable
			myCollider = GetComponent<Collider>();
		}

		protected virtual void Start()
		{
			// initialize state
			InteractiveEngine.chemistry.SetEntityWithChemical(this, this.setOnElement);
		}

		protected virtual void OnCollisionEnter(Collision other)
		{
			InteractiveEntity ent;

			ent = other.transform.GetComponentInParent<InteractiveEntity>();
			if(ent != null) {
				InteractiveEngine.InteractionBetween(this, ent, other);
			}
		}

		protected void SetInteractiveState(ChemicalElementEntity e, ChemicalMaterialEntity m, PhysicalStateEntity p)
		{
			this.status.state = p;
			this.status.element = e;
			this.material = m;
		}

		// life
		protected virtual void OnUpdateLife(){}

		// elements
		public delegate void SetOnElement(bool active);
		public SetOnElement setOnElement;

		public virtual void SetOnVoidd(bool active){}
		public virtual void SetOnFire(bool active){}
		public virtual void SetOnWater(bool active){}
		public virtual void SetOnWind(bool active){}
		public virtual void SetOnEarth(bool active){}
		public virtual void SetOnLightning(bool active){}
		public virtual void SetOnMagma(bool active){}
		public virtual void SetOnSteam(bool active){}
		public virtual void SetOnIce(bool active){}

		public abstract void InteractivelyReactWith(InteractiveStatus s, PhysicalInteractionEntity i);

		public override string ToString() => $"{gameObject.name} (Interactive Entity: status = {status} and material = {material})";
	}
}


