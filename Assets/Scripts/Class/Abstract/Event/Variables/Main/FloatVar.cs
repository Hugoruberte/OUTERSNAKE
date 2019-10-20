using UnityEngine;

[CreateAssetMenu(fileName = "Float", menuName = "Scriptable Object/Variable/Float", order = 3)]
public class FloatVar : Variable
{
	[Header("Variable")]
	[SerializeField] private float _value;
	[System.NonSerialized] private float _runtimeValue;
	public float value {
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