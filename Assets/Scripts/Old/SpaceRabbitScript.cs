using UnityEngine;
using System.Collections;

public class SpaceRabbitScript : MonoBehaviour
{
	private Transform myTransform;

	private float OmegaX;
	private float OmegaY;
	private float OmegaZ;
	private float loop;

	private bool visible = true;

	[HideInInspector]
	public Vector3 startPosition;
	private Vector3 targetPosition;
	private Vector3 reference = Vector3.zero;


	void Awake()
	{
		myTransform = transform;
		myTransform.rotation = Random.rotation;

		OmegaX = Random.Range(100.0f, 200.0f);
		OmegaY = Random.Range(100.0f, 200.0f);
		OmegaZ = Random.Range(100.0f, 200.0f);
	}

	void Start()
	{
		targetPosition = startPosition + Random.insideUnitSphere * 50.0f;
		loop = Random.Range(0.5f, 2.0f);
		
		StartCoroutine(Fly());
		StartCoroutine(Target());
	}

	private IEnumerator Fly()
	{
		while(true)
		{
			if(visible)
			{
				myTransform.Rotate(Vector3.right * OmegaX * Time.deltaTime);
				myTransform.Rotate(Vector3.up * OmegaY * Time.deltaTime);
				myTransform.Rotate(Vector3.forward * OmegaZ * Time.deltaTime);

				myTransform.position = Vector3.SmoothDamp(myTransform.position, targetPosition, ref reference, 5.0f);
			}

			yield return null;
		}
	}

	private IEnumerator Target()
	{
		float clock;

		while(true)
		{
			clock = 0.0f;
			while(clock < loop && Vector3.Distance(myTransform.position, targetPosition) > 2.5f)
			{
				clock += Time.deltaTime;
				yield return null;
			}

			targetPosition = startPosition + Random.insideUnitSphere * 50.0f;
			loop = Random.Range(0.5f, 2.0f);
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