using UnityEngine;
using System.Collections;
using System.Linq;

public class RocketCreator : MonoBehaviour
{
	private Transform Planet;
	private Transform OppositePlanet;
	private Transform RocketsPooling;

	private ArmchairScript looneyScript;
	private PlanetScript planetScript;
	private PlanetScript OppositeScript;

	private Vector3 Position;
	private Quaternion Rotation;

	private int index = 0;
	private int LooneyFace;

	private CellEnum[] Grid;

	[Range(0, 4)]
	public int margin = 0;
	private float Height;

	private bool Stop = false;


	void Awake()
	{
		looneyScript = GameObject.Find("Armchair").GetComponent<ArmchairScript>();
	}

	public void Rockets(Transform myPlanet)
	{
		if(RocketsPooling == null)
			RocketsPooling = GameObject.Find("RocketsPooling").transform;
		
		Planet = myPlanet;
		Height = (Planet.Find("Body").localScale.x/2) + 0.5f;

		planetScript = Planet.GetComponent<PlanetScript>();
		Grid = planetScript.Grid;

		LooneyFace = (int)looneyScript.Face;

		StartCoroutine(FunctionSetup());
	}

	private IEnumerator FunctionSetup()
	{
		Stop = true;

		bool already = false;
		for(int i = 0; i < Grid.Length; i++)
		{
			if(Grid[i] == CellEnum.Rocket)
			{
				already = true;
				break;
			}
		}

		yield return null;

		Stop = false;

		if(already)
			ReGrid();
		else
			SetGrid();
	}

	private void SetGrid()
	{
		int count = 3;
		int nb_rocket_settle = 0;
		int cell;
		int inc = 0;
		int inc2 = 0;
		int[] UsedFace = new int[6] {-1, -1, -1, -1, -1, -1};
		int used_index = 0;
		int face;

		index = 0;

		while(nb_rocket_settle < count && ++ inc2 < 50)
		{
			do
			{
				face = Random.Range(0, 6);
				if(Stop)
					return;
			}
			while(UsedFace.Contains(face));

			Transform rocket = RocketsPooling.GetChild(index);

			inc = 0;
			do
			{
				cell = Random.Range(22*22*face +(22+1)*margin, 22*22*(face + 1) -(22+1)*margin);
				if(Stop)
					return;
			}
			while((planetScript.Grid[cell] != CellEnum.Empty || !CheckMargin(cell)) && inc ++ < 100);

			if(inc < 100)
			{
				Rotation = CellToRotation(cell);
				Position = CellToPosition(cell);
				rocket.position = Position;
				rocket.rotation = Rotation;

				int opposite_cell = FindOppositeCell(cell);
				bool check = CheckRocketPosition(rocket, opposite_cell);

				if(check)
				{
					if(cell >= 22*22*LooneyFace && cell < 22*22*(LooneyFace + 1))
					{
						rocket.localPosition = Vector3.zero;
						rocket.gameObject.SetActive(false);
					}
					else
					{
						rocket.GetComponent<RocketScript>().OppositePlanet = OppositePlanet;
						rocket.GetComponent<RocketScript>().MyPlanet = Planet;
						rocket.gameObject.SetActive(true);

						index ++;
					}

					nb_rocket_settle ++;
					
					UsedFace[used_index ++] = face;

					planetScript.Grid[cell] = CellEnum.Rocket;
					OppositeScript.Grid[opposite_cell] = CellEnum.Occuped;
				}
			}
			else
			{
				Debug.LogWarning("Not enough space on face " + face + ": can't find a cell !");
			}
			
			if(Stop)
				return;
		}

		/*if(inc2 >= 50 && nb_rocket_settle == 0)
			Debug.LogWarning("Can't setup any Rocket on this planet !");*/
	}

	private void ReGrid()
	{
		index = 0;
		int nb_rocket_settle = 0;

		for(int i = 0; i < Grid.Length; i++)
		{
			if(Stop)
			{
				return;
			}
			else if(Grid[i] == CellEnum.Rocket)
			{
				Rotation = CellToRotation(i);
				Position = CellToPosition(i);
				SetRocket(i);
				nb_rocket_settle ++;
				if(nb_rocket_settle == 3)
					break;
			}
		}
	}

	private bool CheckMargin(int nb)
	{
		int face = nb /(22*22);
		int reste = nb % 22;
		int quotient = (nb - 22*22*face) / 22;

		return !(reste > 21 - margin || reste < margin || quotient > 21 - margin || quotient < margin);
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

		return Planet.position + pointeur*Height + lign*10.5f - column*10.5f + column*(nb%22) - lign*((nb-22*22*face)/22);
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

	private int FindOppositeCell(int cell)
	{
		int face = cell/(22*22);
		int opp;

		if(face < 3)
			opp = 22*22*face + 1935 -(cell%22)*22 -(cell-face*22*22)/22;
		else
			opp = (22*22*(face+1)-1) - 1935 +(21 - cell%21)*22 +(21 -(cell-face*22*22)/22);

		return opp;
	}

	private void SetRocket(int cell)
	{
		Transform rocket = RocketsPooling.GetChild(index ++);

		if(cell >= 22*22*LooneyFace && cell < 22*22*(LooneyFace + 1))
		{
			rocket.localPosition = Vector3.zero;
			rocket.gameObject.SetActive(false);
		}
		else
		{
			rocket.position = Position;
			rocket.rotation = Rotation;

			bool check = CheckRocketPosition(rocket, -1);

			if(!check)
			{
				Debug.LogError("[" + Time.time + "]: Probleme lors du replacement de cette Rocket !", rocket);
				return;
			}
			
			rocket.GetComponent<RocketScript>().OppositePlanet = OppositePlanet;
			rocket.GetComponent<RocketScript>().MyPlanet = Planet;
			rocket.gameObject.SetActive(true);
		}
	}

	private bool CheckRocketPosition(Transform trans, int oppocell)
	{
		Vector3 direction = trans.up;
			
		RaycastHit[] hits;
		hits = Physics.RaycastAll(trans.position, direction, 125);
		Debug.DrawRay(trans.position, 125 * direction, Color.yellow);

		for(int j = 0; j < hits.Length; j++)
		{
			RaycastHit hit = hits[j];

			if(hit.transform.CompareTag("Planet"))
			{
				OppositePlanet = hit.transform;
				OppositeScript = OppositePlanet.GetComponent<PlanetScript>();
				if(oppocell < 0 || OppositeScript.Grid[oppocell] == CellEnum.Empty)
					return true;
			}
		}

		return false;
	}
}