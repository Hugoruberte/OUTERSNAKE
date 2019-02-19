/*using UnityEngine;
using System.Collections;
using Tools;

public enum Faces
{
	FaceX1,
	FaceY1,
	FaceZ1,
	FaceX2,
	FaceY2,
	FaceZ2
};

public enum SnakeState
{
	Run,
	Stop,
	Wait
};

public class SnakeControllerV2 : MonoBehaviour 
{
	[HideInInspector]
	public Transform Heart;
	[Header("Prefab")]
	public GameObject SnakeBodyPrefab;
	private Transform myTransform;
	private Transform Cube;
	private Transform RedRabbitPooling;
	private Transform LazerPooling;

	private GameManagerV1 gameManager;
	private PlanetScript planetScript;

	private DifficultyScript hardScript;
	private SuperLazerCreator suplazerCreator;
	private CasterBlasterCreator casterblasterCreator;
	private SawCreator sawCreator;
	private MeteoreCreator meteoreCreator;

	private WaitForSeconds waitforseconds_03 = new WaitForSeconds(0.3f);

	[HideInInspector]
	public Vector3 targetPosition = Vector3.zero;
	[HideInInspector]
	public Quaternion targetRotation = Quaternion.identity;

	private Vector3[] ligns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
	private Vector3[] columns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
	private Vector3 lign;
	private Vector3 column;
	private Vector3 zero;
	private Vector3 pos;

	[Header("Settings")]
	[Range(0.0f, 30.0f)]
	public float Speed = 10.0f;
	[Range(0.01f, 0.5f)]
	[Tooltip("The accuracy of the deplacement of Snake, less is more accurrate (0.175f is good).")]
	public float PositionAccuracy = 0.175f;		//plus c'est bas, plus c'est précis
	[Range(0, 10)]
	public int RotationAccuracy = 3;

	[HideInInspector]
	public int right = 0;
	[HideInInspector]
	public int up = 0;

	private float Omega = 25.0f;
	private int WantUp = 0;
	private int WantRight = 0;

	[Header("States")]
	public SnakeState State = SnakeState.Stop;


	[Header("Planet")]
	public Faces Face = Faces.FaceY1;
	public int myCell = 714;
	
	[HideInInspector]
	public bool Switch = false;
	private bool Pivote = false;
	private bool Clonage = false;


	void Awake()
	{
		myTransform = transform;
		
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		suplazerCreator = GameObject.Find("LevelManager/Creator").GetComponent<SuperLazerCreator>();
		casterblasterCreator = GameObject.Find("LevelManager/Creator").GetComponent<CasterBlasterCreator>();
		sawCreator = GameObject.Find("LevelManager/Creator").GetComponent<SawCreator>();
		meteoreCreator = GameObject.Find("LevelManager/Creator").GetComponent<MeteoreCreator>();
		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();
		hardScript = GameObject.Find("LevelManager").GetComponent<DifficultyScript>();

		Cube = myTransform.Find("SnakeDetector");
		Cube.parent = null;
		Cube.SetAsFirstSibling();

		targetPosition = myTransform.position;
		targetRotation = myTransform.rotation;

		Heart = gameManager.MainPlanet.Find("Heart").transform;
	}

	void Start()
	{
		PositionToCell();
		planetScript.Grid[myCell] = Cell.Snake;

		if(gameManager.State != Scenes.Hell)
		{
			RedRabbitPooling = GameObject.Find("ObjectPoolingStock/RedRabbitsPooling").transform;
			LazerPooling = GameObject.Find("ObjectPoolingStock/LazersPooling").transform;
		}
	}

	void Update()
	{
		if(State != SnakeState.Stop && !gameManager.Pause)
		{
			if((State == SnakeState.Run || (State == SnakeState.Wait && (up != 0 || right != 0))) && !gameManager.Safe)
				SnakeBody();

			UpdateSnakeMovement();
			PositionToCell();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("ZoneSnake"))
		{
			Faces theface = FindFace(other.name);

			if(theface != Face && State == SnakeState.Run)
			{
				Vector3 axis = TransformExtension.VectorRound(myTransform.right);
				Heart.rotation *= Quaternion.Euler(0, 90 * Mathf.RoundToInt(Vector3.Dot(Heart.up, axis)), 0);
				Heart.rotation *= Quaternion.Euler(90 * Mathf.RoundToInt(Vector3.Dot(Heart.right, axis)), 0, 0);

				targetRotation *= Quaternion.Euler(90, 0, 0);

				Clonage = false;

				Vector3 EdgePosition = myTransform.AbsolutePosition();

				if(theface == Faces.FaceX1 || theface == Faces.FaceX2)
				{
					EdgePosition.x = Mathf.RoundToInt(other.transform.position.x);
				}
				else if(theface == Faces.FaceY1 || theface == Faces.FaceY2)
				{
					EdgePosition.y = Mathf.RoundToInt(other.transform.position.y);
				}
				else if(theface == Faces.FaceZ1 || theface == Faces.FaceZ2)
				{
					EdgePosition.z = Mathf.RoundToInt(other.transform.position.z);
				}

				myTransform.position = EdgePosition;
				targetPosition = EdgePosition;

				Face = theface;

				if(gameManager.State == Scenes.Hell)
				{
					Debug.LogWarning("A placer dans un autre script !");
					HellSetup(Face);
				}
			}
		}
	}

	private void HellSetup(Faces theface)
	{
		if(theface == Faces.FaceZ2 || theface == Faces.FaceX2)
		{
			casterblasterCreator.MinTime = 0.0f;
			casterblasterCreator.MaxTime = 0.15f;

			meteoreCreator.MinTime = 0.0f;
			meteoreCreator.MaxTime = 0.15f;
		}
		else if(casterblasterCreator.MaxTime != 0.5f)
		{
			casterblasterCreator.MinTime = 0.0f;
			casterblasterCreator.MaxTime = 0.5f;

			meteoreCreator.MinTime = 0.1f;
			meteoreCreator.MaxTime = 0.5f;
		}
	}

	private bool CanTurn()	//fonction qui determine si on est en position absolue
	{
		float Pos = Vector3.Distance(myTransform.AbsolutePosition(), myTransform.position);
		float Rot = Quaternion.Angle(targetRotation, myTransform.rotation);

		return (Pos <= PositionAccuracy && Rot <= RotationAccuracy);
	}

	public void ActivateCreator()
	{
		if(suplazerCreator && !suplazerCreator.SuperLazerActivated && hardScript.superLazerThreshold <= hardScript.Difficulty)
			suplazerCreator.LaunchSuperLazer();
		if(sawCreator && !sawCreator.SawActivated && hardScript.sawThreshold <= hardScript.Difficulty)
			sawCreator.LaunchSaw();
		if(casterblasterCreator && !casterblasterCreator.CasterBlasterActivated && hardScript.casterBlasterThreshold <= hardScript.Difficulty)
			casterblasterCreator.StartCoroutine(casterblasterCreator.LaunchCasterBlaster());
		if(meteoreCreator && !meteoreCreator.MeteoreActivated && hardScript.meteoreThreshold <= hardScript.Difficulty)
			meteoreCreator.StartCoroutine(meteoreCreator.LaunchMeteore());

		if(RedRabbitPooling)
		{
			int length = RedRabbitPooling.childCount;
			Transform child;
			for(int i = 0; i < length; i++)
			{
				child = RedRabbitPooling.GetChild(i);
				if(child.gameObject.activeInHierarchy)
					child.GetComponent<RedRabbitController>().StartCoroutine(child.GetComponent<RedRabbitController>().RedRabbitManager(true));
			}
		}
	}

	public void DesactivateCreator()
	{
		if(sawCreator)
			sawCreator.SawActivated = false;
		if(suplazerCreator)
			suplazerCreator.SuperLazerActivated = false;
		if(casterblasterCreator)
			casterblasterCreator.CasterBlasterActivated = false;
		if(meteoreCreator)
			meteoreCreator.MeteoreActivated = false;

		int length;
		Transform child;
		if(LazerPooling)
		{
			length = LazerPooling.childCount;
			for(int i = 0; i < length; i++)
			{
				child = LazerPooling.GetChild(i);
				if(child.gameObject.activeInHierarchy)
					child.GetComponent<LazerScript>().SnakeExit();
			}
		}

		if(RedRabbitPooling)
		{
			length = RedRabbitPooling.childCount;
			for(int i = 0; i < length; i++)
			{
				child = RedRabbitPooling.GetChild(i);
				if(child.gameObject.activeInHierarchy)
					child.GetComponent<RedRabbitController>().StartCoroutine(child.GetComponent<RedRabbitController>().RedRabbitManager(false));
			}
		}
	}

	private Faces FindFace(string name)
	{
		Faces myface = Faces.FaceX1;

		if(name.Contains("ZoneX"))
		{
			myface = (name.Contains("Reverse")) ? Faces.FaceX1 : Faces.FaceX2;
		}
		else if(name.Contains("ZoneY"))
		{
			myface = (name.Contains("Reverse")) ? Faces.FaceY2 : Faces.FaceY1;
		}
		else if(name.Contains("ZoneZ"))
		{
			myface = (name.Contains("Reverse")) ? Faces.FaceZ2 : Faces.FaceZ1;
		}

		return myface;
	}

	private void Control()
	{
		int vertical = (int)Input.GetAxisRaw("Vertical");
		int horizontal = (int)Input.GetAxisRaw("Horizontal");
			
		if(horizontal != 0 && right == 0)
		{
			WantRight = horizontal;
			WantUp = 0;
			Pivote = true;
		}	
		if(vertical != 0 && up == 0)
		{
			WantUp = vertical;
			WantRight = 0;
			Pivote = true;
		}
		
		if(State == SnakeState.Wait && (horizontal != 0 || vertical != 0))
		{
			if(vertical == -1)
			{
				targetRotation *= Quaternion.Euler(0, 0, 180);
				myTransform.rotation = targetRotation;
			}
			else if(horizontal != 0)
			{
				targetRotation *= Quaternion.Euler(0, 0, -horizontal * 90);
				myTransform.rotation = targetRotation;
			}
				
			State = SnakeState.Run;
			ActivateCreator();
		}
	}

	private void SnakeBody()
	{
		Vector3 absoluPos = myTransform.AbsolutePosition();
		float actualDistance = Vector3.Distance(myTransform.position, absoluPos);

		if(actualDistance < 0.25f && !Clonage)
		{
			Instantiate(SnakeBodyPrefab, absoluPos, myTransform.AbsoluteRotation());
			Clonage = true;
		}
		else if(actualDistance > 0.275f)
		{
			Clonage = false;
		}
	}

	private void UpdateSnakeMovement()
	{
		if(!gameManager.Safe)
			Control();

		if(!Switch && CanTurn())
		{
			if(WantUp != 0 || WantRight != 0)
			{
				up = WantUp;
				right = WantRight;

				WantUp = 0;
				WantRight = 0;
			}

			if(Pivote)		//peut etre possibilité d'optimisation sans if ...
			{
				if(right != 0)
				{
					targetRotation *= Quaternion.Euler(0, 0, -Mathf.RoundToInt(Vector3.Dot(myTransform.right, Heart.right)) * right * 90);
					Pivote = false;
				}
				else if(up != 0)
				{
					targetRotation *= Quaternion.Euler(0, 0, -Mathf.RoundToInt(Vector3.Dot(myTransform.right, Heart.up)) * up * 90);
					Pivote = false;
				}
			}
		}
		
		targetPosition = myTransform.AbsolutePosition() + up * Heart.up + right * Heart.right;
		Cube.position = targetPosition;

		myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, Speed * Time.deltaTime);
		myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, Time.deltaTime * Omega);
	}

	public void Rebooting()
	{
		WantUp = 0;
		WantRight = 0;
		up = 0;
		right = 0;
	}

	public IEnumerator SwitchFace(Transform other)
	{
		Switch = true;

		yield return waitforseconds_03;

		Switch = false;
	}

	private void PositionToCell()
	{
		int a;
		int b;
		int myface = (int)Face;
		Vector3 vector;

		lign = (myface > 2) ? ligns[(myface + 2) % 3] : ligns[(myface + 1) % 3];
		column = (myface > 2) ? columns[(myface + 1) % 3] : columns[(myface + 2) % 3];

		zero = Heart.position + 10.5f * lign - 10.5f * column;
		pos = myTransform.AbsolutePosition();

		vector = Vector3.Scale((zero - pos), lign);
		if(Mathf.RoundToInt(vector.x) != 0)
			a = Mathf.RoundToInt(vector.x);
		else if(Mathf.RoundToInt(vector.y) != 0)
			a = Mathf.RoundToInt(vector.y);
		else
			a = Mathf.RoundToInt(vector.z);

		vector = Vector3.Scale((pos - zero), column);
		if(Mathf.RoundToInt(vector.x) != 0)
			b = Mathf.RoundToInt(vector.x);
		else if(Mathf.RoundToInt(vector.y) != 0)
			b = Mathf.RoundToInt(vector.y);
		else
			b = Mathf.RoundToInt(vector.z);

		if(a >= 0 && b >= 0 && a <= 21 && b <= 21)
		{
			myCell = myface*22*22 + a*22 + b;
			if(planetScript.Grid[myCell] == Cell.Empty || planetScript.Grid[myCell] == Cell.Apple)
				planetScript.Grid[myCell] = Cell.Snake;
		}
	}
}*/