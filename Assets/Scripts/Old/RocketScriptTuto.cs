using UnityEngine;
using System.Collections;
using Tools;

public class RocketScriptTuto : MonoBehaviour
{
	private Transform Planet;
	private Transform myTransform;
	private Transform targetCam;
	private Transform Heart;
	private GameObject Snake_GameObject;
	private Transform Snake_Transform;
	private Transform Body;
	private Transform Axis;
	private GameObject Race;
	private GameObject Lives;
	private Transform myCamera;

	private Collider SnakeColl;
	private Renderer SnakeRend;
	private Renderer bodyRend;

	private ParticleSystem Flame;
	private ParticleSystem Landing;

	private bool ReadyToFly = false;
	private bool GetIn = false;

	private float distance;

	private Vector3 Destination = new Vector3(800, 502, 500);
	private Vector3 Center;
	private Vector3 Axe;

	private GameManagerV1 gameManager;
	private SnakeControllerV3 snakeScript;
	private CameraScript cameraScript;
	private TutorielManager tutoScript;
	private ArmchairScript armchairScript;
	private BunneyTutoriel bunneyScript;
	private PlanetSetup planetSetup;


	void Awake()
	{
		myTransform = transform;
		Axis = myTransform.Find("Axis");
		Body = Axis.Find("Body");

		Planet = GameObject.Find("Planets/Planet_1").transform;
		Heart = Planet.Find("Heart");
		Center = new Vector3(-0.25f + (Planet.position.x + myTransform.position.x)/2.0f, myTransform.position.y, myTransform.position.z);
		Axe = -myTransform.forward;

		Lives = GameObject.Find("Canvas/InGame/Life");
		Race = GameObject.Find("Race");

		Snake_GameObject = GameObject.FindWithTag("Player");
		Snake_Transform = Snake_GameObject.transform;

		myCamera = GameObject.Find("MainCamera").transform;

		snakeScript = Snake_GameObject.GetComponent<SnakeControllerV3>();
		tutoScript = GameObject.Find("LevelManager").GetComponent<TutorielManager>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		armchairScript = GameObject.FindWithTag("Armchair").GetComponent<ArmchairScript>();
		bunneyScript = GameObject.Find("LevelManager").GetComponent<BunneyTutoriel>();
		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();
		
		SnakeColl = Snake_GameObject.GetComponent<Collider>();
		SnakeRend = Snake_GameObject.GetComponent<Renderer>();

		bodyRend = Body.GetComponent<Renderer>();
		bodyRend.enabled = true;

		targetCam = myTransform.Find("targetCam");
		
		Flame = Body.Find("flame").GetComponent<ParticleSystem>();
		Landing = myTransform.Find("landing").GetComponent<ParticleSystem>();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") && !GetIn)
		{
			GetIn = true;
			StartCoroutine(SetupFlight());
		}
	}

	private IEnumerator SetupFlight()
	{
		gameManager.Rocket = true;

		snakeScript.State = SnakeState.Stopped;
		SnakeColl.enabled = false;
		SnakeRend.enabled = false;

		snakeScript.up = 0;
		snakeScript.right = 0;

		Snake_Transform.position = myTransform.position;
		Snake_Transform.rotation = TransformExtension.AbsoluteRotation(Snake_Transform.rotation);

		/*targetCam.localPosition = new Vector3(-1, -0.4f, -0.5f);
		targetCam.localRotation = Quaternion.Euler(-58, 80, 0);*/
		targetCam.localPosition = new Vector3(-2, -0.4f, -1.3f);
		targetCam.localRotation = Quaternion.Euler(-65, 75, 0);

		yield return null;

		cameraScript.TargetSetup(targetCam.position, targetCam.rotation, 0.25f);
		cameraScript.Heart = Heart;

		StartCoroutine(DelayedCamera());

		Flame.Play();

		float clock = 0.0f;
		while(clock < 1.25f)
		{
			cameraScript.Shake(Shaketype.Gentle);
			clock += Time.deltaTime;
			yield return null;
		}

		Snake_GameObject.SetActive(false);
		snakeScript.Heart = Heart;
		snakeScript.Face = Faces.FaceY1;

		Heart.localRotation = Quaternion.Euler(90, 0, 270);

		ReadyToFly = true;
		distance = Vector3.Distance(myTransform.position, Destination);

		yield return new WaitForSeconds(0.15f);

		cameraScript.Shake(Shaketype.Nuclear);

		yield return new WaitForSeconds(0.1f);

		StartCoroutine(FlightLoop());	//Fonction qui fait voler la fusée (mouvement)
		StartCoroutine(Fly());			//Fonction qui gere ce qui ce passe pendant le vol (action)
	}

	private IEnumerator FlightLoop()
	{
		while(ReadyToFly && distance > 0.05f)
		{
			myTransform.RotateAround(Center, Axe, 50.0f * Time.deltaTime);
			distance = Vector3.Distance(myTransform.position, Destination);
			yield return null;
		}
	}

	private IEnumerator Fly()
	{
		StartCoroutine(Roll());

		planetSetup.MainPlanetSetObjectsWithoutArmchair();
		armchairScript.SetMugedFace();

		while(distance > 1.0f)
			yield return null;

		ReadyToFly = false;

		Flame.Stop();
		Landing.Play();
		myTransform.position = Destination;
		cameraScript.Shake(Shaketype.Rocket);

		SnakeRend.enabled = true;
		Snake_GameObject.SetActive(true);
			
		Snake_Transform.position = new Vector3(800, 501, 500);
		snakeScript.targetPosition = Snake_Transform.position;
		Snake_Transform.rotation = TransformExtension.AbsoluteRotation(Heart.rotation);
		snakeScript.targetRotation = Snake_Transform.rotation;

		bodyRend.enabled = false;

		StartCoroutine(LandingCamera());

		if(PlayerPrefs.GetInt("Death") == 1)
			Lives.SetActive(true);

		SnakeColl.enabled = true;
		gameManager.Rocket = false;
	}

	private IEnumerator DelayedCamera()
	{
		yield return new WaitForSeconds(2.3f);

		targetCam.localPosition = new Vector3(0.4f, -0.4f, -4f);
		targetCam.localRotation = Quaternion.Euler(-75, 5, 84.25f);
		cameraScript.RocketSetup(targetCam, 0.15f, 5.0f);
		StartCoroutine(LandingTargetCam());
	}

	private IEnumerator LandingTargetCam()
	{
		Vector3 targetLocalPosition = new Vector3(0, -6.5f, 0);
		Vector3 reference = Vector3.zero;
		Quaternion targetLocalRotation = Quaternion.Euler(270, 0, 90);
		Quaternion fromRotation = targetCam.localRotation;
		float diff = Vector3.Distance(targetCam.localPosition, targetLocalPosition);
		float dist = diff;

		while(dist > 0.02f)
		{
			targetCam.localPosition = Vector3.SmoothDamp(targetCam.localPosition, targetLocalPosition, ref reference, 1.1f);
			dist = Vector3.Distance(targetCam.localPosition, targetLocalPosition);
			targetCam.localRotation = Quaternion.Slerp(fromRotation, targetLocalRotation, 1.0f - (dist/diff));
			yield return null;
		}

		targetCam.localPosition = targetLocalPosition;
		targetCam.localRotation = targetLocalRotation;
	}

	private IEnumerator LandingCamera()
	{
		yield return new WaitForSeconds(1.5f);

		Vector3 targetPlanet = new Vector3(800, 501, 500) - Heart.forward * cameraScript.height;
		cameraScript.TargetSetup(targetPlanet, Heart.rotation, 0.3f);

		while(Vector3.Distance(myCamera.position, targetPlanet) > 0.05f)
			yield return null;

		cameraScript.NormalSetup();
		bunneyScript.StartCoroutine(bunneyScript.Playground());
		tutoScript.Phase_2 = true;
		Destroy(Race);
	}

	private IEnumerator Roll()
	{
		while(ReadyToFly)
		{
			Axis.Rotate(Vector3.up * 250.0f * Time.deltaTime);
			yield return null;
		}

		Axis.localRotation = Quaternion.identity;
	}
}