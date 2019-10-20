using UnityEngine;
using My.Tools;
using My.Events;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Object/Event/GameEvent", order = 3)]
public class GameEvent : ScriptableObject
{
	public ActionEvent gameEvent;
	public IntEvent intEvent;
	public FloatEvent floatEvent;
	public BoolEvent boolEvent;
	public StringEvent stringEvent;
	public Vector3Event vector3Event;
	public QuaternionEvent quaternionEvent;
	public _TransformEvent _transformEvent;


	public void Invoke() => this.gameEvent?.Invoke();

	public void Invoke(int value)
	{
		this.Invoke();
		this.intEvent?.Invoke(value);
	}

	public void Invoke(float value)
	{
		this.Invoke();
		this.floatEvent?.Invoke(value);
	}

	public void Invoke(bool value)
	{
		this.Invoke();
		this.boolEvent?.Invoke(value);
	}

	public void Invoke(string value)
	{
		this.Invoke();
		this.stringEvent?.Invoke(value);
	}

	public void Invoke(Vector3 value)
	{
		this.Invoke();
		this.vector3Event?.Invoke(value);
	}

	public void Invoke(Quaternion value)
	{
		this.Invoke();
		this.quaternionEvent?.Invoke(value);
	}

	public void Invoke(_Transform value)
	{
		this.Invoke();
		this._transformEvent?.Invoke(value);
	}
}
