using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeScript : MonoBehaviour
{
	private Transform myCamera;

	private IEnumerator shake_coroutine;

	private float current_intensity = -1.0f;

	void Awake()
	{
		myCamera = transform.Find("Camera");
	}


	public void Shake(float intensity)
	{
		if(shake_coroutine != null)
		{
			if(intensity > current_intensity)
				StopCoroutine(shake_coroutine);
			else
				return;
		}
		current_intensity = intensity;
		shake_coroutine = ShakeCoroutine(intensity);
		StartCoroutine(shake_coroutine);
	}
	public void Shake(Shaketype type)
	{
		switch(type)
		{
			case Shaketype.Nuclear:
				Shake(20.0f);
				break;

			case Shaketype.Rocket:
				Shake(2.5f);
				break;

			case Shaketype.Bomb:
				Shake(3.0f);
				break;

			case Shaketype.Gentle:
				Shake(0.1f);
				break;

			default:
				Debug.LogError("La configuration '" + name + "' n'existe pas !");
				break;
		}
	}
	private IEnumerator ShakeCoroutine(float intensity)
	{
		float current = intensity;
		float speed;
		Vector3 shakePosition;

		while(current > 0.05f)
		{
			shakePosition = Random.insideUnitSphere * current;
			speed = Vector3.Distance(myCamera.localPosition, shakePosition) / 0.05f;
			while(Vector3.Distance(myCamera.localPosition, shakePosition) > 0.01f)
			{
				myCamera.localPosition = Vector3.MoveTowards(myCamera.localPosition, shakePosition, speed * Time.deltaTime);
				yield return null;
			}
			current /= 1.75f;	//?
		}
		speed = Vector3.Distance(myCamera.localPosition, Vector3.zero) / 0.05f;
		while(Vector3.Distance(myCamera.localPosition, Vector3.zero) > 0.01f)
		{
			myCamera.localPosition = Vector3.MoveTowards(myCamera.localPosition, Vector3.zero, speed * Time.deltaTime);
			yield return null;
		}

		current_intensity = -1.0f;
		myCamera.localPosition = Vector3.zero;
	}
}
