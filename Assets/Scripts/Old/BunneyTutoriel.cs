using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using Tools;

public class BunneyTutoriel : MonoBehaviour
{
	[Header("Text")]
	public TextAsset myText;
	[Header("Sprite")]
	public Sprite BunneySprite;
	[Header("Other")]

	public GameObject WYRabbitPrefab;

	private Transform myCamera;
	private Transform Snake;
	private Transform Heart;
	private GameObject Rocket;
	private GameObject Timer;
	private GameObject Lives;
	private GameObject Teleporters;

	private Image Outerspace;

	private DialogScript dialogScript;
	private CameraScript cameraScript;
	private TutorielManager tutoManager;
	private SnakeControllerV3 snakeScript;
	private DeathScript deathScript;
	private SaveScript saveScript;

	public bool SkipPresentation = false;

	private float reference = 0.0f;
	private float ApplePosition;
	private float LazerPosition;
	private float SawPosition;
	private float FinishPosition;

	void Awake()
	{
		dialogScript = GameObject.Find("Canvas/Dialog").GetComponent<DialogScript>();
		dialogScript.myText = myText;

		myCamera = GameObject.Find("MainCamera").transform;
		Snake = GameObject.FindWithTag("Player").transform;
		Heart = GameObject.Find("Planets/Planet_1/Heart").transform;
		Rocket = GameObject.Find("Race/Rocket");
		Timer = GameObject.Find("Canvas/InGame/Timer");
		Lives = GameObject.Find("Canvas/InGame/Life");
		Teleporters = GameObject.Find("Teleporters");

		Outerspace = GameObject.Find("Canvas/InGame/Outerspace").GetComponent<Image>();
		Rocket.SetActive(false);

		ApplePosition = GameObject.Find("Race/Apples").transform.position.x;
		LazerPosition = GameObject.Find("Race/Lazers").transform.position.x;
		SawPosition = GameObject.Find("Race/Saws").transform.position.x;
		FinishPosition = GameObject.Find("Race/Finish").transform.position.x;

		cameraScript = myCamera.GetComponent<CameraScript>();
		tutoManager = GameObject.Find("LevelManager").GetComponent<TutorielManager>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		deathScript = GetComponent<DeathScript>();
		saveScript = GetComponent<SaveScript>();
	}

	void Start()
	{
		StartCoroutine(Opening());
	}

	private IEnumerator Opening()
	{
		if(!SkipPresentation)
		{
			yield return null;
			dialogScript.Body(Styles.Undertale);
			dialogScript.CharacterSprite = BunneySprite;
			dialogScript.Open();
			yield return new WaitForSeconds(0.2f);
			dialogScript.GoTo(1);
			dialogScript.Read(3, true);

			while(dialogScript.NbLineRead != 3)	//apple
				yield return null;

			dialogScript.Read(1, true);
			while(myCamera.position.x < ApplePosition - 0.1f)
			{
				myCamera.SetPositionX(Mathf.SmoothDamp(myCamera.position.x, ApplePosition, ref reference, 0.25f));
				yield return null;
			}

			while(dialogScript.NbLineRead != 1)	//lazer
				yield return null;

			dialogScript.Read(1, true);
			while(myCamera.position.x < LazerPosition - 0.1f)
			{
				myCamera.SetPositionX(Mathf.SmoothDamp(myCamera.position.x, LazerPosition, ref reference, 0.25f));
				yield return null;
			}

			while(dialogScript.NbLineRead != 1)	//saw
				yield return null;

			dialogScript.Read(1, true);
			while(myCamera.position.x < SawPosition - 0.1f)
			{
				myCamera.SetPositionX(Mathf.SmoothDamp(myCamera.position.x, SawPosition, ref reference, 0.25f));
				yield return null;
			}
			
			while(dialogScript.NbLineRead != 1) //finish
				yield return null;

			dialogScript.Read(1, true);
			while(myCamera.position.x < FinishPosition - 0.1f)
			{
				myCamera.SetPositionX(Mathf.SmoothDamp(myCamera.position.x, FinishPosition, ref reference, 0.25f));
				yield return null;
			}

			while(dialogScript.NbLineRead != 1)
				yield return null;
		}

		yield return null;

		cameraScript.State = CameraState.Moving;
		cameraScript.NormalSetup();

		if(!SkipPresentation)
		{
			dialogScript.Read(3, 0.2f, 130);
			dialogScript.Close(3);

			while(dialogScript.Active)
				yield return null;
		}

		tutoManager.Run();
	}

	public IEnumerator Rocketing(int state)
	{
		dialogScript.Open();
		dialogScript.GoTo(state + 2);	//2, 3 ou 4
		dialogScript.Read(2, true);

		while(dialogScript.NbLineRead != 2)
			yield return null;

		Rocket.SetActive(true);

		dialogScript.Read(3, true);

		while(dialogScript.NbLineRead != 3)
			yield return null;

		dialogScript.Close();
		Timer.SetActive(false);

		snakeScript.up = 0;
		snakeScript.right = 0;
		snakeScript.State = SnakeState.Waiting;
	}

	public IEnumerator Death()
	{
		yield return new WaitForSeconds(0.75f);

		while(dialogScript.Active)
			yield return null;

		dialogScript.Open();

		if(saveScript.playerData.HasSaved || PlayerPrefs.GetInt("Death") == 1)
		{
			dialogScript.GoTo(5);
		}
		else
		{
			dialogScript.GoTo(6);
		}
		
		dialogScript.Read(3, true);

		while(dialogScript.NbLineRead != 3)
			yield return null;

		Lives.SetActive(true);

		dialogScript.Read(8, true);
		dialogScript.Close(8);
		
		while(dialogScript.Active)
			yield return null;

		deathScript.Interact();
	}

	public IEnumerator Playground()
	{
		dialogScript.Open();
		dialogScript.GoTo(7);
		dialogScript.Read(1, true);

		while(dialogScript.NbLineRead != 1)
			yield return null;

		Instantiate(WYRabbitPrefab, new Vector3(800, 501, 505), Quaternion.Euler(0, 180, 0));
		Instantiate(WYRabbitPrefab, new Vector3(800, 501, 495), Quaternion.identity);
		Instantiate(WYRabbitPrefab, new Vector3(805, 501, 500), Quaternion.Euler(0, 270, 0));

		dialogScript.Read(2, true);

		while(dialogScript.NbLineRead != 2)
			yield return null;

		dialogScript.Close();
		snakeScript.State = SnakeState.Waiting;
	}

	public IEnumerator WhiteAndYellow(Transform trans)
	{
		WhiteYellowRabbitController script = trans.GetComponent<WhiteYellowRabbitController>();

		dialogScript.CharacterSprite = null;
		dialogScript.Open();
		dialogScript.SetPosition(Vector2.zero);
		dialogScript.GoTo(8);
		dialogScript.Read(1, false, 180);

		while(dialogScript.NbLineRead != 1)
			yield return null;

		dialogScript.Close();
		script.StartCoroutine(script.TurnBack());

		while(!script.TurnedBack)
			yield return null;

		yield return new WaitForSeconds(0.5f);

		Lives.SetActive(false);

		Vector3 targetPos = trans.position - 2.5f * trans.right + trans.up + 5.5f * trans.forward;
		Quaternion targetQuat = trans.rotation * Quaternion.Euler(3.5f, 130, 0);
		cameraScript.TargetSetup(targetPos, targetQuat, 0.2f);

		while(Vector3.Distance(myCamera.position, targetPos) > 0.05f)
			yield return null;

		yield return new WaitForSeconds(0.3f);

		script.StartCoroutine(script.Threshold());

		while(!script.Thresholded)
			yield return null;

		yield return new WaitForSeconds(1.5f);

		int murdering = tutoManager.RabbitKilled + tutoManager.WYRabbitCaught;

		dialogScript.CharacterBody = trans.Find("Body");
		dialogScript.Value = murdering;
		dialogScript.Body(Styles.Phylactery);
		dialogScript.Tip(new Vector2(400, -170), new Vector2(50, 120), 225);
		dialogScript.Open();
		dialogScript.SetRectTransform(new Vector2(-200, 180), new Vector2(700, 225));
		dialogScript.Read(1, true, 75);

		while(dialogScript.NbLineRead != 1)
			yield return null;

		int path;
		if(tutoManager.RabbitKilled > 0)
			path = 9;
		else if(tutoManager.WYRabbitCaught > 0)
			path = 10;
		else
			path = 11;

		dialogScript.GoTo(path);
		dialogScript.Read(5, true, 75);
		
		while(dialogScript.NbLineRead != 5)
			yield return null;

		dialogScript.GoTo(12);
		dialogScript.Ask(3, 75);

		while(dialogScript.Answer == Responses.None)
			yield return null;

		int teleporter_quantity;
		int remain_quantity;
		if(dialogScript.Answer == Responses.Left)
		{
			path = (murdering > 0) ? 13 : 14;
			teleporter_quantity = 7;
			remain_quantity = 9;
		}
		else
		{
			path = (murdering > 0) ? 15 : 16;
			teleporter_quantity = 2;
			remain_quantity = 4;
		}

		yield return null;
		dialogScript.GoTo(path);
		dialogScript.Read(teleporter_quantity, true, 75);

		while(dialogScript.NbLineRead != teleporter_quantity)
			yield return null;

		//show the teleporter
		Teleporters.SetActive(true);
		yield return null;
		cameraScript.TargetSetup(new Vector3(800, 503, 497), Quaternion.Euler(27, 35, 0), 0f);
		dialogScript.Tip(new Vector2(250, -75), new Vector2(50, 120), 250);
		dialogScript.SetRectTransform(new Vector2(375, 200), new Vector2(350, 150));
		dialogScript.FontDialogSetup(50, -1, new Vector2(30, -50));
		yield return new WaitForEndOfFrame();
		dialogScript.Read(1, true);

		while(dialogScript.NbLineRead != 1)
			yield return null;

		//come back to rubbey
		cameraScript.TargetSetup(targetPos, targetQuat, 0f);
		dialogScript.Tip(new Vector2(400, -170), new Vector2(50, 120), 225);
		dialogScript.SetRectTransform(new Vector2(-200, 180), new Vector2(700, 225));
		dialogScript.Read(remain_quantity, true, 75);

		while(dialogScript.NbLineRead != remain_quantity)
			yield return null;

		dialogScript.Close();

		yield return null;

		float clock = 0.0f;
		bool outerlaunch = false;
		IEnumerator outer_coroutine = tutoManager.OuterRotation();
		while(Input.GetAxisRaw("Horizontal") == 0.0f && Input.GetAxisRaw("Vertical") == 0.0f)
		{
			clock += Time.deltaTime;
			if(clock > 10.0f && !outerlaunch)
			{
				outerlaunch = true;
				tutoManager.StartCoroutine(outer_coroutine);
				tutoManager.StartCoroutine(tutoManager.Outer(trans));
			}
			yield return null;
		}

		if(outerlaunch)
		{
			while(Outerspace.color.a > 0.005f)
				yield return null;

			yield return new WaitForSeconds(0.5f);
			StopCoroutine(outer_coroutine);
		}
		
		Lives.SetActive(true);

		Vector3 target = Snake.position - Snake.forward * cameraScript.height;
		cameraScript.TargetSetup(target, Heart.rotation, 0.25f);
		while(Vector3.Distance(myCamera.position, target) > 0.05f)
			yield return null;

		trans.Find("Body").GetComponent<Collider>().enabled = true;

		cameraScript.NormalSetup();
		snakeScript.State = SnakeState.Waiting;
		snakeScript.up = 0;
		snakeScript.right = 0;
	}

	public IEnumerator RepetitionUnlessDisappear()
	{
		dialogScript.Body(Styles.Undertale);
		dialogScript.Open();
		dialogScript.SetPosition(new Vector2(-200, 200));

		int size;
		if(tutoManager.Repetition == 1)
		{
			dialogScript.GoTo(17);
			size = 75;
		}
		else if(tutoManager.Repetition < 9)
		{
			dialogScript.GoTo(18);
			size = 75;
		}
		else
		{
			tutoManager.Suicidal = true;
			dialogScript.GoTo(19);
			size = 55;
		}

		dialogScript.Read(1, false, size);

		while(dialogScript.NbLineRead != 1)
			yield return null;

		dialogScript.Close();

		snakeScript.State = SnakeState.Waiting;
		snakeScript.up = 0;
		snakeScript.right = 0;
	}
}