using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetManager : Singleton<PlanetManager>
{
	[HideInInspector]
	public Planet[] planets;

	void Awake()
	{
		instance = this;

		planets = FindObjectsOfType<Planet>() as Planet[];
	}
}
