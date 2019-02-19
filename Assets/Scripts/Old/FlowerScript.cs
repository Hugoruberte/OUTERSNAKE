using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class FlowerScript : MonoBehaviour
{
	private Renderer myRend;
	private Collider myColl;

	private ParticleSystem Petals;
	private ParticleSystem Fire;
	private ParticleSystem Aches;

	private WaitForSeconds waitforseconds_01 = new WaitForSeconds(0.1f);
	private WaitForSeconds waitforseconds_1 = new WaitForSeconds(1f);

	private IEnumerator fire_coroutine;
	private IEnumerator aches_coroutine;

	private bool Burnt = false;
	private bool Burning = false;

	private Color YellowColor;

	public int myCell = -1;

	void Awake()
	{
		myRend = transform.Find("Body").GetComponent<Renderer>();
		myColl = GetComponent<Collider>();

		Petals = transform.Find("Petals").GetComponent<ParticleSystem>();
		Fire = transform.Find("Fire").GetComponent<ParticleSystem>();
		Aches = transform.Find("Aches").GetComponent<ParticleSystem>();

		YellowColor = myRend.materials[1].color;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") || other.CompareTag("Rabbit"))
		{
			if(Burnt)
			{
				if(aches_coroutine != null)
					StopCoroutine(aches_coroutine);
				aches_coroutine = AchesCoroutine();
				StartCoroutine(aches_coroutine);
			}
			else
			{
				Petals.Play();
			}
		}
		else if(other.CompareTag("Fire") && !Burning && !Burnt)
		{
			if(fire_coroutine != null)
				StopCoroutine(fire_coroutine);
			fire_coroutine = FireCoroutine();
			StartCoroutine(fire_coroutine);
		}
	}

	private IEnumerator FireCoroutine()
	{
		Burning = true;
		Fire.Play();

		float value = 0.0f;
		while(value < 0.99f)
		{
			myRend.materials[1].color = Color.Lerp(YellowColor, Color.black, value);
			value += 0.5f * Time.deltaTime;
			yield return null;
		}

		yield return new WaitForSeconds(Fire.main.duration);

		Burning = false;
		Burnt = true;
	}

	private IEnumerator AchesCoroutine()
	{
		Aches.Play();

		yield return waitforseconds_01;

		myRend.enabled = false;
		myColl.enabled = false;

		yield return waitforseconds_1;

		transform.localPosition = Vector3.zero;
		myRend.enabled = true;
		myColl.enabled = true;
		gameObject.SetActive(false);
	}

	public void SetFlowerAspect(bool burn)
	{
		if(fire_coroutine != null)
			StopCoroutine(fire_coroutine);
		if(aches_coroutine != null)
			StopCoroutine(aches_coroutine);

		myRend.enabled = true;
		myColl.enabled = true;

		Burning = false;
		Burnt = burn;
		myRend.materials[1].color = (burn == true) ? Color.black : YellowColor;
	}
}
