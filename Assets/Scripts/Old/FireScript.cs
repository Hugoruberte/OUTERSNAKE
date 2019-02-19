using UnityEngine;
using System.Collections;
using Tools;

public class FireScript : MonoBehaviour
{
	private Transform myTransform;
	private Renderer Spectrum;

	private WaitForSeconds waitforseconds_2 = new WaitForSeconds(2.0f);

	private Light myLight;
	private ParticleSystem myParticle;

	private bool TagChanged = false;

	private float start_intensity;
	private float lifetime;

	void Awake()
	{
		myTransform = transform;

		myLight = myTransform.Find("Light").GetComponent<Light>();
		myParticle = myTransform.Find("Flame").GetComponent<ParticleSystem>();
		if(myTransform.Find("Spectrum") != null)
			Spectrum = myTransform.Find("Spectrum").GetComponent<Renderer>();		

		start_intensity = myLight.intensity;
		lifetime = myParticle.main.duration;
	}

	void Start()
	{
		StartCoroutine(Life());
	}

	private IEnumerator Life()
	{
		float value = myLight.intensity;
		while(value > 0.0f)
		{
			value = Mathf.MoveTowards(value, 0.0f, (start_intensity/lifetime) * 1.0f *Time.deltaTime);
			myLight.intensity = value;

			if(value < 0.9f && !TagChanged)
			{
				myTransform.tag = "Untagged";
				TagChanged = true;
			}
			yield return null;
		}

		yield return waitforseconds_2;

		if(Spectrum)
		{
			value = Spectrum.material.color.a;
			while(value > 0.0f)
			{
				value = Mathf.MoveTowards(value, 0.0f, 1.0f * Time.deltaTime);
				Spectrum.SetColorA(value);
				yield return null;
			}
		}
		
		GameObject Poubelle = GameObject.Find("Poubelle");

		if(Poubelle)
		{
			myTransform.parent = Poubelle.transform;
			myTransform.localPosition = Vector3.zero;
			gameObject.SetActive(false);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Bounds"))
		{
			GameObject Poubelle = GameObject.Find("Poubelle");

			if(Poubelle)
			{
				myTransform.parent = Poubelle.transform;
				myTransform.localPosition = Vector3.zero;
				gameObject.SetActive(false);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}