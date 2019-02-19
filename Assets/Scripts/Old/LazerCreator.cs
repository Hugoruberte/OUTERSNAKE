using UnityEngine;
using System.Collections;

public class LazerCreator : MonoBehaviour
{
	private Transform Planet;
	private Transform LazersPooling;

	private ArmchairScript looneyScript;
	private PlanetScript planetScript;
	private GameManagerV1 gameManager;

	private Vector3 Position;
	private Quaternion Rotation;

	private int index = 0;
	private int LooneyFace;

	private bool Stop = false;

	public Choice Disposition = Choice.EachFace;

	private CellEnum[] Grid;

	[Range(0, 4)]
	public int margin = 0;
	private float Height;

	void Awake()
	{
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		looneyScript = GameObject.Find("Armchair").GetComponent<ArmchairScript>();
	}

	public void Lazers(Transform myPlanet)
	{
		if(LazersPooling == null)
			LazersPooling = GameObject.Find("LazersPooling").transform;
		
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
			if(Grid[i] == CellEnum.Lazer)
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
		int count = gameManager.WorldSetting.LazerAmount;
		int cell;
		int inc;
		int i;

		index = 0;

		if(Disposition == Choice.EachFace)
		{
			int face = Random.Range(0, 6);

			for(i = 0; i < count; i++)
			{
				if(Stop)
					return;

				inc = 0;
				do
				{
					cell = Random.Range(22*22*face +(22+1)*margin, 22*22*(face + 1) -(22+1)*margin);
				}
				while((planetScript.Grid[cell] != CellEnum.Empty || !CheckMargin(cell)) && inc ++ < 100);

				face = (face + 1) % 6;

				Rotation = CellToRotation(cell);
				Position = CellToPosition(cell);
				StartCoroutine(SetLazer(cell, Position, Rotation));

				planetScript.Grid[cell] = CellEnum.Lazer;
			}
		}
		else
		{
			for(i = 0; i < count; i++)
			{
				if(Stop)
					return;

				inc = 0;
				do
				{
					cell = Random.Range(0, 22*22*6);
				}
				while((planetScript.Grid[cell] != CellEnum.Empty || !CheckMargin(cell)) && inc ++ < 100);

				Rotation = CellToRotation(cell);
				Position = CellToPosition(cell);
				StartCoroutine(SetLazer(cell, Position, Rotation));

				planetScript.Grid[cell] = CellEnum.Lazer;
			}
		}
	}

	private void ReGrid()
	{
		index = 0;

		for(int i = 0; i < Grid.Length; i++)
		{
			if(Stop)
			{
				return;
			}
			else if(Grid[i] == CellEnum.Lazer)
			{
				Rotation = CellToRotation(i);
				Position = CellToPosition(i);
				StartCoroutine(SetLazer(i, Position, Rotation));
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

	private IEnumerator SetLazer(int cell, Vector3 pos, Quaternion quat)
	{
		Transform lazer = LazersPooling.GetChild(index ++);

		if(cell >= 22*22*LooneyFace && cell < 22*22*(LooneyFace + 1))
		{
			lazer.localPosition = Vector3.zero;
			lazer.gameObject.SetActive(false);
		}
		else
		{
			Transform Canon = lazer.Find("Body/Canon");
			Canon.Find("Effect").GetComponent<TrailRenderer>().time = 0.0f;
			yield return null;

			lazer.position = pos;
			lazer.rotation = quat;

			yield return null;
			Canon.Find("Effect").GetComponent<TrailRenderer>().time = 0.25f;
			lazer.gameObject.SetActive(true);
		}
	}
}