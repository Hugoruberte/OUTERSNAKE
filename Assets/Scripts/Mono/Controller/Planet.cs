using UnityEngine;

public class Planet : MonoBehaviour
{
	public Surface surface;

	private void Awake()
	{
		this.surface = new Surface(transform);
	}

	public bool IsPlanetOf(Transform t) => (this.GetGrillOf(t) != null);

	public Cell SetElementOnPlanet(Transform t)
	{
		Grill g = this.GetGrillOf(t);

		if(g == null) {
			Debug.LogError("ERROR : This transform was not on this planet ! Check before using this function.\nUse 'CellableEntity.InitializeSurfaceAndCell' to avoid this error.");
		}

		return g?.GetCellWithPosition(t.position);
	}

	private Grill GetGrillOf(Transform t)
	{
		foreach(Grill g in this.surface.grills)
		{
			if(g.IsGrillOf(t)) {
				return g;
			}
		}

		return null;
	}
}
