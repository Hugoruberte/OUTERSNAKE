using UnityEngine;

public class GarbageManager : MonoSingleton<GarbageManager>
{
	private Transform garbage;

	protected override private void Awake()
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
