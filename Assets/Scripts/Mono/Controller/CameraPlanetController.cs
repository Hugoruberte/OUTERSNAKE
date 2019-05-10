using UnityEngine;
using System.Collections;
using My.Tools;

namespace Cameras
{
	public enum CameraMoveState
	{
		Idle = 0,
		Move
	}

	public class CameraPlanetController : MonoSingleton<CameraPlanetController>
	{
		[Header("State")]
		[SerializeField] private Rigidbody target = null;
		public CameraMoveState state = CameraMoveState.Idle;

		[Header("¨Parameters")]
		[SerializeField, Tooltip("Distance camera <-> target"), Range(5, 75)] private int height = 25;
		[SerializeField] private AnimationCurve smoothCurve = new AnimationCurve();

		[Header("Data")]
		[SerializeField] private SnakeData snakeData = null;


		private Transform myTransform;

		private const float SMOOTH_OMEGA = 3f;
		private const float SMOOTH_TRANSITION = 75f;
		private const float SMOOTH_LOCK_THRESHOLD = 20f;

		private Vector3 targetPosition;
		private Vector3 velocity = Vector3Extension.ZERO;
		private IEnumerator smoothRotationCoroutine = null;
		private Quaternion previousTarget;



		protected override void Awake()
		{
			base.Awake();
			
			this.myTransform = transform;
			this.previousTarget = this.myTransform.rotation;
		}

		void Start()
		{
			HeartManager.instance.onRotate += this.OnHeartRotate;

			if(!this.target) {
				Debug.LogWarning("WARNING : Camera does not have any target to follow !");
			}
		}

		void Update()
		{
			if(this.state == CameraMoveState.Idle || !this.target) {
				return;
			}

			// Position
			this.SmoothPosition();
		}


		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* ------------------------------------ POSITION FUNCTION -------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		/* --------------------------------------------------------------------------------------------*/
		private void SmoothPosition()
		{
			this.targetPosition = this.target.position + (this.target.rotation * Vector3Extension.UP) * this.height;
			this.myTransform.position = Vector3.SmoothDamp(
				this.myTransform.position,
				this.targetPosition,
				ref this.velocity,
				this.smoothCurve.Evaluate(this.snakeData.speed)
			);
		}




		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* ----------------------------------- ROTATION FUNCTIONS ------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		private void OnHeartRotate(_Transform previous, _Transform current)
		{
			Quaternion targetRotation;
			float angle;
			bool locked;

			this.TryStopCoroutine(ref this.smoothRotationCoroutine);

			angle = Quaternion.Angle(this.myTransform.rotation, this.previousTarget);
			locked = (angle < SMOOTH_LOCK_THRESHOLD);
			targetRotation = current.rotation * Quaternion.Euler(90, 0, 0);
			this.previousTarget = targetRotation;
			this.StartAndStopCoroutine(ref this.smoothRotationCoroutine, this.SmoothRotationCoroutine(previous, current, targetRotation, locked));
		}

		private IEnumerator SmoothRotationCoroutine(_Transform previous, _Transform current, Quaternion targetRotation, bool locked)
		{
			Vector3 lookUp, targetRight;
			Vector3 tgtp;
			Quaternion tgtr;
			int rightDot, forwardDot;
			float previousAngle, angle;


			targetRight = this.target.rotation * Vector3Extension.RIGHT;
			rightDot = Mathf.RoundToInt(Vector3.Dot(targetRight, current.right));
			forwardDot = Mathf.RoundToInt(Vector3.Dot(previous.up, -rightDot * current.forward));
			lookUp = current.forward + rightDot * forwardDot * current.up;

			// Camera smoothly reaches 'lookat' rotation
			do {
				tgtp = this.GetTargetLockAxis(previous, rightDot, locked);
				tgtr = Quaternion.LookRotation(tgtp - this.myTransform.position, lookUp);
				this.myTransform.rotation = Quaternion.Slerp(this.myTransform.rotation, tgtr, SMOOTH_TRANSITION * Time.deltaTime);
				angle = Quaternion.Angle(this.myTransform.rotation, tgtr);

				yield return null;
			}
			while(angle > 1f);

			// Camera looks at target until nearly reaches target rotation
			do {
				tgtp = this.GetTargetLockAxis(previous, rightDot, locked);
				this.myTransform.LookAt(tgtp, lookUp);
				angle = Quaternion.Angle(this.myTransform.rotation, targetRotation);

				yield return null;
			}
			while(angle > 10f);

			// Camera continues to look at target until it reaches best angle possible
			do {
				tgtp = this.GetTargetLockAxis(previous, rightDot, locked);
				this.myTransform.LookAt(tgtp, lookUp);
				previousAngle = angle;
				angle = Quaternion.Angle(this.myTransform.rotation, targetRotation);

				yield return null;
			}
			while(previousAngle > angle);

			// Camera finishes its rotation to match heart rotation
			while(Quaternion.Angle(this.myTransform.rotation, targetRotation) > 0.1f) {
				this.myTransform.rotation = Quaternion.Slerp(this.myTransform.rotation, targetRotation, SMOOTH_OMEGA * Time.deltaTime);
				yield return null;
			}

			this.myTransform.rotation = targetRotation;
		}

		private Vector3 GetTargetLockAxis(_Transform previous, int dot, bool locked)
		{
			if(!locked) {
				return this.target.position;
			}

			Vector3 tgtp = this.target.position;

			// dot == 0 -> heart rotate around its z axis
			// dot != 0 -> heart rotate around its x axis

			// We use old heart rotation for calculs as nex heart rotation miss some informations

			if(dot == 0)
			{
				if(Mathf.RoundToInt(Vector3.Dot(previous.forward, Vector3Extension.RIGHT)) != 0) {
					tgtp.Set(this.myTransform.position.x, tgtp.y, tgtp.z);
				} else if(Mathf.RoundToInt(Vector3.Dot(previous.forward, Vector3Extension.UP)) != 0) {
					tgtp.Set(tgtp.x, this.myTransform.position.y, tgtp.z);
				} else {
					tgtp.Set(tgtp.x, tgtp.y, this.myTransform.position.z);
				}
			}
			else
			{
				if(Mathf.RoundToInt(Vector3.Dot(previous.right, Vector3Extension.RIGHT)) != 0) {
					tgtp.Set(this.myTransform.position.x, tgtp.y, tgtp.z);
				} else if(Mathf.RoundToInt(Vector3.Dot(previous.right, Vector3Extension.UP)) != 0) {
					tgtp.Set(tgtp.x, this.myTransform.position.y, tgtp.z);
				} else {
					tgtp.Set(tgtp.x, tgtp.y, this.myTransform.position.z);
				}
			}

			return tgtp;
		}
	}
}