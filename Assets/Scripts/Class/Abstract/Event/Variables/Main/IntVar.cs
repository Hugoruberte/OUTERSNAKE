using UnityEngine;

[CreateAssetMenu(fileName = "Int", menuName = "Scriptable Object/Variable/Int", order = 3)]
public class IntVar : Variable
{
	[Header("Variable")]
	[SerializeField] private int _value;
	[System.NonSerialized] private int _runtimeValue;
	public int value {
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