using UnityEngine;
using Snakes;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
	private Transform garbage;

	protected override void Awake()
	{
		base.Awake();

		this.CreateNewGarbage();
	}

	private void CreateNewGarbage()
	{
		GameObject g = new GameObject();
		g.name = "Garbage";
		this.garbage = g.transform;
	}

	public void PutInGarbage(GameObject o)
	{
		o.SetActive(false);

		MonoBehaviour[] monos = o.GetComponentsInChildren<MonoBehaviour>();
		foreach(MonoBehaviour m in monos) {
			m.StopAllCoroutines();
		}

		o.transform.parent = this.garbage;
	}

	public void EmptyTheGarbage()
	{
		Destroy(this.garbage.gameObject);

		this.CreateNewGarbage();
	}
}
