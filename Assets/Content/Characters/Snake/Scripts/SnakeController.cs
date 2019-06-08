using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using My.Tools;
using My.Events;
using Snakes;


[RequireComponent(typeof(Rigidbody), typeof(SnakeCharacter))]
public class SnakeController : MonoBehaviour 
{
	public struct SnakeMovementEvents
	{
		public ActionEvent onStartStep;
		public ActionEvent onEndStep;
		
		public Vector3Vector3Event onStartStepTo;
		public Vector3Event onEndStepTo;
	}

	private Rigidbody myRigidbody;
	private _Transform heart;

	private IEnumerator moveCoroutine = null;
	
	private Vector3 targetPosition;
	private Vector3 direction;

	private int right = 0;
	private int forward = 0;
	private int right_cache = 0;
	private int forward_cache = 0;
	private const int WALL_DELAY = 2;
	private const int LEDGE_DELAY = 5;
	private int targetMask;

	private Cellable cellable;

	private FaceSwitcher faceswitcher = new FaceSwitcher(true);

	private bool cancelInput = false;


	[Header("Snake Data")]
	public SnakeData data;




	private void Awake()
	{
		this.myRigidbody = this.GetComponent<Rigidbody>();
		this.cellable = this.GetComponent<SnakeCharacter>().cellable;

		this.targetMask = 1 << LayerMask.NameToLayer("Ground");

		// Data
		this.data.snakeRigidbody = this.myRigidbody;
		this.data.snakeMovementEvents = new SnakeMovementEvents();
		this.data.snakeController = this;
	}

	private void Start()
	{
		this.myRigidbody.rotation = transform.AbsoluteRotation();
		this.targetPosition = this.cellable.currentCell.position + this.cellable.currentCell.normal * 0.5f;

		this.heart = HeartManager.instance.heart;
		// this.heart.rotation = Quaternion.LookRotation(this.myRigidbody.rotation * Shared.vector3Up);

		this.StartSnakeMovement();
	}

	private void Update()
	{
		this.GetInputs();
	}








	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* --------------------------------- MANAGE SNAKE MOVEMENTS ----------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	public void StartSnakeMovement()
	{
		StopAllCoroutines();
		this.moveCoroutine = UpdateSnakeMovementCoroutine();
		StartCoroutine(this.moveCoroutine);
	}

	public void StopSnakeMovement()
	{
		this.cancelInput = true;

		StopAllCoroutines();
		this.moveCoroutine = StopSnakeMovementCoroutine();
		StartCoroutine(this.moveCoroutine);
	}

	public void SetCancelInput(bool cancel)
	{
		this.cancelInput = cancel;

		this.data.onCancelInput?.Invoke(cancel);
	}

	public void SetDirection(int horizontal, int vertical)
	{
		horizontal = Mathf.RoundToInt(Mathf.Clamp(horizontal, -1f, 1f));
		vertical = Mathf.RoundToInt(Mathf.Clamp(vertical, -1f, 1f));

		this.ApplyInputs(horizontal, vertical);
	}

	public void Turn(bool toTheRight)
	{
		int vertical, horizontal;

		if(toTheRight) {
			vertical = -this.right;
			horizontal = this.forward;
		} else {
			vertical = this.right;
			horizontal = -this.forward;
		}

		if(vertical == 0 && horizontal == 0) {
			horizontal = (toTheRight) ? 1 : -1;
		}

		this.ApplyInputs(horizontal, vertical);
	}

	









	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* ------------------------------ SNAKE MOVEMENT MAIN FUNCTIONS -------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	private void GetInputs()
	{
		if(this.cancelInput) {
			return;
		}
			
		int vertical = (int)Input.GetAxisRaw("Vertical");
		int horizontal = (int)Input.GetAxisRaw("Horizontal");
			
		this.ApplyInputs(horizontal, vertical);
	}

	private void ApplyInputs(int h, int v)
	{
		if(h != 0 && this.right == 0) {
			this.right_cache = h;
			this.forward_cache = 0;
		} else if(v != 0 && this.forward == 0) {
			this.forward_cache = v;
			this.right_cache = 0;
		}
		
		if(this.data.state == SnakeMoveState.Idle && (h != 0 || v != 0))
		{
			if(v == -1) {
				this.myRigidbody.rotation = this.heart.rotation * Quaternion.Euler(0, 180, 0);
			} else if(h != 0) {
				this.myRigidbody.rotation = this.heart.rotation * Quaternion.Euler(0, h * 90, 0);
			}
				
			this.data.state = SnakeMoveState.Run;
		}
	}

	private IEnumerator UpdateSnakeMovementCoroutine()
	{
		while(true)
		{
			if(this.data.state != SnakeMoveState.Run) {
				yield return null;
				continue;
			}

			// We are not switching faces and/or the switching is done : get inputs + snake rotation
			this.ApplyInputsAndRotateSnake();

			// Calcul this.direction vector
			this.direction = this.forward * this.heart.forward + this.right * this.heart.right;
			
			// Calcul next position
			this.targetPosition = this.targetPosition + this.direction;

			// If we are not currently changing faces :
			if(this.faceswitcher.switching == false) {
				// Search for possible new faces
				this.SearchForNewFace(this.direction);
			}

			// If found a new face :
			if(this.faceswitcher.switching == true) {
				// Rotate snake over face + adapt target position
				this.targetPosition = this.ManageFaceRotationAndUpdateTargetPosition(this.direction);
			}
			
			// Move snake according to previous calculated target position
			if(Vector3.Distance(this.myRigidbody.position, this.targetPosition) > this.data.accuracy) {
				// Events -> will reserve next cell + optional callback
				this.data.snakeMovementEvents.onStartStep?.Invoke();
				this.data.snakeMovementEvents.onStartStepTo?.Invoke(this.targetPosition, this.myRigidbody.rotation * Shared.vector3Up);

				do {
					this.myRigidbody.position = Vector3.MoveTowards(this.myRigidbody.position, this.targetPosition, this.data.speed * Time.deltaTime);
					yield return null;
				}
				while(Vector3.Distance(this.myRigidbody.position, this.targetPosition) > this.data.accuracy);

				this.myRigidbody.position = this.targetPosition;

				// Event -> reserved cell become current cell + optional callback 
				this.data.snakeMovementEvents.onEndStep?.Invoke();
				this.data.snakeMovementEvents.onEndStepTo?.Invoke(this.targetPosition);
			}
			else
			{
				// skip frame to avoid freeze
				yield return null;
			}
		}
	}

	private IEnumerator StopSnakeMovementCoroutine()
	{
		while(Vector3.Distance(this.myRigidbody.position, this.targetPosition) > this.data.accuracy) {
			this.myRigidbody.position = Vector3.MoveTowards(this.myRigidbody.position, this.targetPosition, this.data.speed * Time.deltaTime);
			yield return null;
		}

		this.myRigidbody.position = this.targetPosition;

		// Event
		this.data.snakeMovementEvents.onEndStep?.Invoke();
		this.data.snakeMovementEvents.onEndStepTo?.Invoke(this.targetPosition);

		this.cancelInput = false;
	}










	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------- SNAKE MOVEMENT FUNCTIONS ---------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	private void ApplyInputsAndRotateSnake()
	{
		// If we are not switching faces
		if(!this.faceswitcher.switching)
		{
			// Pass input value to direction values + rotate snake
			if(this.forward_cache != 0 || this.right_cache != 0)
			{
				this.forward = this.forward_cache;
				this.right = this.right_cache;
				this.forward_cache = 0;
				this.right_cache = 0;

				if(this.right == 0) {
					this.myRigidbody.rotation = this.heart.rotation * Quaternion.Euler(0, (1 - this.forward) * 90, 0);
				} else {
					this.myRigidbody.rotation = this.heart.rotation * Quaternion.Euler(0, (this.right) * 90, 0);
				}
			}
		}
	}

	private void SearchForNewFace(Vector3 dir)
	{
		Debug.DrawRay(this.myRigidbody.position, dir * 1f, Color.blue);
		Debug.DrawRay(this.targetPosition, -this.heart.up * 1f, Color.green);

		// Wall
		if(Physics.Raycast(this.myRigidbody.position, dir, 1f, this.targetMask)) {
			this.faceswitcher.SetFaceSwitching(WALL_DELAY, -dir, this.myRigidbody.position);

		// Ledge
		} else if(!Physics.Raycast(this.targetPosition, -this.heart.up, 1f, this.targetMask)) {
			this.faceswitcher.SetFaceSwitching(LEDGE_DELAY, dir, this.targetPosition);
		}
	}

	private Vector3 ManageFaceRotationAndUpdateTargetPosition(Vector3 dir)
	{
		Vector3 target;

		if(Vector3.Distance(this.myRigidbody.position, this.faceswitcher.destination) < this.data.accuracy)
		{
			dir.Normalize();

			// snake rotation
			this.myRigidbody.rotation = this.SnakeRotationOverFace(dir);
			// this.heart rotation
			this.heart.rotation = this.HeartRotationOverFace(dir);
			// recalculate target position
			target = this.myRigidbody.position + (this.forward * this.heart.forward + this.right * this.heart.right);
		}
		else
		{
			target = this.targetPosition;
		}

		return target;
	}

	private Quaternion HeartRotationOverFace(Vector3 dir)
	{
		Vector3 axis;
		Quaternion newRotation;
		float forwardDot;
		float rightDot;
		float dirDot;

		axis = Vector3Int.RoundToInt(this.myRigidbody.rotation * Shared.vector3Right);
		forwardDot = Vector3.Dot(this.heart.forward, axis);
		rightDot = Vector3.Dot(this.heart.right, axis);
		dirDot = Vector3.Dot(dir, this.faceswitcher.normal);

		newRotation = this.heart.rotation;
		newRotation *= Quaternion.Euler(90 * rightDot * dirDot, 0, 90 * forwardDot * dirDot);
		newRotation = newRotation.AbsoluteRotation();

		return newRotation;
	}

	private Quaternion SnakeRotationOverFace(Vector3 dir)
	{
		float dot;
		Quaternion rotator;
		Quaternion newRotation;

		dot = Vector3.Dot(dir, this.faceswitcher.normal);
		rotator = Quaternion.Euler(dot * 90, 0, 0);
		newRotation = this.myRigidbody.rotation * rotator;

		return newRotation;
	}







	private struct FaceSwitcher {
		private int delay;

		private bool _switching;
		public bool switching {
			get {
				if(_switching) {
					if(delay > 0) { delay --; }
					if(delay == 0) { _switching = false;} 
				}
				return _switching;
			}
		}

		public Vector3 normal;
		public Vector3 destination;

		public FaceSwitcher(bool mdr)
		{
			this._switching = false;
			this.delay = 0;
			this.normal = Shared.vector3Zero;
			this.destination = Shared.vector3Zero;
		}

		public void SetFaceSwitching(int d, Vector3 n, Vector3 dest)
		{
			this._switching = true;
			this.delay = d;
			this.normal = n;
			this.destination = dest;
		}
	}
}