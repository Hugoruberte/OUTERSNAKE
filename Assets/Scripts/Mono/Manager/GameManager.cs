using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager _instance = null;
	public static GameManager instance {
		get {
			if(_instance == null) {
				Debug.LogError("Instance is null, either you tried to access it from the Awake function or it has not been initialized in its own Awake function");
			}
			return _instance;
		}
		set {
			_instance = value;
		}
	}

	private static Transform _garbage = null;

	void Awake()
	{
		instance = this;

		CreateNewGarbage();
	}

	private static void CreateNewGarbage()
	{
		GameObject g = new GameObject();
		g.name = "Garbage";

		_garbage = g.transform;
		_garbage.parent = _instance.transform;
	}

	public static void PutInGarbage(GameObject o)
	{
		o.SetActive(false);

		o.transform.parent = _garbage;
	}

	public static void CleanTheGarbage()
	{
		Destroy(_garbage.gameObject);

		CreateNewGarbage();
	}
}
