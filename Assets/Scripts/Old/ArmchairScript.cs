using UnityEngine;
// using UnityEngine.PostProcessing;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Tools;

public class ArmchairScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform myBody;
	private Transform Target;
	private Transform targetSnake;
	private Transform SnakeMug;
	private Transform mySnake;
	private GameObject Body;
	private GameObject Candle;
	private GameObject Welcome;
	private GameObject SelectionImage;
	private Transform MugFolder;
	public GameObject MugPrefab;

	private WaitForSeconds waitforseconds_1 = new WaitForSeconds(1.0f);
	private WaitForSeconds waitforseconds_02 = new WaitForSeconds(0.2f);

	[HideInInspector]
	public Vector3[] Positions;

	private RectTransform SelectRect;
	private RectTransform Cursor;

	[HideInInspector]
	public Vector3 initialPosition;
	[HideInInspector]
	public Quaternion initialRotation;
	private Quaternion initialBodyRotation;
	private Quaternion snakeInitialRotation;

	private Image SaveImage;
	private Image Image_2;
	private Text TipText;
	private Text TextComponent;
	private Text SaveText;
	private Text ReturnText;
	private Text BackText;
	private Text WelcomeText;

	private IEnumerator muged_coroutine;

	private Vector2 SelectionScale;

	private Collider SnakeColl;
	private Renderer SnakeRend;
	private Renderer SnakeHoodRend;

	private SkyScript skyScript;
	private CameraScript cameraScript;
	private GameManagerV1 gameManager;
	private SaveScript saveScript;
	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;
	private UnityStandardAssets.ImageEffects.BloomOptimized bloom;
	// private PostProcessingBehaviour postProcess;

	public Faces Face = Faces.FaceY2;

	[HideInInspector]
	public bool Breath = false;
	[HideInInspector]
	public bool AboveHole = false;
	private bool Looping = false;
	private bool Saving = true;
	private bool Return = false;
	private bool MugDone = false;

	private Color32 yellowColor = new Color32(255, 240, 0, 255);
	private Color32 pinkColor = new Color32(255, 115, 190, 255);


	void Awake()
	{
		myTransform = transform;
		myBody = myTransform.Find("Body");
		Target = myTransform.Find("CameraTarget");
		targetSnake = myTransform.Find("SnakeTarget");
		Candle = myBody.Find("Candle").gameObject;
		SnakeMug = myBody.Find("SnakeMug");

		Candle.SetActive(false);

		mySnake = GameObject.FindWithTag("Player").transform;

		SnakeColl = mySnake.GetComponent<Collider>();
		SnakeRend = mySnake.GetComponent<Renderer>();
		SnakeHoodRend = mySnake.Find("Hood").GetComponent<Renderer>();

		snakeScript = mySnake.GetComponent<SnakeControllerV3>();
		snakeManag = mySnake.GetComponent<SnakeManagement>();
		bloom = GameObject.Find("MainCamera/Camera").GetComponent<UnityStandardAssets.ImageEffects.BloomOptimized>();
		// postProcess = GameObject.Find("MainCamera/Camera").GetComponent<PostProcessingBehaviour>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		saveScript = GameObject.Find("LevelManager").GetComponent<SaveScript>();
		skyScript = GameObject.Find("Sky").GetComponent<SkyScript>();

		GameObject.Find("Canvas/InGame/Armchair").SetActive(true);
		SelectionImage = GameObject.Find("Canvas/InGame/Armchair/Selection").gameObject;
		Welcome = GameObject.Find("Canvas/InGame/Armchair/Welcome").gameObject;

		SelectRect = SelectionImage.GetComponent<RectTransform>();
		SaveImage = SelectionImage.GetComponent<Image>();
		Body = SelectionImage.transform.Find("Body").gameObject;
		TipText = Body.transform.Find("Tip").GetComponent<Text>();
		TextComponent = Body.transform.Find("Text").GetComponent<Text>();
		Cursor = SelectionImage.transform.Find("Body/Cursor").GetComponent<RectTransform>();
		Image_2 = SelectionImage.transform.Find("Image2").GetComponent<Image>();
		SaveText = SelectionImage.transform.Find("Body/Choices/Save").GetComponent<Text>();
		ReturnText = SelectionImage.transform.Find("Body/Choices/Return").GetComponent<Text>();
		BackText = SelectionImage.transform.Find("Body/Back").GetComponent<Text>();
		WelcomeText = Welcome.GetComponent<Text>();

		SetupMugFolder();
	}

	void Start()
	{
		initialPosition = myTransform.AbsolutePosition();
		initialRotation = myTransform.AbsoluteRotation();
		initialBodyRotation = TransformExtension.AbsoluteRotation(myBody.localRotation);

		SelectionImage.SetActive(false);
		Image_2.SetColorA(1.0f);
		SelectionScale = SelectRect.sizeDelta;
	}

	private void SetupMugFolder()
	{
		if(myTransform.Find("MugFolder") != null)
			return;

		GameObject obj = new GameObject();
		obj.name = "MugFolder";
		MugFolder = obj.transform;
		MugFolder.parent = myTransform;
		MugFolder.SetAsFirstSibling();
		MugFolder.localPosition = Vector3.zero;
		MugFolder.localRotation = Quaternion.identity;
		MugFolder.localScale = Vector3.one;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			StartCoroutine(EnterSetup(false));
		}
	}

	public IEnumerator EnterSetup(bool fromMenu)
	{
		gameManager.Safe = true;
		snakeManag.Health = SnakeHealth.Alive;

		SnakeColl.enabled = false;
		SnakeRend.enabled = false;
		SnakeHoodRend.enabled = false;

		if(!fromMenu)
		{
			StartCoroutine(Welcoming());

			GameObject Poubelle = GameObject.Find("Poubelle");
			Destroy(Poubelle);
			Poubelle = new GameObject();
			Poubelle.name = "Poubelle";
			Poubelle.transform.position = Vector3.zero;

			System.GC.Collect();

			yield return waitforseconds_1;
		}

		Candle.SetActive(true);

		initialPosition = myTransform.AbsolutePosition();
		initialRotation = myTransform.AbsoluteRotation();
		myTransform.position += myTransform.forward * 2;

		Transform SnakeBody = GameObject.Find("SnakeBody").transform;
		for(int i = 0; i < SnakeBody.childCount; i++)
		{
			Destroy(SnakeBody.GetChild(i).gameObject);
			if(i % 20 == 0)
				yield return null;
		}

		cameraScript.State = CameraState.Moving;
		cameraScript.SafeSetup(Target);
		Welcome.SetActive(false);

		snakeScript.State = SnakeState.Stopped;
		snakeScript.up = 0;
		snakeScript.right = 0;

		Vector3 pos = targetSnake.AbsolutePosition();
		Quaternion rot = gameManager.MainPlanet.Find("Heart").AbsoluteRotation();

		mySnake.position = pos;
		mySnake.rotation = Quaternion.identity;
		snakeScript.targetPosition = pos;
		snakeScript.targetRotation = rot;
		snakeInitialRotation = rot;
		
		SnakeRend.enabled = true;
		SnakeHoodRend.enabled = true;

		yield return null;

		bloom.intensity = 0.4f;
		bloom.blurIterations = 4;
		bloom.blurSize = 5.5f;
		// postProcess.profile.depthOfField.enabled = true;
		// postProcess.profile.colorGrading.enabled = true;

		Breath = true;

		StartCoroutine(SnakeMugingOn());
		StartCoroutine(LevitatingArmchair());
		StartCoroutine(OpenChoice(fromMenu));
		if(gameManager.State != Scenes.Tutoriel)
			StartCoroutine(TimeLapse());

		int length = MugFolder.childCount;
		Transform mug;
		for(int k = 0; k < length; k++)
		{
			mug = MugFolder.GetChild(k);
			mug.GetComponent<MugScript>().StartCoroutine(mug.GetComponent<MugScript>().MugingBreath());
		}
	}

	private IEnumerator TimeLapse()
	{
		float value = skyScript.TimeOfDay;
		float sign = 1.0f;
		while(Breath)
		{
			value += sign * 0.1f * Time.deltaTime;
			skyScript.TimeOfDay = value;
			if(value > 1.0f || value < 0.0f)
				sign = -sign;

			yield return null;
		}
	}

	private IEnumerator SnakeMugingOn()
	{
		MugDone = false;
		Breath = true;

		SnakeMug.parent = null;
		SnakeMug.localScale = new Vector3(0.4f, 0.4f, 0.5f);
		SnakeMug.rotation = myTransform.rotation * Quaternion.Euler(0, 0, 270);
		SnakeMug.position = myTransform.position - 3 * myTransform.right - 2 * myTransform.up + 5 * myTransform.forward;
		SnakeMug.gameObject.SetActive(true);
		yield return null;

		Vector3 targetMug = mySnake.position + myTransform.forward * 0.75f;

		while(Vector3.Distance(SnakeMug.position, targetMug) > 0.05f && Breath)
		{
			SnakeMug.position = Vector3.MoveTowards(SnakeMug.position, targetMug, 3.5f * Time.deltaTime);
			yield return null;
		}

		if(Breath)
		{
			SnakeMug.position = targetMug;
			SnakeMug.parent = mySnake;
			MugDone = true;
		}
	}

	private IEnumerator SnakeMugingOff()
	{
		SnakeMug.parent = myTransform;
		while(SnakeMug.localPosition.z < 5.0f)
		{
			SnakeMug.SetLocalPositionZ(Mathf.MoveTowards(SnakeMug.localPosition.z, 7.0f, 15.0f * Time.deltaTime));
			yield return null;
		}

		SnakeMug.gameObject.SetActive(false);
	}

	private IEnumerator Welcoming()
	{
		WelcomeText.fontSize = 30;
		WelcomeText.SetColorA(0.0f);
		Welcome.SetActive(true);

		float mySpeed = 600.0f;

		while(WelcomeText.fontSize < 299)
		{
			WelcomeText.fontSize = (int)Mathf.MoveTowards(WelcomeText.fontSize, 300.0f, mySpeed * Time.deltaTime);
			WelcomeText.SetColorA(Mathf.MoveTowards(WelcomeText.color.a, 1.0f, 3.5f * Time.deltaTime));
			yield return null;
		}
	}

	private IEnumerator OpenChoice(bool fromMenu)
	{
		SelectionImage.SetActive(true);
		Body.SetActive(false);

		TextComponent.color = pinkColor;
		SaveImage.color = pinkColor;
		TipText.color = pinkColor;
		
		SelectRect.SetSizeDeltaX(100.0f);
		SelectRect.SetSizeDeltaY(0.0f);

		TextComponent.text = "-Looney's Coffee-";

		Image_2.SetColorA(1.0f);

		while(SelectRect.sizeDelta.y != SelectionScale.y)
		{
			SelectRect.SetSizeDeltaY(Mathf.MoveTowards(SelectRect.sizeDelta.y, SelectionScale.y, 2000.0f * Time.deltaTime));
			yield return null;
		}
		while(SelectRect.sizeDelta.x != SelectionScale.x)
		{
			SelectRect.SetSizeDeltaX(Mathf.MoveTowards(SelectRect.sizeDelta.x, SelectionScale.x, 2000.0f * Time.deltaTime));
			yield return null;
		}

		Image_2.SetColorA(0.0f);
		Body.SetActive(true);

		if(!fromMenu)
		{
			StartCoroutine(Choice());
		}
		else
		{
			SaveText.text = System.String.Empty;
			ReturnText.text = System.String.Empty;
			BackText.text = "Go back to the\nharsh reality.";

			Cursor.SetAnchoredPositionX(-220.0f);
			Cursor.SetAnchoredPositionY(20.0f);

			while(!Input.GetKeyDown("return"))
				yield return null;

			SelectionImage.SetActive(false);
			ExitSetup();
		}
	}

	private IEnumerator Choice()
	{
		Looping = true;
		Saving = true;
		Return = false;

		SaveText.color = yellowColor;
		ReturnText.color = pinkColor;

		SaveText.text = "Save";
		ReturnText.text = "Return";
		BackText.text = System.String.Empty;

		Cursor.SetAnchoredPositionX(-210.0f);
		Cursor.SetAnchoredPositionY(-5.0f);
		
		TextComponent.color = pinkColor;
		SaveImage.color = pinkColor;
		TipText.color = pinkColor;

		while(Looping)
		{
			if(Input.GetKeyDown("return"))
			{
				if(Saving && SaveText.text != "Game Saved.")
				{
					saveScript.Save();
					SaveText.text = "Game Saved.";
					ReturnText.text = System.String.Empty;
					SaveImage.color = yellowColor;
					TextComponent.color = yellowColor;
					TipText.color = yellowColor;
					SaveText.color = yellowColor;
					ReturnText.color = yellowColor;
				}
				else if(Return || (Saving && SaveText.text == "Game Saved."))
				{
					SelectionImage.SetActive(false);
					ExitSetup();
					break;
				}
			}

			if(Input.GetAxisRaw("Horizontal") == -1.0f)
			{			
				Saving = true;
				Return = false;

				SaveText.color = yellowColor;
				ReturnText.color = pinkColor;

				Cursor.SetAnchoredPositionX(-210.0f);
			}
			else if(Input.GetAxisRaw("Horizontal") == 1.0f)
			{
				Saving = false;
				Return = true;

				SaveText.color = pinkColor;
				ReturnText.color = yellowColor;
				
				Cursor.SetAnchoredPositionX(13.0f);
			}
			yield return null;
		}

		Looping = false;
	}

	private void ExitSetup()
	{
		Breath = false;

		StartCoroutine(SnakeMugingOff());

		bloom.blurSize = 3.0f;
		bloom.blurIterations = 2;
		// postProcess.profile.depthOfField.enabled = false;
		// postProcess.profile.colorGrading.enabled = false;
	}

	private IEnumerator LevitatingArmchair()
	{
		Vector3 targetPosition = myTransform.position;
		Vector3 center = targetPosition;

		Vector3 myReference = Vector3.zero;
		Vector3 snakeReference = Vector3.zero;
		float snakeOmegaReference = 0.0f;
		float omegaReference = 0.0f;

		float omega = 0.0f;
		float snake_omega = 0.0f;

		while(Breath)
		{
			targetPosition = center + myTransform.forward * (Mathf.PingPong(Time.time * 0.05f, 0.2f) - 0.1f);
			omega = Mathf.SmoothDamp(omega, Mathf.PingPong(Time.time * 2.5f, 10.0f) - 5.0f, ref omegaReference, 0.7f);

			myTransform.position = Vector3.SmoothDamp(myTransform.position, targetPosition, ref myReference, 0.8f);
			myBody.localRotation = Quaternion.Euler(omega, 0.0f, 0.0f);

			if(MugDone)
			{
				snake_omega = Mathf.SmoothDamp(snake_omega, omega, ref snakeOmegaReference, 0.7f);
				mySnake.position = Vector3.SmoothDamp(mySnake.position, targetSnake.position, ref snakeReference, 1.0f);
				mySnake.rotation = Quaternion.Euler(snake_omega, 0, 0);
			}

			yield return null;
		}

		float dist = Vector3.Distance(myTransform.position, initialPosition);
		float diff = dist;
		Quaternion fromRotation = myBody.localRotation;

		while(diff > 0.05f)
		{
			diff = Vector3.Distance(myTransform.position, initialPosition);
			myTransform.position = Vector3.MoveTowards(myTransform.position, initialPosition, 12.0f * Time.deltaTime);
			myBody.localRotation = Quaternion.Slerp(fromRotation, initialBodyRotation, 1.0f - (diff / dist));

			mySnake.position = targetSnake.position;
			yield return null;
		}

		cameraScript.Shake(Shaketype.Gentle);

		mySnake.rotation = snakeInitialRotation;
		myBody.localRotation = initialBodyRotation;

		yield return waitforseconds_02;

		SnakeColl.enabled = true;
		snakeScript.State = SnakeState.Waiting;
		gameManager.Safe = false;

		cameraScript.ExitSafeSetup();
	}

	public void SetHoledFace()
	{
		if(muged_coroutine != null)
			StopCoroutine(muged_coroutine);
		muged_coroutine = SetHoledFaceCoroutine();
		StartCoroutine(muged_coroutine);
	}
	private IEnumerator SetHoledFaceCoroutine()
	{
		SetupMugFolder();

		int nb = Random.Range(20, 50);
		Vector3 Planet = gameManager.MainPlanet.position;
		Positions = new Vector3[nb];
		int pos_index = 0;

		for(int i = 0; i < nb; i++)
		{
			GameObject obj = Instantiate(MugPrefab, Vector3.zero, Quaternion.identity);
			Transform transf = obj.transform;

			obj.name = "Mug";
			transf.parent = MugFolder;
			transf.localScale = new Vector3(0.4f, 0.4f, 0.5f);
			transf.localRotation = Quaternion.identity;
			Vector3 pos;
			do
			{
				int sign = 1 - Random.Range(0, 2) * 2;
				if(Random.Range(0, 2) == 0)
				{
					pos = Planet + myTransform.forward * 15.25f + sign* (Random.Range(3, 11)+0.5f)*myTransform.up + (Random.Range(-10, 11)+0.5f)*myTransform.right;
				}
				else
				{
					pos = Planet + myTransform.forward * 15.25f + (Random.Range(-10, 11)+0.5f)*myTransform.up + sign* (Random.Range(3, 11)+0.5f)*myTransform.right;
				}

				yield return null;
			}
			while(Positions.Contains(pos));
			
			obj.GetComponent<MugScript>().pos_index = pos_index;
			Positions[pos_index ++] = pos;
			transf.position = pos;

			yield return null;
		}

		myTransform.position += myTransform.forward * 2;

		yield return null;

		int length = MugFolder.childCount;
		Transform mug;
		for(int k = 0; k < length; k++)
		{
			mug = MugFolder.GetChild(k);
			mug.GetComponent<MugScript>().StartCoroutine(mug.GetComponent<MugScript>().MugingBreath());
		}

		Vector3 targetPosition = myTransform.position;
		Vector3 center = targetPosition;
		Vector3 myReference = Vector3.zero;

		float omegaReference = 0.0f;
		float omega = 0.0f;

		AboveHole = true;
		while(AboveHole)
		{
			targetPosition = center + myTransform.forward * (Mathf.PingPong(Time.time * 0.05f, 0.5f) - 0.25f);
			omega = Mathf.SmoothDamp(omega, Mathf.PingPong(Time.time * 15.0f, 70.0f) - 35.0f, ref omegaReference, 0.85f);

			myTransform.position = Vector3.SmoothDamp(myTransform.position, targetPosition, ref myReference, 0.8f);
			myBody.SetLocalRotationX(omega);
			myBody.Rotate(Vector3.forward * (Mathf.PingPong(Time.time * 5.0f, 100.0f) - 50.0f) * Time.deltaTime);

			yield return null;
		}

		myTransform.position = initialPosition;
		myBody.localRotation = initialBodyRotation;
	}

	public void SetMugedFace()
	{
		if(muged_coroutine != null)
			StopCoroutine(muged_coroutine);
		muged_coroutine = SetMugedFaceCoroutine();
		StartCoroutine(muged_coroutine);
	}
	private IEnumerator SetMugedFaceCoroutine()
	{
		SetupMugFolder();

		int nb = Random.Range(30, 40);
		Vector3 Planet = gameManager.MainPlanet.position;
		Positions = new Vector3[nb];
		int pos_index = 0;

		for(int i = 0; i < nb; i++)
		{
			GameObject obj = Instantiate(MugPrefab, Vector3.zero, Quaternion.identity);
			Transform transf = obj.transform;

			obj.name = "Mug";
			transf.parent = MugFolder;
			transf.localScale = new Vector3(0.4f, 0.4f, 0.5f);
			transf.localRotation = Quaternion.identity;
			Vector3 pos;
			do
			{
				int sign = 1 - Random.Range(0, 2) * 2;
				if(Random.Range(0, 2) == 0)
				{
					pos = Planet + myTransform.forward * 15.25f + sign*(Random.Range(4, 11)+0.5f)*myTransform.up + (Random.Range(-10, 11)+0.5f)*myTransform.right;
				}
				else
				{
					pos = Planet + myTransform.forward * 15.25f + (Random.Range(-10, 11)+0.5f)*myTransform.up + sign*(Random.Range(4, 11)+0.5f)*myTransform.right;
				}

				yield return null;
			}
			while(Positions.Contains(pos));
			
			obj.GetComponent<MugScript>().pos_index = pos_index;
			Positions[pos_index ++] = pos;
			transf.position = pos;

			yield return null;
		}

		muged_coroutine = null;
	}

	void OnBecameInvisible()
	{
		if(Candle.activeInHierarchy)
			Candle.SetActive(false);
	}

	void OnApplicationQuit()
	{
		// postProcess.profile.depthOfField.enabled = false;
		// postProcess.profile.colorGrading.enabled = false;
	}
}
