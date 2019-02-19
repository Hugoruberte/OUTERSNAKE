using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FlowerCreator : MonoBehaviour
{
	/*private Transform Planet;
	private Transform FlowersPooling;

	private ArmchairScript looneyScript;
	private PlanetScript planetScript;
	private GameManagerV1 gameManager;

	private Vector3 Position;
	private Quaternion Rotation;

	private int LooneyFace;
	private int index = 0;
	private float Height;

	private IEnumerator creative_coroutine;

	private Cell[] Grid;


	void Awake()
	{
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		looneyScript = GameObject.Find("Armchair").GetComponent<ArmchairScript>();
	}

	public void Flowers(Transform myPlanet)
	{
		if(FlowersPooling == null)
			FlowersPooling = GameObject.Find("FlowersPooling").transform;
		
		Planet = myPlanet;
		Height = (Planet.Find("Body").localScale.x/2) + 0.5f;

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
			if(Grid[i] == Cell.Flower)
			{
				already = true;
				break;
			}
		}

		float percent = gameManager.WorldSetting.FlowerPercent;
		int count = Mathf.RoundToInt(FlowersPooling.childCount * percent);
		index = 0;

		if(already)
		{
			Cell type;
			for(int i = 0; i < Grid.Length; i++)
			{
				type = Grid[i];

				if(type == Cell.Flower)
				{
					Rotation = CellToRotation(i);
					Position = CellToPosition(i);
					SetFlower(i, false);
				}
				else if(type == Cell.FlowerBurn)
				{
					Rotation = CellToRotation(i);
					Position = CellToPosition(i);
					SetFlower(i, true);
				}

				if(index > count)
					break;
				else if(i%10 == 0)
					yield return null;
			}
		}
		else
		{
			int cell = 0;
			int first = 0;
			int quantity = count/6;
			int point_quantity = 0;
			int q = 0;
			int total = 0;
			int width = 0;
			int point = 0;
			int point_index = 0;
			int[] points;
			bool extremities;

			for(int face = 0; face < 6; face++)
			{
				point = Random.Range(2, 4);
				point_index = 0;
				points = new int[point];
				for(int k = 0; k < point; k++)
				{
					do
					{
						first = face*22*22 + Random.Range(2, 21) * 22 + Random.Range(2, 21);
					}
					while(points.Contains(first));
					points[point_index ++] = first;
				}
		
				point_quantity = quantity/point;

				for(int p = 0; p < point; p++)
				{
					q = 0;
					total = 0;
					width = 1;
					extremities = false;
					while(q < point_quantity && total < 300)
					{
						for(int i = -width; i <= width; i++)
						{
							for(int j = -width; j <= width; j++)
							{
								cell = points[p] + i*22 + j;
								total++;

								if(cell > -1 && cell < 22*22*(face+1) && planetScript.Grid[cell] == Cell.Empty)
								{
									q++;

									if(q >= point_quantity)
										break;
									else if(q%10 == 0)
										yield return null;

									if(!(extremities && Mathf.Abs(i*j) == width*width))
									{
										Rotation = CellToRotation(cell);
										Position = CellToPosition(cell);
										SetFlower(cell, false);

										planetScript.Grid[cell] = Cell.Flower;
									}
								}
							}

							if(q >= point_quantity)
								break;
						}

						width ++;
						if(total + width * 8 >= point_quantity)
							extremities = true;
					}
				}
			}
		}

		ClearPrefab();
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
		for(int i = -1; i < range*2; i++)
		{
			for(int j = -1; j < range*2; j++)
			{
				planetScript.Grid[cell + i*22 + j] = Cell.Occuped;
			}
		}
	}

	private bool GetOccupedCell(int cell, int range)
	{
		for(int i = -1; i < range*2; i++)
		{
			for(int j = -1; j < range*2; j++)
			{
				if(cell + i*22 + j >= planetScript.Grid.Length || cell + i*22 + j < 0)
					Debug.LogError("Out of Grid ! cell = " + cell);
				if(planetScript.Grid[cell + i*22 + j] != Cell.Empty)
					return false;
			}
		}

		return true;
	}

	private void SetFlower(int cell, bool burn)
	{
		int face = cell /(22*22);

		if(face != LooneyFace)
		{
			Transform flower = FlowersPooling.GetChild(index ++);
			FlowerScript script = flower.GetComponent<FlowerScript>();

			flower.position = Position;
			flower.rotation = Rotation * Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);

			script.SetFlowerAspect(burn);
			script.myCell = cell;	

			flower.gameObject.SetActive(true);
		}
	}

	private void ClearPrefab()
	{
		int len = FlowersPooling.childCount;
		Transform flower;
		for(int i = index; i < len; i++)
		{
			flower = FlowersPooling.GetChild(i);
			flower.localPosition = Vector3.zero;
			flower.gameObject.SetActive(false);
		}
	}*/
}
