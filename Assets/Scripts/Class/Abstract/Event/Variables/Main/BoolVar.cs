using UnityEngine;

[CreateAssetMenu(fileName = "Bool", menuName = "Scriptable Object/Variable/Bool", order = 3)]
public class BoolVar : Variable
{
	[Header("Variable")]
	[SerializeField] private bool _value;
	[System.NonSerialized] private bool _runtimeValue;
	public bool value {
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