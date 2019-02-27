using UnityEngine;
using Snakes;


using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
	public GameData gameData;

	public Transform heart;

	protected override void Awake()
	{
		base.Awake();
		
		if(!heart) {
			this.CreateNewHeart();
		}

		this.CreateNewGarbage();
	}

	private void CreateNewHeart()
	{
		GameObject heartObject;

		heartObject = new GameObject();
		heartObject.name = "Heart";
		heart = heartObject.transform;
	}

	private void CreateNewGarbage()
	{
		GameObject g = new GameObject();
		g.name = "Garbage";
		gameData.garbage = g.transform;
	}

	public static void PutInGarbage(GameObject o)
	{
		o.SetActive(false);

		MonoBehaviour[] monos = o.GetComponentsInChildren<MonoBehaviour>();
		foreach(MonoBehaviour m in monos) {
			m.StopAllCoroutines();
		}

		o.transform.parent = instance.gameData.garbage;
	}

	public static void EmptyTheGarbage()
	{
		Destroy(instance.gameData.garbage.gameObject);

		instance.CreateNewGarbage();
	}






	// protected override void Start()
	// {
	// 	IEnumerator co = CO1();
	// 	this.StartCoroutine(co);
	// 	this.StopCoroutine(co);
	// }


	// private IEnumerator CO1()
	// {
	// 	Debug.Log("CO1 started at : " + Time.time);

	// 	yield return StartCoroutine(this.CO2());
	// 	// yield return this.CO2();

	// 	Debug.Log("CO1 ended at : " + Time.time);
	// }

	// private IEnumerator CO2()
	// {
	// 	Debug.Log("CO2 started at : " + Time.time);

	// 	yield return StartCoroutine(this.CO3());
	// 	// yield return this.CO3();

	// 	yield return StartCoroutine(this.CO3());
	// 	// yield return this.CO3();

	// 	Debug.Log("CO2 ended at : " + Time.time);
	// }

	// private IEnumerator CO3()
	// {
	// 	Debug.Log("CO3 started at : " + Time.time);

	// 	yield return new WaitForSeconds(1f);

	// 	Debug.Log("CO3 ended at : " + Time.time);
	// }
}
