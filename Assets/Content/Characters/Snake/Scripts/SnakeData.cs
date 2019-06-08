using UnityEngine;
using System;
using Snakes;
using My.Events;

[CreateAssetMenu(fileName = "SnakeData", menuName = "Scriptable Object/Data/SnakeData", order = 3)]
public class SnakeData : ScriptableSingleton<SnakeData>, ISerializationCallbackReceiver
{
	[NonSerialized] public GameObject snakeObject;
	[NonSerialized] public Transform snakeTransform;

	[NonSerialized] public Collider snakeCollider;
	[NonSerialized] public Rigidbody snakeRigidbody;

	[NonSerialized] public SnakeController snakeController;
	[NonSerialized] public SnakeController.SnakeMovementEvents snakeMovementEvents;

	[NonSerialized] public BoolEvent onCancelInput;


	[Header("Motion")]
	[Range(0.0f, 100.0f)] public float speed = 10f;
	[Range(0.01f, 0.2f)] public float accuracy = 0.1f;
	
	[Header("States")]
	[SerializeField] private SnakeMoveState _state = SnakeMoveState.Idle;
	private SnakeMoveState _runtimeState = SnakeMoveState.Idle;
	public SnakeMoveState state {
		get => (Application.isPlaying) ? this._runtimeState : this._state;
		set {
			if(Application.isPlaying) {
				this._runtimeState = value;
			} else {
				this._state = value;
			}
		}
	}


	public void OnAfterDeserialize() => this._runtimeState = this._state;

	public void OnBeforeSerialize() {}
}
