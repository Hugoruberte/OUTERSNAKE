using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tools;

public enum SnakeHealth
{
	Alive,
	Invincible,
	Burnt,
	Dead
};

public class SnakeManagement : MonoBehaviour
{
	[HideInInspector]
	public Transform SnakeBody;
	private Transform myTransform;
	private GameObject Hood;

	private SnakeControllerV3 snakeScript;
	private GameManagerV1 gameManager;
	private BunneyArcade bunneyScript;
	private SaveScript saveScript;
	private CameraScript cameraScript;

	private IEnumerator reduceCoroutine = null;
	private WaitForSeconds waitforseconds_01 = new WaitForSeconds(0.1f);

	private ParticleSystem Explosion;
	private ParticleSystem Fire;

	private Renderer HoodRend;
	private Renderer myRend;
	private Collider myColl;

	[Space(10)]
	public SnakeHealth Health = SnakeHealth.Alive;

	private bool Destruction = false;

	[HideInInspector]
	public List<int> SnakeBodyCells = new List<int>();
	private List<int> ReduceValues = new List<int>();

	
	private int bodynumber = 10;
	private float floatbodynumber = 10.0f;
	public int BodyNumber
	{
		get
		{
			return bodynumber;
		}
		set
		{
			bodynumber = value;
			floatbodynumber = value;

			if(bodynumber < 2 && snakeScript.State == SnakeState.Run && Health != SnakeHealth.Dead 
			&& !Destruction && !gameManager.Rocket && !gameManager.Safe)
			{
				if(gameManager.KilledBy == Killer.Nobody)
					gameManager.KilledBy = Killer.Lack;

				DestroySnake();
			}
		}
	}
	public float FloatBodyNumber
	{
		get
		{
			return floatbodynumber;
		}
		set
		{
			floatbodynumber = value;
			bodynumber = Mathf.CeilToInt(floatbodynumber);

			if(bodynumber < 2 && snakeScript.State == SnakeState.Run && Health != SnakeHealth.Dead 
			&& !Destruction && !gameManager.Rocket && !gameManager.Safe)
			{
				if(gameManager.KilledBy == Killer.Nobody)
					gameManager.KilledBy = Killer.Lack;

				DestroySnake();
			}
		}
	}


	void Awake()
	{
		myTransform = transform;

		GameObject mybody = new GameObject();
		mybody.name = "SnakeBody";
		SnakeBody = mybody.transform;
		
		snakeScript = GetComponent<SnakeControllerV3>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		bunneyScript = GameObject.Find("LevelManager").GetComponent<BunneyArcade>();
		saveScript = GameObject.Find("LevelManager").GetComponent<SaveScript>();
		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		
		Explosion = myTransform.Find("Explosion").GetComponent<ParticleSystem>();
		Fire = myTransform.Find("Fire").GetComponent<ParticleSystem>();

		HoodRend = myTransform.Find("Hood").GetComponent<Renderer>();
		myRend = GetComponent<Renderer>();
		myColl = GetComponent<Collider>();
	}

	void Start()
	{
		SnakeBody.SetSiblingIndex(myTransform.GetSiblingIndex() + 1);
	}
		
	void OnTriggerEnter(Collider other)
	{
		if(snakeScript.State == SnakeState.Run)
		{
			if(other.CompareTag("Apple"))
			{
				AppleType apple_type = other.transform.GetComponent<AppleScript>().State;
				switch(apple_type)
				{
					case AppleType.Red:
						BodyNumber += 1;
						break;

					case AppleType.Rotten:
						if(Health != SnakeHealth.Invincible)
						{
							BodyNumber -= (gameManager.State == Scenes.Hell) ? 3 : 2;
							if(bodynumber < 2)
								gameManager.KilledBy = Killer.Rotten;
						}
						break;

					case AppleType.Dung:
						if(Health != SnakeHealth.Invincible)
						{
							BodyNumber -= (gameManager.State == Scenes.Hell) ? 5 : 3;
							if(bodynumber < 2)
								gameManager.KilledBy = Killer.Dung;
						}
						break;
				}
			}
			else if(other.CompareTag("Rabbit"))
			{
				RabbitState rabbit_type = other.GetComponent<RabbitManagement>().State;

				if(rabbit_type == RabbitState.Yellow)
				{
					BodyNumber += 5;
					saveScript.playerData.RabbitKilled ++;
				}
			}
			else if(other.CompareTag("Bounds"))
			{
				gameManager.StartCoroutine(gameManager.Blink());
				gameManager.KilledBy = Killer.Hole;

				if(!saveScript.gameData.hole && gameManager.State != Scenes.Hell)
					bunneyScript.StartCoroutine(bunneyScript.Hole());

				Vector3 border = other.transform.parent.position;
				Vector3 pos = Vector3Extension.RoundToInt(border + (myTransform.position - border).normalized * 13.0f);

				myTransform.position = pos;
				snakeScript.targetPosition = pos;

				DestroySnake();
			}
			else if(Health != SnakeHealth.Invincible)
			{
				if(other.CompareTag("SnakeBody") && other.GetComponent<SnakeFollow>().Ready)
				{
					cameraScript.Shake(1.0f);
					gameManager.KilledBy = Killer.Yourself;
					gameManager.StartCoroutine(gameManager.Blink());
					DestroySnake();
				}
				else if(other.CompareTag("Meteore"))
				{
					gameManager.StartCoroutine(gameManager.Blink());
					BodyNumber -= (gameManager.State == Scenes.Hell) ? 5 : 2;
					if(bodynumber < 2)
						gameManager.KilledBy = Killer.Meteore;
				}
				else if(other.CompareTag("Saw"))	// SawScript gere le gameManager.Blink()
				{
					gameManager.KilledBy = Killer.Saw;
					DestroySnake();
				}
				else if(other.name == "Trunk")
				{
					cameraScript.Shake(1.0f);
					gameManager.KilledBy = Killer.Tree;

					gameManager.StartCoroutine(gameManager.Blink());
					DestroySnake();
				}
				else if(other.CompareTag("Fire"))
				{
					gameManager.StartCoroutine(gameManager.Blink());
					StartCoroutine(FireSetup());
				}
				else if(other.CompareTag("Barrier"))
				{
					cameraScript.Shake(1.0f);
					gameManager.KilledBy = Killer.Barrier;
					gameManager.StartCoroutine(gameManager.Blink());
					DestroySnake();
				}
			}
		}
	}

	public void SetRenderer(bool active)
	{
		myRend.enabled = active;
		HoodRend.enabled = active;
	}

	public IEnumerator HoodInvinsibleSetup()
	{
		while(Health == SnakeHealth.Invincible)
		{
			HoodRend.material = myRend.material;
			yield return null;
		}
	}

	private IEnumerator FireSetup()
	{
		Health = SnakeHealth.Burnt;

		Fire.Stop();
		yield return null;
		Fire.Play();

		while(Fire.isPlaying && Health != SnakeHealth.Dead)
		{
			FloatBodyNumber -= gameManager.WorldSetting.FireDamagePerSecond * Time.deltaTime;

			if(bodynumber < 2)
				gameManager.KilledBy = Killer.Fire;
			yield return null;
		}

		Fire.Stop();
		if(Health != SnakeHealth.Dead)
			Health = SnakeHealth.Alive;
	}


	public void ReduceSnake(int value)
	{
		ReduceValues.Add(value);

		if(reduceCoroutine != null)
			StopCoroutine(reduceCoroutine);
		reduceCoroutine = ReduceCoroutine();
		StartCoroutine(reduceCoroutine);
	}
	private IEnumerator ReduceCoroutine()
	{
		yield return waitforseconds_01;
		ReduceValues.Sort();
		BodyNumber -= ReduceValues[ReduceValues.Count - 1];;
		ReduceValues.Clear();
	}

	public void DestroySnake()
	{
		if(!Destruction)
			StartCoroutine(DestroySnakeCoroutine());
	}
	private IEnumerator DestroySnakeCoroutine()
	{
		if(Health == SnakeHealth.Invincible && gameManager.KilledBy != Killer.Hole)
			Debug.LogError("Snake was invincible but died anyway ! Killer : " + gameManager.KilledBy);

		Destruction = true;

		if(snakeScript.switchNotch != 0)
		{
			yield return new WaitUntil(() => snakeScript.switchNotch == 0);

			myRend.enabled = false;
			myColl.enabled = false;
			HoodRend.enabled = false;

			myTransform.position = myTransform.AbsolutePosition() + myTransform.up;
			snakeScript.targetPosition = myTransform.position;
		}

		Health = SnakeHealth.Dead;
		snakeScript.State = SnakeState.Stopped;
		snakeScript.up = 0;
		snakeScript.right = 0;
		gameManager.GameOver = true;

		myTransform.position = myTransform.AbsolutePosition();
		snakeScript.targetPosition = myTransform.position;

		if(SnakeBody.childCount > 0)
			StartCoroutine(DestroySnakeBody());

		myRend.enabled = false;
		myColl.enabled = false;
		HoodRend.enabled = false;

		Explosion.Play();

		gameManager.StartCoroutine(gameManager.GameOverSetup());

		Destruction = false;
	}

	private IEnumerator DestroySnakeBody()
	{
		for(int i = 0; i < SnakeBody.childCount; i++)
		{
			SnakeBody.GetChild(i).GetComponent<SnakeFollow>().ExplosionWithoutRecursion();
			if(i % 15 == 0)
				yield return null;
		}
	}
}