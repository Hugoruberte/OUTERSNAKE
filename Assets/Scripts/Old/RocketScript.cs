using UnityEngine;
using System.Collections;
using Tools;

public class RocketScript : MonoBehaviour
{
	[HideInInspector]
	public Transform MyPlanet;
	[HideInInspector]
	public Transform OppositePlanet;

	private Transform myTransform;
	private Transform targetCam;
	private Transform Heart;
	private Transform OppositeHeart;
	private GameObject Snake_GameObject;
	private Transform Snake_Transform;
	private Transform RocketsPooling;
	private Transform Body;
	private Transform Axis;

	private WaitForSeconds waitforseconds_05 = new WaitForSeconds(0.5f);
	private WaitForSeconds waitforseconds_01 = new WaitForSeconds(0.1f);
	private WaitForSeconds waitforseconds_1 = new WaitForSeconds(1.0f);

	private Collider SnakeColl;
	private Renderer SnakeRend;

	private Renderer bodyRend;

	private ParticleSystem Flame;
	private ParticleSystem Landing;

	private bool ReadyToFly = false;

	private int bodyNumber;
	private float distance;
	private float Length = 66.4f;

	private Vector3 Destination;

	private GameManagerV1 gameManager;
	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;
	private CameraScript cameraScript;
	private DifficultyScript difficultyScript;
	private PlanetSetup planetSetup;


	void Awake()
	{
		myTransform = transform;
		Axis = myTransform.Find("Axis");
		Body = Axis.Find("Body");

		Snake_GameObject = GameObject.FindWithTag("Player");
		Snake_Transform = Snake_GameObject.transform;

		snakeScript = Snake_GameObject.GetComponent<SnakeControllerV3>();
		snakeManag = Snake_GameObject.GetComponent<SnakeManagement>();
		
		SnakeColl = Snake_GameObject.GetComponent<Collider>();
		SnakeRend = Snake_GameObject.GetComponent<Renderer>();

		bodyRend = Body.GetComponent<Renderer>();
		bodyRend.enabled = true;

		targetCam = myTransform.Find("targetCam");
		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		difficultyScript = GameObject.Find("LevelManager").GetComponent<DifficultyScript>();
		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();
		
		Flame = Body.Find("flame").GetComponent<ParticleSystem>();
		Landing = myTransform.Find("landing").GetComponent<ParticleSystem>();
	}

	void Start()
	{
		RocketsPooling = GameObject.Find("RocketsPooling").transform;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") && snakeScript.State == SnakeState.Run && !gameManager.Rocket)
		{
			StartCoroutine(SetupFlight());
		}
		else if(other.name == "Boundarie")
		{
			myTransform.localPosition = Vector3.zero;
			myTransform.gameObject.SetActive(false);
		}
	}

	private IEnumerator SetupFlight()
	{
		gameManager.Rocket = true;
		difficultyScript.Difficulty += 10;

		Destination = Body.position + Length * Body.up;

		OppositePlanet.GetComponent<PlanetScript>().SetRotation(false);
		
		OppositeHeart = OppositePlanet.Find("Heart");
		Heart = MyPlanet.Find("Heart");

		OppositeHeart.rotation = TransformExtension.AbsoluteRotation(Heart.rotation * Quaternion.Euler(180, 0, 0));
			
		snakeScript.State = SnakeState.Stopped;
		SnakeColl.enabled = false;
		SnakeRend.enabled = false;
				
		bodyNumber = snakeManag.BodyNumber;
		Snake_Transform.position = myTransform.position;
		Snake_Transform.rotation = Snake_Transform.AbsoluteRotation();
		
		myTransform.parent = null;		//je libère la fusée pour qu'il n'y pas de probleme lors du vol

		yield return null;

		gameManager.SetMainPlanet(OppositePlanet);

		targetCam.position = Body.position - Heart.forward * 35 - Heart.up * 7.5f;
		targetCam.rotation = Heart.rotation * Quaternion.Euler(-25, 0, 0);

		cameraScript.RocketSetup(targetCam);
		cameraScript.Heart = OppositeHeart;

		while(snakeManag.BodyNumber > 2)
		{
			snakeManag.BodyNumber = (int)Mathf.MoveTowards(snakeManag.BodyNumber, 0, (bodyNumber/0.25f)*Time.deltaTime);
			yield return null;
		}

		SnakeColl.enabled = false;
		ReadyToFly = true;
		distance = 100.0f;

		Flame.Play();
		cameraScript.Shake(Shaketype.Rocket);

		StartCoroutine(FlightLoop());	//Fonction qui fait voler la fusée
		StartCoroutine(Fly());		//Fonction qui gere ce qui ce passe pendant le vol
	}

	private IEnumerator FlightLoop()
	{
		while(ReadyToFly && distance > 0.05f)
		{
			myTransform.position = Vector3.MoveTowards(myTransform.position, Destination, 50.0f * Time.deltaTime);
			distance = Vector3.Distance(myTransform.position, Destination);
			yield return null;
		}
	}

	private IEnumerator Fly()
	{
		while(distance > 50.0f)
			yield return null;

		planetSetup.MainPlanetSetObjects();
		targetCam.position = Body.position - Heart.up * 7.5f + Heart.forward * 12;
		targetCam.rotation = Heart.rotation * Quaternion.Euler(-160, 0, 0);

		while(distance > 35.0f)
			yield return null;

		StartCoroutine(Barrel());
		StartCoroutine(Roll());
		
		MyPlanet.GetComponent<PlanetScript>().MainSetup(false);
		snakeScript.Heart = OppositeHeart;
		snakeScript.Face = (Faces)(((int)snakeScript.Face + 3) % 6);

		while(distance > 0.5f)
			yield return null;


		ReadyToFly = false;

		Flame.Stop();
		Landing.Play();
		myTransform.position = Destination;
		cameraScript.Shake(Shaketype.Rocket);

		SnakeRend.enabled = true;
		SnakeColl.enabled = true;
			
		Snake_Transform.position = Vector3Extension.RoundToInt(myTransform.position + myTransform.up);
		snakeScript.targetPosition = Snake_Transform.position;
		Snake_Transform.rotation = OppositeHeart.AbsoluteRotation();
		snakeScript.targetRotation = Snake_Transform.rotation;

		bodyRend.enabled = false;

		yield return null;

		SnakeColl.enabled = true;
		snakeManag.BodyNumber = bodyNumber;
			
		gameManager.Rocket = false;
		StartCoroutine(DelayedCleanUp());

		yield return waitforseconds_01;

		snakeScript.State = SnakeState.Waiting;
		snakeScript.upStored = 1;
		snakeScript.rightStored = 0;
	}

	private IEnumerator DelayedCleanUp()
	{
		yield return waitforseconds_05;

		cameraScript.NormalSetup();

		yield return waitforseconds_1;

		myTransform.parent = RocketsPooling;
		myTransform.localPosition = Vector3.zero;
		myTransform.localRotation = Quaternion.identity;

		yield return null;

		bodyRend.enabled = true;

		gameObject.SetActive(false);
	}

	private IEnumerator Barrel()
	{
		float value = 0.0f;
		float reference = 0.0f;
		float target = 2.5f;
		while(value < target - 0.01f && ReadyToFly)
		{
			value = Mathf.SmoothDamp(value, target, ref reference, 0.1f);
			Body.SetLocalPositionX(value);
			yield return null;
		}

		while(value > 0.01f && ReadyToFly)
		{
			value = Mathf.SmoothDamp(value, 0.0f, ref reference, 0.1f);
			Body.SetLocalPositionX(value);
			yield return null;
		}

		Body.SetLocalPositionX(0.0f);
	}

	private IEnumerator Roll()
	{
		while(ReadyToFly)
		{
			Axis.Rotate(Vector3.up * 500.0f * Time.deltaTime);
			yield return null;
		}

		Axis.localRotation = Quaternion.identity;
	}
}