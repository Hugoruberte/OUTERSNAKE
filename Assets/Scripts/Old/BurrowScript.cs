using UnityEngine;
using System.Collections;
using System.Linq;

public class BurrowScript : MonoBehaviour
{
	private Transform myTransform;
	public Transform myRabbit;

	private WaitForSeconds waitforseconds_5 = new WaitForSeconds(5.0f);
	private WaitForSeconds waitforseconds_05 = new WaitForSeconds(0.5f);

	private RabbitManagement rabbitManag;
	private RabbitControllerV0 rabbitScript;
	private GameManagerV1 gameManager;
	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;
	private PlanetScript planetScript;

	public int myCell;
	private int bombProb;

	private ParticleSystem myParticle;

	void Awake()
	{
		myTransform = transform;

		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		snakeManag = GameObject.FindWithTag("Player").GetComponent<SnakeManagement>();
	}

	void Start()
	{
		rabbitManag = myRabbit.GetChild(0).GetComponent<RabbitManagement>();
		rabbitScript = myRabbit.GetComponent<RabbitControllerV0>();

		if(gameManager.MainPlanet.GetComponent<PlanetScript>().DestroyedFaces.Contains((Faces)(myCell/(22*22))))
		{
			myRabbit.localPosition = Vector3.zero;
			myTransform.localPosition = Vector3.zero;

			myRabbit.gameObject.SetActive(false);
			gameObject.SetActive(false);
			return;
		}

		bombProb = Random.Range(0, gameManager.WorldSetting.RabbitBombProb);
		//plus gameManager.WorldSetting.BombProb est grand plus il y a de chances d'avoir une bombe
		
		myRabbit.position = myTransform.position + myTransform.forward * 0.25f;
		myRabbit.rotation = myTransform.rotation * Quaternion.Euler(90, 0, 0);
		myRabbit.gameObject.SetActive(true);
		
		rabbitManag.burrowScript = this;
		rabbitManag.SetupState(bombProb != 0);

		myParticle = gameObject.GetComponent<ParticleSystem>();
	}

	public IEnumerator ReloadRabbit()
	{
		int myFace = myCell/(22*22);

		if(planetScript == null)
			planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();

		if(planetScript.DestroyedFaces.Contains((Faces)myFace))
		{
			myRabbit.localPosition = Vector3.zero;
			myTransform.localPosition = Vector3.zero;

			myRabbit.gameObject.SetActive(false);
			gameObject.SetActive(false);
			yield break;
		}

		bombProb = Random.Range(0, gameManager.WorldSetting.RabbitBombProb);
		//plus gameManager.WorldSetting.BombProb est grand plus il y a de chances d'avoir une bombe

		yield return waitforseconds_5;		//on attend 5 seconde avant de faire réapparaitre le lapin

		if(myCell == snakeScript.myCell || snakeManag.SnakeBodyCells.Contains(myCell))
		{
			yield return new WaitUntil(() => (myCell != snakeScript.myCell && !snakeManag.SnakeBodyCells.Contains(myCell)));
			yield return waitforseconds_05;
		}

		myRabbit.gameObject.SetActive(true);
		myParticle.Play();

		rabbitScript.StartCoroutine(rabbitScript.ReloadPosition(myTransform, myCell));	//on remet le lapin à sa place
		rabbitManag.SetupState(bombProb != 0);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Bounds"))
		{
			myRabbit.localPosition = Vector3.zero;
			myTransform.localPosition = Vector3.zero;

			myRabbit.gameObject.SetActive(false);
			gameObject.SetActive(false);
		}
	}
}