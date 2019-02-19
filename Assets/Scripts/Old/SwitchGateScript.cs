using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Tools;

public class SwitchGateScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform Switches;
	private GameObject Switch;

	private bool Activated = false;

	private GameManagerV1 gameManager;

	private int switchActualNumber = 0;
	private int SwitchNumber = 1; // On commence au moins avec un switch
	public int SwitchActualNumber
	{
		set
		{
			switchActualNumber = value;

			if(value == SwitchNumber)
			{
				for(int i = 0; i < Switches.childCount; i++)
					Switches.GetChild(i).GetComponent<SwitchBaseScript>().Validating();
				myFunction.Invoke();
				Activated = true;
			}
			else if(value > SwitchNumber || value < 0)
			{
				Debug.LogError("[WARNING] Switch value error: value = " + value);
			}
		}
		get
		{
			return switchActualNumber;
		}
	}

	public enum SwitchGateForm
	{
		Square,
		L,
		M,
		O,
		A
	};

	public enum SwitchGateMode
	{
		Idle,
		Moving
	};

	[Header("Movement")]
	public SwitchGateMode Mode = SwitchGateMode.Idle;
	[Range(0f, 5f)]
	public float MovingInterval = 0.5f;
	[Range(2f, 20f)]
	public float MovingSpeed = 10f;

	//private static int[] Lform = new int[4] {17, 19, 7, 9};
	private static int[] Lform = new int[] {25, 23, 21, 41, 39, 37};
	private static int[] SquareForm = new int[] {24, 10, 12, 14, 16, 18, 20, 22};
	private static Vector3[] OccupedPosition = null;

	[Header("Formation")]
	public SwitchGateForm Form = SwitchGateForm.L;

	public UnityEvent myFunction;

	[Header("Position")]
	public int myCell = 0;

	void Awake()
	{
		if(myFunction == null)
			myFunction = new UnityEvent();

		myTransform = transform;
		Switches = myTransform.GetChild(0);
		Switch = Switches.GetChild(0).gameObject;

		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
	}

	void Start()
	{
		myCell = myTransform.PositionToCell(gameManager.MainPlanet);
		if(gameManager.MainPlanet.GetComponent<PlanetScript>() != null)
		{
			gameManager.MainPlanet.GetComponent<PlanetScript>().SetCell(myCell, CellEnum.Mechanism);
			gameManager.MainPlanet.SetOccupedCell(myCell, 3);
		}
		
		SetSwitches();
		if(Mode == SwitchGateMode.Moving)
			StartCoroutine(MoveSwitchesCoroutine());
	}

	private IEnumerator MoveSwitchesCoroutine()
	{
		float duration = (1f/MovingSpeed) + 0.1f;
		float interval = (MovingInterval > duration) ? MovingInterval : duration;
		WaitForSeconds waitforseconds = new WaitForSeconds(interval);
		SwitchBaseScript[] scripts = new SwitchBaseScript[SwitchNumber];
		Vector3[] pos = null;

		for(int i = 0; i < SwitchNumber; i++)
			scripts[i] = Switches.GetChild(i).GetComponent<SwitchBaseScript>();

		while(!Activated)
		{
			pos = SetNewPosition(scripts);

			for(int i = 0; i < SwitchNumber; i++)
				scripts[i].MoveTowards(pos[i], MovingSpeed);

			yield return waitforseconds;
		}
	}

	private Vector3[] SetNewPosition(SwitchBaseScript[] scripts)
	{
		Vector3[] Positions = new Vector3[SwitchNumber];
		Vector3 position;
		Vector3 newposition;
		Vector3 direction = myTransform.forward;
		bool chose;
		int count;

		for(int i = 0; i < SwitchNumber; i++)
		{
			count = 0;
			position = Switches.GetChild(i).AbsolutePosition();
			newposition = position;
			chose = scripts[i].Yellow;

			while(!chose)
			{
				direction = Quaternion.AngleAxis(90*Random.Range(0,4), myTransform.up) * direction;
				newposition = position + direction;
				chose = (!OccupedPosition.Contains(newposition) && Vector3.Distance(myTransform.position, newposition) < 4.25f);
				if(++ count > 15)
				{
					newposition = position;
					break;
				}
			}

			OccupedPosition[i] = newposition;
			Positions[i] = newposition;
		}

		return Positions;
	}

	private void SetSwitches()
	{
		Transform SwitchTrans = Switch.transform;
		Vector3 init_position = SwitchTrans.AbsolutePosition();
		Vector3 position = init_position;
		Vector3 direction = myTransform.forward;
		int[] coordinate = GetCoordinate();

		SwitchNumber = coordinate.Length;
		OccupedPosition = new Vector3[SwitchNumber];

		for(int i = 0; i < SwitchNumber - 1; i++)
		{
			position = GetPositionFromIndex(coordinate[i]);
			OccupedPosition[i] = position;
			GameObject sw = Instantiate(Switch, position, myTransform.rotation);
			sw.name = string.Format("Switch ({0})", i);
			sw.transform.parent = Switches;
		}

		position = GetPositionFromIndex(coordinate[SwitchNumber-1]);
		OccupedPosition[SwitchNumber-1] = position;
		SwitchTrans.position = position;
		Switch.name = string.Format("Switch ({0})", SwitchNumber - 1);
		SwitchTrans.SetAsLastSibling();
	}

	private int[] GetCoordinate()
	{
		int[] coord;

		switch(Form)
		{
			case SwitchGateForm.L:
				coord = Lform;
				break;

			case SwitchGateForm.Square:
				coord = SquareForm;
				break;

			default:
				Debug.LogWarning("Form '" + Form + "' is not defined yet.");
				coord = new int[1] {0};
				break;
		}

		return coord;
	}

	private Vector3 GetPositionFromIndex(int index)
	{
		Vector3 init_position = Switch.transform.AbsolutePosition();
		Vector3 position = init_position;
		Vector3 direction = myTransform.forward;
		int rotate = 1;
		int row = 0;

		for(int k = 0; k < index; k++)
		{
			if(k > 0 && k % rotate == 0) // on doit tourner le vecteur direction ici
			{
				direction = Quaternion.AngleAxis(90, myTransform.up) * direction;
				if(++ row == 2)
				{
					row = 0;
					rotate ++;
				}
			}

			position += direction;
		}

		return position;
	}
}
