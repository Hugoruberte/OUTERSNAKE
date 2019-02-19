using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactive.Engine
{
	public class InteractiveEngine : MonoBehaviour
	{
		private static List<InteractiveExtensionEngine> extensions = new List<InteractiveExtensionEngine>();

		public static ChemistryEngine chemistry = new ChemistryEngine();
		public static PhysicEngine physic = new PhysicEngine();

		void Awake()
		{
			// Add extension engine :
			extensions.Add(new FoodChainEngine());
		}

		public static void InteractionBetween(InteractiveEntity main, InteractiveEntity other, Collision collision)
		{
			// declaration
			PhysicalInteractionEntity interaction;
			ChemicalElementEntity element;

			// interactions
			element = chemistry.InteractionBetween(main, other);
			interaction = physic.InteractionBetween(main, other, collision);
			
			// reactions
			Reaction(main, element, interaction);
			chemistry.SetEntityWithChemical(main, main.setOnElement);

			// extensions interaction + reaction
			foreach(InteractiveExtensionEngine ext in extensions) {
				ext.InteractionBetween(main, other);
			}
		}

		public static void Reaction(InteractiveEntity main, ChemicalElementEntity element, PhysicalInteractionEntity interaction)
		{
			InteractiveStatus status;

			// Update entity interactive status
			status = main.physical * element;
			main.physical = status.state;
			main.chemical = status.element;

			// main manage its new status && the interaction with the unknown entity
			main.InteractivelyReactWith(status, interaction);
		}
	}

	public struct InteractiveStatus
	{
		public PhysicalStateEntity state;
		public ChemicalElementEntity element;
		
		public InteractiveStatus(PhysicalStateEntity s, ChemicalElementEntity e)
		{
			this.state = s;
			this.element = e;
		}

		public override string ToString() => $"{state}; {element}";
	}
}


