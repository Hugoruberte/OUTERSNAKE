using UnityEngine;
using System.Collections;

public class TreeCreator : MonoBehaviour
{
	private Transform Planet;
	private Transform TreesPooling;

	private GameManagerV1 gameManager;
	private ArmchairScript looneyScript;
	private PlanetScript planetScript;

	private Vector3 Position;
	private Quaternion Rotation;

	public Choice Disposition = Choice.EachFace;

	private CellEnum[] Grid;

	[Range(0, 4)]
	public int margin = 1;
	private int index = 0;
	private float Height;
	private int LooneyFace;


	void Awake()
	{
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		looneyScript = GameObject.Find("Armchair").GetComponent<ArmchairScript>();
	}

	public void Trees(Transform myPlanet)
	{
		if(TreesPooling == null)
			TreesPooling = GameObject.Find("TreesPooling").transform;

		Planet = myPlanet;
		Height = (Planet.Find("Body").localScale.x/2) + 0.5f;	//15.5f

		planetScript = Planet.GetComponent<PlanetScript>();
		Grid = planetScript.Grid;

		LooneyFace = (int)looneyScript.Face;

		FunctionSetup();
	}

	private void FunctionSetup()
	{
		bool already = false;
		for(int i = 0; i < Grid.Length; i++)
		{
			if(Grid[i] == CellEnum.Tree)
			{
				already = true;
				break;
			}
		}

		index = 0;

		if(already)
			ReGrid();
		else
			SetGrid();
	}

	private void SetGrid()
	{
		int count = gameManager.WorldSetting.TreeAmountPerFace;
		int cell;
		int inc;

		if(Disposition == Choice.EachFace)
		{
			for(int face = 0; face < 6; face++)
			{
				for(int child = 0; child < count; child++)
				{
					inc = 0;
					do
					{
						cell = Random.Range(22*22*face +(22+1)*margin, 22*22*(face + 1) -(22+1)*margin);
					}
					while((!GetOccupedCell(cell, 1) || !CheckMargin(cell)) && inc ++ < 100);

					Rotation = CellToRotation(cell);
					Position = CellToPosition(cell);
					SetTree(cell, false);

					SetOccupedCell(cell, 1);
					planetScript.Grid[cell] = CellEnum.Tree;
				}
			}
		}
		else
		{
			for(int i = 0; i < count * 5; i++)
			{
				inc = 0;
				do
				{
					cell = Random.Range(0, 22*22*6);
				}
				while((!GetOccupedCell(cell, 1) || !CheckMargin(cell)) && inc ++ < 100);

				Rotation = CellToRotation(cell);
				Position = CellToPosition(cell);
				SetTree(cell, false);

				SetOccupedCell(cell, 1);
				planetScript.Grid[cell] = CellEnum.Tree;
			}
		}

		ClearPrefab();
	}

	private void ReGrid()
	{
		for(int i = 0; i < Grid.Length; i++)
		{
			if(Grid[i] == CellEnum.Tree)
			{
				Rotation = CellToRotation(i);
				Position = CellToPosition(i);
				SetTree(i, false);
			}
			else if(Grid[i] == CellEnum.Trunk)
			{
				Rotation = CellToRotation(i);
				Position = CellToPosition(i);
				SetTree(i, true);
			}
		}

		ClearPrefab();
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

	private void SetTree(int cell, bool burn)
	{
		Transform tree = TreesPooling.GetChild(index ++);

		if(cell >= 22*22*LooneyFace && cell < 22*22*(LooneyFace + 1))
		{
			tree.localPosition = Vector3.zero;
			tree.gameObject.SetActive(false);
		}
		else
		{
			tree.GetComponent<Collider>().enabled = true;
			tree.position = Position;
			tree.rotation = Rotation;

			tree.gameObject.SetActive(true);

			tree.GetComponent<TreeScript>().myCell = cell;
			tree.GetComponent<TreeScript>().StartCoroutine(tree.GetComponent<TreeScript>().SetTreeAspect(burn, planetScript));
		}
	}

	private void ClearPrefab()
	{
		int len = TreesPooling.childCount;
		Transform tree;
		for(int i = index; i < len; i++)
		{
			tree = TreesPooling.GetChild(i);
			tree.localPosition = Vector3.zero;
			tree.gameObject.SetActive(false);
		}
	}
}