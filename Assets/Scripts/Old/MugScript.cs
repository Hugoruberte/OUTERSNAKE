using UnityEngine;
using System.Collections;
using System.Linq;

public class MugScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform Armchair;

	private Vector3 BasicScale;

	private ParticleSystem mySmoke;

	[HideInInspector]
	public int pos_index;

	private ArmchairScript looneyScript;
	private GameManagerV1 gameManager;

	void Awake()
	{
		myTransform = transform;
		Armchair = GameObject.FindWithTag("Armchair").transform;
		looneyScript = Armchair.GetComponent<ArmchairScript>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		mySmoke = myTransform.Find("Smoke").GetComponent<ParticleSystem>();
	}

	void Start()
	{
		BasicScale = myTransform.localScale;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			Vector3[] Positions = looneyScript.Positions;
			Vector3 pos;
			Vector3 Planet = gameManager.MainPlanet.position;

			do
			{
				int sign = 1 - Random.Range(0, 2) * 2;
				if(Random.Range(0, 2) == 0)
				{
					pos = Planet + Armchair.forward * 15.25f + sign*(Random.Range(3, 11)+0.5f)*Armchair.up + (Random.Range(-10, 11)+0.5f)*Armchair.right;
				}
				else
				{
					pos = Planet + Armchair.forward * 15.25f + (Random.Range(-10, 11)+0.5f)*Armchair.up + sign*(Random.Range(3, 11)+0.5f)*Armchair.right;
				}
			}
			while(Positions.Contains(pos));

			myTransform.position = pos;
			looneyScript.Positions[pos_index] = pos;

			if(myTransform.localScale != BasicScale)
				myTransform.localScale = Vector3.zero;
			else
				StartCoroutine(MugingScale());
		}
	}

	public IEnumerator MugingBreath()
	{
		Vector3 center = myTransform.position;
		Vector3 targetPosition = center;
		Vector3 reference = Vector3.zero;
		float freq = Random.Range(0.05f, 0.25f);

		mySmoke.Play();

		while(looneyScript.Breath)
		{
			targetPosition = center + myTransform.forward *(-0.5f + Mathf.PingPong(Time.time * freq, 1.0f));
			myTransform.position = Vector3.SmoothDamp(myTransform.position, targetPosition, ref reference, 0.7f);
			yield return null;
		}

		myTransform.position = center;
		mySmoke.Stop();
	}

	private IEnumerator MugingScale()
	{
		myTransform.localScale = Vector3.zero;
		while(Vector3.Distance(myTransform.localScale, BasicScale) > 0.01f)
		{
			myTransform.localScale = Vector3.MoveTowards(myTransform.localScale, BasicScale, 2.5f * Time.deltaTime);
			yield return null;
		}
	}
}