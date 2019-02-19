using UnityEngine;
using System.Collections;

public class CasterBlasterCreator : MonoBehaviour
{
	private GameManagerV1 gameManager;
	private SnakeControllerV3 snakeScript;
	private DifficultyScript difficultyScript;
	private ArmchairScript armchairScript;
	private PlanetSetup planetSetup;

	private Transform Planet;
	private Transform Heart;
	private Transform Snake;
	private Transform Folder;

	public float MinTime = 2.0f;
	public float MaxTime = 15.0f;
	private float scale;

	public bool CasterBlasterActivated = false;

	void Awake()
	{
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		difficultyScript = GameObject.Find("LevelManager").GetComponent<DifficultyScript>();
		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();

		armchairScript = GameObject.FindWithTag("Armchair").GetComponent<ArmchairScript>();
		
		Planet = gameManager.MainPlanet;
		Snake = GameObject.FindWithTag("Player").transform;
		snakeScript = Snake.GetComponent<SnakeControllerV3>();

		scale = (Planet.Find("Body").localScale.x / 2.0f) + 0.5f;
	}

	void Start()
	{
		Folder = GameObject.Find("ObjectPoolingStock/CasterBlastersPooling").transform;
	}

	public IEnumerator LaunchCasterBlaster()
	{
		CasterBlasterActivated = true;

		while(CasterBlasterActivated && planetSetup.CasterBlaster && difficultyScript.Difficulty >= difficultyScript.casterBlasterThreshold)
		{
			yield return new WaitForSeconds(Random.Range(MinTime, MaxTime));

			if(snakeScript.State != SnakeState.Run)
				break;

			if(snakeScript.Face != armchairScript.Face)
			{
				int nb = Random.Range(2, 4);
				for(int i = 0; i < nb; i++)		//les casterblaster peuvent arriver jusqu'à 3 en meme temps
					StartCoroutine(Setup());
			}
		}

		CasterBlasterActivated = false;
	}

	private IEnumerator Setup()
	{
		Planet = gameManager.MainPlanet;
		Heart = Planet.Find("Heart");

		Vector3 targetPosition = Heart.position + (1 - Random.Range(0, 2) * 2) * Random.Range(5.0f, scale - 3.0f) * Heart.up + (1 - Random.Range(0, 2) * 2) * Random.Range(5.0f, scale - 3.0f) * Heart.right - Heart.forward * scale;
		Quaternion targetRotation = Quaternion.LookRotation(Snake.position - targetPosition);

		Vector3 spawnPosition = targetPosition - (Snake.position - targetPosition).normalized * 10.0f;
		Quaternion spawnRotation = Quaternion.LookRotation(targetPosition - Snake.position);

		Transform C_blaster = null;
		Transform child;
		int length = Folder.childCount;
		for(int i = 0; i < length; i++)
		{
			child = Folder.GetChild(i);
			if(!child.gameObject.activeInHierarchy)
			{
				C_blaster = child;
				break;
			}
		}

		if(C_blaster == null)
		{
			Debug.LogWarning("We reached the max allowed for CasterBlaster ! (max = " + Folder.childCount + ")");
			yield break;
		}

		C_blaster.position = spawnPosition;
		C_blaster.rotation = spawnRotation;
		C_blaster.gameObject.SetActive(true);

		float diff = Vector3.Distance(C_blaster.position, targetPosition);
		float dist = Vector3.Distance(C_blaster.position, targetPosition);
		Vector3 reference = Vector3.zero;

		while(!(Vector3.Distance(C_blaster.position, targetPosition) < 0.5f 
			&& Quaternion.Angle(C_blaster.rotation, targetRotation) < 0.5f))
		{
			dist = Vector3.Distance(C_blaster.position, targetPosition);

			C_blaster.position = Vector3.SmoothDamp(C_blaster.position, targetPosition, ref reference, 0.25f);
			C_blaster.rotation = Quaternion.Slerp(spawnRotation, targetRotation, 1.0f - (dist / diff));
			yield return null;
		}

		C_blaster.GetComponent<CasterBlasterScript>().Go();
	}
}