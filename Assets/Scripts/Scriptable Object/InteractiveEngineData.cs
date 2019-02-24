using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractiveEngineData", menuName = "Scriptable Object/Other/InteractiveEngineData", order = 3)]
public class InteractiveEngineData : ScriptableObject
{
	// CHEMICAL ELEMENT ENTITY
	public Dictionary<ChemicalElement, List<Tuple<ChemicalElement, bool>>> knownWeakAgainst = new Dictionary<ChemicalElement, List<Tuple<ChemicalElement, bool>>>();





	// CHEMICAL ELEMENT MIX ENTITY
	private Dictionary<ChemicalElement, ChemicalElement[]> knownMixesWeaknesses = new Dictionary<ChemicalElement, ChemicalElement[]>();

	private Dictionary<Tuple<ChemicalElement, ChemicalElement>, ChemicalElementEntity> knownMixes = new Dictionary<Tuple<ChemicalElement, ChemicalElement>, ChemicalElementEntity>();

	private ChemicalElementMixEntity[] _mixes = null;
	private ChemicalElementMixEntity[] mixes {
		get {
			if(_mixes == null) { _mixes = this.GetAllMixes(); }
			return _mixes;
		}
	}

	private Type[] _types = null;
	private Type[] types {
		get {
			if(_types == null) { _types = this.GetAllTypes(); }
			return _types;
		}
	}


	// FUNCTIONS
	private Type[] GetAllTypes() {
		Type t = typeof(ChemicalElementMixEntity);
		return Assembly.GetAssembly(t).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(t)).ToArray();
	}

	private ChemicalElementMixEntity[] GetAllMixes() {
		Type[] ts = types;
		ChemicalElementMixEntity[] res = new ChemicalElementMixEntity[ts.Length];
		int i = 0;

		foreach(Type type in ts) {
			res[i++] = (ChemicalElementMixEntity)Activator.CreateInstance(type, ChemicalElementEntity.STANDARD_PARAMS);
		}

		return res;
	}
}
