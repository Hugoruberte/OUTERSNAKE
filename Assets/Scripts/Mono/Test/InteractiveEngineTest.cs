using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactive.Engine;

public class InteractiveEngineTest : MonoBehaviour
{
	void OnEnable()
	{
		ChemicalElementEntity[] elements = new ChemicalElementEntity[] {new Voidd(), new Fire(), new Water(), new Wind(), new Earth(), new Lightning(), new Ice(), new Magma(), new Steam(), new Snow()};

		ChemicalElementEntity result;
		
		// ChemicalMaterialEntity wood = ChemicalMaterialEntity.wood;
		// ChemicalMaterialEntity metal = ChemicalMaterialEntity.metal;

		// PhysicalStateEntity explosive = PhysicalStateEntity.explosive;
		// PhysicalStateEntity frozen = PhysicalStateEntity.frozen;
		// PhysicalStateEntity burn = PhysicalStateEntity.burn;


		// Debug.Log("\n\nELEMENT * ELEMENT:");
		for(int i = 0; i < elements.Length; i++) {
			for(int j = i; j < elements.Length; j++) {
				result = elements[i] * elements[j];
				// Debug.Log($"{elements[i]} * {elements[j]} = {result}");
			}
			// Debug.Log("\n");
		}
		// Debug.Log("Done");

		


		// Debug.LogWarning("\n\nELEMENT * MATERIAL:");
		// Debug.LogWarning($"{fire} * {wood} = {fire * wood}");
		// Debug.LogWarning($"{water} * {wood} = {water * wood}");
		// Debug.LogWarning($"{fire} * {metal} = {fire * metal}");
		// Debug.LogWarning($"{lightning} * {metal} = {lightning * metal}");
		


		// Debug.LogWarning("\n\nELEMENT * STATE");
		// Debug.LogWarning($"{burn} * {fire} = {burn * fire}");
		// Debug.LogWarning($"{explosive} * {fire} = {explosive * fire}");
		// Debug.LogWarning($"{frozen} * {fire} = {frozen * fire}");



		// Debug.LogWarning("\n\nSTATE * STATE:");
		// Debug.LogWarning($"{frozen} * {explosive} = {frozen * explosive}");
		// Debug.LogWarning($"{frozen} * {burn} = {frozen * burn}");
		
		//Debug.LogWarning($"{fire} * {wood} * {frozen} = ({fire * wood}) * {frozen} = {fire * wood * frozen}");
	}
}
