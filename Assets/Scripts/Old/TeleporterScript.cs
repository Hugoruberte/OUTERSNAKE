using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeleporterMode
{
	Direct,
	Transfert
};

public enum TeleporterType
{
	Local,
	Planet
};

public class TeleporterScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform TeleporterA;
	private Transform TeleporterB;

	public Transform PlanetA;
	public Transform PlanetB;
	
	private TeleporterStationScript teleAScript;
	private TeleporterStationScript teleBScript;

	public TeleporterType Type = TeleporterType.Local;
	public TeleporterMode Mode = TeleporterMode.Transfert;

	[Range(0.1f, 10.0f)]
	public float Duration = 0.25f;

	void Awake()
	{
		myTransform = transform;
		TeleporterA = myTransform.Find("TeleporterA");
		TeleporterB = myTransform.Find("TeleporterB");

		teleAScript = TeleporterA.GetComponent<TeleporterStationScript>();
		teleBScript = TeleporterB.GetComponent<TeleporterStationScript>();

		teleAScript.Receptor = TeleporterB;
		teleBScript.Receptor = TeleporterA;

		if(PlanetA && PlanetB)
		{
			teleAScript.Planet = PlanetA;
			teleBScript.Planet = PlanetB;
		}
		else
		{
			Debug.LogWarning("Please assign a planet to both variables <Planet>.");
		}
	}
}
