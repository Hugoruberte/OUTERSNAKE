using UnityEngine;
using System.Collections;

public class TreeScript : MonoBehaviour
{
	private Transform Body;
	private GameObject BodyObject;
	private Transform BurnArea;

	private Renderer BodyRenderer;

	private SnakeControllerV3 snakeScript;
	private GameManagerV1 gameManager;
	private PlanetScript planetScript;

	private ParticleSystem Leaf;
	private ParticleSystem Fire;
	private ParticleSystem Aches;

	private Vector3 InitialScale;
	private Vector3 InitialArea;

	private Light myLight;

	private Color GreenColor;
	private Color BurnColor = new Color32(20, 20, 20, 255);
	private Color FireColor = new Color32(255, 165, 0, 255);

	public bool Burnt = false;
	private bool Skip = false;

	public int myCell;
	private float lifetime;

	void Awake()
	{
		Body = transform.Find("Body");
		BodyObject = Body.gameObject;
		BodyRenderer = Body.GetComponent<Renderer>();
		InitialScale = Body.localScale;

		BurnArea = transform.Find("Burn");
		InitialArea = BurnArea.localScale;
		BurnArea.gameObject.SetActive(false);

		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();

		Leaf = transform.Find("Leaf").GetComponent<ParticleSystem>();
		Fire = Body.Find("Fire").GetComponent<ParticleSystem>();
		Aches = Body.Find("Aches").GetComponent<ParticleSystem>();
		myLight = Body.Find("Light").GetComponent<Light>();

		lifetime = Fire.main.duration;
	}

	void Start()
	{
		Body.localScale = InitialScale;
		BodyRenderer.material.color = (gameManager.State != Scenes.Hell) ? new Color32(0, 215, 0, 255) : new Color32(225, 105, 0, 255);
		GreenColor = BodyRenderer.material.color;
		myLight.color = GreenColor;
	}

	void OnTriggerEnter(Collider other)
	{
		if(((other.CompareTag("Player") && snakeScript.State == SnakeState.Run) || other.CompareTag("Saw")) && !Burnt)
		{
			Leaf.Play();
		}
		else if(other.CompareTag("Fire") && !Burnt && BodyObject.activeInHierarchy)
		{
			Burnt = true;

			Fire.Play();
			Aches.Play();

			planetScript.Grid[myCell] = CellEnum.Trunk;

			StartCoroutine(Intensity());
			StartCoroutine(Aspect());
			StartCoroutine(Size());
			StartCoroutine(Area());
		}
		else if(other.CompareTag("Bounds"))
		{
			transform.localPosition = Vector3.zero;
			gameObject.SetActive(false);
		}
	}

	private IEnumerator Intensity()
	{
		myLight.color = FireColor;
		float start = myLight.intensity;
		float value = start;
		while(value > 0.0f && !Skip)
		{
			value = Mathf.MoveTowards(value, 0.0f, (start/lifetime) * 1.0f * Time.deltaTime);
			myLight.intensity = value;
			yield return null;
		}
	}

	private IEnumerator Aspect()
	{
		float value = 0.0f;
		while(value < 1.0f && !Skip)
		{
			value = Mathf.MoveTowards(value, 1.0f,(3.0f/lifetime) * Time.deltaTime);
			BodyRenderer.material.color = Color.Lerp(GreenColor, BurnColor, value);
			yield return null;
		}
	}

	private IEnumerator Size()
	{
		float value = 1.0f;
		while(value > 0.0f && !Skip)
		{
			value = Mathf.MoveTowards(value, 0.0f,(1.0f/lifetime) * Time.deltaTime);
			Body.localScale = InitialScale * value;
			yield return null;
		}

		BodyObject.SetActive(false);
		Body.localScale = InitialScale;
	}

	private IEnumerator Area()
	{
		BurnArea.gameObject.SetActive(true);
		Vector3 target = new Vector3(5, 5, 5);

		while(BurnArea.localScale != target && !Skip)
		{
			BurnArea.localScale = Vector3.MoveTowards(BurnArea.localScale, target, (30.0f/lifetime) * Time.deltaTime);
			yield return null;
		}

		BurnArea.localScale = InitialArea;
		BurnArea.gameObject.SetActive(false);
	}

	public IEnumerator SetTreeAspect(bool burnt, PlanetScript script)
	{
		Burnt = burnt;
		planetScript = script;

		Skip = true;
		yield return null;
		
		Fire.Stop();
		Aches.Stop();

		BodyRenderer.material.color = GreenColor;
		BodyObject.SetActive(!burnt);
		Skip = false;
	}
}