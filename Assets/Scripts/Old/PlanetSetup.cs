using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSetup : MonoBehaviour
{
	[Header("Environment")]
	public bool Apple = true;
	public bool Rabbit = true;
	public bool Rocket = true;
	public bool Trees = true;

	[Header("Trap")]
	public bool Lazer = true;
	public bool RedRabbit = true;
	public bool NuclearSwitch = true;

	[Header("Event")]
	public bool SuperLazer = true;
	public bool CircularSaw = true;
	public bool CasterBlaster = true;
	public bool Meteore = true;

	private GameManagerV1 gameManager;
	private ObjectPooling poolingScript;

	private DifficultyScript hardScript;

	private ArmchairCreator armchairCreator;
	private AppleCreator appleCreator;
	private LazerCreator lazerCreator;
	private BurrowCreator burrowCreator;
	private RocketCreator rocketCreator;
	private RedRabbitCreator redrabbitCreator;
	private NuclearSwitchCreator nuclearCreator;
	private TreeCreator treeCreator;

	private PlanetScript[] planetScripts = null;

	[HideInInspector]
	public bool DonePlacingObject = false;

	[HideInInspector]
	public List<string> DestroyedFacesName = new List<string>();

	void Awake()
	{
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		hardScript = GameObject.Find("LevelManager").GetComponent<DifficultyScript>();

		GameObject Creator = GameObject.Find("LevelManager/Creator");

		poolingScript = Creator.GetComponent<ObjectPooling>();
		armchairCreator = Creator.GetComponent<ArmchairCreator>();
		appleCreator = Creator.GetComponent<AppleCreator>();
		lazerCreator = Creator.GetComponent<LazerCreator>();
		burrowCreator = Creator.GetComponent<BurrowCreator>();
		rocketCreator = Creator.GetComponent<RocketCreator>();
		redrabbitCreator = Creator.GetComponent<RedRabbitCreator>();
		nuclearCreator = Creator.GetComponent<NuclearSwitchCreator>();
		treeCreator = Creator.GetComponent<TreeCreator>();

		planetScripts = GetComponentsInChildren<PlanetScript>();
	}

	public void SetPlanetRotation()
	{
		int count = 0;
		foreach(PlanetScript script in planetScripts)
		{
			if(script.MainPlanet)
				count ++;

			script.MainSetup(script.MainPlanet);
		}

		if(count != 1)
			Debug.LogError("There are more than one planet which is MainPlanet !");
	}

	public void MainPlanetSetObjects()
	{
		DonePlacingObject = false;
		StartCoroutine(MainPlanetSetObjectsCoroutine(true, false));
	}
	public void MainPlanetSetObjectsAll()
	{
		DonePlacingObject = false;
		StartCoroutine(MainPlanetSetObjectsCoroutine(false, true));
	}
	public void MainPlanetSetObjectsWithoutArmchair()
	{
		DonePlacingObject = false;
		StartCoroutine(MainPlanetSetObjectsCoroutine(false, false));
	}
	private IEnumerator MainPlanetSetObjectsCoroutine(bool armchair, bool all)
	{
		if(!poolingScript)
			yield return new WaitUntil(() => poolingScript != null);
		if(!poolingScript.Done)
			yield return new WaitUntil(() => poolingScript.Done);

		gameManager.planetScript.MainSetup(true);
		Transform MainPlanet = gameManager.MainPlanet;

		if(armchair)
		{
			armchairCreator.Create();
			yield return new WaitUntil(() => armchairCreator.Done);
		}

		if(Trees || all)
			treeCreator.Trees(MainPlanet);
		if(Rabbit || all)
			burrowCreator.Burrows(MainPlanet);
		if(Rocket || all)
			rocketCreator.Rockets(MainPlanet);
		if((RedRabbit && hardScript.redRabbitThreshold <= hardScript.Difficulty) || all)
			redrabbitCreator.RedRabbits(MainPlanet);
		if((NuclearSwitch && hardScript.nuclearThreshold <= hardScript.Difficulty) || all)
			nuclearCreator.Nuclears(MainPlanet);
		if((Lazer && hardScript.lazerThreshold <= hardScript.Difficulty) || all)
			lazerCreator.Lazers(MainPlanet);

		// Apple toujours en dernier
		if(Apple || all)
			appleCreator.Apples(MainPlanet);

		yield return new WaitUntil(() => appleCreator.Done);

		DonePlacingObject = true;
	}






	public void DestroyPlanetFaces()
	{
		string planet;
		string name;
		string[] tmp;
		PlanetScript script;
		int len = DestroyedFacesName.Count;

		for(int i = 0; i < len; i++)
		{
			name = DestroyedFacesName[i];
			tmp = name.Split("/"[0]);
			planet = tmp[0];

			script = GameObject.Find("Planets/" + planet).GetComponent<PlanetScript>();
			script.Explode(GameObject.Find("Planets/" + name).transform);
		}
	}
	public void RepairPlanetFaces()
	{
		string planet = System.String.Empty;
		string name;
		string[] tmp;
		PlanetScript script;
		int len = DestroyedFacesName.Count;

		for(int i = 0; i < len; i++)
		{
			name = DestroyedFacesName[i];
			tmp = name.Split("/"[0]);
			planet = tmp[0];

			script = GameObject.Find("Planets/" + planet).GetComponent<PlanetScript>();
			script.Repair(GameObject.Find(name).transform);
			script.DestroyedFaces.Clear();
		}

		DestroyedFacesName.Clear();
	}
}
