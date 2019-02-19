using UnityEngine;
using System.Collections;

public class SuperLazerScript : MonoBehaviour
{
	private GameManagerV1 gameManager;
	private SnakeManagement snakeManag;
	private SnakeControllerV3 snakeScript;

	private Transform myTransform;
	private Transform Armchair;

	private WaitForSeconds waitforseconds_05 = new WaitForSeconds(0.5f);
	private WaitForSeconds waitforseconds_loading;

	private Transform[] targets = new Transform[4];

	private Vector3 location1 = new Vector3(-30.5f, 0, 0);
	private Vector3 location2 = new Vector3(0, -30.5f, 0);

	private LineRenderer[] lines = new LineRenderer[4];
	private Vector3[] directions;

	private ParticleSystem particle1;
	private ParticleSystem particle2;
	private ParticleSystem particle3;
	private ParticleSystem particle4;

	private bool Fire = false;

	private Vector3 targetScale = Vector3.zero;

	private float jerk = 0.0f;
	private float scale = 0.0f;
	private float duration = 0.0f;
	private int index = -1;


	void Awake()
	{
		myTransform = transform;
		Armchair = GameObject.FindWithTag("Armchair").transform;

		for(int i = 0; i < 4; i++)
		{
			targets[i] = myTransform.Find("target" + (i+1).ToString());
			targets[i].localScale = Vector3.zero;

			lines[i] = targets[i].GetComponent<LineRenderer>();
			lines[i].startWidth = 0;
			lines[i].endWidth = 0;
			lines[i].useWorldSpace = false;
		}

		lines[0].SetPosition(1, location1);
		lines[1].SetPosition(1, location2);
		lines[2].SetPosition(1, location2);
		lines[3].SetPosition(1, location1);
		
		snakeManag = GameObject.FindWithTag("Player").GetComponent<SnakeManagement>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();

		duration = gameManager.WorldSetting.SuperLazerDuration;
		waitforseconds_loading = new WaitForSeconds(gameManager.WorldSetting.SuperLazerLoading);
		
		particle1 = targets[0].Find("Particle1").GetComponent<ParticleSystem>();
		particle2 = targets[1].Find("Particle2").GetComponent<ParticleSystem>();
		particle3 = targets[2].Find("Particle3").GetComponent<ParticleSystem>();
		particle4 = targets[3].Find("Particle4").GetComponent<ParticleSystem>();
	}

	void Update()
	{
		if(Fire && snakeScript.State == SnakeState.Run && snakeManag.Health != SnakeHealth.Invincible)
		{
			for(int i = 0; i < 4; i++)
			{
				if(i != index)
					Detection(targets[i].position, targets[(i+1)%4].position);
			}
		}
	}

	private void Detection(Vector3 start, Vector3 end)
	{
		RaycastHit[] hits;
		RaycastHit hit;
		hits = Physics.RaycastAll(start, end - start, 30.0f);
		int Place;

		for(int j = 0; j < hits.Length; j++)
		{
			hit = hits[j];

			if(hit.transform.CompareTag("Player"))
			{
				gameManager.StartCoroutine(gameManager.Blink());
				gameManager.KilledBy = Killer.SuperLazer;

				Fire = false;
				snakeManag.DestroySnake();
			}
			else if(hit.transform.CompareTag("SnakeBody"))
			{
				SnakeFollow script = hit.transform.GetComponent<SnakeFollow>();
				Place = script.Place;

				snakeManag.ReduceSnake(Place);
				gameManager.StartCoroutine(gameManager.Blink());
				script.Explosion();
			}
		}
	}

	public IEnumerator Setup()
	{
		FindOrientation();

		particle1.Play();
		particle2.Play();
		particle3.Play();
		particle4.Play();

		int i;

		targetScale = Vector3.zero;
		while(Vector3.Distance(targetScale, Vector3.one) > 0.1 && snakeScript.State == SnakeState.Run)
		{
			targetScale = Vector3.MoveTowards(targetScale, Vector3.one, 10.0f * Time.deltaTime);
			for(i = 0; i < 4; i++)
			{
				targets[i].localScale = targetScale;
			}
			yield return null;
		}

		for(i = 0; i < 4; i++)
		{
			targets[i].localScale = Vector3.one;
		}

		scale = 0.0f;

		while(scale < 0.074f && snakeScript.State == SnakeState.Run)
		{
			scale = Mathf.SmoothDamp(scale, 0.075f, ref jerk, 0.5f);
			for(i = 0; i < 4; i++)
			{
				if(i != index)
				{
					lines[i].startWidth = scale;
					lines[i].endWidth = scale;
				}
			}
			yield return null;
		}

		for(i = 0; i < 4; i++)
		{
			if(i != index)
			{
				lines[i].startWidth = 0.075f;
				lines[i].endWidth = 0.075f;
			}
		}

		if(snakeScript.State == SnakeState.Run)
			yield return waitforseconds_loading;

		particle1.Stop();
		particle2.Stop();
		particle3.Stop();
		particle4.Stop();

		if(snakeScript.State == SnakeState.Run)
			yield return waitforseconds_05;

		scale = 0.0f;

		while(scale < 0.95f && snakeScript.State == SnakeState.Run)
		{
			scale = Mathf.SmoothDamp(scale, 1.0f, ref jerk, 0.025f);
			for(i = 0; i < 4; i++)
			{
				if(i != index)
				{
					lines[i].startWidth = scale;
					lines[i].endWidth = scale;
				}
			}
			yield return null;
		}

		scale = 1.0f;
		Fire = true;

		float clock = 0.0f;
		while(clock < duration && snakeScript.State == SnakeState.Run)
		{
			clock += Time.deltaTime;
			yield return null;
		}

		StartCoroutine(Close());
	}

	private IEnumerator Close()
	{
		int i;

		while(scale > 0.01f)
		{
			scale = Mathf.SmoothDamp(scale, 0.0f, ref jerk, 0.5f);

			if(scale < 0.1f && Fire)
				Fire = false;
			for(i = 0; i < 4; i++)
			{
				if(i != index)
				{
					lines[i].startWidth = scale;
					lines[i].endWidth = scale;
				}
			}
			yield return null;
		}

		scale = 0.0f;

		for(i = 0; i < 4; i++)
		{
			if(i != index)
			{
				lines[i].startWidth = 0.0f;
				lines[i].endWidth = 0.0f;
			}
		}

		while(Vector3.Distance(targetScale, Vector3.zero) > 0.1)
		{
			targetScale = Vector3.MoveTowards(targetScale, Vector3.zero, 10.0f * Time.deltaTime);
			for(i = 0; i < 4; i++)
			{
				targets[i].localScale = targetScale;
			}
			yield return null;
		}

		for(i = 0; i < 4; i++)
		{
			targets[i].localScale = Vector3.zero;
		}

		yield return waitforseconds_05;

		gameObject.SetActive(false);
	}

	private void FindOrientation()
	{
		directions = new Vector3[4] {myTransform.up, -myTransform.forward, -myTransform.up, myTransform.forward};
		for(int i = 0; i < 4; i++)
		{
			if(Mathf.RoundToInt(Vector3.Dot(directions[i], Armchair.forward)) == 1)
			{
				index = i;
				return;
			}
		}

		index = -1;
	}
}