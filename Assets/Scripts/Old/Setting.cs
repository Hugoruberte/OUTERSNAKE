using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Setting
{
	[Header("Quantity")]
	[Tooltip("How many apple do you want ?")]
	[Range(1, 500)]
	public int AppleAmount = 30;
	[Tooltip("Percentage of Apple which will be rotten")]
	[Range(0.0f, 1.0f)]
	public float RottenApplePercent = 0.1f;
	[Tooltip("How many rabbit do you want ?")]
	[Range(1, 30)]
	public int RabbitAmount = 6;
	[Tooltip("How many lazer do you want ?")]
	[Range(1, 30)]
	public int LazerAmount = 6;
	[Tooltip("How many tree per face ?")]
	[Range(1, 15)]
	public int TreeAmountPerFace = 6;
	[Tooltip("How many Nuclear Switch ?")]
	[Range(1, 6)]
	public int NuclearAmount = 2;
	[Tooltip("How many Red Rebbit ?")]
	[Range(1, 5)]
	public int RedRabbitAmount = 3;

	[Header("Snake")]
	[Range(0.0f, 10.0f)]
	public float InvulnerableDelay = 2.0f;

	[Header("Lazer")]
	[Range(0.0f, 20.0f)]
	public float LazerDamagePerSecond = 1f;
	[Range(5, 50)]
	public int LazerRange = 18;
	[Range(0.5f, 10.0f)]
	public float LazerLoading = 1.5f;

	[Header("Saw")]
	[Tooltip("Speed of the circular saw")]
	[Range(10, 20)]
	public float SawSpeed = 12;

	[Header("Fire")]
	[Range(0, 20)]
	public int FireDamagePerSecond = 1;

	[Header("Nuclear")]
	[Tooltip("How long the Nuclear Bomb last until explosion?")]
	[Range(1.0f, 10.0f)]
	public float NuclearDelay = 5.0f;

	[Header("RedRabbit")]
	[Range(5.0f, 50.0f)]
	public float RedRabbitSpeed = 20.0f;
	[Range(7.0f, 20.0f)]
	public float RedRabbitRange = 10.0f;
	[Range(0.1f, 5.0f)]
	public float RedRabbitDelay = 2.0f;
	[Range(0.0f, 1.0f)]
	public float RedRabbitAccuracy = 0.5f;
	[Range(0, 10)]
	public int MissileDamage = 2;
	[HideInInspector]
	public float MissileSpeed = 2.5f;

	[Header("Rabbit")]
	[Range(0, 10)]
	public int RabbitBombProb = 0;
	[Range(1, 10)]
	public int RabbitEnergy = 5;

	[Header("Meteore")]
	[Range(1.0f, 50.0f)]
	public float MeteoreSpeed = 25.0f;
	[Range(20, 75)]
	public int MeteoreHeight = 35;

	[Header("CasterBlaster")]
	[Range(0.01f, 2.0f)]
	public float CasterBlasterDuration = 0.075f;
	[Range(0.0f, 5.0f)]
	public float CasterBlasterLoading = 0.0f;
	[Range(0, 10)]
	public int CasterBlasterDamage = 2;

	[Header("SuperLazer")]
	[Range(0.1f, 5.0f)]
	public float SuperLazerDuration = 2.0f;
	[Range(0.5f, 5.0f)]
	public float SuperLazerLoading = 2.5f;
}