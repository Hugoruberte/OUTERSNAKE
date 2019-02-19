using UnityEngine;
using Interactive.Engine;

public abstract class LivingFoodChainEntity : LivingEntity, IFoodChainEntity
{
	[HideInInspector] public bool isTired = false;
	[HideInInspector] public bool isHungry = false;
	[HideInInspector] public bool isScared = false;




	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ----------------------------------- FOOD CHAIN VARIABLES ------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public float foodChainValue { get; protected set; }
	public int foodChainRank { get; protected set; }
}
