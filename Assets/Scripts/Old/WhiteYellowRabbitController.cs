using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Tools;

public class WhiteYellowRabbitController : MonoBehaviour
{
	private Transform myTransform;
	private Transform Body;
	private Transform Snake;
	private GameObject Eyes;
	private Transform Planet;

	public int myCell = -1;
	private float distance;
	private float delay = 0.75f;
	private float min = 0.05f;
	private float max = 0.5f;
	private float change_face_time = 0.0f;
	private float moveSpeed = 10.0f;
	private float jumpSpeed = 20.0f;
	private float scaleSpeed = 5.0f;
	private float start_time = 0.0f;

	private Renderer myRend;
	private Collider myColl;

	private Image myScreen;

	private Faces myFace;

	private Vector3[] ligns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
	private Vector3[] columns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};

	private IEnumerator move_coroutine;
	private IEnumerator speak_coroutine;

	private ParticleSystem Sweat;
	private ParticleSystem Explosion;

	private Quaternion targetRotation;
	private Vector3 targetHeight = new Vector3(0, 2, 0);
	private Vector3 targetScale = new Vector3(1, 0.5f, 1);
	private Vector3 targetLocalPosition = new Vector3(0, -0.25f, 0);
	private Vector3 targetPosition;
	private Vector3 Lign;
	private Vector3 Column;

	private bool Jumping = false;
	private bool Dead = false;
	private bool Switch = false;
	[HideInInspector]
	public bool TurnedBack = false;
	[HideInInspector]
	public bool Thresholded = false;

	private GameManagerV1 gameManager;
	private PlanetScript planetScript;
	private ArmchairScript armchairScript;
	private SnakeControllerV3 snakeScript;
	private TutorielManager tutoScript;
	private BunneyTutoriel bunneyScript;

	void Awake()
	{
		myTransform = transform;
		Body = myTransform.Find("Body");
		Snake = GameObject.FindWithTag("Player").transform;
		Eyes = Body.Find("Eyes").gameObject;

		myScreen = GameObject.Find("Canvas/InGame/Screen").GetComponent<Image>();

		Eyes.SetActive(false);

		myRend = Body.GetComponent<Renderer>();
		myColl = GetComponent<Collider>();

		myColl.enabled = false;

		Sweat = Body.Find("Sweat").GetComponent<ParticleSystem>();
		Explosion = myTransform.Find("Explosion").GetComponent<ParticleSystem>();
		
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		armchairScript = GameObject.FindWithTag("Armchair").GetComponent<ArmchairScript>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		planetScript = GameObject.Find("Planets/Planet_1").GetComponent<PlanetScript>();
		tutoScript = GameObject.Find("LevelManager").GetComponent<TutorielManager>();
		bunneyScript = GameObject.Find("LevelManager").GetComponent<BunneyTutoriel>();
	}

	void Start()
	{
		myFace = Faces.FaceY1;
		Planet = gameManager.MainPlanet;

		int myface = (int)myFace;
		Lign = (myface > 2) ? ligns[(myface + 2) % 3] : ligns[(myface + 1) % 3];
		Column = (myface > 2) ? columns[(myface + 1) % 3] : columns[(myface + 2) % 3];

		myCell = PositionToCell(myTransform.position);

		min = Random.Range(0.0f, 0.025f);

		move_coroutine = Move();
		StartCoroutine(move_coroutine);
	}

	void Update()
	{
		if(!Dead)
		{
			distance = Vector3.Distance(Snake.position, myTransform.position);
			if(distance < 5.0f)
			{
				if(Switch)
				{
					delay = 0.0f;
					scaleSpeed = 50.0f;
					jumpSpeed = 40.0f;
					moveSpeed = 25.0f;
				}
				else
				{
					delay = min;
					scaleSpeed = 25.0f;
					jumpSpeed = 20.0f;
					moveSpeed = 10.0f;
				}
			}
			else if(distance < 10.0f)
			{
				if(Sweat.isStopped && tutoScript.WYRabbitCaught+tutoScript.RabbitKilled > 0)
					Sweat.Play();
				delay = ((max - min)/(10 - 5)) * distance + (min - 5 * ((max - min)/(10 - 5)));
				scaleSpeed = (20.0f/(min - max)) * delay + (5.0f - (20.0f/(min - max)) * max);
				jumpSpeed = 20.0f;
				moveSpeed = 10.0f;
			}
			else
			{
				if(Sweat.isPlaying)
					Sweat.Stop();
				delay = max;
				scaleSpeed = 5.0f;
				jumpSpeed = 20.0f;
				moveSpeed = 10.0f;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Rabbit"))
		{
			other.transform.parent.gameObject.SetActive(false);
		}
		else if(other.CompareTag("Apple"))
		{
			other.gameObject.SetActive(false);
		}
	}

	private IEnumerator Move()
	{
		Vector3[] Directions;
		Vector3 cell_position;
		int[] Cells = new int[4] {-1, -1, -1, -1};
		int[] TargetCells = new int[4] {-1, -1, -1, -1};
		int target_index;
		int choice;
		int cell;
		float max_dist;
		float dist_between_snake;
		float clock;

		while(true)
		{
			if(snakeScript.State == SnakeState.Run)
			{
				if(start_time < 0.01f)
					start_time = Time.time;

				target_index = 0;
				choice = 0;
				max_dist = 0.0f;
				dist_between_snake = 0.0f;
				Directions = new Vector3[4] {myTransform.forward, myTransform.right, -myTransform.forward, -myTransform.right};

				for(int i = 0; i < 4; i++)
				{
					cell = PositionToCell(TransformExtension.AbsolutePosition(myTransform) + Directions[i] * 2);
					Cells[i] = cell;

					if(cell >= 0 && planetScript.Grid[cell] == CellEnum.Empty && cell/(22*22) != (int)armchairScript.Face)
					{
						TargetCells[target_index ++] = i;
					}
				}

				if(target_index > 0)
				{
					if(distance < 10.0f)
					{
						for(int k = 0; k < target_index; k++)
						{
							cell_position = CellExtension.CellToPosition(Cells[TargetCells[k]], Planet);
							dist_between_snake = Vector3.Distance(cell_position, Snake.position);
							if(dist_between_snake > max_dist)
							{
								choice = TargetCells[k];
								max_dist = dist_between_snake;
							}
						}
					}
					else
					{
						choice = TargetCells[Random.Range(0, target_index)];
					}

					myTransform.rotation *= Quaternion.Euler(0, choice * 90, 0);

					yield return null;

					if(Cells[choice]/(22*22) == (int)myFace)
					{
						targetPosition = TransformExtension.AbsolutePosition(myTransform) + 2 * myTransform.forward;
						while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
						{
							Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
							Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
							yield return null;
						}
						StartCoroutine(Jump());
						while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
						{
							myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
							yield return null;
						}
						myTransform.position = targetPosition;

						if(planetScript.Grid[myCell] == CellEnum.Rabbit)
							planetScript.Grid[myCell] = CellEnum.Empty;

						myCell = Cells[choice];
					}
					else
					{
						Switch = true;
						change_face_time = Time.time;

						int face = myCell/(22*22);
						int lig = Mathf.RoundToInt(Vector3.Dot(myTransform.forward, Lign));
						int dist = 0;
						if(lig != 0)
							dist = ((myCell-22*22*face)/22 == 0 || (myCell-22*22*face)/22 == 21) ? 5 : 6;
						else
							dist = (myCell%22 == 0 || myCell%22 == 21) ? 5 : 6;

						Vector3 change_facePosition = TransformExtension.AbsolutePosition(myTransform) + myTransform.forward*dist - myTransform.up*(dist - 4);

						targetPosition = TransformExtension.AbsolutePosition(myTransform) + 2 * myTransform.forward;
						while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
						{
							Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
							Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
							yield return null;
						}
						StartCoroutine(Jump());
						while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
						{
							myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
							yield return null;
						}
						myTransform.position = targetPosition;

						do
							yield return null;
						while(Jumping);

						clock = 0.0f;
						while(clock < delay)
						{
							clock += Time.deltaTime;
							yield return null;
						}
						
						targetPosition = TransformExtension.AbsolutePosition(myTransform) + 2 * myTransform.forward;
						while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
						{
							Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
							Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
							yield return null;
						}
						StartCoroutine(Jump());
						while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
						{
							myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
							yield return null;
						}
						myTransform.position = targetPosition;

						do
							yield return null;
						while(Jumping);

						clock = 0.0f;
						while(clock < delay)
						{
							clock += Time.deltaTime;
							yield return null;
						}

						while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
						{
							Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
							Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
							yield return null;
						}
						StartCoroutine(Jump());

						yield return new WaitForSeconds(0.03f);

						targetPosition = change_facePosition;
						targetRotation = TransformExtension.AbsoluteRotation(myTransform.rotation) * Quaternion.Euler(90, 0, 0);
						Quaternion fromRotation = myTransform.rotation;
						float slerp_diff = Vector3.Distance(myTransform.position, targetPosition);
						float slerp_dist = slerp_diff;
						while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
						{
							slerp_dist = Vector3.Distance(myTransform.position, targetPosition);
							myTransform.rotation = Quaternion.Slerp(fromRotation, targetRotation, 1.0f - (slerp_dist / slerp_diff));
							myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
							yield return null;
						}
						myTransform.position = targetPosition;
						myTransform.rotation = targetRotation;

						do
							yield return null;
						while(Jumping);

						clock = 0.0f;
						while(clock < delay)
						{
							clock += Time.deltaTime;
							yield return null;
						}

						targetPosition = TransformExtension.AbsolutePosition(myTransform) + 2 * myTransform.forward;
						while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
						{
							Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
							Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
							yield return null;
						}
						StartCoroutine(Jump());
						while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
						{
							myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
							yield return null;
						}
						myTransform.position = targetPosition;

						do
							yield return null;
						while(Jumping);

						clock = 0.0f;
						while(clock < delay)
						{
							clock += Time.deltaTime;
							yield return null;
						}

						targetPosition = TransformExtension.AbsolutePosition(myTransform) + 2 * myTransform.forward;
						while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
						{
							Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
							Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
							yield return null;
						}
						StartCoroutine(Jump());
						while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
						{
							myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
							yield return null;
						}
						myTransform.position = targetPosition;

						if(planetScript.Grid[myCell] == CellEnum.Rabbit)
							planetScript.Grid[myCell] = CellEnum.Empty;

						myCell = Cells[choice];

						int myface = myCell/(22*22);
						myFace = (Faces)myface;
						Lign = (myface > 2) ? ligns[(myface + 2) % 3] : ligns[(myface + 1) % 3];
						Column = (myface > 2) ? columns[(myface + 1) % 3] : columns[(myface + 2) % 3];

						Switch = false;
					}

					planetScript.Grid[myCell] = CellEnum.Rabbit;
				}

				do
					yield return null;
				while(Jumping);
			}

			clock = 0.0f;
			while(clock < delay)
			{
				clock += Time.deltaTime;
				yield return null;
			}
		}
	}

	private IEnumerator MoveTowards(Vector3 target)
	{
		Vector3[] Directions;
		Vector3 cell_position;
		int[] Cells = new int[4] {-1, -1, -1, -1};
		int[] TargetCells = new int[4] {-1, -1, -1, -1};
		int target_index;
		int choice;
		int cell;
		float min_dist;
		float dist_between_target;
		float clock;
		bool towards = true;

		while(towards)
		{
			target_index = 0;
			choice = 0;
			min_dist = 100000.0f;
			dist_between_target = 0.0f;
			Directions = new Vector3[4] {myTransform.forward, myTransform.right, -myTransform.forward, -myTransform.right};

			for(int i = 0; i < 4; i++)
			{
				cell = PositionToCell(TransformExtension.AbsolutePosition(myTransform) + Directions[i] * 2);
				Cells[i] = cell;

				if(cell >= 0 && planetScript.Grid[cell] == CellEnum.Empty && cell/(22*22) != (int)armchairScript.Face)
				{
					TargetCells[target_index ++] = i;
				}
			}

			if(target_index > 0)
			{
				for(int k = 0; k < target_index; k++)
				{
					cell_position = CellExtension.CellToPosition(Cells[TargetCells[k]], Planet);
					dist_between_target = Vector3.Distance(cell_position, target);
					if(dist_between_target < min_dist)
					{
						min_dist = dist_between_target;
						choice = TargetCells[k];
					}
				}

				myTransform.rotation *= Quaternion.Euler(0, choice * 90, 0);

				yield return null;

				if(Cells[choice]/(22*22) == (int)myFace)
				{
					targetPosition = TransformExtension.AbsolutePosition(myTransform) + 2 * myTransform.forward;
					while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
					{
						Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
						Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
						yield return null;
					}
					StartCoroutine(Jump());
					while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
					{
						myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
						yield return null;
					}
					myTransform.position = targetPosition;
					towards = (Vector3.Distance(targetPosition, target) > 0.1f);

					if(planetScript.Grid[myCell] == CellEnum.Rabbit)
						planetScript.Grid[myCell] = CellEnum.Empty;

					myCell = Cells[choice];
				}
				else
				{
					Switch = true;
					change_face_time = Time.time;

					int face = myCell/(22*22);
					int lig = Mathf.RoundToInt(Vector3.Dot(myTransform.forward, Lign));
					int dist = 0;
					if(lig != 0)
						dist = ((myCell-22*22*face)/22 == 0 || (myCell-22*22*face)/22 == 21) ? 5 : 6;
					else
						dist = (myCell%22 == 0 || myCell%22 == 21) ? 5 : 6;

					Vector3 change_facePosition = TransformExtension.AbsolutePosition(myTransform) + myTransform.forward*dist - myTransform.up*(dist - 4);

					targetPosition = TransformExtension.AbsolutePosition(myTransform) + 2 * myTransform.forward;
					while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
					{
						Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
						Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
						yield return null;
					}
					StartCoroutine(Jump());
					while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
					{
						myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
						yield return null;
					}
					myTransform.position = targetPosition;

					do
						yield return null;
					while(Jumping);

					clock = 0.0f;
					while(clock < delay)
					{
						clock += Time.deltaTime;
						yield return null;
					}
					
					targetPosition = TransformExtension.AbsolutePosition(myTransform) + 2 * myTransform.forward;
					while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
					{
						Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
						Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
						yield return null;
					}
					StartCoroutine(Jump());
					while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
					{
						myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
						yield return null;
					}
					myTransform.position = targetPosition;

					do
						yield return null;
					while(Jumping);

					clock = 0.0f;
					while(clock < delay)
					{
						clock += Time.deltaTime;
						yield return null;
					}

					while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
					{
						Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
						Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
						yield return null;
					}
					StartCoroutine(Jump());

					yield return new WaitForSeconds(0.03f);

					targetPosition = change_facePosition;
					targetRotation = TransformExtension.AbsoluteRotation(myTransform.rotation) * Quaternion.Euler(90, 0, 0);
					Quaternion fromRotation = myTransform.rotation;
					float slerp_diff = Vector3.Distance(myTransform.position, targetPosition);
					float slerp_dist = slerp_diff;
					while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
					{
						slerp_dist = Vector3.Distance(myTransform.position, targetPosition);
						myTransform.rotation = Quaternion.Slerp(fromRotation, targetRotation, 1.0f - (slerp_dist / slerp_diff));
						myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
						yield return null;
					}
					myTransform.position = targetPosition;
					myTransform.rotation = targetRotation;

					do
						yield return null;
					while(Jumping);

					clock = 0.0f;
					while(clock < delay)
					{
						clock += Time.deltaTime;
						yield return null;
					}

					targetPosition = TransformExtension.AbsolutePosition(myTransform) + 2 * myTransform.forward;
					while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
					{
						Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
						Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
						yield return null;
					}
					StartCoroutine(Jump());
					while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
					{
						myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
						yield return null;
					}
					myTransform.position = targetPosition;

					do
						yield return null;
					while(Jumping);

					clock = 0.0f;
					while(clock < delay)
					{
						clock += Time.deltaTime;
						yield return null;
					}

					targetPosition = TransformExtension.AbsolutePosition(myTransform) + 2 * myTransform.forward;
					while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
					{
						Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
						Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
						yield return null;
					}
					StartCoroutine(Jump());
					while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
					{
						myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
						yield return null;
					}
					myTransform.position = targetPosition;
					towards = (Vector3.Distance(targetPosition, target) > 0.1f);

					if(planetScript.Grid[myCell] == CellEnum.Rabbit)
						planetScript.Grid[myCell] = CellEnum.Empty;

					myCell = Cells[choice];

					int myface = myCell/(22*22);
					myFace = (Faces)myface;
					Lign = (myface > 2) ? ligns[(myface + 2) % 3] : ligns[(myface + 1) % 3];
					Column = (myface > 2) ? columns[(myface + 1) % 3] : columns[(myface + 2) % 3];

					Switch = false;
				}

				planetScript.Grid[myCell] = CellEnum.Rabbit;
			}

			do
				yield return null;
			while(Jumping);

			clock = 0.0f;
			while(clock < delay)
			{
				clock += Time.deltaTime;
				yield return null;
			}
		}
	}

	public IEnumerator Encounter()
	{
		float caught_time = Time.time - start_time;

		if(Switch && (tutoScript.WYRabbitCaught == 2 || (tutoScript.WYRabbitCaught+tutoScript.RabbitKilled == 0 && caught_time >= 15.0f)))
			yield break;

		Vector3 axis = Snake.right;

		Dead = true;
		Sweat.Stop();
		
		StopCoroutine(move_coroutine);

		if(tutoScript.Repetition < 9 && (tutoScript.WYRabbitCaught == 2 || (tutoScript.WYRabbitCaught+tutoScript.RabbitKilled == 0 && caught_time >= 15.0f)))
		{
			bunneyScript.StartCoroutine(bunneyScript.WhiteAndYellow(myTransform));

			snakeScript.State = SnakeState.Stopped;
			Snake.rotation = Snake.AbsoluteRotation();
			snakeScript.targetRotation = Snake.rotation;
			Snake.position = Snake.AbsolutePosition();
			snakeScript.targetPosition = Snake.position;
			snakeScript.up = 0;
			snakeScript.right = 0;

			myTransform.position = Snake.AbsolutePosition() + Snake.up;
			myTransform.rotation = Snake.rotation * Quaternion.Euler(-90, 0, 0);

			myColl.enabled = true;
			yield return null;
			myColl.enabled = false;
		}
		else
		{
			Body.localPosition = Vector3.zero;
			Body.localScale = Vector3.one;

			float omega = -15.0f;

			targetPosition = myTransform.position + 4 * myTransform.up;
			while(Vector3.Distance(myTransform.position, targetPosition) > 0.1f)
			{
				myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, 5.0f * Time.deltaTime);
				omega = Mathf.MoveTowards(omega, 0.0f, 20f * Time.deltaTime);
				Body.rotation = Quaternion.AngleAxis(omega, axis) * Body.rotation;
				yield return null;
			}

			yield return new WaitForSeconds(0.25f);

			myRend.enabled = false;
			Eyes.SetActive(false);

			Sweat.Stop();
			Explosion.Play();

			tutoScript.WYRabbitCaught ++;

			yield return new WaitForSeconds(1.0f);

			Destroy(gameObject);
		}
	}

	public IEnumerator TurnBack()
	{
		Vector3 dir = (myTransform.position - Snake.position).normalized;
		targetPosition = TransformExtension.AbsolutePosition(myTransform) + 2 * dir;
		while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
		{
			Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
			Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
			yield return null;
		}
		StartCoroutine(Jump());
		while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
		{
			myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
			yield return null;
		}
		myTransform.position = targetPosition;

		do
			yield return null;
		while(Jumping);

		while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
		{
			Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, scaleSpeed * Time.deltaTime);
			Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, scaleSpeed/2 * Time.deltaTime);
			yield return null;
		}
		StartCoroutine(Jump());

		myTransform.rotation *= Quaternion.Euler(0, 180, 0);

		do
			yield return null;
		while(Jumping);

		TurnedBack = true;
	}

	public IEnumerator Threshold()
	{
		float range = 0.05f;
		int nb = 150;
		for(int i = 0; i < nb; i++)
		{
			targetLocalPosition = new Vector3(Random.Range(-range, range), Random.Range(0.0f, range), Random.Range(-range, range));
			range += 0.35f/nb;
			while(Vector3.Distance(Body.localPosition, targetLocalPosition) > 0.1f)
			{
				Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, 15.0f * Time.deltaTime);
				yield return null;
			}
		}
		while(Vector3.Distance(Body.localPosition, Vector3.zero) > 0.01f)
		{
			Body.localPosition = Vector3.MoveTowards(Body.localPosition, Vector3.zero, 15.0f * Time.deltaTime);
			yield return null;
		}

		Body.localPosition = Vector3.zero;

		myScreen.color = Color.white;
		myScreen.SetColorA(1.0f);
		Eyes.SetActive(true);
		while(myScreen.color.a > 0.01f)
		{
			myScreen.SetColorA(Mathf.MoveTowards(myScreen.color.a, 0.0f, 3.0f * Time.deltaTime));
			yield return null;
		}
		myScreen.SetColorA(0.0f);

		Thresholded = true;
	}

	private int PositionToCell(Vector3 position)
	{
		int cell = 0;

		int a;
		int b;
		int myface = (int)myFace;

		Vector3 vector;
		Vector3 zero;

		zero = Planet.position + 10.5f * Lign - 10.5f * Column;

		vector = Vector3.Scale((zero - position), Lign);
		if(Mathf.RoundToInt(vector.x) != 0)
			a = Mathf.RoundToInt(vector.x);
		else if(Mathf.RoundToInt(vector.y) != 0)
			a = Mathf.RoundToInt(vector.y);
		else
			a = Mathf.RoundToInt(vector.z);

		vector = Vector3.Scale((position - zero), Column);
		if(Mathf.RoundToInt(vector.x) != 0)
			b = Mathf.RoundToInt(vector.x);
		else if(Mathf.RoundToInt(vector.y) != 0)
			b = Mathf.RoundToInt(vector.y);
		else
			b = Mathf.RoundToInt(vector.z);

		if(a >= 0 && b >= 0 && a <= 21 && b <= 21)
			cell = myface*22*22 + a*22 + b;
		else if(change_face_time + 2.5f < Time.time)
			cell = NewfaceToCell((position - TransformExtension.AbsolutePosition(myTransform)).normalized);
		else
			cell = -1;

		return cell;
	}

	private int NewfaceToCell(Vector3 dir)
	{
		int cell = 0;
		int face = (int)myFace;
		int lig = Mathf.RoundToInt(Vector3.Dot(dir, Lign));
		int col = Mathf.RoundToInt(Vector3.Dot(dir, Column));
		int newface;
		int a;
		int b;
		int dist;

		Vector3 vector;
		Vector3 lign;
		Vector3 column;
		Vector3 zero;
		Vector3 location;

		if(lig != 0)
			dist = ((myCell-22*22*face)/22 == 0 || (myCell-22*22*face)/22 == 21) ? 5 : 6;
		else
			dist = (myCell%22 == 0 || myCell%22 == 21) ? 5 : 6;

		if(face < 3)
		{
			if(lig == 1)
				newface = (face + 1) % 3;
			else if(lig == -1)
				newface = ((face + 1) % 3) + 3;
			else if(col == 1)
				newface = (face + 2) % 3;
			else
				newface = ((face + 2) % 3) + 3;
		}
		else
		{
			face = (face + 3) % 6;

			if(lig == 1)
				newface = (face + 2) % 3;
			else if(lig == -1)
				newface = ((face + 2) % 3) + 3;
			else if(col == 1)
				newface = (face + 1) % 3;
			else
				newface = ((face + 1) % 3) + 3;
		}

		lign = (newface > 2) ? ligns[(newface + 2) % 3] : ligns[(newface + 1) % 3];
		column = (newface > 2) ? columns[(newface + 1) % 3] : columns[(newface + 2) % 3];

		zero = Planet.position + 10.5f * lign - 10.5f * column;
		location = TransformExtension.AbsolutePosition(myTransform) + dir*dist - myTransform.up*dist;

		vector = Vector3.Scale((zero - location), lign);
		if(Mathf.RoundToInt(vector.x) != 0)
			a = Mathf.RoundToInt(vector.x);
		else if(Mathf.RoundToInt(vector.y) != 0)
			a = Mathf.RoundToInt(vector.y);
		else
			a = Mathf.RoundToInt(vector.z);

		vector = Vector3.Scale((location - zero), column);
		if(Mathf.RoundToInt(vector.x) != 0)
			b = Mathf.RoundToInt(vector.x);
		else if(Mathf.RoundToInt(vector.y) != 0)
			b = Mathf.RoundToInt(vector.y);
		else
			b = Mathf.RoundToInt(vector.z);

		if(a >= 0 && b >= 0 && a <= 21 && b <= 21)
		{
			cell = newface*22*22 + a*22 + b;
		}
		else
		{
			Debug.LogError("Mal visé ! a = " + a + " & b = " + b);
		}

		return cell;
	}

	private IEnumerator Jump()
	{
		Jumping = true;

		while(Vector3.Distance(Body.localPosition, targetHeight) > 0.05f)
		{
			Body.localScale = Vector3.MoveTowards(Body.localScale, Vector3.one, scaleSpeed * Time.deltaTime);
			Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetHeight, jumpSpeed * Time.deltaTime);
			yield return null;
		}
		while(Vector3.Distance(Body.localPosition, Vector3.zero) > 0.05f)
		{
			Body.localPosition = Vector3.MoveTowards(Body.localPosition, Vector3.zero, jumpSpeed * Time.deltaTime);
			yield return null;
		}
		Body.localPosition = Vector3.zero;
		Body.localScale = Vector3.one;

		Jumping = false;
	}
}