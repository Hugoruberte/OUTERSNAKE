using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleScript2D : MonoBehaviour {

	public int MaxApple = 15;
	public float Lifespan = 3.0f;
	public float Clock = 0.0f;

	public GameObject ApplePrefab;
	private GameObject AppleStock;

	void Start()
	{
		AppleStock = new GameObject();
		AppleStock.name = "AppleStock";

		StartCoroutine(Delete());

		for(int i = 0; i < MaxApple; i++)
		{
			Create();
		}
	}

	IEnumerator Delete()
	{
		while(true)
		{
			Clock += Time.deltaTime;

			if(AppleStock.transform.childCount > 0 && Clock > Lifespan)
			{
				int choice = Random.Range(0, AppleStock.transform.childCount);
				Destroy(AppleStock.transform.GetChild(choice).gameObject);
				Clock = 0.0f;

				Create();
			}

			yield return null;
		}
	}

	public void Create()
	{
		int x = (int)Random.Range(-13, 15);
		int z = (int)Random.Range(-13, 15);

		GameObject apple = (GameObject)Instantiate(ApplePrefab, new Vector3(x, 0, z), Quaternion.identity);
		apple.name = "Apple";
		apple.transform.parent = AppleStock.transform;
	}
}
