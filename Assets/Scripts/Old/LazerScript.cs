using UnityEngine;
using System.Collections;
using Tools;

public class LazerScript : MonoBehaviour
{
	private Transform Canon;
	private Transform Snake;
	private Transform Sight;
	private Transform SightSize;
	private Transform Spark;
	private Transform LazerEffect;
	private Transform myTransform;
	private Transform Star;

	private ParticleSystem myParticle;
	private LineRenderer myLine;
	private TrailRenderer myTrail;
	private LensFlare myFlare;
	private LineRenderer mySightLine;
	private ParticleSystem mySparkParticle;
	private LensFlare mySparkFlare;

	private IEnumerator blink_coroutine;
	private IEnumerator damage_coroutine;

	private BoxCollider ColliderRange;

	private Renderer SightRend;

	private Color SightColor;

	private float latence = 0.7f;

	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;
	private GameManagerV1 gameManager;
	private CameraScript cameraScript;

	private bool Pursuit = false;
	private bool Contact = false;

	private Vector3 LazerEnd;


	void Awake()
	{
		myTransform = transform;
		Snake = GameObject.FindWithTag("Player").transform;

		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		snakeManag = GameObject.FindWithTag("Player").GetComponent<SnakeManagement>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();

		ColliderRange = GetComponent<BoxCollider>();
		ColliderRange.size = new Vector3(gameManager.WorldSetting.LazerRange, 0.3f, gameManager.WorldSetting.LazerRange);

		LazerEnd = myTransform.position;
		
		Canon = myTransform.Find("Body/Canon");
		Spark = myTransform.Find("Spark");
		Star = myTransform.Find("Star");
		LazerEffect = Canon.Find("Effect");

		myTrail = LazerEffect.GetComponent<TrailRenderer>();
		myLine = Canon.GetComponent<LineRenderer>();
		myParticle = LazerEffect.GetComponent<ParticleSystem>();

		myFlare = LazerEffect.GetComponent<LensFlare>();
		
		mySparkParticle = Spark.GetComponent<ParticleSystem>();
		mySparkFlare = Spark.GetComponent<LensFlare>();
		
		Sight = myTransform.Find("Sight");
		SightSize = Sight.Find("Center");
		mySightLine = SightSize.GetChild(0).GetComponent<LineRenderer>();
		SightColor = SightSize.GetChild(0).GetComponent<LineRenderer>().startColor;
		SightRend = Sight.Find("Point").GetComponent<Renderer>();
	}

	void Start()
	{
		Star.gameObject.SetActive(false);
		myTrail.enabled = false;
		myLine.useWorldSpace = true;
		myLine.enabled = false;
		myFlare.enabled = false;
		mySparkFlare.enabled = false;
		Sight.gameObject.SetActive(false);
	}

	void OnEnable()
	{
		StartCoroutine(CanonRotationLoop());
		StartCoroutine(SnakeEnter());
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.name.Equals("SnakeDetector") && snakeScript.State == SnakeState.Run && !gameManager.Rocket)
		{
			Pursuit = true;
		}
		else if(other.name == "Boundarie")
		{
			myTransform.localPosition = Vector3.zero;
			gameObject.SetActive(false);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.name.Equals("SnakeDetector"))
		{
			SnakeExit();
		}
	}

	private IEnumerator SnakeEnter()
	{
		while(true)
		{
			while(!Pursuit)
				yield return null;

			LazerEnd = myTransform.position;

			Sight.gameObject.SetActive(true);
			myTrail.enabled = true;

			blink_coroutine = SightBlink();
			StartCoroutine(blink_coroutine);
			StartCoroutine(SightScaleAndPosition());

			float delay = gameManager.WorldSetting.LazerLoading;
			float clock = 0.0f;
			while(Pursuit && clock < delay - 0.2f)
			{
				clock += Time.deltaTime;
				yield return null;
			}
			if(Pursuit)
			{
				myParticle.Play();
			}
			while(Pursuit && clock < delay)
			{
				clock += Time.deltaTime;
				yield return null;
			}

			myTrail.enabled = false;

			if(Pursuit)
			{
				damage_coroutine = DamageLoop();
				StartCoroutine(damage_coroutine);
				StartCoroutine(LineScale());


				myFlare.enabled = true;

				mySparkFlare.enabled = true;
				mySparkFlare.brightness = 0.2f;
				mySparkParticle.Play();
				Star.gameObject.SetActive(true);

				bool touch = false;
				while(Pursuit && !gameManager.Rocket)
				{
					Contact = false;

					bool aim = false;
					RaycastHit hit;
					Transform trans;
					if(Physics.Raycast(LazerEffect.position, Canon.forward, out hit, 100.0f))
					{
						trans = hit.transform;

						if(trans == Snake || trans.CompareTag("SnakeBody"))
						{
							aim = true;
						}
						else if(trans.CompareTag("Rabbit"))
						{
							trans.GetComponent<RabbitManagement>().SetFire(true);
						}
					}

					Vector3 TheEnd = hit.point;

					if(Vector3.Distance(myTransform.position, TheEnd) > Vector3.Distance(myTransform.position, LazerEnd) 
						&& Vector3.Distance(LazerEnd, TheEnd) > 0.25f)
					{
						LazerEnd = Vector3.MoveTowards(LazerEnd, TheEnd, 70.0f * Time.deltaTime);
					}
					else
					{
						LazerEnd = TheEnd;
						Contact = aim;
					}

					if(!touch && Contact && snakeManag.Health != SnakeHealth.Invincible)
					{
						touch = true;
						gameManager.StartCoroutine(gameManager.Blink());
						cameraScript.Shake(1.0f);
					}

					myLine.SetPosition(0, LazerEffect.position);
					myLine.SetPosition(1, LazerEnd);

					Star.position = myTransform.position + myTransform.up * 0.8f + Canon.forward * 1.1f;

					Spark.position = LazerEnd + Spark.forward * 0.1f;
					Spark.rotation = Quaternion.LookRotation(Canon.position - Spark.position);
					
					yield return null;
				}

				StopCoroutine(blink_coroutine);
				StopCoroutine(damage_coroutine);
				Pursuit = false;

				myFlare.enabled = false;

				mySparkFlare.enabled = false;
				Star.gameObject.SetActive(false);
				mySparkParticle.Stop();

				Vector3 target = LazerEffect.position;
				while(Vector3.Distance(target, LazerEnd) > 0.1f)
				{
					target = Vector3.MoveTowards(target, LazerEnd, 35.0f * Time.deltaTime);
					myLine.SetPosition(0, target);
					yield return null;
				}
			}
		}
	}

	public void SnakeExit()
	{
		if(blink_coroutine != null)
			StopCoroutine(blink_coroutine);
		if(damage_coroutine != null)
			StopCoroutine(damage_coroutine);
		Pursuit = false;
	}

	private IEnumerator DamageLoop()
	{
		while(true)
		{
			if(Contact && snakeManag.Health != SnakeHealth.Invincible)
			{
				snakeManag.FloatBodyNumber -= gameManager.WorldSetting.LazerDamagePerSecond * Time.deltaTime;

				if(snakeManag.BodyNumber < 2)
				{
					StopCoroutine(blink_coroutine);
					Pursuit = false;
					cameraScript.Shake(1.0f);
					gameManager.StartCoroutine(gameManager.Blink());
					gameManager.KilledBy = Killer.Lazer;
					yield break;
				}
			}

			yield return null;
		}
	}

	private IEnumerator CanonRotationLoop()
	{
		while(true)
		{
			if(Pursuit)
			{
				Quaternion target = Quaternion.LookRotation(Snake.position - Canon.position);
				Canon.rotation = Quaternion.RotateTowards(Canon.rotation, target, 1000.0f * Time.deltaTime);
			}
			else
			{
				Canon.Rotate(Vector3.up, Time.deltaTime * 40.0f);

				if(Canon.localEulerAngles.x != 0)
				{
					Canon.localEulerAngles = new Vector3(0, Canon.localEulerAngles.y, 0);
				}
			}

			yield return null;
		}
	}

	private IEnumerator LineScale()
	{
		myLine.enabled = true;
		myLine.widthMultiplier = 0.0f;

		myLine.startColor.SetColorA(1.0f);
		myLine.endColor.SetColorA(1.0f);

		float value = 0.0f;

		while(Pursuit && value < 0.5f)
		{
			value = Mathf.MoveTowards(value, 0.5f, 5.0f * Time.deltaTime);
			myLine.widthMultiplier = value;
			yield return null;
		}

		while(Pursuit && value > 0.1f)
		{
			value = Mathf.MoveTowards(value, 0.1f, 5.0f * Time.deltaTime);
			myLine.widthMultiplier = value;
			yield return null;
		}

		while(Pursuit)
			yield return null;

		while(value > 0.005f)
		{
			value = Mathf.MoveTowards(value, 0.0f, 0.5f * Time.deltaTime);
			myLine.widthMultiplier = value;

			myLine.startColor.SetColorA(value * 10.0f);
			myLine.endColor.SetColorA(value * 10.0f);
			yield return null;
		}

		myLine.SetPosition(0, LazerEffect.position);
		myLine.SetPosition(1, LazerEffect.position);

		myLine.enabled = false;
	}

	private IEnumerator SightBlink()
	{
		float length;
		float clock;
		float chrono = gameManager.WorldSetting.LazerLoading;
		float fmax = 0.1f;
		float fmin = 0.035f;

		while(chrono > 0.0f)
		{
			length = (chrono > latence) ? fmax : fmin;

			SightRend.material.color = Color.white;

			clock = 0.0f;
			while(clock < length)
			{
				chrono -= Time.deltaTime;
				clock += Time.deltaTime;
				yield return null;
			}

			SightRend.material.color = SightColor;

			clock = length;
			while(clock > 0.0f)
			{
				chrono -= Time.deltaTime;
				clock -= Time.deltaTime;
				yield return null;
			}
		}
	}

	private IEnumerator SightScaleAndPosition()
	{
		Vector3 startsize = new Vector3(4, 1, 4);
		Vector3 endsize = new Vector3(1.5f, 1, 1.5f);
		float value = 0.0f;
		float speed = Vector3.Distance(startsize, endsize) / (1.5f - latence);

		mySightLine.startColor.SetColorA(0.0f);
		mySightLine.endColor.SetColorA(0.0f);
		SightSize.localScale = startsize;

		while(Pursuit && !(value > 0.99f && Vector3.Distance(SightSize.localScale, endsize) < 0.05f))
		{
			SightSize.localScale = Vector3.MoveTowards(SightSize.localScale, endsize, speed * Time.deltaTime);
			Sight.position = Snake.position - Snake.forward;

			value = Mathf.MoveTowards(value, 1.0f, 5.0f * Time.deltaTime);
			mySightLine.startColor.SetColorA(value);
			mySightLine.endColor.SetColorA(value);
			yield return null;
		}

		while(Pursuit)
		{
			Sight.position = Snake.position - Snake.forward;
			yield return null;
		}

		while(!Pursuit && value > 0.0f)
		{
			value = Mathf.MoveTowards(value, 0.0f, 5.0f * Time.deltaTime);
			mySightLine.startColor.SetColorA(value);
			mySightLine.endColor.SetColorA(value);
			yield return null;
		}

		if(Pursuit)
			yield break;

		mySightLine.startColor.SetColorA(0.0f);
		mySightLine.endColor.SetColorA(0.0f);

		Sight.gameObject.SetActive(false);
		Sight.localPosition = new Vector3(0, 1, 0);
	}
}