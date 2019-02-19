using UnityEngine;
using System.Collections;
using Tools;

public class SnakeFollow : MonoBehaviour
{
	private Transform Snake;
	private Transform SnakeBody;
	private GameObject Poubelle;
	private Transform myTransform;
	private GameObject myGameObject;

	private IEnumerator reductionCoroutine = null;

	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;
	private GameManagerV1 gameManager;
	private PlanetScript planetScript;

	public int Place = 0;
	public int myCell = 0;

	private Renderer SnakeRend;

	private Renderer myRend;
	private Collider myColl;

	private WaitForSeconds waitforseconds = new WaitForSeconds(0.75f);

	private ParticleSystem myParticle;

	public bool Ready = false;
	public bool Reusable = true;

	void Awake()
	{
		myTransform = transform;
		myGameObject = gameObject;
		myGameObject.name = "SnakePart";
		myGameObject.tag = "SnakeBody";

		myRend = GetComponent<Renderer>();
		myColl = GetComponent<Collider>();

		Snake = GameObject.FindWithTag("Player").transform;
		SnakeRend = Snake.GetComponent<Renderer>();

		snakeScript = Snake.GetComponent<SnakeControllerV3>();
		snakeManag = Snake.GetComponent<SnakeManagement>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();
		
		myParticle = myGameObject.GetComponent<ParticleSystem>();

		GetComponent<ParticleSystemRenderer>().material = myRend.material;
	}

	void Start()
	{
		Poubelle = GameObject.Find("Poubelle");
		SnakeBody = snakeManag.SnakeBody;
		myTransform.parent = SnakeBody;

		Setup();
	}

	public void Setup()
	{
		Ready = false;

		myRend.enabled = SnakeRend.enabled;
		myColl.enabled = true;

		if(reductionCoroutine != null)
		{
			StopCoroutine(reductionCoroutine);
			reductionCoroutine = null;
			if(myCell >= 0 && planetScript && planetScript.Grid.Length < myCell && planetScript.Grid[myCell] == CellEnum.Snake)
				planetScript.Grid[myCell] = CellEnum.Empty;
			snakeManag.SnakeBodyCells.Remove(myCell);
		}

		myTransform.localScale = Vector3.one;
		myCell = snakeScript.myCell;
		snakeManag.SnakeBodyCells.Add(myCell);			

		Place = snakeManag.BodyNumber;
		myTransform.SetAsFirstSibling();
		
		StartCoroutine(SetupChildPlaceAndWaitForReady());
	}

	private IEnumerator SetupChildPlaceAndWaitForReady()
	{
		for(int i = 1; i < SnakeBody.childCount; i++)
		{
			SnakeBody.GetChild(i).GetComponent<SnakeFollow>().Place = (Place - i < 0) ? 0 : Place - i;
			if(i % 10 == 0)
				yield return null;
		}

		yield return new WaitUntil(() => myTransform.GetSiblingIndex() > 1);

		Ready = true;
	}

	void Update()
	{
		if(snakeManag.Health == SnakeHealth.Invincible)
			myRend.SetFlatColor(SnakeRend.GetFlatColor());

		if(Reusable && myTransform.GetSiblingIndex() > snakeManag.BodyNumber - 2)
		{
			if(reductionCoroutine != null)
				StopCoroutine(reductionCoroutine);
			reductionCoroutine = ReductionCoroutine(false);
			StartCoroutine(reductionCoroutine);
		}
	}

	public void ReductionNextPoubelle()
	{
		if(reductionCoroutine != null)
			StopCoroutine(reductionCoroutine);
		reductionCoroutine = ReductionCoroutine(true);
		StartCoroutine(reductionCoroutine);
	}

	private IEnumerator ReductionCoroutine(bool directGoToPoubelle)
	{
		if(directGoToPoubelle)
		{
			while(Poubelle == null)
			{
				Poubelle = GameObject.Find("Poubelle");
				yield return null;
			}
			myTransform.parent = Poubelle.transform;
		}

		Place = 0;
		Ready = false;
		float reductSpeed = (13f/40f) * snakeScript.Speed + (15f/4f);

		while(myTransform.localScale.x > 0.05f)
		{
			myTransform.localScale = Vector3.MoveTowards(myTransform.localScale, Vector3.zero, reductSpeed * Time.deltaTime);
			yield return null;
		}

		myTransform.localScale = Vector3.zero;
		myColl.enabled = false;

		if(myCell >= 0 && planetScript && planetScript.Grid.Length < myCell && planetScript.Grid[myCell] == CellEnum.Snake)
			planetScript.Grid[myCell] = CellEnum.Empty;
		snakeManag.SnakeBodyCells.Remove(myCell);

		if(myTransform.GetSiblingIndex() > snakeManag.BodyNumber + 3 || directGoToPoubelle)
			GoToPoubelle();
	}

	public void GoToPoubelle()
	{
		if(Poubelle != null && myTransform.parent == Poubelle)
			return;
		StartCoroutine(GotToPoubelleCoroutine());
	}

	private IEnumerator GotToPoubelleCoroutine()
	{
		while(Poubelle == null)
		{
			Poubelle = GameObject.Find("Poubelle");
			yield return null;
		}
		myTransform.parent = Poubelle.transform;
		myGameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider other)
	{
		if((other.CompareTag("Saw") || other.CompareTag("Bounds")) 
			&& snakeScript.State == SnakeState.Run 
			&& snakeManag.Health != SnakeHealth.Invincible)
		{
			snakeManag.ReduceSnake(Place);
			Explosion();
		}
	}

	public void Explosion()
	{
		if(reductionCoroutine != null)
			StopCoroutine(reductionCoroutine);

		if(!Reusable || !myColl.enabled)
			return;
		Reusable = false;
		StartCoroutine(ExplosionCoroutine(true));
	}

	public void ExplosionWithoutRecursion()
	{
		if(reductionCoroutine != null)
			StopCoroutine(reductionCoroutine);

		if(!Reusable || !myColl.enabled)
			return;
		Reusable = true;
		StartCoroutine(ExplosionCoroutine(false));
	}

	private IEnumerator ExplosionCoroutine(bool recursion)
	{
		Ready = false;
		myRend.enabled = false;
		myColl.enabled = false;
		myParticle.Play();

		if(myCell >= 0 && planetScript && planetScript.Grid.Length < myCell && planetScript.Grid[myCell] == CellEnum.Snake)
			planetScript.Grid[myCell] = CellEnum.Empty;
		snakeManag.SnakeBodyCells.Remove(myCell);

		if(recursion)
		{
			int index = myTransform.GetSiblingIndex();
			for(int i = index+1; i < SnakeBody.childCount; i++)
			{
				SnakeBody.GetChild(i).GetComponent<SnakeFollow>().ExplosionWithoutRecursion();
				if(i % 15 == 0)
					yield return null;
			}
		}
		
		yield return waitforseconds;

		GoToPoubelle();
	}
}