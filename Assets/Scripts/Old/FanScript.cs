using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform Body;
	private Transform Snake;

	private Quaternion targetRotation;

	private float delay;

	void Awake()
	{
		myTransform = transform;
		Body = myTransform.Find("Body");
		Snake = GameObject.FindWithTag("Player").transform;
	}

	void Start()
	{
		delay = Random.Range(0.0f, 5.0f);
	}

	void Update()
	{
		targetRotation = Quaternion.LookRotation(Snake.position - myTransform.position);
		myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, targetRotation, 250f * Time.deltaTime);
		Body.position = myTransform.position + myTransform.up * 0.5f * Mathf.Sin((Time.time + delay) * 2f);
	}

	void OnBecameVisible()
	{
		this.enabled = true;
	}

	void OnBecameInvisible()
	{
		this.enabled = false;
	}
}
