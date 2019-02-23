using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
	private static PlanetManager _instance = null;
	public static PlanetManager instance {
		get {
			if(_instance == null) {
				Debug.LogError("Instance is null, either you tried to access it from the Awake function or it has not been initialized in its own Awake function");
			}
			return _instance;
		}
		private set {
			if(_instance != null) {
				Debug.LogWarning("Several instance has been set ! Check it out.");
				Destroy(_instance);
			}
			_instance = value;
		}
	}

	public Planet[] planets;

	void Awake()
	{
		instance = this;

		this.planets = FindObjectsOfType<Planet>() as Planet[];
	}
}
