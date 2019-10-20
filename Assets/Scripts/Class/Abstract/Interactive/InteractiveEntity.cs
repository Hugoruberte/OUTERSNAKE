using UnityEngine;

namespace Interactive.Engine
{
    [RequireComponent(typeof(Collider))]
	public abstract class InteractiveEntity : MonoBehaviour
	{
		public Transform myTransform { get; private set; }
		protected Transform body { get; private set; }


		private InteractiveStatus status;

		public PhysicalStateEntity physical {
			get { return this.status.state; }
			set { this.status.state = value; }
		}
		public ChemicalElementEntity chemical {
			get { return this.status.element; }
			set { this.status.element = value; }
		}
		public ChemicalMaterialEntity material { get; protected set; } = ChemicalMaterialEntity.flesh;
	

		protected delegate void SetOnElement(bool active);
		protected SetOnElement currentSetOnElement;


		// Nearly everything is cellable
		public readonly Cellable cellable = new Cellable();


		




		protected virtual void Awake()
		{
			// Initialize variable
			this.myTransform = transform;
			this.body = this.myTransform.Find("Body");

			this.status = new InteractiveStatus(PhysicalStateEntity.neutral, new Void());

			if(!this.body) {
				Debug.LogWarning("WARNING : This entity does not have a \"Body\". Is this wanted ?", this.myTransform);
			}
		}

		protected virtual void Start()
		{
			// initialize state
			this.InteractWith(this.status, null);
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

		public virtual void InteractWith(InteractiveStatus s, PhysicalInteractionEntity i) {}

		public override string ToString() => $"{gameObject.name} (Interactive Entity: status = {status} and material = {material})";
	}
}


