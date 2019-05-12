﻿using UnityEngine;

public abstract class CharacterEntity : LivingEntity, IFoodChainEntity
{
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------- FOOD CHAIN -----------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public int foodChainRank { get; protected set; }
	public float foodChainValue { get; protected set; }

	public virtual void FoodChainInteraction(float value)
	{
		throw new System.NotImplementedException("Need to implement food chain logic.");
	}
}