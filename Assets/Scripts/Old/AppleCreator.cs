using UnityEngine;
using System.Collections;

public class AppleCreator : MonoBehaviour
{
	private Transform Planet;
	private Transform ApplesPooling;

	private ArmchairScript looneyScript;
	private PlanetScript planetScript;
	private GameManagerV1 gameManager;

	private Vector3 Position;
	private Quaternion Rotation;

	private int LooneyFace;
	private int index = 0;
	private float myHeight;

	private bool Setting = false;
	[HideInInspector]
	public bool Done = false;

	private CellEnum[] Grid;


	void Awake()
	{
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		looneyScript = GameObject.Find("Armchair").GetComponent<ArmchairScript>();

		StartCoroutine(FunctionSetup());
	}

	public void Apples(Transform myPlanet)
	{
		if(ApplesPooling == null)
			ApplesPooling = GameObject.Find("ApplesPooling").transform;
		
		Planet = myPlanet;
		myHeight = (Planet.Find("Body").localScale.x/2) + 0.25f;

		planetScript = Planet.GetComponent<PlanetScript>();
		Grid = planetScript.Grid;

		LooneyFace = (int)looneyScript.Face;

		Done = false;
		Setting = true;
	}

	private IEnumerator FunctionSetup() // Cette coroutine tourne continuellement
	{
		while(true)
		{
			yield return new WaitUntil(() => Setting);
			Setting = false;

			bool already = false;
			for(int i = 0; i < Grid.Length; i++)
			{
				if(Grid[i] == CellEnum.Apple)
				{
					already = true;
					break;
				}
			}

			int count = gameManager.WorldSetting.AppleAmount;
			index = 0;

			if(already)
			{
				CellEnum type;
				for(int i = 0; i < Grid.Length; i++)
				{
					if(Setting)
						break;
						
					type = Grid[i];

					if(type == CellEnum.Apple)
					{
						Rotation = CellToRotation(i);
						Position = CellToPosition(i);
						SetApple(i, false);
					}
					else if(type == CellEnum.AppleBurn)
					{
						Rotation = CellToRotation(i);
						Position = CellToPosition(i);
						SetApple(i, true);
					}

					if(index > count)
						break;
					else if(i%(count/12) == 0)
						yield return null;
				}
			}
			else
			{
				int cell;
				int inc;

				for(int i = 0; i < count; i++)
				{
					if(Setting)
						break;

					inc = 0;
					do
					{
						cell = Random.Range(0, 22*22*6);
					}
					while(planetScript.Grid[cell] != CellEnum.Empty && inc ++ < 100);

					if(inc > 100)
					{
						Debug.LogWarning("Couldn't place this apple !");
						break;
					}

					Rotation = CellToRotation(cell);
					Position = CellToPosition(cell);
					SetApple(cell, false);

					planetScript.Grid[cell] = CellEnum.Apple;

					if(i%(count/12) == 0)
						yield return null;
				}
			}

			ClearPrefab();
			Done = true;
		}
	}

	private Vector3 CellToPosition(int nb)
	{
		int face = nb /(22*22);
		Vector3[] ligns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
		Vector3[] columns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
		Vector3[] pointeurs = new Vector3[6] {Vector3.right, Vector3.up, Vector3.forward, -Vector3.right, -Vector3.up, -Vector3.forward};

		Vector3 lign;
		Vector3 column;
		Vector3 pointeur;

		if(face > 2)	//Reverse
		{
			lign = ligns[(face + 2) % 3];
			column = columns[(face + 1) % 3];
		}
		else
		{
			lign = ligns[(face + 1) % 3];
			column = columns[(face + 2) % 3];
		}

		pointeur = pointeurs[face];

		return Planet.position + pointeur*myHeight + lign*10.5f - column*10.5f + column*(nb%22) - lign*((nb-22*22*face)/22);
	}

	private Quaternion CellToRotation(int nb)
	{
		int face = nb/(22*22);
		Quaternion rot = Quaternion.identity;

		switch(face)
		{
			//X
			case 0:
			case 3:
				rot = (face == 0) ? Quaternion.Euler(0,0,270) : Quaternion.Euler(0,0,90);
			break;

			//Y
			case 1:
			case 4:
				rot = (face == 1) ? Quaternion.identity : Quaternion.Euler(180,0,0);
			break;

			//Z
			case 2:
			case 5:
				rot = (face == 2) ? Quaternion.Euler(90,0,0) : Quaternion.Euler(270,0,0);
			break;

			default:
				Debug.LogError("Grid too big : cell " + nb);
			break;
		}

		return rot;
	}

	private void SetApple(int cell, bool burn)
	{
		Transform apple = ApplesPooling.GetChild(index ++);
		apple.gameObject.SetActive(false);

		if(cell >= 22*22*LooneyFace && cell < 22*22*(LooneyFace + 1))
		{
			apple.localPosition = Vector3.zero;
			apple.gameObject.SetActive(false);
		}
		else
		{
			AppleScript script = apple.GetComponent<AppleScript>();

			apple.position = Position;
			apple.rotation = Rotation;

			script.AppleBurnt(burn);

			script.myCell = cell;
			apple.GetComponent<Collider>().enabled = true;

			int rotten = Mathf.RoundToInt(gameManager.WorldSetting.RottenApplePercent * 100f);
			if(Random.Range(0, 100) < rotten)
			{
				script.State = (Random.Range(0, 5) == 0) ? AppleType.Dung : AppleType.Rotten;
				apple.name = "RottenApple";
			}
			
			apple.gameObject.SetActive(true);

			script.StartCoroutine(script.Scale());
		}
	}

	private void ClearPrefab()
	{
		int len = ApplesPooling.childCount;
		Transform apple;
		for(int i = index; i < len; i++)
		{
			apple = ApplesPooling.GetChild(i);
			apple.localPosition = Vector3.zero;
			apple.gameObject.SetActive(false);
		}
	}
}