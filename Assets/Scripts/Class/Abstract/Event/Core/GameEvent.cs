using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Object/Event/GameEvent", order = 3)]
public class GameEvent : ScriptableObject
{
	private List<IGameEventListener> listeners = new List<IGameEventListener>();
	private List<IGameEventListener<int>> listeners_int = new List<IGameEventListener<int>>();
	private List<IGameEventListener<float>> listeners_float = new List<IGameEventListener<float>>();
	private List<IGameEventListener<bool>> listeners_bool = new List<IGameEventListener<bool>>();
	private List<IGameEventListener<string>> listeners_string = new List<IGameEventListener<string>>();
	private List<IGameEventListener<Vector3>> listeners_vector3 = new List<IGameEventListener<Vector3>>();
	private List<IGameEventListener<Quaternion>> listeners_quaternion = new List<IGameEventListener<Quaternion>>();

	public void AddListener(IGameEventListener listener) => this.listeners.Add(listener);
	public void AddListener(IGameEventListener<int> listener) => this.listeners_int.Add(listener);
	public void AddListener(IGameEventListener<float> listener) => this.listeners_float.Add(listener);
	public void AddListener(IGameEventListener<bool> listener) => this.listeners_bool.Add(listener);
	public void AddListener(IGameEventListener<string> listener) => this.listeners_string.Add(listener);
	public void AddListener(IGameEventListener<Vector3> listener) => this.listeners_vector3.Add(listener);
	public void AddListener(IGameEventListener<Quaternion> listener) => this.listeners_quaternion.Add(listener);

	public void RemoveListener(IGameEventListener listener) => this.listeners.Remove(listener);
	public void RemoveListener(IGameEventListener<int> listener) => this.listeners_int.Remove(listener);
	public void RemoveListener(IGameEventListener<float> listener) => this.listeners_float.Remove(listener);
	public void RemoveListener(IGameEventListener<bool> listener) => this.listeners_bool.Remove(listener);
	public void RemoveListener(IGameEventListener<string> listener) => this.listeners_string.Remove(listener);
	public void RemoveListener(IGameEventListener<Vector3> listener) => this.listeners_vector3.Remove(listener);
	public void RemoveListener(IGameEventListener<Quaternion> listener) => this.listeners_quaternion.Remove(listener);


	public void Invoke()
	{
		for(int i = 0; i < this.listeners.Count; i++) {
			this.listeners[i].OnEventInvoked();
		}
	}

	public void Invoke(int value)
	{
		this.Invoke();
		for(int i = 0; i < this.listeners_int.Count; i++) {
			this.listeners_int[i].OnEventInvoked(value);
		}
	}

	public void Invoke(float value)
	{
		this.Invoke();
		for(int i = 0; i < this.listeners_float.Count; i++) {
			this.listeners_float[i].OnEventInvoked(value);
		}
	}

	public void Invoke(bool value)
	{
		this.Invoke();
		for(int i = 0; i < this.listeners_bool.Count; i++) {
			this.listeners_bool[i].OnEventInvoked(value);
		}
	}

	public void Invoke(string value)
	{
		this.Invoke();
		for(int i = 0; i < this.listeners_string.Count; i++) {
			this.listeners_string[i].OnEventInvoked(value);
		}
	}

	public void Invoke(Vector3 value)
	{
		this.Invoke();
		for(int i = 0; i < this.listeners_vector3.Count; i++) {
			this.listeners_vector3[i].OnEventInvoked(value);
		}
	}

	public void Invoke(Quaternion value)
	{
		this.Invoke();
		for(int i = 0; i < this.listeners_quaternion.Count; i++) {
			this.listeners_quaternion[i].OnEventInvoked(value);
		}
	}
}
