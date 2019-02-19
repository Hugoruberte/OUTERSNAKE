using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using Tools;

public enum Shaketype
{
	Gentle,
	Rocket,
	Bomb,
	Nuclear
};

public enum Faces
{
	FaceX1,
	FaceY1,
	FaceZ1,
	FaceX2,
	FaceY2,
	FaceZ2,
	None
};

public enum Scenes
{
	Arcade,
	Hell,
	Tutoriel,
	Demo,
	Garage
};

public enum Killer
{
	Yourself = 0,
	Tree,
	Saw,
	Lazer,
	RedRabbit,
	SuicidalRabbit,
	NuclearBomb,
	Hole,
	CasterBlaster,
	Meteore,
	SuperLazer,
	Lack,
	Dung,
	Rotten,
	Fire,
	Barrier,
	Obstacle,
	OccupiedGate,
	Nobody
};

public enum CellEnum
{
	Empty = 0,
	Apple,
	AppleBurn,
	Lazer,
	Rocket,
	Burrow,
	Rabbit,
	Snake,
	NuclearSwitch,
	Hole,
	Tree,
	Trunk,
	Occuped,
	Mechanism
};

public class GameManagerV1 : MonoBehaviour
{
	[Header("Main Planet")]
	public Transform MainPlanet;
	[HideInInspector]
	public Transform OldPlanet;

	private Image myScreen;
	private Image myBlackScreen;
	private Canvas myCanvas;

	private Text liveText;
	private GameObject pausetext;

	private GameObject SnakeGameObject;
	private Transform SnakeTransform;
	private Transform myCamera;
	private Transform ListenerTransform;
	private Transform ListenerTarget;

	private GameObject Poubelle;

	private Renderer SnakeRend;
	private Renderer SnakeHood;
	private Collider SnakeColl;

	private Vector3 FirstPosition;

	[HideInInspector]
	public PlanetScript planetScript;
	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;
	private CameraScript cameraScript;
	private GlitchEffect glitchScript;
	private UnityStandardAssets.ImageEffects.NoiseAndScratches noiseScript;
	private BunneyArcade bunneyScript_Arcade;
	private BunneyTrueHell bunneyScript_Hell;
	private BunneyTutoriel bunneyScript_Tutoriel;
	private SaveScript saveScript;
	private DeathScript deathScript;
	private TrueHellManager trueHellManager;
	private SkyScript skyScript;
	private PlanetSetup planetSetup;
	private DialogScript dialogScript;

	private WaitForSeconds waitforseconds_hell = new WaitForSeconds(0.25f);
	private WaitForSeconds waitforseconds_liveley = new WaitForSeconds(1.0f);
	private WaitForSeconds waitforseconds_gameover = new WaitForSeconds(0.5f);

	[Header("Status")]
	[Range(0, 10)]
	public int Lives = 10;
	
	public Scenes State = Scenes.Arcade;
	[HideInInspector]
	public bool Paused = false;
	[HideInInspector]
	public bool GameOver = false;
	[HideInInspector]
	public bool Rocket = false;
	[HideInInspector]
	public bool Safe = false;
	[HideInInspector]
	public bool WeirdKilledByNuclear = false;

	private bool ThePlayerHasMoved = false;


	private Color32[] SnakeInvinsibleColor = new Color32[2];
	private readonly Color32 SnakeBasicColor = new Color32(60, 60, 60, 255);

	[Header("Death")]
	public Killer KilledBy = Killer.Nobody;
	private readonly string[] KilledByString = new string[] {"yourself", "a $#&@*!# tree", "a freaking saw", "a lazer", "a redrabbit", "a kamikaze rabbit", "a nuclear bomb", "some kind of radiation", "a casterblaster", "a meteore", "a superlazer", "a lack of vital organ", "an indigestion of square-shaped rabbit's dung", "a rotten apple", "a third-degree burn", "having your face burnt by a force field", "an obstacle (a hard one)", "yourself inside a gate", "SOMETHING UNKWNOW"};

	private float BlinkDelay = 0.5f;
	private int DidNotMove = 0;

	[Header("World Setting")]
	public Setting WorldSetting;


		
	void Awake()
	{
		Poubelle = new GameObject();
		Poubelle.name = "Poubelle";
		Poubelle.transform.position = Vector3.zero;

		myCamera = GameObject.Find("MainCamera").transform;
		cameraScript = myCamera.GetComponent<CameraScript>();
		
		myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

		ListenerTarget = transform;
		
		switch(State)
		{
			case Scenes.Tutoriel:
			case Scenes.Arcade:
			case Scenes.Demo:
				ArcadeAwake();
				deathScript = GetComponent<DeathScript>();
				break;

			case Scenes.Hell:
				myBlackScreen = GameObject.Find("Canvas/InGame/Cut").GetComponent<Image>();
				break;
		}
		
		ListenerTransform = transform.Find("Listener");
	}

	private void ArcadeAwake()
	{
		SnakeGameObject = GameObject.FindWithTag("Player");
		SnakeTransform = SnakeGameObject.transform;
		SnakeColl = SnakeGameObject.GetComponent<Collider>();
		SnakeRend = SnakeGameObject.GetComponent<Renderer>();
		SnakeHood = SnakeTransform.Find("Hood").GetComponent<Renderer>();
		FirstPosition = SnakeTransform.AbsolutePosition();

		ListenerTarget = SnakeTransform;

		if(MainPlanet == null)
		{
			Transform Planets = GameObject.Find("Planets").transform;
			Transform child;
			int length = Planets.childCount;
			for(int i = 0; i < length; i++)
			{
				child = Planets.GetChild(i);
				if(child.GetComponent<PlanetScript>().MainPlanet)
				{
					MainPlanet = child;
					OldPlanet = child;
				}
			}
		}

		saveScript = GetComponent<SaveScript>();
		snakeScript = SnakeGameObject.GetComponent<SnakeControllerV3>();
		snakeManag = SnakeGameObject.GetComponent<SnakeManagement>();
		glitchScript = myCamera.Find("Camera").GetComponent<GlitchEffect>();
		noiseScript = myCamera.Find("Camera").GetComponent<UnityStandardAssets.ImageEffects.NoiseAndScratches>();
		bunneyScript_Arcade = GetComponent<BunneyArcade>();
		bunneyScript_Hell = GetComponent<BunneyTrueHell>();
		bunneyScript_Tutoriel = GetComponent<BunneyTutoriel>();
		trueHellManager = GetComponent<TrueHellManager>();
		planetScript = MainPlanet.GetComponent<PlanetScript>();
		skyScript = GameObject.Find("Sky").GetComponent<SkyScript>();
		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();
		dialogScript = GameObject.Find("Canvas/Dialog").GetComponent<DialogScript>();

		myScreen = GameObject.Find("Canvas/InGame/Screen").GetComponent<Image>();

		GameObject.Find("Canvas/Menu").gameObject.SetActive(true);

		liveText = GameObject.Find("Canvas/InGame/Life").GetComponent<Text>();

		pausetext = GameObject.Find("Canvas/InGame/SubPause").gameObject;
		pausetext.SetActive(false);

		DidNotMove = 0;
	}

	/*void Start()
	{
		int current = ((System.DateTime.Now.DayOfYear - 1) * 24 + System.DateTime.Now.Hour) * 3600 + System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second;
	}*/

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F4))
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
		else if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		else if(Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
		{
			ThePlayerHasMoved = true;
		}
		else if(Input.GetKeyDown("space"))
		{
			PauseSetup();
		}

		ListenerTransform.position = ListenerTarget.position;
	}

	public void SetMainPlanet(Transform main)
	{
		OldPlanet = MainPlanet;
		MainPlanet = main;
		planetScript = MainPlanet.GetComponent<PlanetScript>();
	}
	
	public void PlayLiveley()
	{
		if(KilledBy != Killer.NuclearBomb && !WeirdKilledByNuclear)
		{
			StartCoroutine(NeoSnake());

			KilledBy = Killer.Nobody;
			Lives --;

			saveScript.playerData.Lives = Lives;
			liveText.text = Lives.ToString();
		}
		else
		{
			StartCoroutine(CannotPlayLiveley());
		}
	}

	private IEnumerator CannotPlayLiveley()
	{
		glitchScript.intensity = 2.0f;
		noiseScript.grainIntensityMin = 3.0f;
		noiseScript.grainIntensityMax = 5.0f;

		yield return waitforseconds_liveley;

		glitchScript.intensity = 1.0f;
		noiseScript.grainIntensityMin = 1.0f;
		noiseScript.grainIntensityMax = 3.0f;

		if(saveScript.gameData.nuclearContinue)
			deathScript.Interact();
		else
			bunneyScript_Arcade.StartCoroutine(bunneyScript_Arcade.NuclearContinue());
			
	}

	public void PlayLooney()
	{
		WeirdKilledByNuclear = false;
		glitchScript.enabled = false;
		noiseScript.enabled = false;
		cameraScript.NuclearEffectSetup(false);
		
		saveScript.Load();
	}

	public IEnumerator Blink()
	{
		myScreen.color = new Color(1, 0, 0, 0.7f);

		while(myScreen.color.a > 0.0f)
		{
			myScreen.SetColorA(Mathf.MoveTowards(myScreen.color.a, -0.1f, (0.7f/BlinkDelay) * Time.deltaTime));
			yield return null;
		}
	}

	public string KilledByConvertor()
	{
		return KilledByString[(int)KilledBy];
	}

	private void PauseSetup()
	{
		switch(State)
		{
			case Scenes.Hell:
				bunneyScript_Hell.StartCoroutine(bunneyScript_Hell.NoPause());
				break;

			default:
				if(snakeScript.State == SnakeState.Run && !Rocket && !GameOver && !dialogScript.Active)
				{
					Paused = !Paused;

					if(Paused)
					{
						Destroy(Poubelle);
						Poubelle = new GameObject();
						Poubelle.name = "Poubelle";
						Poubelle.transform.position = Vector3.zero;

						System.GC.Collect();
					}

					Time.timeScale = (Paused) ? 0.0001f : 1f;
					pausetext.SetActive(Paused);
					AudioListener.pause = Paused;
				}
				break;
		}
	}

	public IEnumerator GameOverSetup()
	{
		if(Poubelle && Poubelle.transform.childCount != 0)
		{
			Destroy(Poubelle);
			Poubelle = new GameObject();
			Poubelle.name = "Poubelle";
			Poubelle.transform.position = Vector3.zero;

			System.GC.Collect();
		}

		switch(State)
		{
			case Scenes.Hell:
				StartCoroutine(TrueHellReboot());
				break;

			case Scenes.Demo:
			case Scenes.Arcade:
				if(Lives > 0)
				{
					yield return waitforseconds_gameover;
					deathScript.Interact();
				}
				else
				{
					Debug.LogError("Ici ce qui se passe quand on a plus de vie !");
					yield return waitforseconds_gameover;
					deathScript.Interact();
				}
				break;

			/*case Scenes.Arcade:
				if(saveScript.gameData.death)
				{
					if(Lives > 0)
					{
						yield return waitforseconds_gameover;
						deathScript.Interact();
					}
					else
					{
						Debug.LogError("Ici ce qui se passe quand on a plus de vie !");
						yield return waitforseconds_gameover;
						deathScript.Interact();
					}
				}
				else
				{
					bunneyScript_Arcade.StartCoroutine(bunneyScript_Arcade.Death());
				}
				break;*/

			case Scenes.Tutoriel:
				if(PlayerPrefs.GetInt("Death") == 1)
				{
					if(Lives > 0)
					{
						yield return waitforseconds_gameover;
						deathScript.Interact();
					}
					else
					{
						Debug.LogError("Ce qui se passe ici quand on a plus de vie !");
					}
				}
				else
				{
					bunneyScript_Tutoriel.StartCoroutine(bunneyScript_Tutoriel.Death());
				}
				break;

			default:
				Debug.LogError("La mort n'est pas encore gérée dans cette scene !");
				break;
		}
	}



	private IEnumerator NeoSnake()	//fonction qui permet de ramener snake à la vie, fringuant comme un pinçon, et de relancer la partie
	{
		glitchScript.enabled = false;
		noiseScript.enabled = false;
		liveText.gameObject.SetActive(true);

		snakeScript.up = 0;
		snakeScript.right = 0;
		snakeScript.State = SnakeState.Stopped;
		
		Transform heart = MainPlanet.Find("Heart");
		
		SnakeTransform.rotation = heart.rotation;
		SnakeTransform.position = SnakeTransform.AbsolutePosition();
		snakeScript.targetRotation = SnakeTransform.rotation;
		snakeScript.targetPosition = SnakeTransform.position;

		SnakeRend.enabled = true;
		SnakeHood.enabled = true;
		SnakeColl.enabled = true;
		SnakeGameObject.SetActive(true);
		
		yield return null;
		
		snakeScript.State = SnakeState.Waiting;
		snakeManag.BodyNumber = 10;

		StartCoroutine(InvincibleState());
		cameraScript.NuclearEffectSetup(false);

		GameOver = false;
	}

	public IEnumerator InvincibleState()
	{
		snakeManag.Health = SnakeHealth.Invincible;

		snakeManag.StartCoroutine(snakeManag.HoodInvinsibleSetup());

		byte max = (byte)(75 * skyScript.TimeOfDay + 180);	//255 en plein jour
		byte min = (byte)(75 * skyScript.TimeOfDay + 90);	//165 en plein jour
		SnakeInvinsibleColor[0] = new Color32(min, 0, 0, 255);
		SnakeInvinsibleColor[1] = new Color32(max, 0, 0, 255);

		float clock = 0.0f;
		float t = 0.0f;
		float length = WorldSetting.InvulnerableDelay;
		float freq = 5.0f;
		int index = 0;

		while(Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
		{
			if(t < 1.0f)
			{
				t += Time.deltaTime * freq;
			}
			else
			{
				t = 0.0f;
				index = (index + 1) % 2;
			}

			SnakeRend.SetFlatColor(Color.Lerp(SnakeInvinsibleColor[index], SnakeInvinsibleColor[(index + 1) % 2], t));

			yield return null;
		}
		while(clock < length && snakeManag.Health == SnakeHealth.Invincible)
		{
			clock += Time.deltaTime;

			if(t < 1.0f)
			{
				t += Time.deltaTime * freq;
			}
			else
			{
				t = 0.0f;
				index = (index + 1) % 2;
			}

			SnakeRend.SetFlatColor(Color.Lerp(SnakeInvinsibleColor[index], SnakeInvinsibleColor[(index + 1) % 2], t));

			yield return null;
		}
		Color SnakeTempColor = SnakeRend.GetFlatColor();
		t = 0f;
		while(t < 0.99f)
		{
			SnakeRend.SetFlatColor(Color.Lerp(SnakeTempColor, SnakeBasicColor, t));
			t += Time.deltaTime * freq;
			yield return null;
		}

		SnakeRend.SetFlatColor(SnakeBasicColor);

		yield return null;

		snakeManag.Health = SnakeHealth.Alive;
	}

	private IEnumerator TrueHellReboot()
	{
		Debug.LogWarning("A mettre dans un script TrueHellManager !");
		Input.ResetInputAxes();
		yield return waitforseconds_hell;
		
		myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

		yield return null;

		myBlackScreen.SetColorA(1.0f);

		yield return null;

		if(!ThePlayerHasMoved && trueHellManager.HellCounter > 15)
		{
			if(++ DidNotMove < 4)
			{
				TrueHellBlackOut();
			}
			else
			{
				Debug.Log("Escaped !");
				DidNotMove = 0;
				SceneManager.LoadScene("Arcade");
			}

			ThePlayerHasMoved = false;
		}
		else
		{
			ThePlayerHasMoved = false;
			DidNotMove = 0;
			TrueHellBlackOut();
		}
	}

	private void TrueHellBlackOut()
	{
		Debug.LogWarning("A mettre dans un script TrueHellManager !");
		StartCoroutine(CancelInput());

		Quaternion rotation;
		Transform Heart = MainPlanet.Find("Heart");

		snakeScript.up = 0;
		snakeScript.right = 0;
		snakeScript.State = SnakeState.Waiting;

		planetSetup.RepairPlanetFaces();
		
		rotation = Quaternion.Euler(90, 0, 0) * Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 4) * 90);
		Heart.rotation = rotation;
		SnakeTransform.rotation = rotation;
		snakeScript.targetRotation = rotation;

		SnakeTransform.position = FirstPosition;
		snakeScript.targetPosition = FirstPosition;
		myCamera.rotation = rotation;
		snakeScript.Face = Faces.FaceY1;


		SnakeRend.enabled = true;
		SnakeHood.enabled = true;
		SnakeColl.enabled = true;
		SnakeGameObject.SetActive(true);
		
		snakeManag.Health = SnakeHealth.Alive;
		snakeManag.BodyNumber = 10;

		cameraScript.NuclearEffectSetup(false);

		GameOver = false;

		trueHellManager.StartCoroutine(trueHellManager.Launch());
	}

	private IEnumerator CancelInput()
	{
		Debug.LogWarning("A mettre dans un script TrueHellManager !");
		while(snakeScript.State != SnakeState.Run)
		{
			Input.ResetInputAxes();
			yield return null;
		}
	}
}
