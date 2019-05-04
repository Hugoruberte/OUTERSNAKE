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

		private Vector3 targetPosition;
		private Vector3 velocity = Vector3Extension.ZERO;
		private Quaternion targetRotation;
		private IEnumerator smoothRotationCoroutine = null;
		private SnakeController snakeController;


		protected override void Awake()
		{
			base.Awake();
			
			this.myTransform = transform;
		}

		void Start()
		{
			this.heart = HeartManager.instance.heart;
			this.snakeController = SnakeManager.instance.snakeController;

			this.heart.onRotate.AddListener(this.OnHeartRotate);

			this.targetRotation = this.heart.rotation * Quaternion.Euler(90, 0, 0);

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
			this.targetRotation = this.heart.rotation * Quaternion.Euler(90, 0, 0);

			// technique of the ancient, very mystical, much dangerous
			this.oldHeart.Copy(this.cacheHeart);
			this.cacheHeart.Copy(this.heart);

			this.StartAndStopCoroutine(ref this.smoothRotationCoroutine, this.SmoothRotationCoroutine(this.targetRotation));
		}

		private IEnumerator SmoothRotationCoroutine(Quaternion targetHeart)
		{
			Vector3 lookUp;
			Vector3 tgt;
			int rightDot;
			int forwardDot;
			float previousAngle;
			float angle;


			rightDot = Mathf.RoundToInt(Vector3.Dot(this.target.right, this.heart.right));
			forwardDot = Mathf.RoundToInt(Vector3.Dot(this.oldHeart.up, -rightDot * this.heart.forward));
			lookUp = this.heart.forward + rightDot * forwardDot * this.heart.up;


			// Camera looks at target during face rotation
			do {
				tgt = GetTargetLockAxis(rightDot);
				this.myTransform.LookAt(tgt, lookUp);
				angle = Quaternion.Angle(this.myTransform.rotation, targetHeart);

				yield return null;
			}
			while(angle > 10f);

			// Camera continues to look at target until it reaches best angle possible
			do {
				tgt = GetTargetLockAxis(rightDot);
				this.myTransform.LookAt(tgt, lookUp);
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

		private Vector3 GetTargetLockAxis(int dot)
		{
			Vector3 tgt = this.target.position;

			// dot == 0 -> heart rotate around its z axis
			// dot != 0 -> heart rotate around its x axis

			// We use old heart rotation for the calculus

			if(dot == 0)
			{
				if(Mathf.RoundToInt(Vector3.Dot(this.oldHeart.forward, Vector3Extension.RIGHT)) != 0) {
					tgt.Set(this.myTransform.position.x, tgt.y, tgt.z);
				} else if(Mathf.RoundToInt(Vector3.Dot(this.oldHeart.forward, Vector3Extension.UP)) != 0) {
					tgt.Set(tgt.x, this.myTransform.position.y, tgt.z);
				} else {
					tgt.Set(tgt.x, tgt.y, this.myTransform.position.z);
				}
			}
			else
			{
				if(Mathf.RoundToInt(Vector3.Dot(this.oldHeart.right, Vector3Extension.RIGHT)) != 0) {
					tgt.Set(this.myTransform.position.x, tgt.y, tgt.z);
				} else if(Mathf.RoundToInt(Vector3.Dot(this.oldHeart.right, Vector3Extension.UP)) != 0) {
					tgt.Set(tgt.x, this.myTransform.position.y, tgt.z);
				} else {
					tgt.Set(tgt.x, tgt.y, this.myTransform.position.z);
				}
			}

			return tgt;
		}
	}
}