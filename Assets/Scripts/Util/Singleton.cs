using UnityEngine;
using System;
using System.Reflection;


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

	private static T GetNewClass()
	{
		Type t = typeof(T);
		Type generic = t.MakeGenericType();
		return Activator.CreateInstance(generic) as T;
	}

	protected virtual void OnAwake() {}
}








public abstract class MonoSingleton<T> : MonoBehaviour where T : class
{
	private static T _instance = null;
	public static T instance {
		get {
			if(_instance == null) {
				Debug.LogError($"ERROR : Instance of {typeof(T)} is null, either you tried to access it from the Awake function or it has not been initialized in its own Awake function");
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
		} else {
			Destroy(gameObject);
		}
	}

	protected virtual void OnDisable()
	{
		instance = null;
	}
}

public abstract class ScriptableSingleton<T> : ScriptableObject where T : class
{
	private static T _instance = null;
	public static T instance {
		get {
			if(_instance == null) {
				Debug.LogError($"ERROR : Instance of '{typeof(T)}' is null, either you tried to access it from the Awake function or it has not been initialized yet");
			}

			Debug.LogWarning($"WARNING : Access Scriptable MonoSingleton '{typeof(T)}' by instance ! Although it is accepted, it is better to use an object reference to prevent hard link between object.");

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

	public virtual void OnEnable()
	{
		instance = this as T;
	}

	protected virtual void OnDisable()
	{
		instance = null;
	}
}