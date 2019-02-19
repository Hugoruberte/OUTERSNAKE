using UnityEngine;
using System.Collections;

public enum Choice
{
	Random,
	EachFace
};

public class BurrowCreator : MonoBehaviour
{
	private Transform Planet;
	private Transform BurrowsPooling;

	private ArmchairScript looneyScript;
	private PlanetScript planetScript;
	private GameManagerV1 gameManager;

	private Vector3 Position;
	private Quaternion Rotation;

	private Vector3 myLign;
	private Vector3 myColumn;

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


	public void Burrows(Transform myPlanet)
	{
		if(BurrowsPooling == null)
			BurrowsPooling = GameObject.Find("BurrowsPooling").transform;

		Planet = myPlanet;
		Height = (Planet.Find("Body").localScale.x/2) + 0.25f;

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
			if(Grid[i] == CellEnum.Burrow)
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
		int count = gameManager.WorldSetting.RabbitAmount;
		int cell;
		int inc;
		int i;

		index = 0;

		if(Disposition == Choice.EachFace)
		{
			int face = 0;

			for(i = 0; i < count; i++)
			{
				if(Stop)
					return;

				inc = 0;
				do
				{
					cell = Random.Range(22*22*face + (22+1)*margin, 22*22*(face + 1) - (22+1)*margin);
				}
				while((planetScript.Grid[cell] != CellEnum.Empty || !CheckMargin(cell)) && inc ++ < 100);

				face = (face + 1) % 6;

				Rotation = CellToRotation(cell);
				Position = CellToPosition(cell);
				SetBurrow(cell);

				planetScript.Grid[cell] = CellEnum.Burrow;
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
				SetBurrow(cell);

				planetScript.Grid[cell] = CellEnum.Burrow;
			}
		}

		ClearPrefab();
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
			else if(Grid[i] == CellEnum.Burrow)
			{
				Rotation = CellToRotation(i);
				Position = CellToPosition(i);
				SetBurrow(i);
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

		Vector3 pointeur;

		if(face > 2)	//Reverse
		{
			myLign = ligns[(face + 2) % 3];
			myColumn = columns[(face + 1) % 3];
		}
		else
		{
			myLign = ligns[(face + 1) % 3];
			myColumn = columns[(face + 2) % 3];
		}

		pointeur = pointeurs[face];

		return Planet.position + pointeur*Height + myLign*10.5f - myColumn*10.5f + myColumn*(nb%22) - myLign*((nb-22*22*face)/22);
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
				rot = (face == 0) ? Quaternion.Euler(0,90,0) : Quaternion.Euler(0,270,0);
				break;

			//Y
			case 1:
			case 4:
				rot = (face == 1) ? Quaternion.Euler(270,0,0) : Quaternion.Euler(90,0,0);
				break;

			//Z
			case 2:
			case 5:
				rot = (face == 2) ? Quaternion.identity : Quaternion.Euler(0,180,0);
				break;

			default:
				Debug.LogError("Grid too big : cell " + nb);
				break;
		}

		return rot;
	}

	private void SetBurrow(int cell)
	{
		Transform burrow = BurrowsPooling.GetChild(index ++);
		BurrowScript script = burrow.GetComponent<BurrowScript>();

		if(cell >= 22*22*LooneyFace && cell < 22*22*(LooneyFace + 1))
		{
			burrow.localPosition = Vector3.zero;
			script.myRabbit.localPosition = Vector3.zero;

			script.myCell = cell;
			burrow.gameObject.SetActive(false);
			script.myRabbit.gameObject.SetActive(false);
		}
		else
		{
			burrow.position = Position;
			burrow.rotation = Rotation;

			burrow.gameObject.SetActive(true);
			script.myRabbit.gameObject.SetActive(true);

			script.myCell = cell;
			script.myRabbit.rotation = burrow.rotation * Quaternion.Euler(90, 0, 0);
			
			RabbitControllerV0 rabbit_script = script.myRabbit.GetComponent<RabbitControllerV0>();
			rabbit_script.Lign = myLign;
			rabbit_script.Column = myColumn;
			rabbit_script.StartCoroutine(rabbit_script.ReloadPosition(burrow, cell));
		}
	}

	private void ClearPrefab()
	{
		int len = BurrowsPooling.childCount;
		BurrowScript script;
		Transform burrow;
		for(int i = index; i < len; i++)
		{
			burrow = BurrowsPooling.GetChild(i);
			script = burrow.GetComponent<BurrowScript>();
			burrow.localPosition = Vector3.zero;
			script.myRabbit.localPosition = Vector3.zero;

			burrow.gameObject.SetActive(false);
			script.myRabbit.gameObject.SetActive(false);
		}
	}
}