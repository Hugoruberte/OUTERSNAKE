using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBaseScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform Main;
	private Renderer QuadRenderer;

	private IEnumerator activatedCoroutine = null;
	private IEnumerator moveCoroutine = null;

	private bool Validated = false;
	[HideInInspector]
	public bool Yellow = false;

	private SwitchGateScript switchScript;
	private SnakeManagement snakeManag;
	private GameManagerV1 gameManager;
	private CameraScript cameraScript;

	void Awake()
	{
		myTransform = transform;
		QuadRenderer = myTransform.Find("Body/Quad").GetComponent<Renderer>();

		snakeManag = GameObject.FindWithTag("Player").GetComponent<SnakeManagement>();
		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
	}

	void Start()
	{
		Main = myTransform.parent.parent;
		switchScript = Main.GetComponent<SwitchGateScript>();
		QuadRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.75f,0.75f));
		Yellow = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") && !Validated)
		{
			if(snakeManag.Health != SnakeHealth.Invincible && activatedCoroutine != null)
			{
				cameraScript.Shake(1.0f);
				gameManager.KilledBy = Killer.OccupiedGate;
				gameManager.StartCoroutine(gameManager.Blink());
				snakeManag.DestroySnake();
				return;
			}

			switchScript.SwitchActualNumber ++;
			QuadRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.5f,0.5f));
			Yellow = true;
			activatedCoroutine = ActivatedCoroutine(other.transform.forward);
			StartCoroutine(activatedCoroutine);
		}
	}

	private IEnumerator ActivatedCoroutine(Vector3 direction)
	{
		float clock = 0f;
		Transform hit;
		RaycastHit[] hits;
		bool Activated = true;

		while(Activated && !Validated)
		{
			clock += Time.deltaTime;

			if(clock > 0.1f)
			{
				hits = Physics.RaycastAll(myTransform.position - direction*1.5f, direction, 3f);

				for(int i = 0; i < hits.Length; i++)
				{
					hit = hits[i].transform;
					Activated = (hit.CompareTag("Player") || hit.CompareTag("SnakeBody"));
					if(Activated)
						break;
				}

				clock = 0.0f;
			}

			yield return null;
		}

		QuadRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.75f,0.75f));
		Yellow = false;
		switchScript.SwitchActualNumber --;
		activatedCoroutine = null;
	}

	public void Validating()
	{
		QuadRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.25f,0.5f));
		Yellow = false;
		Validated = true;
		StartCoroutine(BlinkingCoroutine());
	}

	private IEnumerator BlinkingCoroutine()
	{
		int count = 0;
		WaitForSeconds waitforseconds_01 = new WaitForSeconds(0.1f);
		Yellow = false;

		while(count ++ < 10)
		{
			if(count%2==0)
				QuadRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.25f,0.5f));
			else
				QuadRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.75f,0.75f));

			yield return waitforseconds_01;
		}
	}

	public void MoveTowards(Vector3 pos, float speed)
	{
		/*if(yellow)
			return;*/

		if(moveCoroutine != null)
			StopCoroutine(moveCoroutine);
		moveCoroutine = MoveTowardsCoroutine(pos, speed);
		StartCoroutine(moveCoroutine);
	}

	private IEnumerator MoveTowardsCoroutine(Vector3 pos, float speed)
	{
		while(Vector3.Distance(myTransform.position, pos) > 0.05f)
		{
			myTransform.position = Vector3.MoveTowards(myTransform.position, pos, speed * Time.deltaTime);
			yield return null;
		}

		myTransform.position = pos;
	}
}
