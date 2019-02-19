using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class TurnOffScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform Snake;
	private Transform Arrow;

	private Renderer ArrowRenderer;

	private SnakeControllerV3 snakeScript;
	private GameManagerV1 gameManager;

	public enum TurnOffMode
	{
		Idle,
		Rotating
	};
	[Header("Parameters")]
	public TurnOffMode Mode = TurnOffMode.Idle;

	private bool MakeSnakeTurn = false;

	private WaitForSeconds waitforseconds = new WaitForSeconds(0.25f);

	[Range(0.05f, 5f)]
	public float Interval = 1f;
	[Header("Position")]
	public int myCell = 0;

	private IEnumerator rotatingCoroutine = null;

	void Awake()
	{
		myTransform = transform;

		Snake = GameObject.FindWithTag("Player").transform;
		Arrow = transform.Find("Body/Arrow");

		ArrowRenderer = Arrow.GetComponent<Renderer>();
		ArrowRenderer.material.SetTextureOffset("_MainTex", new Vector2(0,0.666f));

		snakeScript = Snake.GetComponent<SnakeControllerV3>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			snakeScript.CancelInput = true;
			MakeSnakeTurn = true;

			Transform Heart = gameManager.MainPlanet.Find("Heart");

			int value = Mathf.RoundToInt(Vector3.Dot(Arrow.right, Heart.right));
			if(value != 0)
			{
				if(snakeScript.right == 0 || snakeScript.right == value)
				{
					snakeScript.rightStored = value;
					snakeScript.upStored = 0;
					ArrowRenderer.material.SetTextureOffset("_MainTex", new Vector2(0,0.333f));
				}
				else
				{
					ArrowRenderer.material.SetTextureOffset("_MainTex", new Vector2(0,0));
				}
			}
			else
			{
				value = Mathf.RoundToInt(Vector3.Dot(Arrow.right, Heart.up));
				if(snakeScript.up == 0 || snakeScript.up == value)
				{
					snakeScript.upStored = value;
					snakeScript.rightStored = 0;

					ArrowRenderer.material.SetTextureOffset("_MainTex", new Vector2(0,0.333f));
				}
				else
				{
					ArrowRenderer.material.SetTextureOffset("_MainTex", new Vector2(0,0));
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			snakeScript.CancelInput = false;
			StartCoroutine(AfterChangeDirectionCoroutine());
		}
	}

	private IEnumerator AfterChangeDirectionCoroutine()
	{
		Vector3 pos = transform.position;
		yield return new WaitUntil(() => Vector3.Distance(Snake.position, pos) > 0.75f);
		MakeSnakeTurn = false;
		ArrowRenderer.material.SetTextureOffset("_MainTex", new Vector2(0,0.666f));
	}

	void OnEnable()
	{
		if(rotatingCoroutine != null)
			StopCoroutine(rotatingCoroutine);

		if(Mode == TurnOffMode.Rotating)
		{
			rotatingCoroutine = RotatingCoroutine();
			StartCoroutine(rotatingCoroutine);
		}

		myCell = myTransform.PositionToCell(gameManager.MainPlanet);
		if(gameManager.MainPlanet.GetComponent<PlanetScript>() != null)
			gameManager.MainPlanet.GetComponent<PlanetScript>().SetCell(myCell, CellEnum.Mechanism);
	}

	void OnDisable()
	{
		if(rotatingCoroutine != null)
			StopCoroutine(rotatingCoroutine);
	}

	private IEnumerator RotatingCoroutine()
	{
		while(true)
		{
			if(MakeSnakeTurn)
			{
				yield return new WaitUntil(() => !MakeSnakeTurn);
				yield return waitforseconds;
			}
			Arrow.localRotation *= Quaternion.Euler(0,0,90);
			yield return new WaitForSeconds(Interval);
		}
	}
}
