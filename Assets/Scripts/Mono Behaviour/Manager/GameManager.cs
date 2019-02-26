using UnityEngine;
using Snakes;

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
}
