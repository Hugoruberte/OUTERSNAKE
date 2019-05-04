using UnityEngine;
using System.Collections;
using Tools;

namespace Cameras
{
	public enum CameraMoveState
	{
		Idle = 0,
		Move
	}

	public class CameraPlanetController : Singleton<CameraPlanetController>
	{
		[Header("State")]
		public Transform target;
		public CameraMoveState state = CameraMoveState.Idle;

		[Header("Settings")]
		[SerializeField, Tooltip("Distance camera <-> target"), Range(5, 75)]
		private int height = 25;

		[SerializeField]
		private AnimationCurve smoothCurve = new AnimationCurve();

		private Transform myTransform;
		private _Transform heart;
		private _Transform cacheHeart = new _Transform();
		private _Transform oldHeart = new _Transform();

		private const float SMOOTH_OMEGA = 3f;
		private const float SMOOTH_TRANSITION = 75f;
		private const float SMOOTH_LOCK_THRESHOLD = 20f;

		private Vector3 targetPosition;
		private Vector3 velocity = Vector3Extension.ZERO;
		private IEnumerator smoothRotationCoroutine = null;
		private SnakeController snakeController;
		private Quaternion previousTarget;



		protected override void Awake()
		{
			base.Awake();
			
			this.myTransform = transform;
			this.previousTarget = this.myTransform.rotation;
		}

		void Start()
		{
			this.heart = HeartManager.instance.heart;
			this.snakeController = SnakeManager.instance.snakeController;

			this.heart.onRotate.AddListener(this.OnHeartRotate);

			this.cacheHeart.Copy(this.heart);
			this.oldHeart.Copy(this.heart);

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
			this.targetPosition = this.target.position + this.target.up * this.height;
			this.myTransform.position = Vector3.SmoothDamp(this.myTransform.position, this.targetPosition, ref this.velocity, this.smoothCurve.Evaluate(this.snakeController.speed));
		}




		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* ----------------------------------- ROTATION FUNCTIONS ------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		private void OnHeartRotate()
		{
			Quaternion targetHeart;
			float angle;
			bool locked;

			this.TryStopCoroutine(ref this.smoothRotationCoroutine);

			// art of the ancient, very mystical, much dangerous, no touchy
			this.oldHeart.Copy(this.cacheHeart);
			this.cacheHeart.Copy(this.heart);

			angle = Quaternion.Angle(this.myTransform.rotation, this.previousTarget);
			locked = (angle < SMOOTH_LOCK_THRESHOLD);
			targetHeart = this.heart.rotation * Quaternion.Euler(90, 0, 0);
			this.previousTarget = targetHeart;
			this.StartAndStopCoroutine(ref this.smoothRotationCoroutine, this.SmoothRotationCoroutine(targetHeart, locked));
		}

		private IEnumerator SmoothRotationCoroutine(Quaternion targetHeart, bool locked)
		{
			Vector3 lookUp;
			Vector3 tgtp;
			Quaternion tgtr;
			int rightDot;
			int forwardDot;
			float previousAngle;
			float angle;


			rightDot = Mathf.RoundToInt(Vector3.Dot(this.target.right, this.heart.right));
			forwardDot = Mathf.RoundToInt(Vector3.Dot(this.oldHeart.up, -rightDot * this.heart.forward));
			lookUp = this.heart.forward + rightDot * forwardDot * this.heart.up;

			// Camera smoothly reaches 'lookat' rotation
			do {
				tgtp = this.GetTargetLockAxis(rightDot, locked);
				tgtr = Quaternion.LookRotation(tgtp - this.myTransform.position, lookUp);
				this.myTransform.rotation = Quaternion.Slerp(this.myTransform.rotation, tgtr, SMOOTH_TRANSITION * Time.deltaTime);
				angle = Quaternion.Angle(this.myTransform.rotation, tgtr);

				yield return null;
			}
			while(angle > 1f);

			// Camera looks at target until nearly reaches target rotation
			do {
				tgtp = this.GetTargetLockAxis(rightDot, locked);
				this.myTransform.LookAt(tgtp, lookUp);
				angle = Quaternion.Angle(this.myTransform.rotation, targetHeart);

				yield return null;
			}
			while(angle > 10f);

			// Camera continues to look at target until it reaches best angle possible
			do {
				tgtp = this.GetTargetLockAxis(rightDot, locked);
				this.myTransform.LookAt(tgtp, lookUp);
				previousAngle = angle;
				angle = Quaternion.Angle(this.myTransform.rotation, targetHeart);

				yield return null;
			}
			while(previousAngle > angle);

			// Camera finishes its rotation to match heart rotation
			while(Quaternion.Angle(this.myTransform.rotation, targetHeart) > 0.1f) {
				this.myTransform.rotation = Quaternion.Slerp(this.myTransform.rotation, targetHeart, SMOOTH_OMEGA * Time.deltaTime);
				yield return null;
			}

			this.myTransform.rotation = targetHeart;
		}

		private Vector3 GetTargetLockAxis(int dot, bool locked)
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
				if(Mathf.RoundToInt(Vector3.Dot(this.oldHeart.forward, Vector3Extension.RIGHT)) != 0) {
					tgtp.Set(this.myTransform.position.x, tgtp.y, tgtp.z);
				} else if(Mathf.RoundToInt(Vector3.Dot(this.oldHeart.forward, Vector3Extension.UP)) != 0) {
					tgtp.Set(tgtp.x, this.myTransform.position.y, tgtp.z);
				} else {
					tgtp.Set(tgtp.x, tgtp.y, this.myTransform.position.z);
				}
			}
			else
			{
				if(Mathf.RoundToInt(Vector3.Dot(this.oldHeart.right, Vector3Extension.RIGHT)) != 0) {
					tgtp.Set(this.myTransform.position.x, tgtp.y, tgtp.z);
				} else if(Mathf.RoundToInt(Vector3.Dot(this.oldHeart.right, Vector3Extension.UP)) != 0) {
					tgtp.Set(tgtp.x, this.myTransform.position.y, tgtp.z);
				} else {
					tgtp.Set(tgtp.x, tgtp.y, this.myTransform.position.z);
				}
			}

			return tgtp;
		}
	}
}