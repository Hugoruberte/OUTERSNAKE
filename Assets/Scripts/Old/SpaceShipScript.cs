using UnityEngine;
using System.Collections;

public class SpaceShipScript : MonoBehaviour
{
	private Transform myTransform;

	private float OmegaX;
	private float OmegaY;
	private float OmegaZ;


	void Awake()
	{
		myTransform = transform;
		myTransform.rotation = Random.rotation;

		OmegaX = Random.Range(0.0f, 5.0f);
		OmegaY = Random.Range(0.0f, 5.0f);
		OmegaZ = Random.Range(0.0f, 5.0f);
	}


	void Update()
	{
		myTransform.Rotate(Vector3.right * OmegaX * Time.deltaTime);
		myTransform.Rotate(Vector3.up * OmegaY * Time.deltaTime);
		myTransform.Rotate(Vector3.forward * OmegaZ * Time.deltaTime);
	}


	void OnBecameVisible()
	{
		enabled = true;
	}

	void OnBecameInvisible()
	{
		enabled = false;
	}
}
