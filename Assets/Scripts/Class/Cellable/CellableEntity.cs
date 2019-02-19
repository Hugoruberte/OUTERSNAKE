﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactive.Engine;


public abstract class CellableEntity : InteractiveEntity
{
	protected Transform body;
	protected Transform myTransform;
	private Vector3 currentNormal;

	private Cell reservedCell = null;
	public Cell currentCell = null;

	public Surface currentSurface { get; private set; }

	public bool isWalkable { get; protected set; } = true;


	protected override void Awake()
	{
		base.Awake();
		
		this.myTransform = transform;
		this.currentNormal = myTransform.up;
		this.body = this.myTransform.Find("Body");
	}

	protected override void Start()
	{
		base.Start();
		
		// must be called in 'Start'
		this.InitializeSurfaceAndCell();
	}

	protected virtual void OnDestroy()
	{
		ClearCell(this.currentCell);
		ClearCell(reservedCell);
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
		reservedCell = c;
		reservedCell?.AddElement(this);
	}

	public void ReserveNextCell(Vector3 pos, Vector3 normal)
	{
		reservedCell = this.currentSurface.GetCellWithPositionAndFaceNormal(pos, normal);
		reservedCell?.AddElement(this);
	}

	public void UpdateCurrentCell()
	{
		this.currentCell?.RemoveElement(this);
		this.currentCell = reservedCell;
		reservedCell = null;
	}

	public void UpdateCurrentCell(Vector3 pos, Vector3 normal)
	{
		if(this.currentSurface == null) {
			this.InitializeSurfaceAndCell();
		}
		reservedCell = this.currentSurface.GetCellWithPositionAndFaceNormal(pos, normal);
		reservedCell?.AddElement(this);

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
		foreach(Planet p in planets) {
			if(p.IsPlanetOf(myTransform)) {

				p.SetElementOnPlanet(myTransform, ref this.currentCell);
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
	private void UpdateCellWithPositionOnSameFace(Vector3 newPosition)
	{
		if(this.currentCell == null) {
			Debug.LogError("ERROR : Could not use this function because 'currentCell' value is null.", myTransform);
			return;
		}

		this.currentCell.RemoveElement(this);

		// This function uses the previous cell to rapidly find the new cell
		this.currentCell = this.currentSurface.GetCellWithPositionOnSameFace(newPosition, this.currentCell);

		// It is possible for 'currentCell' to be null here
		// For example when snake switch face...
		this.currentCell?.AddElement(this);
	}
	private void UpdateCellWithPositionAndFaceNormal(Vector3 newPosition, Vector3 faceNormal)
	{
		this.currentCell?.RemoveElement(this);

		// This function uses the face normal to select all 
		// possible grill and then search the new cell among them
		this.currentCell = this.currentSurface.GetCellWithPositionAndFaceNormal(newPosition, faceNormal);

		this.currentCell?.AddElement(this);
	}
	private void UpdateCellWithOnlyPosition(Vector3 newPosition)
	{
		this.currentCell?.RemoveElement(this);

		// This function browse every cell of the surface and so could be expensive...
		this.currentCell = this.currentSurface.GetCellWithOnlyPosition(newPosition);

		this.currentCell?.AddElement(this);
	}
}


