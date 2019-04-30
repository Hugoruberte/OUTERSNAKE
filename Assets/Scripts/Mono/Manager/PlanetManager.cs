﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetManager : Singleton<PlanetManager>
{
	[HideInInspector]
	public Planet[] planets;

	protected override void Awake()
	{
		base.Awake();
		
		planets = FindObjectsOfType<Planet>() as Planet[];
	}
}