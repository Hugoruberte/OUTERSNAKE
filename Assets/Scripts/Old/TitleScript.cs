using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using Tools;

public class TitleScript : MonoBehaviour
{
	private UnityStandardAssets.ImageEffects.SunShafts shaftScript;
	private UnityStandardAssets.ImageEffects.BloomOptimized bloomScript;
	private UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration vignetteScript;
	private UnityStandardAssets.ImageEffects.Fisheye fisheyeScript;
	private CameraScript cameraScript;
	private BunneyArcade bunneyScript;
	private SaveScript saveScript;
	private PerformanceScript perfScript;
	private GameManagerV1 gameManager;
	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;
	

	private WaitForSeconds waitforseconds_025 = new WaitForSeconds(0.25f);
	private WaitForSeconds waitforseconds_05 = new WaitForSeconds(0.5f);
	private WaitForSeconds waitforseconds_02 = new WaitForSeconds(0.2f);
	private WaitForSeconds waitforseconds_003 = new WaitForSeconds(0.03f);

	private Light myLight;

	private IEnumerator buttonScaleCoroutine = null;
	private IEnumerator selectionCoroutine = null;
	private IEnumerator rocketShakeCoroutine = null;

	private ParticleSystem RocketParticleLanding;
	private ParticleSystem RocketParticleFlaming;

	private GameObject Sun;
	private Transform myCamera;
	private Transform myTransform;
	private Transform Letter;
	private Transform B_continue;
	private Transform B_newgame;
	private Transform myRocket;
	private Transform MainPlanet;
	private GameObject ObjectAxis;
	private Transform ObjectAxisTransform;
	private GameObject Life;
	private GameObject Score;
	private Transform Snake;
	private GameObject Effects;

	private Renderer myWhite;
	private Renderer RocketRenderer;

	private RectTransform Check_continue;
	private RectTransform Check_newgame;

	private Image myScreen;
	private Text myText;

	private TextMesh T_continue;
	private TextMesh T_newgame;

	private Image I_white;

	enum TitleChoice
	{
		Continue,
		Newgame,
		None
	};

	private TitleChoice Choice = TitleChoice.None;

	public bool FromIntro = false;
	private bool ExitTitle = false;
	private bool Interaction = false;
	private bool LetterStart = false;
	private bool RocketReachedGame = false;
	private bool CameraReachedGame = false;

	private Color yellow = new Color(1.0f, 0.98f, 0.0f, 1.0f);
	private Color black = new Color(0.1176f, 0.1176f, 0.1176f, 1.0f);

	private Vector3 targetPosition;
	private Vector3 vector = Vector3.zero;
	private Vector3 targetNewgame;
	private Vector3 targetContinue;
	private Vector3 yellowVect = new Vector3(1.0f, 0.98f, 0.0f);
	private Vector3 blackVect = new Vector3(0.1176f, 0.1176f, 0.1176f);

	private float durationToReachGame = 0.8f;


	void Awake()
	{
		myTransform = transform;
		Letter = myTransform.Find("Body/Letter");
		B_continue = myTransform.Find("Body/ButtonContinue");
		B_newgame = myTransform.Find("Body/ButtonNewgame");
		myRocket = myTransform.Find("Rocket");
		T_continue = B_continue.GetChild(0).GetChild(0).GetComponent<TextMesh>();
		T_newgame = B_newgame.GetChild(0).GetChild(0).GetComponent<TextMesh>();
		myScreen = GameObject.Find("Canvas/InGame/Screen").GetComponent<Image>();
		myText = GameObject.Find("Canvas/InGame/Screen/Text").GetComponent<Text>();

		Effects = myTransform.Find("Effects").gameObject;

		Life = GameObject.Find("Canvas/InGame/Life");
		Score = GameObject.Find("Canvas/InGame/Score");
		
		Snake = GameObject.FindWithTag("Player").transform;

		myLight = myTransform.Find("Light").GetComponent<Light>();
		I_white = GameObject.Find("Canvas/Menu/White").GetComponent<Image>();
		myWhite = myTransform.Find("White").GetComponent<Renderer>();

		myCamera = GameObject.Find("MainCamera").transform;

		RocketParticleFlaming = myRocket.Find("Axis/Body/Flaming").GetComponent<ParticleSystem>();
		RocketParticleLanding = myRocket.Find("Landing").GetComponent<ParticleSystem>();
		RocketRenderer = myRocket.Find("Axis/Body").GetComponent<Renderer>();

		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		bunneyScript = GameObject.Find("LevelManager").GetComponent<BunneyArcade>();
		saveScript = GameObject.Find("LevelManager").GetComponent<SaveScript>();
		perfScript = GameObject.Find("LevelManager").GetComponent<PerformanceScript>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		snakeScript = Snake.GetComponent<SnakeControllerV3>();
		snakeManag = Snake.GetComponent<SnakeManagement>();
		
		bloomScript = GameObject.Find("Camera").GetComponent<UnityStandardAssets.ImageEffects.BloomOptimized>();
		shaftScript = GameObject.Find("Camera").GetComponent<UnityStandardAssets.ImageEffects.SunShafts>();
		vignetteScript = GameObject.Find("Camera").GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>();
		fisheyeScript = GameObject.Find("Camera").GetComponent<UnityStandardAssets.ImageEffects.Fisheye>();
	}

	void Start()
	{
		MainPlanet = GameObject.Find("Planets/Planet_01").transform;
		ObjectAxis = new GameObject();
		ObjectAxis.name = "TitleAxis";
		ObjectAxisTransform = ObjectAxis.transform;
		ObjectAxisTransform.position = MainPlanet.position;
		myTransform.parent = ObjectAxisTransform;

		snakeManag.SetRenderer(false);
		snakeManag.BodyNumber = 10;
		snakeScript.State = SnakeState.Stopped;
		snakeManag.Health = SnakeHealth.Alive;

		B_continue.gameObject.SetActive(true);
		B_newgame.gameObject.SetActive(true);

		vignetteScript.enabled = true;
		vignetteScript.chromaticAberration = 0.0f;
		vignetteScript.intensity = 0.1f;

		myLight.intensity = 0.0f;

		I_white.SetColorA(1.0f);
		myWhite.SetColorA(1.0f);

		targetContinue = B_continue.localPosition;
		targetNewgame = B_newgame.localPosition;

		B_continue.localPosition -= Vector3.up * 6f;
		B_newgame.localPosition -= Vector3.up * 6f;

		StartCoroutine(WaitForBegin());
	}

	private IEnumerator WaitForBegin()
	{
		int len = Letter.childCount;
		for(int j = 0; j < len; j++)
			StartCoroutine(SwitchOnLetterColor(Letter.GetChild(j).gameObject, j));

		yield return new WaitUntil(() => perfScript.Done);

		Setup();
	}


	void Update()
	{
		if(ObjectAxis != null)
		{
			myTransform.RotateAround(MainPlanet.position, ObjectAxisTransform.right, 10f * Time.deltaTime);
			ObjectAxisTransform.Rotate(Vector3.up * 5f * Time.deltaTime);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("SpaceObject"))
		{
			other.GetComponent<Renderer>().enabled = false;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("SpaceObject"))
		{
			other.GetComponent<Renderer>().enabled = true;
		}
	}






	//fonctions qui gerent la camera, les boutons, le titre, la lumière et les effets

	private void Setup()
	{
		bloomScript.enabled = true;
		shaftScript.enabled = true;

		bloomScript.intensity = 0.5f;

		shaftScript.maxRadius = 0.6f;
		shaftScript.sunShaftBlurRadius = 2f;
		shaftScript.radialBlurIterations = 2;
		shaftScript.sunShaftIntensity = 0.0f;

		myCamera.parent = myTransform;

		myCamera.localPosition = new Vector3(0, -2.1699f, 11.6099f);
		myCamera.localEulerAngles = new Vector3(-10, 180, 0);
		targetPosition = new Vector3(0, -3.9528f, 10.6481f);
		if(FromIntro)
		{
			myCamera.position -= myCamera.forward * 10.0f;
			targetPosition = new Vector3(0, -1.5f, 9f);
		}
		else
		{
			targetPosition = new Vector3(0, -3.9528f, 10.6481f);
		}

		Material mat;
		Color startEmission = new Color(0, 0, 0, 1);

		for(int i = 0; i < 10; i++)
		{
			mat = Letter.GetChild(i).GetComponent<Renderer>().material;
			mat.SetColor("_EmissionColor", startEmission);
			mat.color = black;
		}

		mat = B_continue.GetChild(0).GetComponent<Renderer>().material;
		mat.SetColor("_EmissionColor", startEmission);
		mat.color = black;

		mat = B_newgame.GetChild(0).GetComponent<Renderer>().material;
		mat.SetColor("_EmissionColor", startEmission);
		mat.color = black;

		T_continue.SetColorA(0.0f);
		T_newgame.SetColorA(0.0f);

		StartCoroutine(Launch());
	}

	private IEnumerator Launch()
	{
		yield return new WaitUntil(() => perfScript.Done);

		if(!FromIntro)
			yield return waitforseconds_025;

		StartCoroutine(CameraPosition());
		rocketShakeCoroutine = RocketShake();
		StartCoroutine(rocketShakeCoroutine);

		float clock = 0.0f;
		while(clock < 2.0f && !Input.anyKeyDown)
		{
			clock += Time.deltaTime;
			yield return null;
		}

		StartCoroutine(LightOn());

		yield return waitforseconds_05;

		LetterStart = true;
	}

	private IEnumerator CameraPosition()
	{
		float smooth = 2.0f;
		float transp = 1.0f;
		bool change = false;

		while((Vector3.Distance(myCamera.localPosition, targetPosition) > 0.05f || transp > -0.01f) && !ExitTitle)
		{
			myCamera.localPosition = Vector3.SmoothDamp(myCamera.localPosition, targetPosition, ref vector, smooth);

			if(!change)
			{
				if(Vector3.Distance(myCamera.localPosition, targetPosition) < 2f)
				{
					if(FromIntro)
						targetPosition = new Vector3(0, -3.9528f, 10.6481f);
					smooth = 1.5f;
					change = true;
					StartCoroutine(ButtonPosition());
				}
				else if(Input.anyKeyDown)
				{
					if(FromIntro)
						targetPosition = new Vector3(0, -3.9528f, 10.6481f);
					smooth = 0.3f;
					change = true;
					StartCoroutine(ButtonPosition());
				}
			}
			else if(Input.anyKeyDown)
			{
				smooth = 0.3f;
			}

			if(transp > -0.01f)
			{
				transp -= 1.0f * Time.deltaTime;
				I_white.SetColorA(transp);
			}
			
			yield return null;
		}
	}

	private IEnumerator ButtonPosition()
	{
		Vector3 referenceB1 = Vector3.zero;
		Vector3 referenceB2 = Vector3.zero;
		float smooth = 1f;

		while(Vector3.Distance(B_continue.localPosition, targetContinue) > 1f && !ExitTitle)
		{
			B_continue.localPosition = Vector3.SmoothDamp(B_continue.localPosition, targetContinue, ref referenceB1, smooth);
			B_newgame.localPosition = Vector3.SmoothDamp(B_newgame.localPosition, targetNewgame, ref referenceB2, smooth);

			if(Input.anyKeyDown)
				smooth = 0.3f;

			yield return null;
		}

		if(!Interaction)
			Interact(true);

		while(Vector3.Distance(B_continue.localPosition, targetContinue) > 0.01f && !ExitTitle)
		{
			B_continue.localPosition = Vector3.SmoothDamp(B_continue.localPosition, targetContinue, ref referenceB1, smooth);
			B_newgame.localPosition = Vector3.SmoothDamp(B_newgame.localPosition, targetNewgame, ref referenceB2, smooth);

			if(Input.anyKeyDown)
				smooth = 0.3f;

			yield return null;
		}
	}

	private IEnumerator ButtonScale(Transform zoom, Transform reduce, float smooth, float pro)
	{
		Vector3 defaultScale = new Vector3(1.5f, 1.0f, 0.3f);
		Vector3 targetScale = defaultScale * 0.8f;
		Vector3 Rscale = Vector3.zero;
		Vector3 Zscale = Vector3.zero;

		if(pro == 0.0f)
		{
			while(Vector3.Distance(zoom.localScale, defaultScale) > 0.01f)
			{
				zoom.localScale = Vector3.SmoothDamp(zoom.localScale, defaultScale, ref Zscale, smooth);
				reduce.localScale = Vector3.SmoothDamp(reduce.localScale, defaultScale, ref Rscale, smooth);
				yield return null;
			}
		}
		else
		{
			while(!(Vector3.Distance(reduce.localScale, targetScale) < 0.01f && Vector3.Distance(zoom.localScale, defaultScale * pro) < 0.01f))
			{
				reduce.localScale = Vector3.SmoothDamp(reduce.localScale, targetScale, ref Rscale, smooth);
				zoom.localScale = Vector3.SmoothDamp(zoom.localScale, defaultScale * pro, ref Zscale, smooth);
				yield return null;
			}
		}
	}

	private IEnumerator LightOn()
	{
		float myVar = 0.0f;
		float jerk = 0f;

		while(myVar < 0.99f)
		{
			myVar = Mathf.SmoothDamp(myVar, 1.0f, ref jerk, 0.25f);
			bloomScript.intensity = 0.5f + myVar * 0.75f;
			myLight.intensity = myVar * 1.5f;
			shaftScript.sunShaftIntensity = myVar * 2.5f;
			yield return null;
		}

		StartCoroutine(SwitchOnButtonColor());

		myVar = 1.0f;
		bloomScript.intensity = 1.25f;
		myLight.intensity = 0.75f;
		shaftScript.sunShaftIntensity = 2.5f;

		while(myVar > 0.01f && !ExitTitle)
		{
			myVar = Mathf.SmoothDamp(myVar, 0.0f, ref jerk, 1.0f);
			bloomScript.intensity = 0.5f + myVar * 0.75f;
			myLight.intensity = 0.5f + myVar * 1.0f;
			shaftScript.sunShaftIntensity = 1.5f + myVar * 0.5f;
			yield return null;
		}

		if(!ExitTitle)
		{
			myVar = 0.0f;
			bloomScript.intensity = 0.5f;
			myLight.intensity = 0.5f;
			shaftScript.sunShaftIntensity = 1.5f;
		}
	}

	private IEnumerator WhiteCoroutine()
	{
		float transp = 1f;
		myWhite.SetColorA(1f);

		while(transp > -0.1f)
		{
			transp -= 1f * Time.deltaTime;
			myWhite.SetColorA(transp);

			yield return null;
		}
	}

	private IEnumerator SwitchOnLetterColor(GameObject obj, int index)
	{
		float emission = 0.0f;
		float reference = 0.0f;
		Vector3 vector = Vector3.zero;
		Vector3 myVect = blackVect;
		Material material = obj.GetComponent<Renderer>().material;

		yield return new WaitUntil(() => LetterStart);

		for(int i = 0; i < index; i++)
			yield return waitforseconds_003;

		obj.layer = 0;

		while(emission < 0.2f)
		{
			emission = Mathf.SmoothDamp(emission, 0.25f, ref reference, 0.15f);
			material.SetColor("_EmissionColor", yellow * emission);

			myVect = Vector3.SmoothDamp(myVect, yellowVect, ref vector, 0.15f);
			material.color = new Color(myVect.x, myVect.y, myVect.z, 1);

			yield return null;
		}
		material.SetColor("_EmissionColor", yellow * 1.0f);
		material.color = yellow;

		while(emission > 0.01f)
		{
			emission = Mathf.SmoothDamp(emission, 0.0f, ref reference, 0.03f);
			material.SetColor("_EmissionColor", yellow * emission);

			myVect = Vector3.SmoothDamp(myVect, blackVect, ref vector, 0.03f);
			material.color = new Color(myVect.x, myVect.y, myVect.z, 1);

			yield return null;
		}
		material.SetColor("_EmissionColor", yellow * 0.0f);
		material.color = black;

		while(emission < 0.2f)
		{
			emission = Mathf.SmoothDamp(emission, 0.25f, ref reference, 0.03f);
			material.SetColor("_EmissionColor", yellow * emission);

			myVect = Vector3.SmoothDamp(myVect, yellowVect, ref vector, 0.03f);
			material.color = new Color(myVect.x, myVect.y, myVect.z, 1);

			yield return null;
		}
		material.SetColor("_EmissionColor", yellow * 1.0f);
		material.color = yellow;
	}

	private IEnumerator SwitchOnButtonColor()
	{
		B_newgame.GetChild(0).gameObject.layer = 0;
		B_continue.GetChild(0).gameObject.layer = 0;

		Material B1 = B_newgame.GetChild(0).GetComponent<Renderer>().material;
		Material B2 = B_continue.GetChild(0).GetComponent<Renderer>().material;

		float emission = 0.0f;
		float reference = 0.0f;
		float colorValue = 0.0f;
		float transp = 1.0f;

		Vector3 myVect = blackVect;
		Vector3 vector = Vector3.zero;
		

		while(emission < 0.2f)
		{
			emission = Mathf.SmoothDamp(emission, 0.25f, ref reference, 0.15f);
			B1.SetColor("_EmissionColor", yellow * emission);
			B2.SetColor("_EmissionColor", yellow * emission);

			myVect = Vector3.SmoothDamp(myVect, yellowVect, ref vector, 0.15f);
			B1.color = new Color(myVect.x, myVect.y, myVect.z, 1);
			B2.color = new Color(myVect.x, myVect.y, myVect.z, 1);

			colorValue = emission * 4f;
			T_newgame.SetColorA(colorValue);
			T_continue.SetColorA(colorValue);

			if(transp > -0.1f)
			{
				transp -= 1f * Time.deltaTime;
				myWhite.SetColorA(transp);
			}
			
			yield return null;
		}

		B1.SetColor("_EmissionColor", yellow * 1.0f);
		B2.SetColor("_EmissionColor", yellow * 1.0f);
		B1.color = yellow;
		B2.color = yellow;
		T_newgame.SetColorA(1.0f);
		T_continue.SetColorA(1.0f);

		while(emission > 0.01f)
		{
			emission = Mathf.SmoothDamp(emission, 0.0f, ref reference, 0.03f);
			B1.SetColor("_EmissionColor", yellow * emission);
			B2.SetColor("_EmissionColor", yellow * emission);

			myVect = Vector3.SmoothDamp(myVect, blackVect, ref vector, 0.03f);
			B1.color = new Color(myVect.x, myVect.y, myVect.z, 1);
			B2.color = new Color(myVect.x, myVect.y, myVect.z, 1);

			colorValue = emission * 4f;
			T_newgame.SetColorA(colorValue);
			T_continue.SetColorA(colorValue);

			if(transp > -0.1f)
			{
				transp -= 1f * Time.deltaTime;
				myWhite.SetColorA(transp);
			}

			yield return null;
		}

		B1.SetColor("_EmissionColor", yellow * 0.0f);
		B2.SetColor("_EmissionColor", yellow * 0.0f);
		B1.color = black;
		B2.color = black;
		T_newgame.SetColorA(0.0f);
		T_continue.SetColorA(0.0f);

		while(emission < 0.2f)
		{
			emission = Mathf.SmoothDamp(emission, 0.25f, ref reference, 0.03f);
			B1.SetColor("_EmissionColor", yellow * emission);
			B2.SetColor("_EmissionColor", yellow * emission);

			myVect = Vector3.SmoothDamp(myVect, yellowVect, ref vector, 0.03f);
			B1.color = new Color(myVect.x, myVect.y, myVect.z, 1);
			B2.color = new Color(myVect.x, myVect.y, myVect.z, 1);

			colorValue = emission * 4f;
			T_newgame.SetColorA(colorValue);
			T_continue.SetColorA(colorValue);

			if(transp > -0.1f)
			{
				transp -= 1f * Time.deltaTime;
				myWhite.SetColorA(transp);
			}

			yield return null;
		}

		B1.SetColor("_EmissionColor", yellow * 1.0f);
		B2.SetColor("_EmissionColor", yellow * 1.0f);
		B1.color = yellow;
		B2.color = yellow;

		T_newgame.SetColorA(1.0f);
		T_continue.SetColorA(1.0f);

		while(transp > 0f)
		{
			transp -= 1f * Time.deltaTime;
			myWhite.SetColorA(transp);
			yield return null;
		}

		myWhite.SetColorA(0.0f);
	}

	private IEnumerator ShaftSetup()
	{
		float intensity = shaftScript.sunShaftIntensity;

		while(intensity > 1.0f)
		{
			intensity = Mathf.MoveTowards(intensity, 1.0f, 1.0f/1.5f * Time.deltaTime);
			shaftScript.sunShaftIntensity = intensity;
			yield return null;
		}

		shaftScript.sunShaftIntensity = 0.75f;
	}

	private IEnumerator RocketShake()
	{
		float current = 0.2f;
		float target_omega = 150f;
		float omega = 0f;
		float speed;
		Vector3 shakePosition;
		Vector3 initial_position = myRocket.localPosition;

		while(!ExitTitle)
		{
			shakePosition = initial_position + Random.insideUnitSphere * current;
			speed = Vector3.Distance(myRocket.localPosition, shakePosition) / 0.05f;
			while(Vector3.Distance(myRocket.localPosition, shakePosition) > 0.01f)
			{
				myRocket.Rotate(Vector3.up * omega * Time.deltaTime);
				omega = target_omega - Mathf.PingPong(Time.time * 10f, target_omega * 2f);
				myRocket.localPosition = Vector3.MoveTowards(myRocket.localPosition, shakePosition, speed * Time.deltaTime);
				yield return null;
			}
		}
	}







	//fonctions outils qui gerent les interactions avec le joueur

	public void Interact(bool action)
	{
		Interaction = action;

		if(selectionCoroutine != null)
			StopCoroutine(selectionCoroutine);

		if(action)
		{
			selectionCoroutine = SelectionCoroutine();
			StartCoroutine(selectionCoroutine);
		}
	}

	private IEnumerator SelectionCoroutine()
	{
		while(!ExitTitle)
		{
			if(Interaction)
			{
				if(Input.GetKeyDown("return"))
				{
					if(Choice != TitleChoice.None)
					{
						Interaction = false;
						cameraScript.Shake(1.0f);
						StartCoroutine(WhiteCoroutine());
						Effects.SetActive(false);

						StopCoroutine(buttonScaleCoroutine);
						buttonScaleCoroutine = ButtonScale(B_continue, B_newgame, 0.09f, 0f);
						StartCoroutine(buttonScaleCoroutine);
						
						yield return waitforseconds_02;

						if(Choice == TitleChoice.Continue)
							Continue();
						else if(Choice == TitleChoice.Newgame)
							NewGame();
					}
				}
				else if(Input.GetAxisRaw("Horizontal") == 1.0f)
				{
					Choice = TitleChoice.Continue;
					if(buttonScaleCoroutine != null)
						StopCoroutine(buttonScaleCoroutine);
					buttonScaleCoroutine = ButtonScale(B_continue, B_newgame, 0.09f, 1.1f);
					StartCoroutine(buttonScaleCoroutine);
				}
				else if(Input.GetAxisRaw("Horizontal") == -1.0f)
				{
					Choice = TitleChoice.Newgame;
					if(buttonScaleCoroutine != null)
						StopCoroutine(buttonScaleCoroutine);
					buttonScaleCoroutine = ButtonScale(B_newgame, B_continue, 0.09f, 1.1f);
					StartCoroutine(buttonScaleCoroutine);
				}
				else if(Input.GetAxisRaw("Vertical") != 0.0f)
				{
					Choice = TitleChoice.None;
					if(buttonScaleCoroutine != null)
						StopCoroutine(buttonScaleCoroutine);
					buttonScaleCoroutine = ButtonScale(B_newgame, B_continue, 0.09f, 0.0f);
					StartCoroutine(buttonScaleCoroutine);
				}
			}

			yield return null;
		}
	}

	private void Continue()
	{
		if(saveScript.playerData.HasSaved)
		{
			shaftScript.sunShaftIntensity = 0.75f;
			ExitTitle = true;
		}
		else
		{
			bunneyScript.StartCoroutine(bunneyScript.NoSaving());
		}
		
		StartCoroutine(ScreenExitSetup());
	}

	private void NewGame()
	{
		ExitTitle = true;

		StartCoroutine(GoToGame());
	}










	// After choice

	private IEnumerator BloomSetup()
	{
		bloomScript.enabled = true;
		bloomScript.intensity = 1.5f;

		while(bloomScript.intensity > 0.41f)
		{
			bloomScript.intensity = Mathf.MoveTowards(bloomScript.intensity, 0.4f, 1.1f/(durationToReachGame+1.0f) * Time.deltaTime);
			yield return null;
		}

		bloomScript.intensity = 0.4f;
	}

	private IEnumerator FisheyeSetup()
	{
		fisheyeScript.enabled = true;
		float value = fisheyeScript.strengthX;

		while(Mathf.Abs(value - 0.025f) > 0.01f)
		{
			value = Mathf.MoveTowards(value, 0.025f, 0.01f * Time.deltaTime);
			fisheyeScript.strengthX = value;
			fisheyeScript.strengthY = value;
			yield return null;
		}
	}

	private IEnumerator VignetteSetup()
	{
		vignetteScript.enabled = true;
		vignetteScript.chromaticAberration = -50.0f;
		float value = vignetteScript.chromaticAberration;
		float chromSpeed = 45.0f/(durationToReachGame + 1.5f);
		float intSpeed = 0.05f/durationToReachGame;

		while((value < -5.0f || Mathf.Abs(vignetteScript.intensity - 0.05f) > 0.01f) && !gameManager.Safe)
		{
			value = Mathf.MoveTowards(value, -5.0f, chromSpeed * Time.deltaTime);
			vignetteScript.chromaticAberration = value;
			vignetteScript.intensity = Mathf.MoveTowards(vignetteScript.intensity, 0.05f, intSpeed * Time.deltaTime);
			yield return null;
		}

		vignetteScript.chromaticAberration = -5.0f;
		vignetteScript.intensity = 0.05f;
	}

	private IEnumerator ScreenExitSetup()
	{
		StartCoroutine(BloomSetup());
		StartCoroutine(VignetteSetup());
		StartCoroutine(FisheyeSetup());
		
		myScreen.color = Color.black;
		myText.fontSize = 20;
		myText.SetColorA(0.0f);
		float value = 0.0f;

		while(Mathf.Abs(value - 1.0f) > 0.01f)
		{
			value = Mathf.MoveTowards(value, 1.0f, 2.0f * Time.deltaTime);
			myText.SetColorA(value);
			myText.fontSize = 20 + (int)(myText.color.a * 230.0f);
			yield return null;
		}

		yield return waitforseconds_025;

		myText.SetColorA(0.0f);
		myScreen.SetColorA(0.0f);
		saveScript.Load();

		Debug.LogWarning("-> Destroy(ObjectAxis);");
	}

	private IEnumerator CameraGotoGame()
	{
		CameraReachedGame = false;

		Vector3 targetPosition = Snake.position - Snake.forward * cameraScript.height;
		Vector3 reference = Vector3.zero;
		Quaternion targetRotation = gameManager.MainPlanet.Find("Heart").rotation;

		while(Vector3.Distance(myCamera.position, targetPosition) > 0.1f && snakeScript.State != SnakeState.Run)
		{
			myCamera.position = Vector3.SmoothDamp(myCamera.position, targetPosition, ref reference, durationToReachGame);
			myCamera.rotation = Quaternion.Slerp(myCamera.rotation, targetRotation, 1.85f * Time.deltaTime);
			yield return null;
		}

		cameraScript.State = CameraState.Moving;
		cameraScript.NormalSetup();

		CameraReachedGame = true;
	}

	private IEnumerator RocketGotoGame()
	{
		RocketReachedGame = false;

		StopCoroutine(rocketShakeCoroutine);

		Vector3 targetRocketPosition = Snake.position;
		myRocket.position = Snake.position - Snake.forward * 90f;
		myRocket.rotation = gameManager.MainPlanet.Find("Heart").rotation * Quaternion.Euler(90,0,0);
		myRocket.localScale = myRocket.localScale * 0.75f;

		while(Vector3.Distance(myRocket.position, targetRocketPosition) > 0.1f)
		{
			myRocket.position = Vector3.MoveTowards(myRocket.position, targetRocketPosition, 65f * Time.deltaTime);
			myRocket.Rotate(Vector3.up * 150f * Time.deltaTime);
			yield return null;
		}

		RocketParticleFlaming.Stop();
		RocketParticleLanding.Play();
		cameraScript.Shake(Shaketype.Rocket);
		RocketRenderer.enabled = false;

		snakeManag.SetRenderer(true);
		snakeScript.State = SnakeState.Waiting;

		Life.SetActive(true);
		Life.GetComponent<Text>().text = gameManager.Lives.ToString();
		Score.SetActive(true);
		Score.GetComponent<Text>().text = "0";

		RocketReachedGame = true;
	}

	private IEnumerator GoToGame()
	{
		myTransform.parent = null;
		Destroy(ObjectAxis);

		myTransform.position = new Vector3(580, 585, 596);
		myTransform.eulerAngles  = new Vector3(230, 90, 90);
		myCamera.parent = null;
		myWhite.transform.parent = myCamera;

		StartCoroutine(CameraGotoGame());
		StartCoroutine(RocketGotoGame());

		//StartCoroutine(EffectSetup());
		StartCoroutine(ShaftSetup());
		StartCoroutine(BloomSetup());
		StartCoroutine(VignetteSetup());
		StartCoroutine(FisheyeSetup());

		yield return new WaitUntil(() => RocketReachedGame && CameraReachedGame);

		float duration = RocketParticleLanding.main.startLifetimeMultiplier + 1f;

		yield return new WaitForSeconds(duration);

		StopAllCoroutines();
		Destroy(myWhite.gameObject);
		Destroy(gameObject);
	}
}