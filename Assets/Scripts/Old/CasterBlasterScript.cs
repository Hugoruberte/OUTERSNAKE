using UnityEngine;
using System.Collections;
using Tools;

public class CasterBlasterScript : MonoBehaviour
{
	private SnakeManagement snakeManag;
	private GameManagerV1 gameManager;
	private SnakeControllerV3 snakeScript;

	private Transform myTransform;

	private LineRenderer line;
	private ParticleSystem particle;

	private WaitForSeconds waitforseconds_duration;
	private WaitForSeconds waitforseconds_loading;

	private bool Fire = false;
	private bool Closing = false;
	private bool snake_boolean = false;
	private bool Finished = false;

	private float jerk = 0.0f;
	private float scale = 0.0f;
	private const float Length = 1000.0f;


	void Awake()
	{
		myTransform = transform;

		line = myTransform.Find("Casting").GetComponent<LineRenderer>();

		line.startWidth = 0;
		line.endWidth = 0;
		line.useWorldSpace = false;
		line.SetPosition(1, Vector3.zero);
		
		if(GameObject.FindWithTag("Player") != null)
		{
			snakeManag = GameObject.FindWithTag("Player").GetComponent<SnakeManagement>();
			snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
			snake_boolean = true;
		}
		else
		{
			snake_boolean = false;
		}

		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		waitforseconds_duration = new WaitForSeconds(gameManager.WorldSetting.CasterBlasterDuration);
		waitforseconds_loading = new WaitForSeconds(gameManager.WorldSetting.CasterBlasterLoading);
		
		particle = myTransform.Find("Loading").GetComponent<ParticleSystem>();
	}

	void Update()
	{
		if(Fire && snake_boolean && snakeScript.State == SnakeState.Run && snakeManag.Health != SnakeHealth.Invincible)
		{
			Detection();
		}
	}

	public void Go()
	{
		StartCoroutine(Setup());
	}

	private IEnumerator Setup()
	{
		particle.Play();
		line.SetPosition(1, new Vector3(0, 0, Length));

		line.startColor.SetColorA(1.0f);
		line.endColor.SetColorA(1.0f);

		scale = 0.0f;
		while(scale < 0.045f)
		{
			scale = Mathf.SmoothDamp(scale, 0.05f, ref jerk, 0.4f);
			line.startWidth = scale;
			line.endWidth = scale;
			yield return null;
		}

		line.startWidth = 0.05f;
		line.endWidth = 0.05f;

		yield return waitforseconds_loading;

		scale = 0.0f;
		while(scale < 0.95f)
		{
			scale = Mathf.SmoothDamp(scale, 1.0f, ref jerk, 0.025f);
			line.SetPosition(1, new Vector3(0, 0, scale * Length));
			line.startWidth = scale;
			line.endWidth = scale;
			yield return null;
		}

		Fire = true;
		StartCoroutine(Exit());

		yield return waitforseconds_duration;

		if(!Closing)
			StartCoroutine(Close());

		Finished = true;
	}

	private void Detection()
	{
		RaycastHit[] hits;
		hits = Physics.RaycastAll(myTransform.position, myTransform.forward, 200.0f);

		for(int j = 0; j < hits.Length; j++)
		{
			RaycastHit hit = hits[j];

			if(hit.transform.CompareTag("Player"))
			{
				gameManager.StartCoroutine(gameManager.Blink());
				snakeManag.BodyNumber -= gameManager.WorldSetting.CasterBlasterDamage * 10;
				if(snakeManag.BodyNumber < 2)
					gameManager.KilledBy = Killer.CasterBlaster;
				if(!Closing)
					StartCoroutine(Close());
				Fire = false;
				break;
			}
			else if(hit.transform.CompareTag("SnakeBody"))
			{
				gameManager.StartCoroutine(gameManager.Blink());
				snakeManag.BodyNumber -= gameManager.WorldSetting.CasterBlasterDamage;
				if(snakeManag.BodyNumber < 2)
					gameManager.KilledBy = Killer.CasterBlaster;
				Fire = false;
				break;
			}
			else if(hit.transform.CompareTag("Rabbit"))
			{
				hit.transform.GetComponent<RabbitManagement>().SetFire(true);
			}
		}
	}

	private IEnumerator Close()
	{
		Closing = true;
		while(scale > 0.025f)
		{
			scale = Mathf.SmoothDamp(scale, 0.0f, ref jerk, 0.25f);

			if(scale < 0.5f && Fire)
				Fire = false;

			line.startColor.SetColorA(scale);
			line.endColor.SetColorA(scale);

			line.startWidth = scale;
			line.endWidth = scale;
			yield return null;
		}

		scale = 0.0f;
		line.startWidth = 0.0f;
		line.endWidth = 0.0f;
		Closing = false;
	}

	private IEnumerator Exit()
	{
		Vector3 targetPosition = myTransform.position - myTransform.forward * 500.0f;
		Vector3 velocity = Vector3.zero;

		while(Vector3.Distance(myTransform.position, targetPosition) > 10.0f)
		{
			myTransform.position = Vector3.SmoothDamp(myTransform.position, targetPosition, ref velocity, 0.75f);
			yield return null;
		}

		while(Fire || Closing || !Finished)
			yield return null;

		myTransform.localPosition = Vector3.zero;
		myTransform.gameObject.SetActive(false);
	}
}