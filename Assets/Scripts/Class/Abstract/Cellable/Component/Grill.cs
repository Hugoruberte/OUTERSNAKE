using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

public class Grill
{
	public readonly Surface surface;

	public Cell[] cells { get; private set; }

	public readonly Transform face;

	public Vector3 normal { get { return face.up; }}
	public Vector3 position { get { return face.position; }}

	public int width { get; private set; }
	public int height { get; private set; }
	private float width_corrector = 0f;
	private float height_corrector = 0f;

	private const float MAX_ACCEPTABLE_HEIGHT = 2f;
	private const float MIN_ACCEPTABLE_HEIGHT = -0.25f;

	public Grill(Surface s, Transform f)
	{
		this.surface = s;
		this.face = f;

		this.InitializeCells(face);
	}






	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------------- PUBLIC FUNCTION --------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public int[] GetIndexInGrill(Vector3 pos)
	{
		Vector3 local;

		local = this.face.InverseTransformPointUnscaled(pos);

		Debug.LogWarning("HELP : May be a problem with '.5' in 'width/2' or 'height/2' here...");

		return new int[2] {Mathf.RoundToInt(local.x + width/2), Mathf.RoundToInt(height/2 - local.z)};
	}

	public Cell GetCellWithPosition(Vector3 pos)
	{
		int index;

		index = this.GetIndexFromPosition(pos);

		if(index >= this.cells.Length) {
			Debug.LogError($"ERROR : Index ({index}) out of cells length ({cells.Length}) !", this.face);
			return null;
		} else if(index < 0) {
			return null;
		} else {
			return this.cells[index];
		}
	}

	public Cell GetOneSurroundingCellOf(Cell c, int radius, bool diagonal = true)
	{
		Vector3 p;
		int index;
		int count;
		int w, h;

		count = 100;

		do {
			w = Random.Range(-radius, radius+1);
			h = Random.Range(-radius, radius+1);
			p = c.position + w * face.right + h * face.forward;
			index = this.GetIndexFromPosition(p);
			
			count --;
		}
		while((index < 0 || (w == 0 && h == 0) || (!diagonal && w != 0 && h != 0)) && count > 0);

		if(count == 0) {
			return null;
		}

		return this.cells[index];
	}

	public Cell GetOneCellInThisDirection(Cell c, Vector3 dir, int maxLength)
	{
		Vector3 p;
		int index;

		index = -1;
		dir.Normalize();

		for(int i = maxLength; i >= 1; i--) {
			p = Vector3Int.RoundToInt(c.position + dir * i);
			index = this.GetIndexFromPosition(p);

			if(index >= 0) {
				break;
			}
		}

		if(index < 0) {
			return null;
		}

		return this.cells[index];
	}





	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------- WORK WITH SURFACE FUNCTIONS --------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	// WARNING : These functions are only called from Surface, do not called them from anywhere else !
	public bool IsGrillOf(Transform t)
	{
		// only does that
		return this.IsGrillOf(t.position, t.up);
	}
	public bool IsGrillOf(Vector3 pos, Vector3 n)
	{
		Vector3 local;
		float dot;

		dot = Vector3.Dot(this.normal, n);

		// Is grill oriented like the transform
		if(dot < 0.95f) {
			return false;
		}

		local = face.InverseTransformPointUnscaled(pos);

		// Is transform position fit in grill + is transform on grill
		return (this.IsLocalPositionInGrill(local) && (local.y >= MIN_ACCEPTABLE_HEIGHT && local.y < MAX_ACCEPTABLE_HEIGHT));
	}
	public List<Cell> GetSurroundingCellsOf(Cell c, int radius = 1)
	{
		List<Cell> surroundingCells;
		Vector3 pos;
		Vector3 p;
		int index;
		int step;

		surroundingCells = new List<Cell>();
		pos = c.position;

		for(int r = 1; r < radius; r++) {
			for(int w = -r; w <= r; w++) {
				step = (w == -r || w == r) ? 1 : 2*r;
				for(int h = -r; h <= r; h+=step) {

					p = pos + w * face.right + h * face.forward;
					
					index = this.GetIndexFromPosition(p);	
					if(index >= 0) {
						surroundingCells.Add(this.cells[index]);
					}
				}
			}
		}

		return surroundingCells;
	}

	







	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------------------- UTIL FUNCTION ---------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	private void InitializeCells(Transform face)
	{
		Vector3 pos;
		Cell cell;
		bool bound;
		float lx, lz, anchorx, anchorz;
		int i;

		if(face.gameObject.layer != LayerMask.NameToLayer("Ground")) {
			Debug.LogWarning($"WARNING : The face's ({face.name}) layer is not set to 'Ground' !", face);
		}
		this.width = Mathf.RoundToInt(face.localScale.x * 10);
		if(width < 2) {
			Debug.LogWarning($"WARNING : The face's ({face.name}) width is < 2 ! (width = {width})", face);
		}
		this.height = Mathf.RoundToInt(face.localScale.z * 10);
		if(height < 2) {
			Debug.LogWarning($"WARNING : The face's ({face.name}) height is < 2 ! (height = {height})", face);
		}

		this.width_corrector = 0.5f * (width%2) - 0.5f;
		this.height_corrector = -0.5f * (height%2) + 0.5f;

		anchorx = -(width/2) - width_corrector;
		anchorz = (height/2) - height_corrector;

		this.cells = new Cell[width * height];
		i = 0;

		for(int w = 0; w < width; w++) {
			for(int h = 0; h < height; h++) {

				lx = anchorx + w;
				lz = anchorz - h;
				pos = this.position + lx * face.right + lz * face.forward;

				bound = (w == 0 || w == width-1 || h == 0 || h == height-1);

				cell = new Cell(this, pos, normal, new Vector3(lx, 0, lz), face, bound);
				this.cells[i++] = cell;
			}
		}
	}

	private int GetIndexFromPosition(Vector3 pos)
	{
		int index;
		float columns, rows;
		Vector3 local;

		local = face.InverseTransformPointUnscaled(pos);

		if(!this.IsLocalPositionInGrill(local)) {
			return -1;
		}

		columns = Mathf.Round(local.x + width_corrector + width/2);
		rows = Mathf.Round(local.z + height_corrector - height/2);
		index = Mathf.RoundToInt(columns * height - rows);

		return index;
	}

	private bool IsLocalPositionInGrill(Vector3 local)
	{
		if(local.x > width/2f || local.x < -width/2f) {
			return false;
		}
		if(local.z > height/2f || local.z < -height/2f) {
			return false;
		}

		return true;
	}
}
