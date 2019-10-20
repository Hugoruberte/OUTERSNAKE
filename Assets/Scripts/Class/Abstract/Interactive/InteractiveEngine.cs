using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;


namespace Interactive.Engine
{
    [CreateAssetMenu(fileName = "InteractiveEngine", menuName = "Scriptable Object/Other/InteractiveEngine", order = 1)]
	public class InteractiveEngine : ScriptableSingleton<InteractiveEngine>
	{
		private static List<InteractiveExtensionEngine> extensions = new List<InteractiveExtensionEngine>();
		private static ChemistryEngine chemistry = new ChemistryEngine();
		private static PhysicEngine physic = new PhysicEngine();

		public void OnAwake()
		{
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

		private static void Reaction(InteractiveEntity main, ChemicalElementEntity element, PhysicalInteractionEntity interaction)
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
























		[HideInInspector] protected internal List<ChemicalElementMixEntity> chemicalElementMixEntityPoolList = new List<ChemicalElementMixEntity>();
		[HideInInspector] protected internal List<ChemicalElement> chemicalElementPoolList = new List<ChemicalElement>();

		[HideInInspector] public List<ChemicalToArrayData> primaries = new List<ChemicalToArrayData>();
		[HideInInspector] public List<ChemicalToArrayData> weaknesses = new List<ChemicalToArrayData>();

		[HideInInspector] private List<IntToChemicalData> couples = new List<IntToChemicalData>();
		[HideInInspector] private List<IntToChemicalData> winners = new List<IntToChemicalData>();

		private readonly StringBuilder stringBuilder = new StringBuilder();
		private const string voiddString = "Interactive.Engine.Void";

		[HideInInspector] public bool[] inspector_showDetails;


		public bool HasPrimariesOf(ChemicalElementEntity ent) => this.primaries.Exists(x => x.element == ent.type);

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



		public void ClearAll()
		{
			this.chemicalElementMixEntityPoolList.Clear();
			this.chemicalElementPoolList.Clear();

			this.primaries.Clear();
			this.weaknesses.Clear();

			this.couples.Clear();
			this.winners.Clear();
		}

		public void WarmUp()
		{
			this.ClearAll();

			Type t;
			Type[] types;

			ChemicalElementEntity element;
			PhysicalInteractionEntity interaction;
			InteractiveStatus status;

			ChemicalElementEntity[] elements;
			ChemicalMaterialEntity[] materials;
			PhysicalStateEntity[] states;

			object[] parameters;



			t = typeof(ChemicalElementEntity);
			types = Assembly.GetAssembly(t).GetExportedTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(t)).ToArray();

			parameters = new object[] {0f};

			elements = new ChemicalElementEntity[types.Length];
			for(int i = 0; i < elements.Length; ++i) {
				elements[i] = Activator.CreateInstance(types[i], parameters) as ChemicalElementEntity;
			}

		
			
			t = typeof(ChemicalMaterialEntity);
			types = Assembly.GetAssembly(t).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(t)).ToArray();

			materials = new ChemicalMaterialEntity[types.Length];
			for(int i = 0; i < materials.Length; ++i) {
				materials[i] = t.GetField(types[i].Name.ToLower()).GetValue(null) as ChemicalMaterialEntity;
			}

			
			t = typeof(PhysicalStateEntity);
			types = Assembly.GetAssembly(t).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(t)).ToArray();

			states = new PhysicalStateEntity[types.Length];
			for(int i = 0; i < states.Length; ++i) {
				states[i] = t.GetField(types[i].Name.ToLower()).GetValue(null) as PhysicalStateEntity;
			}

			

			float start = Time.realtimeSinceStartup;

			// ELEMENT * ELEMENT
			for(int i = 0; i < elements.Length; ++i) {
				for(int j = i; j < elements.Length; ++j) {
					element = elements[i] * elements[j];
				}
			}

			// ELEMENT * MATERIAL
			for(int i = 0; i < elements.Length; ++i) {
				for(int j = 0; j < materials.Length; ++j) {
					element = elements[i] * materials[j];
				}
			}

			// ELEMENT * STATE
			for(int i = 0; i < elements.Length; ++i) {
				for(int j = 0; j < states.Length; ++j) {
					status = elements[i] * states[j];
				}
			}


			// STATE * STATE
			for(int i = 0; i < states.Length; ++i) {
				for(int j = i; j < states.Length; ++j) {
					interaction = states[i] * states[j];
				}
			}

			Debug.Log($"Took {Time.realtimeSinceStartup - start} seconds");

			// INSPECTOR
			this.inspector_showDetails = new bool[this.primaries.Count];
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


