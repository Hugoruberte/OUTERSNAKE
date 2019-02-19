using UnityEngine;
using System.Collections;

public class SuperLazerCreator : MonoBehaviour
{
	public GameObject SuperLazerPrefab;
	private Transform myLazer;

	private GameManagerV1 gameManager;
	private SuperLazerScript superlazerScript;
	private SnakeControllerV3 snakeScript;
	private DifficultyScript difficultyScript;
	private PlanetSetup planetSetup;

	private float loop;
	private float time;
	public float MinTime = 15.0f;
	public float MaxTime = 30.0f;

	public bool SuperLazerActivated = false;


	void Awake()
	{
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		difficultyScript = GameObject.Find("LevelManager").GetComponent<DifficultyScript>();

		GameObject lazer = Instantiate(SuperLazerPrefab, Vector3.zero, Quaternion.identity);
		myLazer = lazer.transform;
		myLazer.name = "SuperLazer";
		superlazerScript = myLazer.GetComponent<SuperLazerScript>();

		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();
		
		lazer.SetActive(false);
	}


	public void LaunchSuperLazer()
	{
		SuperLazerActivated = true;
		StartCoroutine(Creator());
	}


	private IEnumerator Creator()
	{
		while(SuperLazerActivated && planetSetup.SuperLazer && difficultyScript.Difficulty >= difficultyScript.superLazerThreshold)
		{
			yield return new WaitForSeconds(Random.Range(MinTime, MaxTime));

			while(myLazer.gameObject.activeInHierarchy)
				yield return null;

			if(snakeScript.State != SnakeState.Run || !SuperLazerActivated)
				break;

			Transform planet = gameManager.MainPlanet;
			Transform heart = planet.Find("Heart");
			
			int rot = Random.Range(0, 2) * 90;
			Vector3 direction;
			int scale = Mathf.RoundToInt(planet.Find("Body").localScale.x/3);

			direction = (rot == 0) ? heart.right : heart.up;

			Quaternion rotation = heart.rotation;
			rotation *= Quaternion.Euler(0, 0, rot);
			
			Vector3 position = planet.position + direction * (Random.Range(-scale, scale + 1) + 0.5f);
			
			myLazer.position = position;
			myLazer.rotation = rotation;

			myLazer.gameObject.SetActive(true);

			superlazerScript.StartCoroutine(superlazerScript.Setup());
		}

		SuperLazerActivated = false;
	}
}