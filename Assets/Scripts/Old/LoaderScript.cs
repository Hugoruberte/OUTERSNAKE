using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Tools;

public class LoaderScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform myBar;
	private GameObject Levels;
	private Transform Snake;
	
	private Renderer myQuadRenderer;
	private Renderer myGreenRenderer;

	private SnakeManagement snakeManag;
	private SnakeControllerV3 snakeScript;
	private GameManagerV1 gameManager;
	private CameraScript cameraScript;

	private IEnumerator loadingCoroutine = null;
	private IEnumerator lowingCoroutine = null;
	private IEnumerator switchCoroutine = null;

	[Header("Parameters")]
	[Tooltip("Duration in seconds to reach 100% loading.")]
	[Range(0.0f, 5.0f)]
	public float LoadingDuration = 1f;
	private float LoadingSpeed = 50.0f;
	private float LoadingValue = 0.0f;
	
	public enum EnumLoadMode
	{
		Smooth,
		Jerk
	};
	public EnumLoadMode LoadingMode = EnumLoadMode.Smooth;

	public UnityEvent myFunction;

	[Header("Position")]
	public int myCell = 0;


	void Awake()
	{
		myTransform = transform;
		myBar = myTransform.Find("Body/Bar");
		Levels = myTransform.Find("Body/Top/Levels").gameObject;
		myQuadRenderer = myTransform.Find("Body/Bar/Quad").GetComponent<Renderer>();
		myGreenRenderer = myTransform.Find("Body/Top/Green").GetComponent<Renderer>();
		Snake = GameObject.FindWithTag("Player").transform;

		if(myFunction == null)
			myFunction = new UnityEvent();

		snakeManag = Snake.GetComponent<SnakeManagement>();
		snakeScript = Snake.GetComponent<SnakeControllerV3>();
		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
	}

	void Start()
	{
		myCell = myTransform.PositionToCell(gameManager.MainPlanet);
		if(gameManager.MainPlanet.GetComponent<PlanetScript>() != null)
		{
			gameManager.MainPlanet.GetComponent<PlanetScript>().SetCell(myCell, CellEnum.Mechanism);
			gameManager.MainPlanet.SetOccupedCell(myCell, 1);
		}

		Levels.SetActive(LoadingMode == EnumLoadMode.Jerk);
		myBar.SetLocalScaleX(0f);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			Vector3 snakeVect = (Vector3Extension.RoundToInt(myTransform.position - Snake.position)).normalized;
			if(Mathf.Abs(Vector3.Dot(snakeVect, myTransform.right)) < 0.9f)
			{
				if(snakeManag.Health != SnakeHealth.Invincible)
				{
					cameraScript.Shake(1.0f);
					gameManager.KilledBy = Killer.Obstacle;
					gameManager.StartCoroutine(gameManager.Blink());
					snakeManag.DestroySnake();
				}
				
				return;
			}

			if(switchCoroutine != null)
				StopCoroutine(switchCoroutine);
			switchCoroutine = SwitchCoroutine();
			StartCoroutine(switchCoroutine);

			if(lowingCoroutine != null)
			{
				StopCoroutine(lowingCoroutine);
				lowingCoroutine = null;
			}

			if(loadingCoroutine == null && LoadingValue < 99.5f)
			{
				loadingCoroutine = LoadingCoroutine();
				StartCoroutine(loadingCoroutine);
			}
		}
	}

	private IEnumerator LoadingCoroutine()
	{
		Transform hit;
		RaycastHit[] hits;
		float clock = 0.0f;
		bool Loading = true;

		LoadingSpeed = 100f/(LoadingDuration+0.01f);

		while(Loading && LoadingValue < 100f)
		{
			LoadingValue += LoadingSpeed * Time.deltaTime;
			clock += Time.deltaTime;

			if(LoadingMode == EnumLoadMode.Smooth)
				myBar.SetLocalScaleX(2.1f * LoadingValue);
			else
				myBar.SetLocalScaleX((Mathf.FloorToInt(2.1f * LoadingValue)/42) * 42); // 210/5 = 42

			if(clock > 0.1f)
			{
				hits = Physics.RaycastAll(myTransform.position-myTransform.right*1.5f, myTransform.right, 3f);

				for(int i = 0; i < hits.Length; i++)
				{
					hit = hits[i].transform;
					Loading = (hit.CompareTag("Player") || hit.CompareTag("SnakeBody"));
					if(Loading)
						break;
				}

				clock = 0.0f;
			}

			yield return null;
		}

		if(LoadingValue > 99f)
		{
			LoadingValue = 100f;
			myBar.SetLocalScaleX(210f);

			myFunction.Invoke();

			StartCoroutine(BlinkingCoroutine());
		}
		else
		{
			lowingCoroutine = LowingCoroutine();
			StartCoroutine(lowingCoroutine);
		}

		loadingCoroutine = null;
	}

	private IEnumerator LowingCoroutine()
	{
		LoadingSpeed = 100f/(LoadingDuration+0.01f);

		while(LoadingValue > 0.0f)
		{
			LoadingValue -= LoadingSpeed * Time.deltaTime;

			if(LoadingMode == EnumLoadMode.Smooth)
				myBar.SetLocalScaleX(2.1f * LoadingValue);
			else
				myBar.SetLocalScaleX((Mathf.FloorToInt(2.1f * LoadingValue)/42) * 42); // -> 210/5 = 42

			yield return null;
		}

		if(LoadingValue < 1f)
			LoadingValue = 0f;
	}

	private IEnumerator BlinkingCoroutine()
	{
		int count = 0;
		WaitForSeconds waitforseconds_01 = new WaitForSeconds(0.1f);

		myQuadRenderer.enabled = false;
		myGreenRenderer.enabled = true;

		while(count ++ < 10)
		{
			myGreenRenderer.enabled = !myGreenRenderer.enabled;
			yield return waitforseconds_01;
		}
	}

	private IEnumerator SwitchCoroutine()
	{
		snakeScript.switchNotch = -1;

		Vector3 snakeVect = (Vector3Extension.RoundToInt(myTransform.position - Snake.position)).normalized;
		Vector3 targetPosition = Snake.AbsolutePosition() + snakeVect * 3.5f;

		float before_distance = Vector3.Distance(Snake.position, targetPosition);
		float now_distance = before_distance + 1f;

		do
		{
			before_distance = now_distance;
			now_distance = Vector3.Distance(Snake.position, targetPosition);
			yield return null;
		}
		while(before_distance > now_distance);

		snakeScript.switchNotch = 0;
	}
}
