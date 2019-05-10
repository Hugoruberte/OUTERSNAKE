using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Interactive.Engine
{
	[CreateAssetMenu(fileName = "InteractiveEngine", menuName = "Scriptable Object/Other/InteractiveEngine", order = 1)]
	public class InteractiveEngine : ScriptableSingleton<InteractiveEngine>
	{
		[HideInInspector] public List<ChemicalElementMixEntity> chemicalElementMixEntityPoolList = new List<ChemicalElementMixEntity>();
		[HideInInspector] public List<ChemicalElement> chemicalElementPoolList = new List<ChemicalElement>();
		[HideInInspector] public List<ChemicalToArrayData> primaries = new List<ChemicalToArrayData>();
		[HideInInspector] public List<ChemicalToArrayData> weaknesses = new List<ChemicalToArrayData>();
		[HideInInspector] public List<IntToChemicalData> couples = new List<IntToChemicalData>();
		[HideInInspector] public List<IntToChemicalData> winners = new List<IntToChemicalData>();

		private readonly StringBuilder stringBuilder = new StringBuilder();
		private const string voiddString = "Interactive.Engine.Void";
		
		private static List<InteractiveExtensionEngine> extensions = new List<InteractiveExtensionEngine>();
		private static ChemistryEngine chemistry = new ChemistryEngine();
		private static PhysicEngine physic = new PhysicEngine();

		protected override void OnEnable()
		{
			base.OnEnable();
			
			extensions.Add(new FoodChainEngine());
		}

		public static void InteractionBetween(InteractiveEntity main, InteractiveEntity other, Collision collision)
		{
			// declaration
			PhysicalInteractionEntity physicalInteraction;
			ChemicalElementEntity chemicalInteraction;

			// interactions
			chemicalInteraction = chemistry.InteractionBetween(main, other);
			physicalInteraction = physic.InteractionBetween(main, other, collision);
			
			// reaction
			Reaction(main, chemicalInteraction, physicalInteraction);
			
			// extensions interaction + reaction
			foreach(InteractiveExtensionEngine ext in extensions) {
				ext.InteractionBetween(main, other);
			}
		}

		public static void Reaction(InteractiveEntity main, ChemicalElementEntity element, PhysicalInteractionEntity interaction)
		{
			InteractiveStatus status;

			// Calculate reaction :
			// 1. Result between 'current physical state' and 'possible element' 
			// For example : Frozen * Fire = Neutral; Water
			status = main.physical * element;
			
			// Update entity interactive status
			main.physical = status.state;
			main.chemical = status.element;

			// main manage its new status && the interaction with the unknown entity
			main.InteractWith(status, interaction);
		}


























		public bool HasPrimariesOf(ChemicalElementEntity ent)
		{
			// only does that
			return this.primaries.Exists(x => x.element == ent.type);
		}
		public void SetPrimariesOf(ChemicalElementEntity ent, ChemicalElement[] ps)
		{
			this.primaries.Add(new ChemicalToArrayData(ent.type, ps));
			this.primaries.Sort(CompareChemicalToArrayData);
		}
		public ChemicalElement[] GetPrimariesOf(ChemicalElementEntity ent)
		{
			ChemicalToArrayData data = this.primaries.Find(x => x.element == ent.type);
			if(data.array != null) {
				return data.array;
			} else {
				Debug.LogWarning($"WARNING : This element ({ent.type}) is not yet registered ! Check it out !");
				return null;
			}
		}



		public bool HasWeaknessesOf(ChemicalElementEntity ent)
		{
			// only does that
			return this.weaknesses.Exists(x => x.element == ent.type);
		}
		public void SetWeaknessesOf(ChemicalElementEntity ent, ChemicalElement[] ps)
		{
			this.weaknesses.Add(new ChemicalToArrayData(ent.type, ps));
			this.weaknesses.Sort(CompareChemicalToArrayData);
		}
		public ChemicalElement[] GetWeaknessesOf(ChemicalElementEntity ent)
		{
			ChemicalToArrayData data = this.weaknesses.Find(x => x.element == ent.type);
			if(data.array != null) {
				return data.array;
			} else {
				Debug.LogWarning($"WARNING : This element ({ent.type}) is not yet registered ! Check it out !");
				return null;
			}
		}


		public bool HasMixOf(ChemicalElementEntity a, ChemicalElementEntity b)
		{
			int couple = (int)(a.type | b.type);
			return this.couples.Exists(x => x.couple == couple);
		}
		public void SetMixOf(ChemicalElementEntity a, ChemicalElementEntity b, ChemicalElementEntity ent)
		{
			int couple = (int)(a.type | b.type);
			if(!this.couples.Exists(x => x.couple == couple)) {
				if(ent == null) {
					this.couples.Add(new IntToChemicalData(couple, ChemicalElement.Void, true));
				} else {
					this.couples.Add(new IntToChemicalData(couple, ent.type));
				}
			} else {
				Debug.LogWarning($"WARNING : This couple ({a} + {b}) is already registered ! Check it out for optimization !");
			}
		}
		public string GetMixOf(ChemicalElementEntity a, ChemicalElementEntity b)
		{
			int couple = (int)(a.type | b.type);
			IntToChemicalData data = this.couples.Find(x => x.couple == couple);
			if(data.couple > 0) {
				if(data.empty) {
					return null;
				}
				this.stringBuilder.Clear();
				this.stringBuilder.Append("Interactive.Engine.").Append(data.type.ToString());
				return this.stringBuilder.ToString();
			} else {
				Debug.LogWarning($"WARNING : This couple ({a} + {b}) is not yet registered ! Check it out !");
				return voiddString;
			}
		}


		public bool HasWinnerBetween(ChemicalElementEntity main, ChemicalElementEntity other)
		{
			int couple = (int)(main.type | other.type);
			return this.winners.Exists(x => x.couple == couple);
		}
		public void SetWinnerBetween(ChemicalElementEntity main, ChemicalElementEntity other, bool isMainWinning)
		{
			int couple = (int)(main.type | other.type);
			if(!this.winners.Exists(x => x.couple == couple)) {
				if(isMainWinning) {
					this.winners.Add(new IntToChemicalData(couple, main.type));
				} else {
					this.winners.Add(new IntToChemicalData(couple, other.type));
				}
			} else {
				Debug.LogWarning($"WARNING : This couple ({main} + {other}) is already registered ! Check it out for optimization !");
			}
		}
		public bool IsWinningAgainst(ChemicalElementEntity main, ChemicalElementEntity other)
		{
			int couple = (int)(main.type | other.type);
			IntToChemicalData data = this.winners.Find(x => x.couple == couple);
			if(data.couple > 0) {
				return (data.type == main.type);
			} else {
				Debug.LogWarning($"WARNING : This couple ({main} + {other}) is not yet registered ! Check it out !");
				return false;
			}
		}





		private static int CompareChemicalToArrayData(ChemicalToArrayData x, ChemicalToArrayData y)
		{
			int xe = (int)x.element;
			int ye = (int)y.element;

			if(xe > ye) {
				return 1;
			} else if(xe < ye) {
				return -1;
			} else {
				return 0;
			}
		}
	}





	[Serializable]
	public struct ChemicalToArrayData {
		public ChemicalElement element;
		public ChemicalElement[] array;

		public ChemicalToArrayData(ChemicalElement e, ChemicalElement[] a) {
			this.element = e;
			this.array = a;
		}
	}

	[Serializable]
	public struct IntToChemicalData {
		public int couple;
		public ChemicalElement type;
		public bool empty;

		public IntToChemicalData(int c, ChemicalElement t, bool e = false) {
			this.couple = c;
			this.type = t;
			this.empty = e;
		}
	}

	public struct InteractiveStatus {
		public PhysicalStateEntity state;
		public ChemicalElementEntity element;
		
		public InteractiveStatus(PhysicalStateEntity s, ChemicalElementEntity e) {
			this.state = s;
			this.element = e;
		}

		public override string ToString() => $"{state}; {element}";
	}
}


