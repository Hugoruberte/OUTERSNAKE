using UnityEngine;
using System.Collections;
using System.Linq;

public class MeteoreCreator : MonoBehaviour
{
	private GameManagerV1 gameManager;
	private SnakeControllerV3 snakeScript;
	private DifficultyScript difficultyScript;
	private ArmchairScript armchairScript;
	private PlanetScript planetScript;
	private PlanetSetup planetSetup;

	private Transform Planet;
	private Transform Heart;
	private Transform Snake;
	private Transform Folder;

	public GameObject MeteorePrefab;

	public float MinTime = 2.0f;
	public float MaxTime = 15.0f;
	private int scale;
	private float height;

	public bool MeteoreActivated = false;

	void Awake()
	{
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		difficultyScript = GameObject.Find("LevelManager").GetComponent<DifficultyScript>();
		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();
		armchairScript = GameObject.FindWithTag("Armchair").GetComponent<ArmchairScript>();

		Planet = gameManager.MainPlanet;
		Heart = Planet.Find("Heart");
		Snake = GameObject.FindWithTag("Player").transform;
		snakeScript = Snake.GetComponent<SnakeControllerV3>();

		scale = Mathf.RoundToInt(Planet.Find("Body").localScale.x / 2.0f) - 6;
		height = (Planet.Find("Body").localScale.x / 2.0f) + 0.5f;
	}

	private void Start()
	{
		GameObject myobject = new GameObject();

		Folder = myobject.transform;
		Folder.name = "MeteoresPooling";
		Folder.parent = GameObject.Find("ObjectPoolingStock").transform;
		Folder.localPosition = Vector3.zero;
	}

	public IEnumerator LaunchMeteore()
	{
		MeteoreActivated = true;

		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();

		while(MeteoreActivated && planetSetup.Meteore && difficultyScript.Difficulty >= difficultyScript.meteoreThreshold)
		{
			yield return new WaitForSeconds(Random.Range(MinTime, MaxTime));

			if(snakeScript.State != SnakeState.Run)
				break;

			Faces snakeface = snakeScript.Face;
			if(snakeface != armchairScript.Face && !planetScript.DestroyedFaces.Contains(snakeface))
			{
				for(int i = 0; i < Random.Range(1, 4); i++)	//les meteores peuvent arriver jusqu'à 3 en meme temps
					Setup();
			}
		}

		MeteoreActivated = false;
	}

	private void Setup()
	{
		Planet = gameManager.MainPlanet;
		Heart = Planet.Find("Heart");

		Vector3 targetPosition = Heart.position + (0.5f + Random.Range(-scale, scale+1)) * Heart.up + (0.5f + Random.Range(-scale, scale+1)) * Heart.right - Heart.forward * height;

		GameObject Meteore = Instantiate(MeteorePrefab, targetPosition, Heart.rotation);
		Meteore.name = "Meteore";
		Meteore.transform.parent = Folder;
		Meteore.GetComponent<MeteoreScript>().Setup();
	}
}