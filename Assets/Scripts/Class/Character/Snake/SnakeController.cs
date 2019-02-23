using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tools;
using Snakes;

public class SnakeController : MonoBehaviour 
{
	private class FaceSwitcher {
		private int delay = 0;

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

		public FaceSwitcher()
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

	public Transform heart;
	private Transform myTransform;

	private IEnumerator moveCoroutine = null;
	
	private Vector3 targetPosition = Vector3.zero;

	[Header("Settings")]
	[Range(0.0f, 100.0f)]
	public float speed = 10.0f;
	[SerializeField, Range(0.01f, 0.2f), Tooltip("The accuracy of the deplacement of Snake, less is more accurrate (0.1f is fine).")]
	private float positionAccuracy = 0.1f;		//plus c'est bas, plus c'est précis

	private int right = 0;
	private int forward = 0;
	private int forwardStored = 0;
	private int rightStored = 0;
	private const int WALL_DELAY = 2;
	private const int LEDGE_DELAY = 5;

	private int targetMask;

	[Header("States")]
	public SnakeMoveState state = SnakeMoveState.Stop;

	[HideInInspector]
	public bool cancelInput = false;

	private FaceSwitcher faceswitcher = new FaceSwitcher();

	public readonly SnakeMovementEvents events = new SnakeMovementEvents();



	void Awake()
	{
		myTransform = transform;
		targetPosition = myTransform.AbsolutePosition();

		targetMask = (1 << LayerMask.NameToLayer("Planet Surface"));
	}

	void Start()
	{
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
		moveCoroutine = UpdateSnakeMovementCoroutine();
		StartCoroutine(moveCoroutine);
	}

	public void StopSnakeMovement()
	{
		cancelInput = true;

		StopAllCoroutines();
		moveCoroutine = StopSnakeMovementCoroutine();
		StartCoroutine(moveCoroutine);
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
		if(cancelInput) {
			return;
		}
			
		int vertical = (int)Input.GetAxisRaw("Vertical");
		int horizontal = (int)Input.GetAxisRaw("Horizontal");
			
		if(horizontal != 0 && right == 0)
		{
			rightStored = horizontal;
			forwardStored = 0;
		}	
		else if(vertical != 0 && forward == 0)
		{
			forwardStored = vertical;
			rightStored = 0;
		}
		
		if(state == SnakeMoveState.Idle && (horizontal != 0 || vertical != 0))
		{
			if(vertical == -1) {
				myTransform.rotation = heart.rotation * Quaternion.Euler(0, 180, 0);
			} else if(horizontal != 0) {
				myTransform.rotation = heart.rotation * Quaternion.Euler(0, horizontal * 90, 0);
			}
				
			state = SnakeMoveState.Run;
		}
	}

	private IEnumerator UpdateSnakeMovementCoroutine()
	{
		Vector3 direction;

		while(true)
		{
			if(state != SnakeMoveState.Run)
			{
				yield return null;
				continue;
			}

			// We are not switching faces and/or the switching is done : get inputs + snake rotation
			ApplyInputs();

			// Calcul direction vector
			direction = forward * heart.forward + right * heart.right;
			
			// Calcul next position
			targetPosition = targetPosition + direction;

			// If we are not currently changing faces :
			if(faceswitcher.switching == false) {
				// Search for possible new faces
				SearchForNewFace(direction);
			}

			// If found a new face :
			if(faceswitcher.switching == true) {
				// Rotate snake over face + adapt target position
				targetPosition = ManageFaceRotationAndAdaptTargetPosition(direction);
			}
			
			// Move snake according to previous calculated target position
			if(Vector3.Distance(myTransform.position, targetPosition) > positionAccuracy) {
				// Events -> will reserve next cell + optional callback
				events.onStartStep.Invoke(targetPosition, myTransform.up);

				while(Vector3.Distance(myTransform.position, targetPosition) > positionAccuracy)
				{
					myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, speed * Time.deltaTime);
					yield return null;
				}

				myTransform.position = targetPosition;

				// Event -> reserved cell become current cell + optional callback 
				events.onEndStep.Invoke();
			}
			else {
				// skip frame to avoid freeze
				yield return null;
			}
		}
	}

	private IEnumerator StopSnakeMovementCoroutine()
	{
		while(Vector3.Distance(myTransform.position, targetPosition) > positionAccuracy) {
			myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, speed * Time.deltaTime);
			yield return null;
		}

		myTransform.position = targetPosition;

		// Event
		events.onEndStep.Invoke();

		cancelInput = false;
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
		if(!faceswitcher.switching)
		{
			// Pass input value to direction values + rotate snake
			if(forwardStored != 0 || rightStored != 0)
			{
				forward = forwardStored;
				right = rightStored;
				forwardStored = 0;
				rightStored = 0;

				if(right == 0) {
					myTransform.rotation = heart.rotation * Quaternion.Euler(0, (1 - forward) * 90, 0);
				} else {
					myTransform.rotation = heart.rotation * Quaternion.Euler(0, (right) * 90, 0);
				}
			}
		}
	}

	private void SearchForNewFace(Vector3 dir)
	{
		// Wall
		if(Physics.Raycast(myTransform.position, dir, 1f, targetMask))
		{
			Debug.DrawRay(myTransform.position, dir, Color.red);
			faceswitcher.SetFaceSwitching(WALL_DELAY, -dir, myTransform.position);
		}
		// Ledge
		else if(!Physics.Raycast(targetPosition, -heart.up, 1f, targetMask))
		{
			Debug.DrawRay(targetPosition, -heart.up, Color.red);
			faceswitcher.SetFaceSwitching(LEDGE_DELAY, dir, targetPosition);
		}
	}

	private Vector3 ManageFaceRotationAndAdaptTargetPosition(Vector3 dir)
	{
		Vector3 target;

		if(Vector3.Distance(myTransform.position, faceswitcher.destination) < positionAccuracy)
		{
			// snake rotation
			myTransform.rotation = SnakeRotationOverFace(dir);
			// heart rotation
			heart.rotation = HeartRotationOverFace(dir);
			// recalculate target position
			target = myTransform.position + (forward * heart.forward + right * heart.right);
		}
		else
		{
			target = targetPosition;
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

		axis = Vector3Extension.RoundToInt(myTransform.right);
		forwardDot = Vector3.Dot(heart.forward, axis);
		rightDot = Vector3.Dot(heart.right, axis);
		dirDot = Vector3.Dot(dir, faceswitcher.normal);

		newRotation = heart.rotation;
		newRotation *= Quaternion.Euler(90 * rightDot * dirDot, 0, 90 * forwardDot * dirDot);
		newRotation = newRotation.AbsoluteRotation();

		return newRotation;
	}

	private Quaternion SnakeRotationOverFace(Vector3 dir)
	{
		float dot;
		Quaternion rotator;
		Quaternion newRotation;

		dot = Vector3.Dot(dir, faceswitcher.normal);
		rotator = Quaternion.Euler(dot * 90, 0, 0);
		newRotation = myTransform.AbsoluteRotation() * rotator;

		return newRotation;
	}
}