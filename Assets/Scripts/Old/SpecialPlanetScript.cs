using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPlanetScript : MonoBehaviour
{
	[Header("State")]
	public bool MainPlanet = true;

	[HideInInspector]
	public Cell[] Grid;
}
