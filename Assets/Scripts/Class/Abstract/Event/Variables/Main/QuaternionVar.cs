using UnityEngine;

[CreateAssetMenu(fileName = "Quaternion", menuName = "Scriptable Object/Variable/Quaternion", order = 3)]
public class QuaternionVar : Variable
{
	[Header("Variable")]
	[SerializeField] private Quaternion _value;
	[System.NonSerialized] private Quaternion _runtimeValue;
	public Quaternion value {
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