using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageManager : MonoBehaviour
{
	private Transform Background;
	private Transform Core;

	private List<IEnumerator> coroutines = new List<IEnumerator>();

	private float OmegaX;
	private float OmegaY;
	private float OmegaZ;
	private float max = 50f;

	void Awake()
	{
		Background = GameObject.Find("Background").transform;
		Core = GameObject.Find("Main/Core").transform;
	}

	void Start()
	{
		OmegaX = Random.Range(-max, max);
		OmegaY = Random.Range(-max, max);
		OmegaZ = Random.Range(-max, max);

		StartRotation();
	}

	void Update()
	{
		Background.Rotate(Vector3.up * 0.04f);
		
		Core.Rotate(Vector3.right * OmegaX * Time.deltaTime);
		Core.Rotate(Vector3.up * OmegaY * Time.deltaTime);
		Core.Rotate(Vector3.forward * OmegaZ * Time.deltaTime);
	}

	private void StartRotation()
	{
		Transform Rail = GameObject.Find("Shapes/Rail").transform;
		Transform child = null;

		float omegaX = 0f;
		float omegaY = 0f;
		float omegaZ = 0f;

		float range = 7.5f;

		for(int i = 0; i < 3; i++)
		{
			child = Rail.GetChild(i);

			for(int j = 0; j < 3; j++)
			{
				omegaX = Random.Range(-range, range);
				omegaY = Random.Range(-range, range);
				omegaZ = Random.Range(-range, range);

				IEnumerator co = RotationCoroutine(child.GetChild(j).GetChild(0), omegaX, omegaY, omegaZ);
				coroutines.Add(co);
				StartCoroutine(co);
			}
		}
	}

	public void StopRotation()
	{
		foreach(IEnumerator co in coroutines)
		{
			StopCoroutine(co);
		}
	}

	private IEnumerator RotationCoroutine(Transform ob, float x, float y, float z)
	{
		while(true)
		{
			ob.Rotate(Vector3.right * OmegaX * Time.deltaTime);
			ob.Rotate(Vector3.up * OmegaY * Time.deltaTime);
			ob.Rotate(Vector3.forward * OmegaZ * Time.deltaTime);

			yield return null;
		}
	}
}
