using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactive.Engine;
using System;
using System.Linq;


[CreateAssetMenu(fileName = "InteractiveEngineData", menuName = "Scriptable Object/Other/InteractiveEngineData", order = 3)]
public class InteractiveEngineData : ScriptableObject
{
	[HideInInspector] public List<ChemicalElementMixEntity> chemicalElementMixEntityPoolList = new List<ChemicalElementMixEntity>();
	[HideInInspector] public List<ChemicalElement> chemicalElementPoolList = new List<ChemicalElement>();

	public Dictionary<ChemicalElement, ChemicalElement[]> primaries = new Dictionary<ChemicalElement, ChemicalElement[]>();
	public Dictionary<ChemicalElement, ChemicalElement[]> weaknesses = new Dictionary<ChemicalElement, ChemicalElement[]>();
	public Dictionary<int, ChemicalElementEntity> couples = new Dictionary<int, ChemicalElementEntity>();
	public Dictionary<int, ChemicalElement> winners = new Dictionary<int, ChemicalElement>();



	
	public bool HasPrimariesOf(ChemicalElementEntity ent)
	{
		return this.primaries.ContainsKey(ent.type);
	}
	public void SetPrimariesOf(ChemicalElementEntity ent, ChemicalElement[] ps)
	{
		if(!this.primaries.ContainsKey(ent.type)) {
			this.primaries.Add(ent.type, ps);
		} else {
			Debug.LogWarning($"WARNING : This element ({ent.type}) is already registered ! Check it out for optimization !");
		}
	}
	public ChemicalElement[] GetPrimariesOf(ChemicalElementEntity ent)
	{
		if(this.primaries.ContainsKey(ent.type)) {
			return this.primaries[ent.type];
		} else {
			Debug.LogWarning($"WARNING : This element ({ent.type}) is not yet registered ! Check it out !");
			return null;
		}
	}



	public bool HasWeaknessesOf(ChemicalElementEntity ent)
	{
		return this.weaknesses.ContainsKey(ent.type);
	}
	public void SetWeaknessesOf(ChemicalElementEntity ent, ChemicalElement[] ws)
	{
		if(!this.weaknesses.ContainsKey(ent.type)) {
			this.weaknesses.Add(ent.type, ws);
		} else {
			Debug.LogWarning($"WARNING : This element ({ent.type}) is already registered ! Check it out for optimization !");
		}
	}
	public ChemicalElement[] GetWeaknessesOf(ChemicalElementEntity ent)
	{
		if(this.weaknesses.ContainsKey(ent.type)) {
			return this.weaknesses[ent.type];
		} else {
			Debug.LogWarning($"WARNING : This element ({ent.type}) is not yet registered ! Check it out !");
			return null;
		}
	}


	public bool HasMixOf(ChemicalElementEntity a, ChemicalElementEntity b)
	{
		int couple = (int)(a.type | b.type);
		return this.couples.ContainsKey(couple);
	}
	public void SetMixOf(ChemicalElementEntity a, ChemicalElementEntity b, ChemicalElementEntity ent)
	{
		int couple = (int)(a.type | b.type);
		if(!this.couples.ContainsKey(couple)) {
			this.couples.Add(couple, ent);
		} else {
			Debug.LogWarning($"WARNING : This couple ({a} + {b}) is already registered ! Check it out for optimization !");
		}
	}
	public ChemicalElementEntity GetMixOf(ChemicalElementEntity a, ChemicalElementEntity b)
	{
		int couple = (int)(a.type | b.type);
		if(this.couples.ContainsKey(couple)) {
			return this.couples[couple].Spawn();
		} else {
			Debug.LogWarning($"WARNING : This couple ({a} + {b}) is not yet registered ! Check it out !");
			return null;
		}
	}


	public bool HasWinnerBetween(ChemicalElementEntity main, ChemicalElementEntity other)
	{
		int couple = (int)(main.type | other.type);
		return this.winners.ContainsKey(couple);
	}
	public void SetWinnerBetween(ChemicalElementEntity main, ChemicalElementEntity other, bool isMainWinning)
	{
		int couple = (int)(main.type | other.type);
		if(!this.winners.ContainsKey(couple)) {
			if(isMainWinning) {
				this.winners.Add(couple, main.type);
			} else {
				this.winners.Add(couple, other.type);
			}
		} else {
			Debug.LogWarning($"WARNING : This couple ({main} + {other}) is already registered ! Check it out for optimization !");
		}
	}
	public bool IsWinningAgainst(ChemicalElementEntity main, ChemicalElementEntity other)
	{
		int couple = (int)(main.type | other.type);
		if(this.winners.ContainsKey(couple)) {
			return (this.winners[couple] == main.type);
		} else {
			Debug.LogWarning($"WARNING : This couple ({main} + {other}) is not yet registered ! Check it out !");
			return false;
		}
	}
}
