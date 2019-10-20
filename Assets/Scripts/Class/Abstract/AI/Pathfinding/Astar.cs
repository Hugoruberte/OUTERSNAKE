/*
Usage:
 
int[,] map = new int[,] 
{
	{0, 0, 0, 0, 0, 0, 0, 0},
	{0, 0, 0, 0, 0, 0, 0, 0},
	{0, 0, 0, 1, 0, 0, 0, 0},
	{0, 0, 0, 1, 0, 0, 0, 0},
	{0, 0, 0, 1, 0, 0, 0, 0},
	{1, 0, 1, 0, 0, 0, 0, 0},
	{1, 0, 1, 0, 0, 0, 0, 0},
	{1, 1, 1, 1, 1, 1, 0, 0},
	{1, 0, 1, 0, 0, 0, 0, 0},
	{1, 0, 1, 2, 0, 0, 0, 0}
};

int[] start	= new int[2] {0, 0};
int[] end	= new int[2] {5, 5};

List<Vector2> path = new Astar(map, start, end, "Manhattan").result;
*/


using System.Collections.Generic;
using UnityEngine;
using PathfindingAI;

public class Astar
{
	private List<Vector2> result;
	private AstarMoveMode find;
	private int[,] grid;
	private int[] from;
	private int[] to;
	
	private class _Object {
		public int x { get; set; }
		public int y { get; set; }
		public double f { get; set; }
		public double g { get; set; }
		public int v { get; set; }
		public _Object p { get; set; }

		public _Object(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public List<Vector2> FindPath(int[,] g, int[] s, int[] e, AstarMoveMode f = AstarMoveMode.Diagonal)
	{
		Initialize(g, s, e, f);

		this.result = new List<Vector2>();

		this.CalculatePath();

		return this.result;
	}

	public void FindPathNonAlloc(List<Vector2> res, int[,] g, int[] s, int[] e, AstarMoveMode f = AstarMoveMode.Diagonal)
	{
		Initialize(g, s, e, f);

		res.Clear();

		this.result = res;

		this.CalculatePath();
	}

	private void Initialize(int[,] g, int[] s, int[] e, AstarMoveMode f = AstarMoveMode.Diagonal)
	{
		this.grid = g;
		this.from = s;
		this.to = e;
		this.find = f;
	}






	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* -------------------------------------- UTIL FUNCTION ---------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	private _Object[] diagonalSuccessors(bool xN, bool xS, bool xE, bool xW, int N, int S, int E, int W, int[,] g, int rows, int cols, _Object[] res, int i)
	{
		if(xN)
		{
			if(xE && g[N,E] == 0)
			{
				res[i++] = new _Object(E, N);
			}
			if(xW && g[N,W] == 0)
			{
				res[i++] = new _Object(W, N);
			}
		}
		if(xS)
		{
			if(xE && g[S,E] == 0)
			{
				res[i++] = new _Object(E, S);
			}
			if(xW && g[S,W] == 0)
			{
				res[i++] = new _Object(W, S);
			}
		}
		return res;
	}
	
	private _Object[] diagonalSuccessorsFree(bool xN, bool xS, bool xE, bool xW, int N, int S, int E, int W, int[,] g, int rows, int cols, _Object[] res, int i)
	{
		xN = N > -1;
		xS = S < rows;
		xE = E < cols;
		xW = W > -1;
		
		if(xE)
		{
			if(xN && g[N,E] == 0)
			{
				res[i++] = new _Object(E, N);
			}
			if(xS && g[S,E] == 0)
			{
				res[i++] = new _Object(E, S);
			}
		}
		if(xW)
		{
			if(xN && g[N,W] == 0)
			{
				res[i++] = new _Object(W, N);
			}
			if(xS && g[S,W] == 0)
			{
				res[i++] = new _Object(W, S);
			}
		}
		return res;
	}
	
	private _Object[] successors(int x, int y, int[,] g, int rows, int cols)
	{
		int N = y - 1;	
		int S = y + 1;
		int E = x + 1;
		int W = x - 1;
		
		bool xN = N > -1 && g[N,x] == 0;
		bool xS = S < rows && g[S,x] == 0;
		bool xE = E < cols && g[y,E] == 0;
		bool xW = W > -1 && g[y,W] == 0;
		
		int i = 0;
		
		_Object[] res = new _Object[8];
		
		if(xN)
		{
			res[i++] = new _Object(x, N);
		}
		if(xE)
		{
			res[i++] = new _Object(E, y);
		}
		if(xS)
		{
			res[i++] = new _Object(x, S);
		}
		if(xW)
		{
			res[i++] = new _Object(W, y);
		}

		_Object[] obj;

		if(this.find == AstarMoveMode.Diagonal || this.find == AstarMoveMode.Euclidean) {
			obj = diagonalSuccessors(xN, xS, xE, xW, N, S, E, W, g, rows, cols, res, i);
		}
		else if(this.find == AstarMoveMode.DiagonalFree || this.find == AstarMoveMode.EuclideanFree) {
			obj = diagonalSuccessorsFree(xN, xS, xE, xW, N, S, E, W, g, rows, cols, res, i);
		}
		else {
			obj = res;
		}
		
		return obj;
	}
	
	private double diagonal(_Object start, _Object end)
	{
		// only does that
		return Mathf.Max(Mathf.Abs(start.x - end.x), Mathf.Abs(start.y - end.y));
	}
	
	private double euclidean(_Object start, _Object end)
	{
		float x = start.x - end.x;
		float y = start.y - end.y;
		
		return Mathf.Sqrt(x * x + y * y);
	}

	private double manhattan(_Object start, _Object end)
	{
		// only does that
		return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
	}

	private void CalculatePath()
	{
		int cols 	= this.grid.GetLength(1);
		int rows 	= this.grid.GetLength(0);
		int limit 	= cols * rows;
		int length 	= 1;
		
		List<_Object> open = new List<_Object>();
		open.Add(new _Object(this.from[0], this.from[1]));
		open[0].f = 0;
		open[0].g = 0;
		open[0].v = this.from[0] + this.from[1] * cols;
		
		_Object current;
	
		List<int> list = new List<int>();

		double distanceS;
		double distanceE;
		
		int i;
		int j;
		
		double max;
		int min;
		
		_Object[] next;
		_Object adj;
		
		_Object end = new _Object(this.to[0], this.to[1]);
		end.v = this.to[0]+this.to[1]*cols;
		
		bool inList;
		
		do {
			max = limit;
			min = 0;
			
			for(i = 0; i < length; ++i)
			{
				if(open[i].f < max)
				{
					max = open[i].f;
					min = i;
				}
			}
			
			current = open[min];
			open.RemoveAt(min);
			
			if(current.v != end.v)
			{
				--length;
				next = successors(current.x, current.y, this.grid, rows, cols);
				
				for(i = 0, j = next.Length; i < j; ++i)
				{
					if(next[i] == null)
					{
						continue;
					}
					
					adj = next[i];
					adj.p = current;
					adj.f = adj.g = 0;
					adj.v = adj.x + adj.y * cols;
					inList = false;
					
					foreach(int key in list)
					{
						if(adj.v == key)
						{
							inList = true;
						}
					}
					
					if(!inList)
					{
						if(this.find == AstarMoveMode.DiagonalFree || this.find == AstarMoveMode.Diagonal)
						{
							distanceS = diagonal(adj, current);
							distanceE = diagonal(adj, end);
						}
						else if(this.find == AstarMoveMode.Euclidean || this.find == AstarMoveMode.EuclideanFree)
						{
							distanceS = euclidean(adj, current);
							distanceE = euclidean(adj, end);
						}
						else
						{
							distanceS = manhattan(adj, current);
							distanceE = manhattan(adj, end);
						}
						
						adj.f =(adj.g = current.g + distanceS) + distanceE;
						open.Add(adj);
						list.Add(adj.v);
						length++;
					}
				}
			}
			else
			{
				i = length = 0;
				do {
					this.result.Add(new Vector2(current.x, current.y));
				}
				while((current = current.p) != null);
				this.result.Reverse();
			}
		}
		while(length != 0);
	}
}
