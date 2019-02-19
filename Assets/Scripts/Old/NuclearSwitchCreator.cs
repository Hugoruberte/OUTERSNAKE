using UnityEngine;
using System.Collections;
using System.Linq;

public class NuclearSwitchCreator : MonoBehaviour
{
	private Transform Planet;
	private Transform NuclearsPooling;

	private GameManagerV1 gameManager;
	private ArmchairScript looneyScript;
	private PlanetScript planetScript;

	private Vector3 Position;
	private Quaternion Rotation;

	private CellEnum[] Grid;

	[Range(0, 4)]
	public int margin = 2;
	private float Height;
	private int index = 0;
	private int LooneyFace;

	private IEnumerator creative_coroutine;


	void Awake()
	{
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		looneyScript = GameObject.Find("Armchair").GetComponent<ArmchairScript>();
	}

	public void Nuclears(Transform myPlanet)
	{
		if(NuclearsPooling == null)
			NuclearsPooling = GameObject.Find("NuclearSwitchsPooling").transform;

		Planet = myPlanet;
		Height = (Planet.Find("Body").localScale.x/2) + 0.08f;

		planetScript = Planet.GetComponent<PlanetScript>();
		Grid = planetScript.Grid;

		LooneyFace = (int)looneyScript.Face;

		if(creative_coroutine != null)
			StopCoroutine(creative_coroutine);
		creative_coroutine = FunctionSetup();
		StartCoroutine(creative_coroutine);
	}

	private IEnumerator FunctionSetup()
	{
		bool already = false;
		for(int i = 0; i < Grid.Length; i++)
		{
			if(Grid[i] == CellEnum.NuclearSwitch)
			{
				already = true;
				break;
			}
		}

		yield return null;

		if(already)
		{
			index = 0;

			for(int i = 0; i < Grid.Length; i++)
			{
				if(Grid[i] == CellEnum.NuclearSwitch)
				{
					Rotation = CellToRotation(i);
					Position = CellToPosition(i);
					SetNuclear(i);
				}
			}
		}
		else
		{
			int count = gameManager.WorldSetting.NuclearAmount;
			int cell;
			int inc;
			int[] UsedFace = new int[6] {-1, -1, -1, -1, -1, -1};
			int used_index = 0;
			int face;

			index = 0;

			for(int i = 0; i < count; i++)
			{
				do
				{
					face = Random.Range(0, 6);
				}
				while(UsedFace.Contains(face));

				UsedFace[used_index ++] = face;

				inc = 0;
				do
				{
					cell = Random.Range(22*22*face +(22+1)*margin, 22*22*(face + 1) -(22+1)*margin);
				}
				while((!GetOccupedCell(cell, 1) || !CheckMargin(cell)) && inc ++ < 100);

				Rotation = CellToRotation(cell);
				Position = CellToPosition(cell);
				SetNuclear(cell);

				SetOccupedCell(cell, 1);
				planetScript.Grid[cell] = CellEnum.NuclearSwitch;
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

	private void SetOccupedCell(int cell, int range)
	{
		int len = planetScript.Grid.Length;
		int cell_index;

		for(int i = -range; i <= range; i++)
		{
			for(int j = -range; j <= range; j++)
			{
				cell_index = cell + i*22 + j;
				if((i != 0 || j != 0) && cell_index > 0 && cell_index < len)
					planetScript.Grid[cell_index] = CellEnum.Occuped;
			}
		}
	}

	private bool GetOccupedCell(int cell, int range)
	{
		for(int i = -range; i <= range; i++)
		{
			for(int j = -range; j <= range; j++)
			{
				if((cell + i*22 + j >= planetScript.Grid.Length || cell + i*22 + j < 0) 
					&& planetScript.Grid[cell + i*22 + j] != CellEnum.Empty)
					return false;
			}
		}

		return true;
	}

	private void SetNuclear(int cell)
	{
		Transform nuclear = NuclearsPooling.GetChild(index ++);

		if(cell >= 22*22*LooneyFace && cell < 22*22*(LooneyFace + 1))
		{
			nuclear.localPosition = Vector3.zero;
			nuclear.gameObject.SetActive(false);
		}
		else
		{
			nuclear.position = Position;
			nuclear.rotation = Rotation;

			nuclear.gameObject.SetActive(true);
			nuclear.GetComponent<NuclearSwitchScript>().SetColorGreen(false);
		}
	}
}