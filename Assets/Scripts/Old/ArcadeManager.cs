using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeManager : MonoBehaviour
{
	private PlanetSetup planetSetup;
	private PerformanceScript perfScript;

	void Awake()
	{
		perfScript = GetComponent<PerformanceScript>();
		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();

		GameObject.Find("Planets/Planet_01").GetComponent<PlanetScript>().MainPlanet = true;

		StartCoroutine(WaitForPerformance());
	}

	void Start()
	{
		planetSetup.SetPlanetRotation();
	}

	private IEnumerator WaitForPerformance()
	{
		perfScript.ArcadePreloading();

		yield return new WaitUntil(() => perfScript.Done);

		planetSetup.MainPlanetSetObjects();
	}
}
