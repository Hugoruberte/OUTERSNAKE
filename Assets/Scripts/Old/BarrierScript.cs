using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class BarrierScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform myScreen;
	private Transform myParticle;
	private Transform mySnake;

	private SnakeControllerV3 snakeScript;

	private ParticleSystem mySplash;

	[Range(0f, 5f)]
	public float ScaleDuration = 1.0f;
	private float ScaleValue = 1.0f;
	private float ScaleSpeed = 10.0f;
	

	void Awake()
	{
		myTransform = transform;
		myScreen = myTransform.Find("Body/Screen");
		myParticle = myTransform.Find("Splash");
		mySnake = GameObject.FindWithTag("Player").transform;

		mySplash = myParticle.GetComponent<ParticleSystem>();
		
		snakeScript = mySnake.GetComponent<SnakeControllerV3>();
	}

	void Start()
	{
		myScreen.SetLocalScaleY(1f);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			if(ScaleValue > 0.05f)
			{
				myParticle.position = other.ClosestPoint(mySnake.position);
				myParticle.rotation = snakeScript.targetRotation * Quaternion.Euler(180,0,0);
				mySplash.Play();
			}
		}
	}

	public void DesactiveBarrier()
	{
		StartCoroutine(DesactiveCoroutine());
	}

	private IEnumerator DesactiveCoroutine()
	{
		ScaleSpeed = 1f/(ScaleDuration+0.01f);

		while(ScaleValue > 0.01f)
		{
			ScaleValue -= ScaleSpeed * Time.deltaTime;
			myScreen.SetLocalScaleY(ScaleValue);
			yield return null;
		}

		ScaleValue = 0f;

		while(Vector3.Distance(myScreen.localScale, Vector3.zero) > 0.01f)
		{
			myScreen.localScale = Vector3.MoveTowards(myScreen.localScale, Vector3.zero, ScaleSpeed*2f*Time.deltaTime);
			yield return null;
		}

		myScreen.localScale = Vector3.zero;
	}
}
