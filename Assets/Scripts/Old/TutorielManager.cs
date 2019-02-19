using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Tools;

public class TutorielManager : MonoBehaviour
{
	private Transform Snake;
	private Transform Arrow;
	private Transform myCamera;
	private Transform myCanvasTransform;
	private GameObject BigRabbit_1;
	private GameObject BigRabbit_2;
	private GameObject Fans;
	public GameObject ShadowCube;
	private GameObject Teleporters;

	private Canvas myCanvas;
	private Image Outerspace;

	public float TimerLength = 10.0f;

	private Renderer Arrowrenderer;
	private Renderer SnakeRend;

	//private Camera CameraComponent;

	private ParticleSystem Fireworks;

	[HideInInspector]
	public int WYRabbitCaught = 0;
	[HideInInspector]
	public int RabbitKilled = 0;
	[HideInInspector]
	public int Repetition = 0;

	[HideInInspector]
	public bool Hint = false;
	[HideInInspector]
	public bool Phase_2 = false;
	[HideInInspector]
	public bool Racedone = false;
	[HideInInspector]
	public bool Suicidal = false;
	[HideInInspector]
	public bool Troll = false;
	[HideInInspector]
	public bool Zoomed = false;

	private RectTransform TimerRect;
	private Text Timer;

	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;
	private CameraScript cameraScript;
	private BunneyTutoriel bunneyScript;
	private PlanetScript planetScript;

	//'Color.orange' = new Color(1, 0.47f, 0, 1)
	private Color[] CtWcolor = new Color[5] {Color.white, Color.cyan, Color.green, Color.yellow, Color.red};

	void Awake()
	{
		Snake = GameObject.FindWithTag("Player").transform;
		Arrow = GameObject.Find("Race/Arrow").transform;
		myCamera = GameObject.Find("MainCamera").transform;
		myCanvasTransform = GameObject.Find("Canvas").transform;
		BigRabbit_1 = GameObject.Find("Race/BigRabbit1");
		BigRabbit_2 = GameObject.Find("Race/BigRabbit2");
		Fans = GameObject.Find("Race/Fans");
		Teleporters = GameObject.Find("Teleporters");

		BigRabbit_1.SetActive(false);
		BigRabbit_2.SetActive(false);

		SnakeRend = Snake.GetComponent<Renderer>();

		myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		Outerspace = GameObject.Find("Canvas/InGame/Outerspace").GetComponent<Image>();

		//CameraComponent = myCamera.Find("Camera").GetComponent<Camera>();

		myCamera.position = new Vector3(400, 526, 500);
		myCamera.rotation = Quaternion.Euler(90, 0, 0);

		TimerRect = GameObject.Find("Canvas/InGame/Timer").GetComponent<RectTransform>();
		Timer = GameObject.Find("Canvas/InGame/Timer").GetComponent<Text>();

		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		bunneyScript = GameObject.Find("LevelManager").GetComponent<BunneyTutoriel>();
		planetScript = GameObject.Find("Planets/Planet_1").GetComponent<PlanetScript>();

		Arrowrenderer = Arrow.GetComponent<Renderer>();
		Arrowrenderer.SetColorA(1.0f);

		Fireworks = GameObject.Find("Race/Fireworks").GetComponent<ParticleSystem>();
	}

	void Start()
	{
		Timer.text = System.String.Empty;
		Arrow.gameObject.SetActive(false);

		cameraScript.State = CameraState.Idle;
		snakeScript.State = SnakeState.Stopped;

		planetScript.Grid[736] = CellEnum.Snake;

		planetScript.Grid[626] = CellEnum.Rabbit;
		planetScript.Grid[846] = CellEnum.Rabbit;
		planetScript.Grid[741] = CellEnum.Rabbit;

		Teleporters.transform.Find("TeleporterA").GetComponent<TeleporterStationScript>().Planet.GetComponent<PlanetScript>().SetOccupedCell(738, 1);
		Teleporters.transform.Find("TeleporterB").GetComponent<TeleporterStationScript>().Planet.GetComponent<PlanetScript>().SetOccupedCell(737, 1);
		Teleporters.SetActive(false);
	}

	public void Run()
	{
		Arrow.gameObject.SetActive(true);
		StartCoroutine(ArrowAspect());
		StartCoroutine(TimerAspect());
		StartCoroutine(TimerColor());
		snakeScript.State = SnakeState.Waiting;
	}

	private IEnumerator ArrowAspect()
	{
		float dist;
		float length = 5.0f;
		float cutoff;

		while(Snake.position.x <= Arrow.position.x + length)
		{
			if(Snake.position.x >= Arrow.position.x)
			{
				dist = Snake.position.x - Arrow.position.x;
				cutoff = (dist * 0.85f/length + 0.16f <= 1.0f) ? dist * 0.85f/length + 0.16f : 1.0f;
				
				Arrowrenderer.material.SetFloat("_Cutoff", cutoff);
				Arrowrenderer.SetColorR(dist * (-0.2157f/length) + 1);
			}

			yield return null;
		}
		Arrowrenderer.material.SetFloat("_Cutoff", 1.0f);
	}

	private IEnumerator TimerAspect()
	{
		float chrono = 0.0f;

		while(!Racedone && chrono < TimerLength)
		{
			Timer.text = chrono.ToString("F2");
			chrono += Time.deltaTime;
			yield return null;
		}

		if(!Racedone)
			TimerRect.rotation = Quaternion.Euler(0, 0, -5);

		while(!Racedone && chrono < 99.99f)
		{
			chrono += Time.deltaTime;
			Timer.text = chrono.ToString("F2");
			yield return null;
		}

		if(!Racedone)
			Timer.text = "99.999999999999999 ur really baaaaaad!!!";

		while(!Racedone)
			yield return null;
			
		Fireworks.Play();

		Fans.SetActive(false);
		BigRabbit_1.SetActive(true);
		BigRabbit_2.SetActive(true);

		Timer.alignment = TextAnchor.MiddleCenter;
		TimerRect.SetAnchoredPositionX(0);
		float floatfont = (float)Timer.fontSize;
		float delay = 1.0f;

		while(!(Timer.fontSize >= 400 && TimerRect.anchoredPosition.y < 125.1f))
		{
			floatfont += (250.0f/delay) * Time.deltaTime;
			Timer.fontSize = (int)floatfont;
			TimerRect.SetAnchoredPositionY(Mathf.MoveTowards(TimerRect.anchoredPosition.y, 125.0f, (175.0f/delay) * Time.deltaTime));
			yield return null;
		}

		if(Hint && chrono <= TimerLength)
			bunneyScript.StartCoroutine(bunneyScript.Rocketing(0));
		else if(Hint && chrono > TimerLength)
			bunneyScript.StartCoroutine(bunneyScript.Rocketing(1));
		else
			bunneyScript.StartCoroutine(bunneyScript.Rocketing(2));
	}

	private IEnumerator TimerColor()
	{
		float chrono = 0.0f;

		float t = 0.0f;
		int index = 0;
		int colorLength = CtWcolor.Length;
		float divided = TimerLength / ((float)(colorLength - 1));

		while(!Racedone && chrono < TimerLength)
		{
			chrono += Time.deltaTime;
			if(t < 1.0f)
			{
				t += Time.deltaTime / divided;
			}
			else
			{
				t = 0.0f;
				index ++;
			}

			Timer.color = Color.Lerp(CtWcolor[index], CtWcolor[index + 1], t);

			yield return null;
		}

		t = 0.0f;

		while(!Racedone && t < 1.0f)
		{
			t = Mathf.MoveTowards(t, 1.0f, 1.0f * Time.deltaTime);
			Timer.color = Color.Lerp(CtWcolor[colorLength - 1], Color.black, t);
			yield return null;
		}
	}

	public IEnumerator Outer(Transform rabbit)
	{
		Renderer rabbitRend = rabbit.Find("Body").GetComponent<Renderer>();

		myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		myCanvas.planeDistance = 5.0f;

		SnakeRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		rabbitRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		GameObject shadow_1 = Instantiate(ShadowCube, Snake.position, Quaternion.identity);
		GameObject shadow_2 = Instantiate(ShadowCube, rabbit.position, Quaternion.identity);

		yield return null;

		Snake.parent = myCanvasTransform;
		rabbit.parent = myCanvasTransform;

		yield return null;

		myCanvas.planeDistance = 2.2f;

		bool move = false;
		while(Outerspace.color.a < 0.995f)
		{
			if(Input.anyKeyDown)
			{
				move = true;
				break;
			}
			Outerspace.SetColorA(Mathf.MoveTowards(Outerspace.color.a, 1.0f, 0.25f * Time.deltaTime));
			yield return null;
		}

		if(!move)
		{
			while(!Input.anyKeyDown)
				yield return null;
		}

		while(Outerspace.color.a > 0.005f)
		{
			Outerspace.SetColorA(Mathf.MoveTowards(Outerspace.color.a, 0.0f, 1.0f * Time.deltaTime));
			yield return null;
		}

		myCanvas.planeDistance = 5.0f;
		Outerspace.SetColorA(0.0f);

		yield return null;

		Snake.parent = null;
		rabbit.parent = null;
		SnakeRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		rabbitRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

		myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

		yield return null;

		Destroy(shadow_1);
		Destroy(shadow_2);
	}

	public IEnumerator OuterRotation()
	{
		Transform OuterTransform = Outerspace.transform;
		float omega = 0.5f;
		while(true)
		{
			OuterTransform.Rotate(Vector3.forward * omega * Time.deltaTime);
			if(omega < 500.0f)
				omega += 0.5f * Time.deltaTime;
			else
				Debug.LogWarning("ça serait bien de mettre un truc marrant ici! Parce que oui, ce jeu est marrant.");
			yield return null;
		}
	}
}