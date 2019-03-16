using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tools;
using Snakes;

public class SnakeController : MonoBehaviour 
{
	private Transform myTransform;
	private _Transform heart;

	private IEnumerator moveCoroutine = null;
	
	private Vector3 targetPosition;
	private Vector3 direction;

	private int right = 0;
	private int forward = 0;
	private int forward_cache = 0;
	private int right_cache = 0;
	private const int WALL_DELAY = 2;
	private const int LEDGE_DELAY = 5;
	private int targetMask;

	private FaceSwitcher faceswitcher = new FaceSwitcher(true);

	private SnakeMovementEvents events;




	[Header("Motion")]
	[Range(0.0f, 100.0f)] public float speed = 10f;
	[Range(0.01f, 0.2f)] public float positionAccuracy = 0.1f;
	
	[HideInInspector] public bool cancelInput = false;

	[Header("States")]
	public SnakeMoveState state = SnakeMoveState.Idle;


	void Awake()
	{
		this.myTransform = transform;
		this.targetPosition = this.myTransform.AbsolutePosition();

		this.targetMask = (1 << LayerMask.NameToLayer("Ground"));
	}

	void Start()
	{
		this.events = SnakeManager.instance.events;
		this.heart = HeartManager.instance.heart;

		this.StartSnakeMovement();
	}

	void Update()
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
			
		if(horizontal != 0 && this.right == 0) {
			this.right_cache = horizontal;
			this.forward_cache = 0;
		} else if(vertical != 0 && this.forward == 0) {
			this.forward_cache = vertical;
			this.right_cache = 0;
		}
		
		if(this.state == SnakeMoveState.Idle && (horizontal != 0 || vertical != 0))
		{
			if(vertical == -1) {
				this.myTransform.rotation = this.heart.rotation * Quaternion.Euler(0, 180, 0);
			} else if(horizontal != 0) {
				this.myTransform.rotation = this.heart.rotation * Quaternion.Euler(0, horizontal * 90, 0);
			}
				
			this.state = SnakeMoveState.Run;
		}
	}

	private IEnumerator UpdateSnakeMovementCoroutine()
	{
		while(true)
		{
			if(this.state != SnakeMoveState.Run) {
				yield return null;
				continue;
			}

			// We are not switching faces and/or the switching is done : get inputs + snake rotation
			this.ApplyInputs();

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
				this.targetPosition = this.ManageFaceRotationAndAdaptTargetPosition(this.direction);
			}
			
			// Move snake according to previous calculated target position
			if(Vector3.Distance(this.myTransform.position, this.targetPosition) > this.positionAccuracy) {
				// Events -> will reserve next cell + optional callback
				this.events.onStartStep.Invoke(this.targetPosition, this.myTransform.up);

				while(Vector3.Distance(this.myTransform.position, this.targetPosition) > this.positionAccuracy) {
					this.myTransform.position = Vector3.MoveTowards(this.myTransform.position, this.targetPosition, this.speed * Time.deltaTime);
					yield return null;
				}

				this.myTransform.position = this.targetPosition;

				// Event -> reserved cell become current cell + optional callback 
				this.events.onEndStep.Invoke();
			} else {
				// skip frame to avoid freeze
				yield return null;
			}
		}
	}

	private IEnumerator StopSnakeMovementCoroutine()
	{
		while(Vector3.Distance(this.myTransform.position, this.targetPosition) > this.positionAccuracy) {
			this.myTransform.position = Vector3.MoveTowards(this.myTransform.position, this.targetPosition, this.speed * Time.deltaTime);
			yield return null;
		}

		this.myTransform.position = this.targetPosition;

		// Event
		this.events.onEndStep.Invoke();

		this.cancelInput = false;
	}










	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------- SNAKE MOVEMENT FUNCTIONS ---------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	/* -------------------------------------------------------------------------------------------*/
	private void ApplyInputs()
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
					this.myTransform.rotation = this.heart.rotation * Quaternion.Euler(0, (1 - this.forward) * 90, 0);
				} else {
					this.myTransform.rotation = this.heart.rotation * Quaternion.Euler(0, (this.right) * 90, 0);
				}
			}
		}
	}

	private void SearchForNewFace(Vector3 dir)
	{
		// Wall
		if(Physics.Raycast(this.myTransform.position, dir, 1f, this.targetMask)) {
			this.faceswitcher.SetFaceSwitching(WALL_DELAY, -dir, this.myTransform.position);

		// Ledge
		} else if(!Physics.Raycast(this.targetPosition, -this.heart.up, 1f, this.targetMask)) {
			this.faceswitcher.SetFaceSwitching(LEDGE_DELAY, dir, this.targetPosition);
		}
	}

	private Vector3 ManageFaceRotationAndAdaptTargetPosition(Vector3 dir)
	{
		Vector3 target;

		if(Vector3.Distance(this.myTransform.position, this.faceswitcher.destination) < this.positionAccuracy)
		{
			dir.Normalize();

			// snake rotation
			this.myTransform.rotation = this.SnakeRotationOverFace(dir);
			// this.heart rotation
			this.heart.rotation = this.HeartRotationOverFace(dir);
			// recalculate target position
			target = this.myTransform.position + (this.forward * this.heart.forward + this.right * this.heart.right);
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

		axis = Vector3Extension.RoundToInt(this.myTransform.right);
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
		newRotation = this.myTransform.AbsoluteRotation() * rotator;

		return newRotation;
	}







	private struct FaceSwitcher {
		private int delay;

		private bool _switching;
		public bool switching {
			get {
				if(_switching) {
					if(delay > 0) delay --;
					if(delay == 0) _switching = false;
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
			this.normal = Vector3.zero;
			this.destination = Vector3.zero;
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