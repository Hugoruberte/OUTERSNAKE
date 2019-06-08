using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetManager : MonoSingleton<PlanetManager>
{
	private Planet[] _planets = null;
	public Planet[] planets {
		get {
			if(this._planets == null) {
				this._planets = FindObjectsOfType<Planet>() as Planet[];
			}

			return this._planets;
		}
	}
}
