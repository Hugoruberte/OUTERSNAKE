using UnityEngine;
using My.Events;

[CreateAssetMenu(fileName = "String", menuName = "Scriptable Object/Variable/String", order = 3)]
public class StringVar : Variable
{
	[Header("Variable")]
	[SerializeField] private string _value;
	[System.NonSerialized] private string _runtimeValue;
	public string value {
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