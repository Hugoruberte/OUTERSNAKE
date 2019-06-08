using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	public Surface surface;

	private void Awake()
	{
		this.surface = new Surface(transform);
	}

	public bool IsPlanetOf(Transform t)
	{
		foreach(Grill g in this.surface.grills) {
			if(g.IsGrillOf(t)) {
				return true;
			}
		}

		return false;
	}

	public Cell SetElementOnPlanet(Transform t)
	{
		foreach(Grill g in this.surface.grills) {
			if(g.IsGrillOf(t)) {
				return g.GetCellWithPosition(t.position);
			}
		}

		Debug.LogError("ERROR : This transform was not on this planet ! Check before using this function.\nUse 'CellableEntity.InitializeSurfaceAndCell' to avoid this error.");

		return null;
	}
}
