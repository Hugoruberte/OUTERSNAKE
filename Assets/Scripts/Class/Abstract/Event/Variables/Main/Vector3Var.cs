using UnityEngine;
using My.Events;

[CreateAssetMenu(fileName = "Vector3", menuName = "Scriptable Object/Variable/Vector3", order = 3)]
public class Vector3Var : Variable
{
	[Header("Variable")]
	[SerializeField] private Vector3 _value;
	[System.NonSerialized] private Vector3 _runtimeValue;
	public Vector3 value {
		get
		{
			return (Application.isPlaying) ? this._runtimeValue : this._value;
		}
		set
		{
			if(Application.isPlaying) {
				this._value = value;
			} else {
				this._runtimeValue = value;
			}

			this.onChangeGameEvent?.Invoke(value);
			this.onChangeActionEvent?.Invoke();
		}
	}

	public override void OnAfterDeserialize() => this._runtimeValue = this._value;
}