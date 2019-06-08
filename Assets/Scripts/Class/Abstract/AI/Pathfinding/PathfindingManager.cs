using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;
using PathfindingAI;

[CreateAssetMenu(fileName = "PathfindingManager", menuName = "Scriptable Object/Other/PathfindingManager", order = 3)]
public class PathfindingManager : ScriptableObject
{
	private Dictionary<Surface, PlanetPathfinding> pathfinders;

	private void Start()
	{
		// Called in Start because planets need to initialize their surface in Awake
		this.InitializePathfinders();
	}





	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ------------------------------------- PUBLIC FUNCTIONS --------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public List<Step> GetPathTo(Cellable e, Vector3 dest, Vector3 normal, AstarMoveMode mode = AstarMoveMode.Diagonal)
	{
		List<Step> steps;
		PlanetPathfinding pathfinder;

		if(!pathfinders.ContainsKey(e.currentSurface)) {
			Debug.LogError($"ERROR : Could not find the pathfinder associated with the surface of {e} !");
			return null;
		}

		pathfinder = pathfinders[e.currentSurface];
		steps = pathfinder.GetPath(e, dest, normal, mode);

		return steps;
	}

	public List<Step> GetAnotherPathFor(Cellable e)
	{
		List<Step> steps;
		PlanetPathfinding pathfinder;

		if(!pathfinders.ContainsKey(e.currentSurface)) {
			Debug.LogError($"ERROR : Could not find the pathfinder associated with the surface of {e} !");
			return null;
		}

		pathfinder = pathfinders[e.currentSurface];
		steps = pathfinder.GetAnotherPath(e);

		return steps;
	}






	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* -------------------------------------- UTIL FUNCTIONS ---------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	private void InitializePathfinders()
	{
		Planet[] planets;

		pathfinders = new Dictionary<Surface, PlanetPathfinding>();

		planets = FindObjectsOfType<Planet>() as Planet[];
		// planets = Resources.FindObjectsOfTypeAll<Planet>() as Planet[];

		foreach(Planet p in planets) {
			pathfinders.Add(p.surface, new PlanetPathfinding(p.surface));
		}
	}
}
