using UnityEngine;
using System.Collections;
using Tools;

public class MeteoreScript : MonoBehaviour
{
	private CameraScript cameraScript;
	private GameManagerV1 gameManager;

	private GameObject Fire;
	private Transform Body;
	private Transform myTransform;

	private WaitForSeconds waitforseconds_destroy = new WaitForSeconds(1.0f);
	private WaitForSeconds waitforseconds_05 = new WaitForSeconds(0.5f);

	private Light myLight;

	private ParticleSystem Flame;
	private ParticleSystem Explosion;
	private ParticleSystem Trail;

	private float Speed = 20.0f;

	private bool visible = true;
	private bool CancelFire = false;

	private Collider BodyCollider;
	private Collider FireCollider;

	private Renderer WarnRenderer;
	private Renderer BodyRenderer;


	void Awake()
	{
		myTransform = transform;
		Fire = myTransform.Find("Fire").gameObject;
		Flame = Fire.transform.Find("Flame").GetComponent<ParticleSystem>();
		myLight = Fire.transform.Find("Light").GetComponent<Light>();
		Body = myTransform.Find("Body");
		FireCollider = GetComponent<Collider>();

		Fire.SetActive(false);
		FireCollider.enabled = false;

		BodyCollider = Body.GetComponent<Collider>();
		BodyRenderer = Body.GetComponent<Renderer>();
		WarnRenderer = myTransform.Find("Warning").GetComponent<Renderer>();

		Explosion = Body.GetComponent<ParticleSystem>();
		Trail = Body.Find("Trail").GetComponent<ParticleSystem>();

		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();

		int start_height = gameManager.WorldSetting.MeteoreHeight;
		Body.localPosition = new Vector3(0, 0, -start_height);
		Speed = gameManager.WorldSetting.MeteoreSpeed;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Fire") && Vector3.Distance(myTransform.position, other.transform.position) < 1.5f)
		{
			CancelFire = true;
		}
		else if(other.CompareTag("Bounds"))
		{
			Destroy(gameObject);
		}
	}

	public void Setup()
	{
		Trail.Play();
		WarnRenderer.enabled = true;
		StartCoroutine(Falling());
	}

	private IEnumerator Falling()
	{
		while(Body.localPosition.z != 0.0f)
		{
			Body.SetLocalPositionZ(Mathf.MoveTowards(Body.localPosition.z, 0.0f, Speed * Time.deltaTime));
			yield return null;
		}

		if(visible && !gameManager.Safe)
			cameraScript.Shake(1.0f);

		WarnRenderer.enabled = false;
		BodyRenderer.enabled = false;
		BodyCollider.enabled = false;

		Explosion.Play();
		Trail.Stop();
		StartCoroutine(Desactive());

		yield return null;

		if(!CancelFire)
		{
			Fire.SetActive(true);
			FireCollider.enabled = true;
			float duration = Flame.main.duration;
			float speed = 6.0f / duration;

			while(Flame.isPlaying)
			{
				myLight.range = Mathf.MoveTowards(myLight.range, 0.0f, speed * Time.deltaTime);
				if(myLight.range < 1.0f)
					FireCollider.enabled = false;
				yield return null;
			}
		}

		yield return waitforseconds_destroy;

		Destroy(gameObject);
	}

	private IEnumerator Desactive()
	{
		yield return waitforseconds_05;
		Body.gameObject.SetActive(false);
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