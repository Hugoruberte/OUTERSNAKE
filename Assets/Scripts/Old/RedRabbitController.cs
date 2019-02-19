using UnityEngine;
using System.Collections;
using System.Linq;

public enum RedRabbitState
{
	Normal,
	Burn,
	Dead
};

public class RedRabbitController : MonoBehaviour
{
	private Transform myTransform;
	private Transform myRabbit;
	private Transform Snake;
	private GameObject Body;
	public GameObject Missile;

	private Vector3 InitialCenter;
	private Quaternion InitialRotation;
	private Quaternion targetRotation;

	private Vector3 Center;
	private Vector3 Axe;
	private Vector3 AxeRotation;

	public Faces Face;

	public RedRabbitState State = RedRabbitState.Normal;

	private float Rayon = 8.5f;
	private float Amplitude = 0.5f;
	private float Speed = 0.0f;
	private float targetSpeed = 0.0f;
	private float jerk = 0.0f;
	private float Delay;
	private float freq = 2.0f;
	private float Accuracy;

	private IEnumerator red_coroutine = null;

	private WaitForSeconds waitforseconds = new WaitForSeconds(1.0f);
	private WaitForSeconds waitforseconds_spawn = new WaitForSeconds(15.0f);
	private WaitForSeconds waitforseconds_particle = new WaitForSeconds(0.6f);

	private ParticleSystem JetPack;
	private ParticleSystem Smoke;
	private ParticleSystem Explosion;
	private ParticleSystem Spawn;

	private bool shouldFireAtSnake = false;
	private bool isFiringMissile = false;
	private bool visible = true;

	private GameManagerV1 gameManager;
	private SnakeControllerV3 snakeScript;
	private CameraScript cameraScript;
	private PlanetScript planetScript;


	void Awake()
	{
		myTransform = transform;
		myRabbit = myTransform.Find("Rabbit");
		Body = myRabbit.Find("Body").gameObject;

		JetPack = myTransform.Find("Rabbit/JetPack").GetComponent<ParticleSystem>();
		Smoke = myTransform.Find("Rabbit/Smoke").GetComponent<ParticleSystem>();
		Explosion = myTransform.Find("Rabbit/Explosion").GetComponent<ParticleSystem>();
		Spawn = myTransform.Find("Rabbit/Spawn").GetComponent<ParticleSystem>();
		
		Snake = GameObject.FindWithTag("Player").transform;
		
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		snakeScript = Snake.GetComponent<SnakeControllerV3>();
		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();

		targetSpeed = gameManager.WorldSetting.RedRabbitSpeed;
		Speed = targetSpeed;
		Delay = gameManager.WorldSetting.RedRabbitDelay;
		Accuracy = 1.0f - gameManager.WorldSetting.RedRabbitAccuracy;
	}

	void Update()
	{
		if(State != RedRabbitState.Dead)
		{
			myTransform.RotateAround(Center, Axe, Speed * Time.deltaTime);

			if(State == RedRabbitState.Normal)
			{
				myRabbit.position = myTransform.position + myTransform.up * Amplitude * Mathf.Sin(Time.time * freq);
			}
			else if(State == RedRabbitState.Burn)
			{
				Speed += Time.deltaTime * 175.0f;
				myRabbit.Rotate(AxeRotation * 600.0f * Time.deltaTime);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Fire") && State == RedRabbitState.Normal)
		{
			JetPackFailure();
		}
		else if(other.CompareTag("Planet") && State == RedRabbitState.Burn)
		{
			StartCoroutine(Destruction());
		}
	}

	public void JetPackFailure()
	{
		State = RedRabbitState.Burn;

		JetPack.Stop();
		Smoke.Play();
		Speed = 0.0f;

		float sign_1 = 1.0f - Random.Range(0, 1) * 2.0f;
		float sign_2 = 1.0f - Random.Range(0, 1) * 2.0f;

		Center=myTransform.position+myTransform.right*sign_1*Random.Range(3.5f, 6.0f)+myTransform.forward*sign_2*Random.Range(3.5f, 6.0f);
		Axe = Vector3.Cross(myTransform.position - Center, myTransform.up).normalized;
		AxeRotation = myTransform.right * Random.Range(-1.0f, 1.0f) + myTransform.forward * Random.Range(-1.0f, 1.0f);
	}

	public void RedRabbitManager(bool act)
	{
		shouldFireAtSnake = act;

		if(red_coroutine != null)
			StopCoroutine(red_coroutine);
		red_coroutine = RedRabbitManagerCoroutine();
		StartCoroutine(red_coroutine);
	}

	private IEnumerator RedRabbitManagerCoroutine()
	{
		if(shouldFireAtSnake)
		{
			float clock = 0.0f;

			while(shouldFireAtSnake && State == RedRabbitState.Normal)
			{
				if(visible)
				{
					float dist = Vector3.Distance(Snake.position, myTransform.position);

					if(dist > gameManager.WorldSetting.RedRabbitRange)
					{
						isFiringMissile = false;
						Speed = Mathf.SmoothDamp(Speed, targetSpeed, ref jerk, 0.25f);
						myRabbit.localRotation = Quaternion.RotateTowards(myRabbit.localRotation, Quaternion.identity, 200.0f * Time.deltaTime);
					}
					else if(snakeScript.State == SnakeState.Run && snakeScript.Face == Face)
					{
						isFiringMissile = true;
						Speed = Mathf.SmoothDamp(Speed, 0.0f, ref jerk, 0.25f);
						targetRotation = Quaternion.LookRotation(Snake.position - Snake.forward * 4.45f - myTransform.position, -Axe);
						myRabbit.rotation = Quaternion.RotateTowards(myRabbit.rotation, targetRotation, 300.0f * Time.deltaTime);

						clock += Time.deltaTime;
						if(clock > Delay && Quaternion.Angle(myRabbit.rotation, targetRotation) < 5.0f)
						{
							float rabbit_dist = Vector3.Distance(myRabbit.position, Snake.position);
							float rayon = (Mathf.Pow(rabbit_dist, 2) / 10.0f) * Random.Range(-Accuracy + 1.0f, 1.0f + Accuracy);
							Quaternion targetMissile = Quaternion.LookRotation(Snake.position + myTransform.up * 4.5f - myTransform.position, -Axe);
							GameObject myMissile = Instantiate(Missile, myTransform.position, targetMissile);
							myMissile.GetComponent<MissileScript>().Launch(myTransform, rayon);

							clock = 0.0f;
						}
					}
				}

				yield return null;
			}

			shouldFireAtSnake = false;
			isFiringMissile = false;
		}
		else
		{
			while(!shouldFireAtSnake && State == RedRabbitState.Normal 
			&& (Speed + 0.1f < targetSpeed || Quaternion.Angle(myRabbit.localRotation, Quaternion.identity) > 1.0f))
			{
				Speed = Mathf.SmoothDamp(Speed, targetSpeed, ref jerk, 0.25f);
				myRabbit.localRotation = Quaternion.RotateTowards(myRabbit.localRotation, Quaternion.identity, 200.0f * Time.deltaTime);
				yield return null;
			}
		}

		red_coroutine = null;
	}

	public void Setup(Vector3 pos, Quaternion quat)
	{
		State = RedRabbitState.Normal;

		InitialCenter = pos;
		Center = pos;

		InitialRotation = quat;
		myTransform.rotation = quat;

		myTransform.position = Center + myTransform.right * Rayon;
		Axe = - myTransform.up;

		Delay = gameManager.WorldSetting.RedRabbitDelay;
		Speed = targetSpeed;

		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();

		RedRabbitManager(true);

		if(!Body.activeInHierarchy)
			StartCoroutine(SpawnCoroutine());
	}

	private IEnumerator SpawnCoroutine()
	{
		Spawn.Play();
		yield return waitforseconds_particle;
		Body.SetActive(true);
	}

	private IEnumerator Destruction()
	{
		State = RedRabbitState.Dead;

		Body.SetActive(false);
		myRabbit.localRotation = Quaternion.identity;
		Smoke.Stop();
		Explosion.Play();

		if(visible)
			cameraScript.Shake(1.0f);

		yield return waitforseconds;

		myTransform.localPosition = Vector3.zero;
		myTransform.localRotation = Quaternion.identity;

		if(planetScript.DestroyedFaces.Contains(Face))
		{
			gameObject.SetActive(false);
		}
		else
		{
			yield return waitforseconds_spawn;
			Setup(InitialCenter, InitialRotation);
		}
	}

	void OnBecameVisible()
	{
		if(State == RedRabbitState.Normal)
			JetPack.Play();
		visible = true;
	}

	void OnBecameInvisible()
	{
		if(State == RedRabbitState.Normal)
			JetPack.Stop();
		if(!isFiringMissile)
			visible = false;
	}
}