using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactive.Engine;

public class InteractiveEngineTest : MonoBehaviour
{
	void OnEnable()
	{
		ChemicalElementEntity voidd = ChemicalElementEntity.voidd;
		ChemicalElementEntity fire = new Fire();
		ChemicalElementEntity water = new Water();
		ChemicalElementEntity wind = new Wind();
		ChemicalElementEntity earth = new Earth();
		ChemicalElementEntity lightning = new Lightning();
		ChemicalElementEntity ice = new Ice();
		ChemicalElementEntity magma = new Magma();
		ChemicalElementEntity steam = new Steam();
		ChemicalElementEntity snow = new Snow();
		
		ChemicalMaterialEntity wood = ChemicalMaterialEntity.wood;
		ChemicalMaterialEntity metal = ChemicalMaterialEntity.metal;

		PhysicalStateEntity explosive = PhysicalStateEntity.explosive;
		PhysicalStateEntity frozen = PhysicalStateEntity.frozen;
		PhysicalStateEntity burn = PhysicalStateEntity.burn;


		Debug.LogWarning("\n\nELEMENT * ELEMENT:");
		Debug.LogWarning($"{fire} * {voidd} = {fire * voidd}");
		Debug.LogWarning($"{fire} * {fire} = {fire * fire}");
		Debug.LogWarning($"{fire} * {water} = {fire * water}");
		Debug.LogWarning($"{fire} * {wind} = {fire * wind}");
		Debug.LogWarning($"{fire} * {earth} = {fire * earth}");
		Debug.LogWarning($"{fire} * {ice} = {fire * ice}");
		Debug.LogWarning($"{fire} * {lightning} = {fire * lightning}");
		Debug.LogWarning($"{fire} * {magma} = {fire * magma}");
		Debug.LogWarning($"{fire} * {steam} = {fire * steam}");

		Debug.LogWarning($"{water} * {voidd} = {water * voidd}");
		Debug.LogWarning($"{water} * {water} = {water * water}");
		Debug.LogWarning($"{water} * {wind} = {water * wind}");
		Debug.LogWarning($"{water} * {earth} = {water * earth}");
		Debug.LogWarning($"{water} * {ice} = {water * ice}");
		Debug.LogWarning($"{water} * {lightning} = {water * lightning}");
		Debug.LogWarning($"{water} * {magma} = {water * magma}");
		Debug.LogWarning($"{water} * {steam} = {water * steam}");

		Debug.LogWarning($"{wind} * {voidd} = {wind * voidd}");
		Debug.LogWarning($"{wind} * {wind} = {wind * wind}");
		Debug.LogWarning($"{wind} * {earth} = {wind * earth}");
		Debug.LogWarning($"{wind} * {ice} = {wind * ice}");
		Debug.LogWarning($"{wind} * {lightning} = {wind * lightning}");
		Debug.LogWarning($"{wind} * {magma} = {wind * magma}");
		Debug.LogWarning($"{wind} * {steam} = {wind * steam}");

		Debug.LogWarning($"{earth} * {voidd} = {earth * voidd}");
		Debug.LogWarning($"{earth} * {earth} = {earth * earth}");
		Debug.LogWarning($"{earth} * {ice} = {earth * ice}");
		Debug.LogWarning($"{earth} * {lightning} = {earth * lightning}");
		Debug.LogWarning($"{earth} * {magma} = {earth * magma}");
		Debug.LogWarning($"{earth} * {steam} = {earth * steam}");

		Debug.LogWarning($"{ice} * {voidd} = {ice * voidd}");
		Debug.LogWarning($"{ice} * {ice} = {ice * ice}");
		Debug.LogWarning($"{ice} * {lightning} = {ice * lightning}");
		Debug.LogWarning($"{ice} * {magma} = {ice * magma}");
		Debug.LogWarning($"{ice} * {steam} = {ice * steam}");

		Debug.LogWarning($"{lightning} * {voidd} = {lightning * voidd}");
		Debug.LogWarning($"{lightning} * {lightning} = {lightning * lightning}");
		Debug.LogWarning($"{lightning} * {magma} = {lightning * magma}");
		Debug.LogWarning($"{lightning} * {steam} = {lightning * steam}");

		Debug.LogWarning($"{magma} * {voidd} = {magma * voidd}");
		Debug.LogWarning($"{magma} * {magma} = {magma * magma}");
		Debug.LogWarning($"{magma} * {steam} = {magma * steam}");

		Debug.LogWarning($"{steam} * {voidd} = {steam * voidd}");
		Debug.LogWarning($"{steam} * {steam} = {steam * steam}");

		Debug.LogWarning($"{voidd} * {voidd} = {voidd * voidd}");

		Debug.LogWarning($"{snow} * {voidd} = {snow * voidd}");
		Debug.LogWarning($"{snow} * {fire} = {snow * fire}");
		Debug.LogWarning($"{snow} * {water} = {snow * water}");
		Debug.LogWarning($"{snow} * {wind} = {snow * wind}");
		Debug.LogWarning($"{snow} * {earth} = {snow * earth}");
		Debug.LogWarning($"{snow} * {ice} = {snow * ice}");
		Debug.LogWarning($"{snow} * {lightning} = {snow * lightning}");
		Debug.LogWarning($"{snow} * {magma} = {snow * magma}");
		Debug.LogWarning($"{snow} * {steam} = {snow * steam}");
		Debug.LogWarning($"{snow} * {snow} = {snow * snow}");
		


		Debug.LogWarning("\n\nELEMENT * MATERIAL:");
		Debug.LogWarning($"{fire} * {wood} = {fire * wood}");
		Debug.LogWarning($"{water} * {wood} = {water * wood}");
		Debug.LogWarning($"{fire} * {metal} = {fire * metal}");
		Debug.LogWarning($"{lightning} * {metal} = {lightning * metal}");
		


		Debug.LogWarning("\n\nELEMENT * STATE");
		Debug.LogWarning($"{burn} * {fire} = {burn * fire}");
		Debug.LogWarning($"{explosive} * {fire} = {explosive * fire}");
		Debug.LogWarning($"{frozen} * {fire} = {frozen * fire}");



		Debug.LogWarning("\n\nSTATE * STATE:");
		Debug.LogWarning($"{frozen} * {explosive} = {frozen * explosive}");
		Debug.LogWarning($"{frozen} * {burn} = {frozen * burn}");
		
		//Debug.LogWarning($"{fire} * {wood} * {frozen} = ({fire * wood}) * {frozen} = {fire * wood * frozen}");
	}
}
