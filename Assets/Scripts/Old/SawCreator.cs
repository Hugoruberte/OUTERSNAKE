using UnityEngine;
using System.Collections;

public class SawCreator : MonoBehaviour
{
	private Transform Planet;

	private Transform SawPathsPooling;

	private SnakeControllerV3 snakeScript;
	private GameManagerV1 gameManager;
	private DifficultyScript difficultyScript;
	private PlanetSetup planetSetup;

	public bool SawActivated = false;

	private Vector3 targetScale = new Vector3(1, 1, 0);

	public float MinTime = 5.0f;
	public float MaxTime = 15.0f;
	private int scale;


	void Awake()
	{
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		difficultyScript = GameObject.Find("LevelManager").GetComponent<DifficultyScript>();

		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();

		Planet = gameManager.MainPlanet;
		scale = Mathf.RoundToInt(Planet.Find("Body").localScale.x / 2.0f) - 7;
	}

	void Start()
	{
		SawPathsPooling = GameObject.Find("SawPathsPooling").transform;
	}

	public void LaunchSaw()
	{
		SawActivated = true;
		StartCoroutine(Creator());
	}

	private IEnumerator Creator()
	{
		while(SawActivated && planetSetup.CircularSaw && difficultyScript.Difficulty >= difficultyScript.sawThreshold)
		{
			yield return new WaitForSeconds(Random.Range(MinTime, MaxTime));

			Planet = gameManager.MainPlanet;

			if(snakeScript.State != SnakeState.Run)
				break;

			for(int i = 0; i < SawPathsPooling.childCount; i++)
			{
				Transform saw = SawPathsPooling.GetChild(i);

				if(!saw.gameObject.activeInHierarchy)
				{
					saw.localScale = targetScale;

					saw.rotation = Planet.Find("Heart").rotation * Quaternion.Euler(-90, 0, 0);
					saw.rotation *= Quaternion.Euler(0, Random.Range(0, 4)*90, 0);
					saw.position = Planet.position + saw.forward * (0.5f + Random.Range(-scale, scale+1));

					yield return null;	//pour laisser le temps aux calculs ...

					saw.gameObject.SetActive(true);
					saw.Find("Saw").GetComponent<SawScript>().StartCoroutine(saw.Find("Saw").GetComponent<SawScript>().Launch());
					break;
				}
			}
		}

		SawActivated = false;
	}
}