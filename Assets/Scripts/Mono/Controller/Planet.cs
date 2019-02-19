using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	public Surface surface;

	void Awake()
	{
		surface = new Surface(transform);
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

	public void SetElementOnPlanet(Transform t, ref Cell c)
	{
		foreach(Grill g in this.surface.grills) {
			if(g.IsGrillOf(t)) {
				
				c = g.GetCellWithPosition(t.position);
				return;
			}
		}

		Debug.LogError("ERROR : This transform was not on this planet ! Check before using this function.\nUse 'CellableEntity.InitializeSurfaceAndCell' to avoid this error.");
	}
}
