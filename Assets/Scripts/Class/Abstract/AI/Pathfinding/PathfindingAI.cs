using System.Collections.Generic;
using UnityEngine;


namespace PathfindingAI
{
    public enum AstarMoveMode {
		Diagonal = 0,
		Euclidean,
		DiagonalFree,
		EuclideanFree,
		Manhattan
	}

	public struct Step
	{
		public Vector3 position;
		public Vector3 normal;

		public Step(Vector3 p, Vector3 n)
		{
			this.position = p;
			this.normal = n;
		}
	}

	public class PlanetPathfinding
	{
		private struct Path
		{
			public List<Vector2> indexes;
			public List<Step> steps;
			public Grill togrill;
			public int[] to;
			public AstarMoveMode mode;

			public void Store(List<Vector2> i, Grill tg, int[] t, AstarMoveMode m)
			{
				this.indexes = i;
				this.togrill = tg;
				this.to = t;
				this.mode = m;
			}
		}

		private Astar astar;

		private Surface surface;
		private Dictionary<Grill, Cell[,]> cells;
		private Dictionary<Grill, int[,]> grids;
		private Dictionary<Cellable, Path> paths;


		public PlanetPathfinding(Surface s)
		{
			this.astar = new Astar();

			this.surface = s;
			this.cells = s.GetAllSurfaceCells();

			this.grids = this.ConvertSurfaceToGrid(this.cells);

			this.paths = new Dictionary<Cellable, Path>();
		}

		public List<Step> GetPath(Cellable e, Vector3 target, Vector3 targetNormal, AstarMoveMode mode = AstarMoveMode.Diagonal)
		{
			Path path;
			int[] from, to;
			List<Vector2> indexes;
			Grill fromgrill, togrill;
			List<Step> steps;
			

			if(!paths.ContainsKey(e)){
				path = new Path();
				this.paths.Add(e, path);
				indexes = new List<Vector2>();
			}
			else{
				path = this.paths[e];
				indexes = path.indexes;
			}


			fromgrill = e.currentCell.grill;
			togrill = this.surface.GetGrillOf(target, targetNormal);

			from = fromgrill.GetIndexInGrill(e.currentCell.position);
			to = togrill.GetIndexInGrill(target);

			// store data
			path.Store(indexes, togrill, to, mode);

			if(fromgrill == togrill) {
				steps = this.FindPath(indexes, fromgrill, from, to, mode);
			}
			else {
				Debug.LogWarning("WARNING : Pathfinding among several faces is not yet implemented and may never be...");
				steps = null;
				// steps = this.FindPathAmongGrill(indexes, fromgrill, togrill, from, to, mode);
			}

			path.steps = steps;

			return steps;
		}

		public List<Step> GetAnotherPath(Cellable e)
		{
			Path path;
			AstarMoveMode mode;
			int[] from, to;
			List<Vector2> indexes;
			Grill fromgrill, togrill;
			List<Step> steps;
			
			if(!paths.ContainsKey(e)){
				Debug.LogError($"ERROR : This planet element {e} never asked for a path to begin with !");
				return null;
			}

			// get stored data
			path = this.paths[e];
			indexes = path.indexes;
			togrill = path.togrill;
			to = path.to;
			mode = path.mode;
			steps = path.steps;

			fromgrill = e.currentCell.grill;
			from = fromgrill.GetIndexInGrill(e.currentCell.position);

			// update grid
			this.UpdateGridOf(fromgrill);

			if(fromgrill == togrill) {
				this.FindPath(indexes, fromgrill, from, to, mode, steps);
			}
			else {
				Debug.LogWarning("WARNING : Pathfinding among several faces is not yet implemented and may never be...");
				steps = null;
				// steps = this.FindPathAmongGrill(indexes, fromgrill, togrill, from, to, mode);
			}

			return steps;
		}











		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* -------------------------------------- UTIL FUNCTION ---------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		private Dictionary<Grill, int[,]> ConvertSurfaceToGrid(Dictionary<Grill, Cell[,]> dgcs)
		{
			int[,] ins;
			Cell[,] cs;
			int gheight, gwidth;
			Dictionary<Grill, int[,]> dgis;

			dgis = new Dictionary<Grill, int[,]>();

			foreach(Grill g in dgcs.Keys) {
				cs = dgcs[g];
				gheight = cs.GetLength(0);
				gwidth = cs.GetLength(1);

				ins = new int[gheight, gwidth];

				for(int w = 0; w < gwidth; ++w) {
					for(int h = 0; h < gheight; ++h) {
						ins[h, w] = (cs[h, w].isWalkable) ? 0 : 1;
					}
				}

				dgis.Add(g, ins);
			}

			return dgis;
		}

		private void UpdateGridOf(Grill g)
		{
			int[,] ins;
			Cell[,] cs;
			int gheight, gwidth;

			cs = this.cells[g];
			ins = this.grids[g];

			gheight = cs.GetLength(0);
			gwidth = cs.GetLength(1);

			for(int w = 0; w < gwidth; ++w) {
				for(int h = 0; h < gheight; ++h) {
					ins[h, w] = (cs[h, w].isWalkable) ? 0 : 1;
				}
			}
		}

		private List<Step> FindPath(List<Vector2> indexes, Grill grill, int[] from, int[] to, AstarMoveMode mode, List<Step> steps = null)
		{
			List<Step> result;

			this.astar.FindPathNonAlloc(indexes, this.grids[grill], from, to, mode);
			result = this.ConvertIndexesToStepsOnGrill(indexes, grill, steps);

			return result;
		}

		private List<Step> ConvertIndexesToStepsOnGrill(List<Vector2> indexes, Grill g, List<Step> steps)
		{
			int gheight;
			Cell c;

			gheight = g.height;
			if(steps == null)
				steps = new List<Step>();
			else
				steps.Clear();

			foreach(Vector2 index in indexes) {
				c = g.cells[Mathf.RoundToInt(index[1] + index[0] * gheight)];
				steps.Add(new Step(c.position, c.normal));
			}

			return steps;
		}











		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* ---------------------------- UTIL FUNCTION NOT YET IMPLEMENTED -----------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		private List<Step> FindPathAmongGrill(List<Vector2> indexes,
											Grill fromgrill,
											Grill togrill,
											int[] from,
											int[] to,
											AstarMoveMode mode)
		{
			// not yet implemented
			return null;

			/*List<Grill> grills;
			List<Step> steps;
			int[][] mids;
			Grill g;
			int count;

			steps = new List<Step>();

			grills = this.surface.GetPathAmongGrill(fromgrill, togrill);
			count = grills.Count;

			mids = this.GetIndexesTransitionBetweenGrills(fromgrill, grills[1]);
			this.astar.FindPathNonAlloc(indexes, this.grids[fromgrill], from, mids[0], mode);
			this.ConvertIndexesToStepsAmongGrill(steps, indexes, fromgrill);

			for(int i = 1; i < count - 1; ++i) {
				g = grills[i];
				mids = this.GetIndexesTransitionBetweenGrills(g, grills[i+1]);
				this.astar.FindPathNonAlloc(indexes, this.grids[g], mids[0], mids[1], mode);
				this.ConvertIndexesToStepsAmongGrill(steps, indexes, g);
			}

			mids = this.GetIndexesTransitionBetweenGrills(grills[count-2], togrill);
			this.astar.FindPathNonAlloc(indexes, this.grids[togrill], mids[1], to, mode);
			this.ConvertIndexesToStepsAmongGrill(steps, indexes, togrill);

			return steps;*/
		}

		private int[][] GetIndexesTransitionBetweenGrills(Grill g1, Grill g2)
		{
			// not yet implemented
			return null;

			/*int[][] result;

			result = new int[][]
			{
				new int[2] {0, 0},
				new int[2] {0, 0}
			};

			return result;*/
		}
	}
}

