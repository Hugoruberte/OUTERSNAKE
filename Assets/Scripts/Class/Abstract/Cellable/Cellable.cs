using UnityEngine;


public class Cellable
{
	private Transform transform;
	private Vector3 normal;

	private Cell reservedCell = null;
	public Cell currentCell { get; private set; } = null;

	public Surface currentSurface { get; private set; } = null;

	public bool isWalkable { get; private set; } = true;
	public bool initialized { get; private set; } = false;



	// must be called after 'Awake' (in 'Start' for example)
	public void Initialize(Transform t)
	{
		this.transform = t;
		this.normal = this.transform.up;

		// this must be called in 'Start'
		this.InitializeSurfaceAndCell();

		this.initialized = true;
	}










	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------- INTERACT WITH PLANET SURFACE FUNCTION ---------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public void ReserveNextCell(Cell c)
	{
		this.reservedCell = c;
		this.reservedCell?.AddElement(this);
	}
	public void ReserveNextCell(Vector3 pos, Vector3 normal)
	{
		this.reservedCell = this.currentSurface.GetCellWithPositionAndFaceNormal(pos, normal);
		this.reservedCell?.AddElement(this);
	}

	public void UpdateCurrentCell()
	{
		this.currentCell?.RemoveElement(this);
		this.currentCell = this.reservedCell;
		this.reservedCell = null;
	}
	public void UpdateCurrentCell(Vector3 pos, Vector3 normal)
	{
		if(this.currentSurface == null) {
			this.InitializeSurfaceAndCell();
		}

		this.reservedCell = this.currentSurface.GetCellWithPositionAndFaceNormal(pos, normal);
		this.reservedCell?.AddElement(this);

		this.UpdateCurrentCell();
	}










	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------------------- UTIL FUNCTION ---------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	private void InitializeSurfaceAndCell()
	{
		Planet[] planets;

		planets = PlanetManager.instance.planets;

		foreach(Planet p in planets)
		{
			if(p.IsPlanetOf(transform)) {

				this.currentCell = p.SetElementOnPlanet(transform);
				this.currentCell.AddElement(this);
				this.currentSurface = this.currentCell.surface;
				return;
			}
		}

		this.currentCell = null;
	}

	private void ClearCell(Cell c)
	{
		c?.RemoveElement(this);

		if(c == this.currentCell) {
			this.currentCell = null;
		}
		if(c == this.reservedCell) {
			this.reservedCell = null;
		}
	}

	// WARNING : These three functions are sorted in terms of optimization and so execution speed !
	// The first one is the faster and should be prioritized !
	// The last one should normally never be used or rarely...
	// -> Rarely used, never found a case to use it...
	private void UpdateCellWithPositionOnSameFace(Vector3 newPosition)
	{
		if(this.currentCell == null) {
			Debug.LogError("ERROR : Could not use this function because 'currentCell' value is null.", transform);
			return;
		}

		this.currentCell.RemoveElement(this);

		// This function uses the previous cell to rapidly find the new cell
		this.currentCell = this.currentSurface.GetCellWithPositionOnSameFace(newPosition, this.currentCell);

		// It is possible for 'currentCell' to be null here
		// For example when snake switch face...
		this.currentCell?.AddElement(this);
	}
	// -> Used most of the time
	private void UpdateCellWithPositionAndFaceNormal(Vector3 newPosition, Vector3 faceNormal)
	{
		this.currentCell?.RemoveElement(this);

		// This function uses the face normal to select all 
		// possible grill and then search the new cell among them
		this.currentCell = this.currentSurface.GetCellWithPositionAndFaceNormal(newPosition, faceNormal);

		this.currentCell?.AddElement(this);
	}
	// -> Nearly never used
	private void UpdateCellWithOnlyPosition(Vector3 newPosition)
	{
		this.currentCell?.RemoveElement(this);

		// This function browse every cell of the surface and so could be expensive...
		this.currentCell = this.currentSurface.GetCellWithOnlyPosition(newPosition);

		this.currentCell?.AddElement(this);
	}
}


