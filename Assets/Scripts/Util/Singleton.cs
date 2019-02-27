using UnityEngine;


public abstract class Singleton<T> : MonoBehaviour where T : class
{
	private static T _instance = default(T);
	public static T instance {
		get {
			if(_instance == null) {
				Debug.LogError($"ERROR : Instance of {typeof(T)} is null, either you tried to access it from the Awake function or it has not been initialized in its own Awake function");
			}

			return _instance;
		}
		set {
			if(_instance != null) {
				Debug.LogWarning("WARNING : Several instance of {typeof(T)} has been set ! Check it out.");
				return;
			}

			_instance = value;
		}
	}

	protected virtual void Awake()
	{
		instance = this as T;
	}
}