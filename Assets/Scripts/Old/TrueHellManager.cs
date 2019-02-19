using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Tools;

public class TrueHellManager : MonoBehaviour
{
	private Transform PlanetA;
	private Transform Heart;
	private Transform Snake;

	private Camera myCam;

	private Canvas myCanvas;
	private Image myCut;

	private WaitForSeconds waitforseconds_025 = new WaitForSeconds(0.25f);

	private UnityStandardAssets.ImageEffects.NoiseAndScratches noiseScript;
	private GlitchEffect glitchScript;
	private SnakeControllerV3 snakeScript;
	private CameraScript cameraScript;
	private BunneyTrueHell bunneyScript;
	private GameManagerV1 gameManager;
	private PlanetSetup planetSetup;

	public int HellCounter = 0;


	void Awake()
	{
		Debug.LogWarning("Il faut dépoussierer <SnakeControllerV3>, <SnakeManagement> et <GameManagerV1> des allusions à Scenes.Hell en créant des 'plugins scripts' comme <SnakeTutoriel>, etc... !");
		gameManager = GetComponent<GameManagerV1>();
		bunneyScript = GetComponent<BunneyTrueHell>();
		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();

		PlanetA = gameManager.MainPlanet;
		planetSetup.MainPlanetSetObjects();
		Heart = PlanetA.Find("Heart");
		Snake = GameObject.FindWithTag("Player").transform;

		myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

		myCut = myCanvas.transform.Find("InGame/Cut").GetComponent<Image>();
		myCam = GameObject.Find("MainCamera/Camera").GetComponent<Camera>();

		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		noiseScript = myCam.GetComponent<UnityStandardAssets.ImageEffects.NoiseAndScratches>();
		glitchScript = myCam.GetComponent<GlitchEffect>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();

		Transform TreesA = GameObject.Find("Trees(A)Pooling").transform;
		Transform TreesB = GameObject.Find("Trees(B)Pooling").transform;

		int i = 0;

		for(i = 0; i < TreesA.childCount; i++)
		{
			TreesA.GetChild(i).GetComponent<Renderer>().material.color = new Color(0.8824f, 0.4118f, 0.0f, 1.0f);
		}
		for(i = 0; i < TreesB.childCount; i++)
		{
			TreesB.GetChild(i).GetComponent<Renderer>().material.color = new Color(0.8824f, 0.4118f, 0.0f, 1.0f);
		}

		HellCounter = 0;
	}

	void Start()
	{
		Quaternion rotation = Quaternion.identity * Quaternion.Euler(90, 0, Random.Range(0, 4) * 90);
		Heart.rotation = rotation;
		myCam.transform.parent.rotation = rotation;
		Snake.rotation = rotation;
		snakeScript.targetRotation = rotation;

		StartCoroutine(Launch());
	}

	public IEnumerator Launch()
	{
		HellCounter ++;

		myCut.SetColorA(1.0f);

		snakeScript.upStored = 1;
		snakeScript.rightStored = 0;
		snakeScript.State = SnakeState.Waiting;

		cameraScript.NormalSetup(0.3f, 2.0f);

		StartCoroutine(Sky());
		StartCoroutine(Noise());
		StartCoroutine(Glitch());

		yield return waitforseconds_025;

		//snakeCreator.ActivateCreator();
		snakeScript.State = SnakeState.Run;

		myCut.SetColorA(0.0f);
		myCanvas.renderMode = RenderMode.ScreenSpaceCamera;

		if(HellCounter == 4)
			bunneyScript.StartCoroutine(bunneyScript.Welcome());
		else if(HellCounter == 15)
			bunneyScript.StartCoroutine(bunneyScript.Escape());
	}

	private IEnumerator Sky()
	{
		float clock = 0.0f;
		while(true)
		{
			clock += Time.deltaTime;
			if(clock > Random.Range(0.1f, 0.25f))
			{
				myCam.backgroundColor = new Color(Random.Range(100.0f, 150.0f)/255.0f, myCam.backgroundColor.g, myCam.backgroundColor.b, 1.0f);
				clock = 0.0f;
			}
			yield return null;
		}
	}

	private IEnumerator Noise()
	{
		float clock = 0.0f;
		int choice;
		float time = Random.Range(0.5f, 2.0f);
		while(true)
		{
			clock += Time.deltaTime;
			if(clock > time)
			{
				choice = Random.Range(0, 6);

				if(choice < 3)
					noiseScript.grainSize = 0.0f;
				else if(choice < 5)
					noiseScript.grainSize = 0.75f;
				else
					noiseScript.grainSize = 1.5f;

				clock = 0.0f;
				time = Random.Range(0.5f, 2.0f);
			}
			yield return null;
		}
	}

	private IEnumerator Glitch()
	{
		float clock = 0.0f;
		int choice;
		float time = Random.Range(0.5f, 2.0f);
		while(true)
		{
			clock += Time.deltaTime;
			if(clock > time)
			{
				choice = Random.Range(0, 3);
				if(choice == 0)
				{
					glitchScript.enabled = true;
					time = 0.75f;
				}
				else
				{
					glitchScript.enabled = false;
					time = Random.Range(2.0f, 5.0f);
				}

				clock = 0.0f;
			}
			yield return null;
		}
	}
}