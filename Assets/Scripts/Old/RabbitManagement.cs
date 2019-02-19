using UnityEngine;
using System.Collections;

public enum RabbitState
{
	Yellow,
	Bomb,
	Dead
};

public class RabbitManagement : MonoBehaviour
{
	private ParticleSystem myPart1;
	private ParticleSystem myPart2;
	private ParticleSystem myPart3;
	private ParticleSystem Fire;

	private Transform myParent;
	private GameObject Snake;
	private GameObject Burn;

	private IEnumerator destruction_coroutine;

	private Light myLight;

	public RabbitState State = RabbitState.Yellow;

	private WaitForSeconds waitforseconds_particle = new WaitForSeconds(0.5f);

	private Renderer myRend;
	private Collider myColl;

	private bool visible = true;

	[HideInInspector]
	public BurrowScript burrowScript;
	private CameraScript cameraScript;
	private GameManagerV1 gameManager;
	private RabbitControllerV0 rabbitScript;
	private SnakeManagement snakeManag;
	private SnakeControllerV3 snakeScript;
	private DifficultyScript hardScript;

	void Awake()
	{
		myParent = transform.parent;
		Burn = myParent.Find("Burn").gameObject;
		Burn.SetActive(false);

		myLight = GetComponent<Light>();
		myRend = GetComponent<Renderer>();
		myColl = GetComponent<Collider>();

		rabbitScript = myParent.GetComponent<RabbitControllerV0>();
		
		Snake = GameObject.FindWithTag("Player");
		snakeManag = Snake.GetComponent<SnakeManagement>();
		snakeScript = Snake.GetComponent<SnakeControllerV3>();

		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		
		myPart1 = transform.Find("Part1").GetComponent<ParticleSystem>();
		myPart2 = transform.Find("Part2").GetComponent<ParticleSystem>();
		myPart3 = transform.Find("Part3").GetComponent<ParticleSystem>();
		
		Fire = transform.Find("Fire").GetComponent<ParticleSystem>();

		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		hardScript = GameObject.Find("LevelManager").GetComponent<DifficultyScript>();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") && snakeScript.State == SnakeState.Run)
		{
			if(State == RabbitState.Bomb && snakeManag.Health != SnakeHealth.Invincible)
			{
				gameManager.KilledBy = Killer.SuicidalRabbit;
				snakeManag.DestroySnake();
			}

			destruction_coroutine = Destruction();
			StartCoroutine(destruction_coroutine);
		}
		else if(other.CompareTag("Saw") || other.CompareTag("Bounds"))
		{
			StartCoroutine(Destruction());
		}
		else if(other.CompareTag("Fire"))
		{
			SetFire(true);
		}
		else if(other.CompareTag("SnakeBody") && other.GetComponent<SnakeFollow>().Ready)
		{
			rabbitScript.DoubleJumping();
		}
	}

	private IEnumerator Destruction()
	{
		bool was_visible = visible;
		myRend.enabled = false;
		myColl.enabled = false;
		myLight.enabled = false;

		if(rabbitScript.move_coroutine != null)
			StopCoroutine(rabbitScript.move_coroutine);

		Fire.Stop();
		Burn.SetActive(false);

		if(State != RabbitState.Bomb)
		{
			myPart3.Play();
		}
		else
		{
			myPart1.Stop();
			myPart2.Play();

			if(was_visible && !gameManager.Safe)
				cameraScript.Shake(Shaketype.Bomb);
		}	

		yield return waitforseconds_particle;

		State = RabbitState.Dead;

		burrowScript.StartCoroutine(burrowScript.ReloadRabbit());
	}

	public void SetupState(bool bomb)
	{
		if(destruction_coroutine != null)
			StopCoroutine(destruction_coroutine);

		if(bomb && hardScript.Difficulty >= hardScript.rabbitBomberThreshold)
		{
			State = RabbitState.Bomb;
			myPart1.Play();
		}
		else
		{
			State = RabbitState.Yellow;
		}

		myRend.enabled = true;
		myColl.enabled = true;
		myLight.enabled = true;
	}

	public void SetFire(bool fire)	//public car aussi appelé depuis le LazerScript
	{
		if(fire)
		{
			if(Fire.isStopped)
			{
				if(State == RabbitState.Bomb)
				{
					StartCoroutine(Destruction());
				}
				else
				{
					rabbitScript.waitforseconds = new WaitForSeconds(0.05f);
					Fire.Play();
					Burn.SetActive(true);
				}
			}
		}
		else
		{
			Fire.Stop();
			Burn.SetActive(false);
		}
	}

	void OnBecameVisible()
	{
		visible = true;
	}

	void OnBecameInvisible()
	{
		visible = false;
	}
}