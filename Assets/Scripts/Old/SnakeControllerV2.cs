/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tools;

public enum SnakeState
{
	Run,
	Stopped,
	Waiting
};

public class SnakeControllerV3 : MonoBehaviour 
{
	[HideInInspector]
	public Transform Heart;
	private Transform myTransform;
	private Transform Cube;
	private Transform edgeTransform = null;
	private Transform SnakeBody;
	[Header("Prefab")]
	public GameObject SnakeBodyPrefab;

	private GameManagerV1 gameManager;
	private PlanetScript planetScript;
	private SnakeManagement snakeManag;

	private IEnumerator move_coroutine;
	
	private Quaternion subtargetRotation = Quaternion.identity;
	[HideInInspector]
	public Quaternion targetRotation
	{
		set
		{
			subtargetRotation = value;
			myTransform.rotation = value;
		}
		get
		{
			return subtargetRotation;
		}
	}
	[HideInInspector]
	public Vector3 targetPosition = Vector3.zero;
	private Vector3 edgePosition = Vector3.zero;

	private Faces edgeFace = Faces.FaceX1;

	[Header("Settings")]
	[Range(0.0f, 50.0f)]
	public float Speed = 10.0f;
	[Range(0.01f, 0.2f)]
	[Tooltip("The accuracy of the deplacement of Snake, less is more accurrate (0.15f is fine).")]
	public float PositionAccuracy = 0.1f;		//plus c'est bas, plus c'est précis

	//[HideInInspector]
	public int switchNotch = 0;
	[HideInInspector]
	public int right = 0;
	[HideInInspector]
	public int up = 0;
	[HideInInspector]
	public int upStored = 0;
	[HideInInspector]
	public int rightStored = 0;

	[Header("States")]
	public SnakeState State = SnakeState.Stopped;

	[Header("Location")]
	public Faces Face = Faces.FaceY1;
	public int myCell = 0;

	[HideInInspector]
	public bool CancelInput = false;
	private bool canIncrementSwitchNotch = false;

	void Awake()
	{
		myTransform = transform;
		
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();
		snakeManag = GetComponent<SnakeManagement>();

		Cube = myTransform.Find("SnakeDetector");
		Cube.parent = null;
		Cube.SetAsFirstSibling();

		targetPosition = myTransform.position;
		targetRotation = myTransform.rotation;

		SnakeBody = snakeManag.SnakeBody;

		Heart = gameManager.MainPlanet.Find("Heart").transform;
	}

	void Start()
	{
		myCell = myTransform.PositionToCell(gameManager.MainPlanet);
		if(myCell >= 0 && planetScript && planetScript.Grid.Length < myCell)
			planetScript.Grid[myCell] = Cell.Snake;
	}

	void OnEnable()
	{
		move_coroutine = UpdateSnakeMovement();
		StartCoroutine(move_coroutine);
	}

	void OnDisable()
	{
		StopCoroutine(move_coroutine);
	}

	void Update()
	{
		if(State != SnakeState.Stopped && !gameManager.Paused && !gameManager.Safe)
		{
			Control();
			Cube.position = myTransform.AbsolutePosition() + up * Heart.up + right * Heart.right;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		Transform otherTransform = other.transform;

		if(other.CompareTag("ZoneSnake") && State == SnakeState.Run)
		{
			Edging(otherTransform);
		}
		else if(other.CompareTag("Border"))
		{
			Debug.Log("1");
			Bordering(otherTransform);
		}
	}

	private void Edging(Transform trans)
	{
		Faces theface = CellExtension.ZoneToFace(trans);

		if(theface == Face)
			return;

		edgePosition = myTransform.AbsolutePosition();

		if(theface == Faces.FaceX1 || theface == Faces.FaceX2)
			edgePosition.x = Mathf.RoundToInt(trans.position.x);
		else if(theface == Faces.FaceY1 || theface == Faces.FaceY2)
			edgePosition.y = Mathf.RoundToInt(trans.position.y);
		else if(theface == Faces.FaceZ1 || theface == Faces.FaceZ2)
			edgePosition.z = Mathf.RoundToInt(trans.position.z);

		edgeTransform = trans;
		edgeFace = theface;
	}
	private void Bordering(Transform trans)
	{
		int i;
		float dist;
		float min = 500.0f;
		Vector3 Abspos = myTransform.AbsolutePosition();
		Vector3 Nearest = Vector3.zero;
		Vector3 dir = Vector3Extension.RoundToInt(Abspos - trans.position).normalized;
		List<Vector3> Borders = new List<Vector3>();

		RaycastHit[] hits = Physics.RaycastAll(Abspos, dir, 500.0f);
		Debug.DrawRay(Abspos, dir * 500f, Color.green);

		for(i = 0; i < hits.Length; i++)
		{
			RaycastHit hit = hits[i];
			if(hit.transform.CompareTag("Border") && hit.transform != trans)
				Borders.Add(TransformExtension.AbsolutePosition(hit.point - dir*0.5f));
		}

		int len = Borders.Count;
		for(i = 0; i < len; i++)
		{
			dist = Vector3.Distance(Abspos, Borders[i]);
			if(dist < min)
			{
				min = dist;
				Nearest = Borders[i];
			}
		}

		targetPosition = Nearest;
		myTransform.position = Nearest;
		switchNotch = 0;
	}

	private void Control()
	{
		if(CancelInput)
			return;
			
		int vertical = (int)Input.GetAxisRaw("Vertical");
		int horizontal = (int)Input.GetAxisRaw("Horizontal");
			
		if(horizontal != 0 && right == 0)
		{
			rightStored = horizontal;
			upStored = 0;
		}	
		else if(vertical != 0 && up == 0)
		{
			upStored = vertical;
			rightStored = 0;
		}
		
		if(State == SnakeState.Waiting && (horizontal != 0 || vertical != 0))
		{
			if(vertical == -1)
				targetRotation *= Quaternion.Euler(0, 0, 180);
			else if(horizontal != 0)
				targetRotation *= Quaternion.Euler(0, 0, -horizontal * 90);
				
			State = SnakeState.Run;
		}
	}

	private IEnumerator UpdateSnakeMovement()
	{
		int rotationValue = 0;
		int count = 0;
		int nb = 0;
		float clock = 100000f;
		Transform LastSnakeBody = null;
		SnakeFollow script = null;

		while(true)
		{
			if(State != SnakeState.Stopped && !gameManager.Paused && !gameManager.Safe)
			{
				if(switchNotch == 0)
				{
					if(upStored != 0 || rightStored != 0)
					{
						up = upStored;
						right = rightStored;

						rotationValue = Mathf.RoundToInt(Vector3.Dot(myTransform.right, Heart.right*Mathf.Abs(right) + Heart.up*Mathf.Abs(up))) * (right+up) * -90;

						if(rotationValue != 0)
						{
							targetRotation *= Quaternion.Euler(0, 0, rotationValue);
							rotationValue = 0;
						}

						upStored = 0;
						rightStored = 0;
					}
				}
				
				targetPosition = myTransform.AbsolutePosition() + up * Heart.up + right * Heart.right;

				if(Vector3.Distance(myTransform.position, targetPosition) > PositionAccuracy)
				{
					while(Vector3.Distance(myTransform.position, targetPosition) > PositionAccuracy)
					{
						myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, Speed * Time.deltaTime);
						yield return null;
					}
				}
				else
				{
					yield return null;
				}

				if(switchNotch > 0 && canIncrementSwitchNotch)
				{
					if(++ switchNotch == 3)
					{
						switchNotch = 0;
						canIncrementSwitchNotch = false;
					}
				}

				if(edgeTransform != null && Vector3.Distance(targetPosition, edgePosition) < 0.1f)
				{
					Vector3 axis = Vector3Extension.RoundToInt(myTransform.right);
					Vector3 dir = Vector3Extension.RoundToInt(myTransform.up);

					Heart.rotation *= Quaternion.Euler(0, 90 * Mathf.RoundToInt(Vector3.Dot(Heart.up, axis)) * Mathf.RoundToInt(Vector3.Dot(dir, edgeTransform.forward)), 0);
					Heart.rotation *= Quaternion.Euler(90 * Mathf.RoundToInt(Vector3.Dot(Heart.right, axis)) * Mathf.RoundToInt(Vector3.Dot(dir, edgeTransform.forward)), 0, 0);
					targetRotation *= Quaternion.Euler(Mathf.RoundToInt(Vector3.Dot(dir, edgeTransform.forward)) * 90, 0, 0);
					Face = edgeFace;

					edgePosition = Vector3.zero;
					edgeTransform = null;

					canIncrementSwitchNotch = true;
				}

				if(State == SnakeState.Run)
				{
					count = SnakeBody.childCount;
					nb = snakeManag.BodyNumber;

					if(count < nb + 3) // +3 pour une petite marge
					{
						Instantiate(SnakeBodyPrefab, targetPosition, targetRotation);
					}
					else
					{
						LastSnakeBody = SnakeBody.GetChild(count-1);
						script = LastSnakeBody.GetComponent<SnakeFollow>();

						if(script.Reusable)
						{
							LastSnakeBody.position = targetPosition;
							LastSnakeBody.rotation = targetRotation;
							script.Setup();
						}
						else
						{
							Instantiate(SnakeBodyPrefab, targetPosition, targetRotation);
						}
					}

					myCell = myTransform.PositionToCell(gameManager.MainPlanet);
					if(myCell >= 0 && planetScript && planetScript.Grid.Length < myCell && (planetScript.Grid[myCell] == Cell.Empty || planetScript.Grid[myCell] == Cell.Apple))
						planetScript.Grid[myCell] = Cell.Snake;
				}
			}
			else if(State == SnakeState.Stopped && SnakeBody.childCount > 0)
			{
				clock += Time.deltaTime;
				if(clock > 1f/Speed)
				{
					LastSnakeBody = SnakeBody.GetChild(SnakeBody.childCount-1);
					if(LastSnakeBody.localScale == Vector3.zero)
					{
						LastSnakeBody.GetComponent<SnakeFollow>().GoToPoubelle();
					}
					else
					{
						LastSnakeBody.GetComponent<SnakeFollow>().ReductionNextPoubelle();
						clock = 0f;
						yield return null;
					}
				}
				else
				{
					yield return null;
				}
			}
			else
			{
				yield return null;
			}
		}
	}
}
*/