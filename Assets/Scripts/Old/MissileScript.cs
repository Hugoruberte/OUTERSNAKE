using UnityEngine;
using System.Collections;
using Tools;

public class MissileScript : MonoBehaviour
{
	public GameObject Fire;
	private Transform myTransform;
	private Transform Snake;
	private Transform myRedRabbit;
	private Transform FiresParent;

	private WaitForSeconds waitforseconds_2 = new WaitForSeconds(2.0f);

	private bool Launched = false;
	private bool Loaded = false;

	private float Speed;
	private float clock = 0.0f;

	private Renderer myRend;
	private Collider myColl;

	private LensFlare myFlare;

	private Vector3 Center;
	private Vector3 Axe;

	private ParticleSystem myFlame;
	private ParticleSystem myExplosion;

	private GameManagerV1 gameManager;
	private CameraScript cameraScript;
	private SnakeManagement snakeManag;
	private RedRabbitController redRabbitScript;


	void Awake()
	{
		myTransform = transform;

		myFlame = myTransform.Find("Flame").GetComponent<ParticleSystem>();
		myExplosion = myTransform.Find("Explosion").GetComponent<ParticleSystem>();

		FiresParent = GameObject.Find("ObjectPoolingStock/FiresPooling").transform;

		Snake = GameObject.FindWithTag("Player").transform;
		snakeManag = Snake.GetComponent<SnakeManagement>();

		myFlare = myTransform.Find("Flash").GetComponent<LensFlare>();

		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();

		myRend = GetComponent<Renderer>();
		myColl = GetComponent<Collider>();
	}

	void Update()
	{
		clock += Time.deltaTime;
		if((clock > 5.0f || redRabbitScript.State == RedRabbitState.Dead) && Launched)
			Explosion();
	}

	public void Launch(Transform myRabbit, float rayon)
	{
		myRedRabbit = myRabbit;
		redRabbitScript = myRabbit.GetComponent<RedRabbitController>();
		Axe = myRedRabbit.Find("Rabbit").right;
		Center = myRedRabbit.position - myRedRabbit.up * rayon;
		Speed = 360.0f * gameManager.WorldSetting.MissileSpeed / rayon;
		
		myFlame.Play();

		StartCoroutine(Move());
	}

	private IEnumerator Move()
	{
		Launched = true;
		while(Launched)
		{
			myTransform.RotateAround(Center, Axe, Speed * Time.deltaTime);
			yield return null;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.transform == Snake)
		{
			Explosion();
			cameraScript.Shake(1.0f);

			if(snakeManag.Health != SnakeHealth.Invincible)
			{
				gameManager.StartCoroutine(gameManager.Blink());
				snakeManag.BodyNumber -= gameManager.WorldSetting.MissileDamage * 8;

				if(snakeManag.BodyNumber < 2)
					gameManager.KilledBy = Killer.RedRabbit;
			}
		}
		else if(other.CompareTag("SnakeBody"))
		{
			Explosion();
			cameraScript.Shake(1.0f);

			if(snakeManag.Health != SnakeHealth.Invincible)
			{
				gameManager.StartCoroutine(gameManager.Blink());
				snakeManag.BodyNumber -= gameManager.WorldSetting.MissileDamage;

				if(snakeManag.BodyNumber < 2)
					gameManager.KilledBy = Killer.RedRabbit;
			}
		}
		else if(other.CompareTag("Fire") || other.CompareTag("Bounds"))
		{
			cameraScript.Shake(Shaketype.Gentle);
			Explosion();
		}
		else if(other.CompareTag("Planet"))
		{
			cameraScript.Shake(Shaketype.Gentle);
			Explosion();

			Quaternion rot = FindRotation(other.transform);
			Vector3 pos = other.ClosestPoint(myTransform.position);

			GameObject fire = Instantiate(Fire, pos, rot);
			fire.name = "Fire";
			fire.transform.position = Vector3Extension.RoundToInt(fire.transform.position + 0.5f * fire.transform.up);

			fire.transform.parent = FiresParent;
		}
		else if(other.CompareTag("RedRabbit") && Loaded)
		{
			cameraScript.Shake(Shaketype.Gentle);
			Explosion();
			redRabbitScript.JetPackFailure();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("RedRabbit"))
		{
			Loaded = true;
		}
	}

	private void Explosion()
	{
		Launched = false;

		myRend.enabled = false;
		myColl.enabled = false;
		myExplosion.Play();
		myFlame.Stop();

		StartCoroutine(Flash());
	}

	private IEnumerator Flash()
	{
		while(myRend.enabled)
			yield return null;

		float value = 0.0f;

		while(value < 0.4f)
		{
			value = Mathf.MoveTowards(value, 0.4f, 3.5f * Time.deltaTime);
			myFlare.brightness = value;
			yield return null;
		}

		while(value > 0.0f)
		{
			value = Mathf.MoveTowards(value, 0.0f, 3.5f * Time.deltaTime);
			myFlare.brightness = value;
			yield return null;
		}

		yield return waitforseconds_2;

		GameObject Poubelle = GameObject.Find("Poubelle");

		if(Poubelle)
		{
			myTransform.parent = Poubelle.transform;
			myTransform.localPosition = Vector3.zero;
			myTransform.rotation = Quaternion.identity;
			gameObject.SetActive(false);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private Quaternion FindRotation(Transform planet)
	{
		Transform Faces = planet.Find("Body/Faces");
		float min = 1000000.0f;
		float dist = 0.0f;
		int index = 0;

		for(int i = 0; i < 6; i++)
		{
			dist = Vector3.Distance(Faces.GetChild(i).position, myTransform.position);
			if(dist < min)
			{
				min = dist;
				index = i;
			}
		}

		return Faces.GetChild(index).rotation * Quaternion.Euler(90, 0, 0);
	}
}