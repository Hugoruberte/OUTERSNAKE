using UnityEngine;
using System.Collections;
using Tools;

public class SawScriptTuto : MonoBehaviour
{
	private Transform myTransform;
	private Transform Body;
	private Transform Snake;
	private Transform Spark;

	private CameraScript cameraScript;

	private WaitForSeconds waitforseconds_05 = new WaitForSeconds(0.5f);

	private ParticleSystem myParticle;

	public enum Sides
	{
		Right,
		Left
	};
	public Sides Side = Sides.Right;

	private float omega = 1500;
	private float speed = 0.5f;
	private float dest;

	private bool Contact = false;
	private bool Havoc = false;

	private Vector3 Oscale;
	private Vector3 reference = Vector3.zero;
	private Vector3 direction;

	void Awake()
	{
		myTransform = transform;
		Snake = GameObject.FindWithTag("Player").transform;
		Body = myTransform.Find("Body");
		Spark = myTransform.Find("Spark");

		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();

		myParticle = Spark.GetComponent<ParticleSystem>();
	}

	void Start()
	{
		Oscale = myTransform.localScale;
		myTransform.localScale = Vector3.zero;

		if(Side == Sides.Right)
		{
			dest = -0.5f;
			myTransform.SetLocalPositionZ(0.5f);
			direction = -Vector3.forward;
			Spark.localEulerAngles = new Vector3(-65, 0, 0);
			Spark.SetLocalPositionZ(2.0f);
		}
		else
		{
			dest = 0.5f;
			myTransform.SetLocalPositionZ(-0.5f);
			direction = Vector3.forward;
			Spark.localEulerAngles = new Vector3(-115, 0, 0);
			Spark.SetLocalPositionZ(-2.0f);
		}

		dest = (Side == Sides.Right) ? -0.5f : 0.5f;
		myTransform.SetLocalPositionZ((Side == Sides.Right) ? 0.5f : -0.5f);
		direction = (Side == Sides.Right) ? -Vector3.forward : Vector3.forward;
		Spark.localEulerAngles = (Side == Sides.Right) ? new Vector3(-65, 0, 0) : new Vector3(-115, 0, 0);
		Spark.SetLocalPositionZ((Side == Sides.Right) ? 2 : -2);

		StartCoroutine(Translation());
		StartCoroutine(Rotation());
		StartCoroutine(Raycastion());
	}

	private IEnumerator Raycastion()
	{
		while(true)
		{
			Contact = false;

			RaycastHit[] hits;
			hits = Physics.RaycastAll(myTransform.position + myTransform.up * 0.5f, direction, 2.1f);
			for(int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				if(hit.transform == Snake || hit.transform.CompareTag("SnakeBody"))
				{
					Contact = true;
					if(!myParticle.isPlaying)
						myParticle.Play();

					if(!Havoc)
					{
						cameraScript.Shake(1.0f);
						Havoc = true;
					}
				}
			}

			if(!Contact)
			{
				Havoc = false;
				myParticle.Stop();
			}

			yield return null;
		}
	}

	private IEnumerator Rotation()
	{
		while(true)
		{
			Body.Rotate(Vector3.right * omega * Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator Translation()
	{
		bool shr = false;

		while(true)
		{
			StartCoroutine(Grow());
			shr = false;
			myTransform.SetLocalPositionZ(-dest);
			while(myTransform.localPosition.z != dest)
			{
				if(!Contact)
					myTransform.SetLocalPositionZ(Mathf.MoveTowards(myTransform.localPosition.z, dest, speed * Time.deltaTime));

				if(Mathf.Abs(dest - myTransform.localPosition.z) < 0.15f && !shr)
				{
					shr = true;
					StartCoroutine(Shrink());
				}

				yield return null;
			}
			myTransform.SetLocalPositionZ(dest);
			yield return waitforseconds_05;
		}
	}

	private IEnumerator Grow()
	{
		while(Vector3.Distance(myTransform.localScale, Oscale) > 0.1f)
		{
			myTransform.localScale = Vector3.SmoothDamp(myTransform.localScale, Oscale, ref reference, 0.2f);
			yield return null;
		}

		myTransform.localScale = Oscale;
	}

	private IEnumerator Shrink()
	{
		while(Vector3.Distance(myTransform.localScale, 0.05f*Oscale) > 0.1f)
		{
			myTransform.localScale = Vector3.SmoothDamp(myTransform.localScale, 0.05f*Oscale, ref reference, 0.2f);
			yield return null;
		}

		myTransform.localScale = 0.05f*Oscale;
	}
}