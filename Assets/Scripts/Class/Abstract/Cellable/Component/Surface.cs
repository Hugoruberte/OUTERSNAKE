using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface
{
	public List<Grill> grills = new List<Grill>();

	private const float MIN_ACCEPTABLE_DIST = 0.49f;

	public Surface(Transform planet)
	{
		InitializeGrills(planet);
	}







	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------- INTERACT WITH PLANET SURFACE FUNCTION ---------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public Dictionary<Grill, Cell[,]> GetAllSurfaceCells()
	{
		int gwidth;
		int gheight;
		Dictionary<Grill, Cell[,]> dgcs;
		Cell[,] cs;
		Grill g;

		dgcs = new Dictionary<Grill, Cell[,]>();

		for(int i = 0; i < this.grills.Count; i++) {

			g = this.grills[i];
			gwidth = g.width;
			gheight = g.height;

			cs = new Cell[gheight, gwidth];

			for(int w = 0; w < gwidth; w++) {
				for(int h = 0; h < gheight; h++) {
					cs[h, w] = g.cells[w * gheight + h];
				}
			}

			dgcs.Add(g, cs);
		}

		return dgcs;
	}

	public Grill GetGrillOf(Vector3 pos, Vector3 normal)
	{
		foreach(Grill g in this.grills) {
			if(g.IsGrillOf(pos, normal)) {
				return g;
			}
		}

		return null;
	}

	public Cell GetCellWithPositionOnSameFace(Vector3 pos, Cell previousCell)
	{
		Grill grill;
			
		grill = previousCell.grill;

		// same grill, good
		if(grill.IsGrillOf(pos, previousCell.normal)) {
			return grill.GetCellWithPosition(pos);
		}
		// same face but we changed our grill !
		else {
			grill = GetGrillWithPositionAndNormal(pos, previousCell.normal);
			return grill?.GetCellWithPosition(pos); 
		}
	}

	public Cell GetCellWithPositionAndFaceTransform(Vector3 pos, Transform f)
	{
		foreach(Grill g in this.grills) {
			if(g.face == f) {
				return g.GetCellWithPosition(pos);
			}
		}

		return null;
	}

	public Cell GetCellWithPositionAndFaceNormal(Vector3 pos, Vector3 faceNormal)
	{
		foreach(Grill g in this.grills) {
			if(g.IsGrillOf(pos, faceNormal)) {
				return g.GetCellWithPosition(pos);
			}
		}

		return null;
	}

	public Cell GetCellWithOnlyPosition(Vector3 pos)
	{
		// only does that
		return this.GetCellWithPositionAmongGrills(pos, this.grills);
	}








	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------------------- UTIL FUNCTION ---------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	private void InitializeGrills(Transform planet)
	{
		Transform body;
		Transform faces;
		Collider[] colls;
		Grill grill;
		
		body = planet.Find("Body");
		if(body == null) {
			Debug.LogError($"ERROR : The planet ({planet.name}) does not have a 'Body' !", planet);
			return;
		}
		faces = body.Find("Faces");
		if(faces == null) {
			Debug.LogError($"ERROR : The planet's ({planet.name}) body does not have 'Faces' !", body);
			return;
		}

		colls = faces.GetComponentsInChildren<Collider>();
		foreach(Collider c in colls) {
			grill = new Grill(this, c.transform);
			this.grills.Add(grill);
		}
	}

	private Cell GetCellWithPositionAmongGrills(Vector3 pos, List<Grill> gs)
	{
		float min;
		float dist;
		Cell bestCell;

		min = float.MaxValue;
		bestCell = null;

		foreach(Grill g in gs) {
			foreach(Cell c in g.cells) {
				dist = Vector3.Distance(pos, c.position);

				if(dist < MIN_ACCEPTABLE_DIST) {
					return c;
				} else if(dist < min) {
					min = dist;
					bestCell = c;
				}
			}
		}

		return bestCell;
	}

	private Grill GetGrillWithPositionAndNormal(Vector3 pos, Vector3 n)
	{
		foreach(Grill g in this.grills) {
			if(g.IsGrillOf(pos, n)) {
				return g;
			}
		}

		return null;
	}




	






	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ------------------------------- NOT YET IMPLEMENTED FUNCTION --------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public List<Grill> GetPathAmongGrill(Grill start, Grill end)
	{
		// not yet implemented
		return null;
	}
}
