using UnityEngine;
using My.Events;

public abstract class Variable : ScriptableObject, ISerializationCallbackReceiver
{
	[Header("Event")]
	public GameEvent onChangeGameEvent;
	public ActionEvent onChangeActionEvent;




	public abstract void OnAfterDeserialize();

	public void OnBeforeSerialize() {}
}