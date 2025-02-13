using UnityEngine;
using UnityEditor;
using System;


public abstract class Singleton<T> where T : Singleton<T>
{
	private static T _instance = null;
	public static T instance {
		get {
			if(_instance == null) {
				_instance = GetNewClass();
				_instance.OnAwake();
			}

			return _instance;
		}
	}

	private static T GetNewClass() => Activator.CreateInstance(typeof(T)) as T;

	protected virtual void OnAwake() {}
}








public abstract class MonoSingleton<T> : MonoBehaviour where T : class
{
	private static T _instance = null;
	public static T instance {
		get {
			if(_instance == null) {
				_instance = FindObjectOfType(typeof(T)) as T;
			}

			if(_instance == null) {
				Debug.LogError($"ERROR : Instance of {typeof(T)} is null, it does not exist.");
			}

			return _instance;
		}
		private set {
			if(value != null && _instance != null) {
				Debug.LogWarning($"WARNING : Several instance of {typeof(T)} has been set ! Check it out.");
				return;
			}

			_instance = value;
		}
	}

	protected virtual void Awake()
	{
		if(_instance == null) {
			instance = this as T;
		}
	}

	protected virtual void OnDisable()
	{
		instance = null;
	}
}


[ExecuteInEditMode]
public abstract class ScriptableSingleton<T> : ScriptableObject where T : class
{
	[SerializeField] private static T _instance = null;
	public static T instance {
		get {
			if(_instance == null) {
				string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
				if(guids.Length > 0) {
					string path = AssetDatabase.GUIDToAssetPath(guids[0]);
					_instance = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
				}
			}

			if(_instance == null) {
				Debug.LogError($"ERROR : Instance of '{typeof(T)}' is null, either you tried to access it from the Awake function or it has not been initialized yet");
			}

			return _instance;
		}
		private set {
			if(value != null && _instance != null) {
				Debug.LogWarning($"WARNING : Several instance of {typeof(T)} has been set ! Check it out.");
				return;
			}

			_instance = value;
		}
	}

	protected virtual void OnEnable()
	{
		instance = this as T;
	}

	protected virtual void OnDisable()
	{
		instance = null;
	}

	protected virtual void OnDestroy()
	{
		instance = null;
	}
}