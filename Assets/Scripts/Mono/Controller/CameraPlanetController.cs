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

		private Transform myTransform;
		private _Transform heart;
		private _Transform cacheHeart = new _Transform();
		private _Transform oldHeart = new _Transform();

		[Header("Settings")]
		[SerializeField, Tooltip("Distance camera <-> target"), Range(5, 75)]
		private int height = 25;

		[Range(0.01f, 0.5f), SerializeField]
		private float smooth = 0.2f;
		private const float SMOOTH_OMEGA = 3f;

		private Vector3 targetPosition;
		private Vector3 velocity = Vector3.zero;

		private Quaternion targetRotation;
		private Quaternion previousTargetRotation = Quaternion.identity;

		private IEnumerator smoothRotationCoroutine = null;


		protected override void Awake()
		{
			base.Awake();
			
			myTransform = transform;
		}

		void Start()
		{
			heart = HeartManager.instance.heart;

			targetRotation = heart.rotation * Quaternion.Euler(90, 0, 0);
			previousTargetRotation = targetRotation;

			cacheHeart.Copy(heart);
			oldHeart.Copy(heart);

			if(!target) {
				Debug.LogWarning("WARNING : Camera does not have any target to follow !");
			}
		}

		void Update()
		{
			if(state == CameraMoveState.Idle || !target) {
				return;
			}

			// Position
			SmoothPosition();		
			// Rotation
			SmoothRotation();
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
			targetPosition = target.position + target.up * height;
			myTransform.position = Vector3.SmoothDamp(myTransform.position, targetPosition, ref velocity, smooth);
		}


		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* ----------------------------------- ROTATION FUNCTIONS ------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		/* -------------------------------------------------------------------------------------------*/
		private void SmoothRotation()
		{
			targetRotation = heart.rotation * Quaternion.Euler(90, 0, 0);

			// A face rotation has been detected !
			// (at this point target has already rotated to the new face)
			if(targetRotation != previousTargetRotation)
			{
				previousTargetRotation = targetRotation;
				// technique of the ancient, very mystical, much dangerous
				oldHeart.Copy(cacheHeart);
				cacheHeart.Copy(heart);

				if(smoothRotationCoroutine != null) {
					StopCoroutine(smoothRotationCoroutine);
				}
				smoothRotationCoroutine = SmoothRotationCoroutine(targetRotation);
				StartCoroutine(smoothRotationCoroutine);
			}
		}

		private IEnumerator SmoothRotationCoroutine(Quaternion targetHeart)
		{
			Vector3 lookUp;
			Vector3 tgt;
			int rightDot;
			int forwardDot;
			float previousAngle;
			float angle;


			rightDot = Mathf.RoundToInt(Vector3.Dot(target.right, heart.right));
			forwardDot = Mathf.RoundToInt(Vector3.Dot(oldHeart.up, -rightDot * heart.forward));
			lookUp = heart.forward + rightDot * forwardDot * heart.up;


			// Camera looks at target during face rotation
			do {
				tgt = GetTargetLockAxis(rightDot);
				myTransform.LookAt(tgt, lookUp);
				angle = Quaternion.Angle(myTransform.rotation, targetHeart);

				yield return null;
			}
			while(angle > 10f);

			// Camera continues to look at target until it reaches best angle
			do {
				tgt = GetTargetLockAxis(rightDot);
				myTransform.LookAt(tgt, lookUp);
				previousAngle = angle;
				angle = Quaternion.Angle(myTransform.rotation, targetHeart);

				yield return null;
			}
			while(previousAngle > angle);

			// Camera rotate to match heart rotation
			while(Quaternion.Angle(myTransform.rotation, targetHeart) > 0.1f)
			{
				myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetHeart, SMOOTH_OMEGA * Time.deltaTime);
				yield return null;
			}

			myTransform.rotation = targetHeart;
			smoothRotationCoroutine = null;
		}

		private Vector3 GetTargetLockAxis(int dot)
		{
			Vector3 tgt = target.position;

			// dot == 0 -> heart rotate around its z axis
			// dot != 0 -> heart rotate around its x axis

			// We use old heart rotation for the calculus

			if(dot == 0) {
				if(Mathf.RoundToInt(Vector3.Dot(oldHeart.forward, Vector3.right)) != 0) {
					tgt.Set(myTransform.position.x, tgt.y, tgt.z);
				} else if(Mathf.RoundToInt(Vector3.Dot(oldHeart.forward, Vector3.up)) != 0) {
					tgt.Set(tgt.x, myTransform.position.y, tgt.z);
				} else {
					tgt.Set(tgt.x, tgt.y, myTransform.position.z);
				}
			} else {
				if(Mathf.RoundToInt(Vector3.Dot(oldHeart.right, Vector3.right)) != 0) {
					tgt.Set(myTransform.position.x, tgt.y, tgt.z);
				} else if(Mathf.RoundToInt(Vector3.Dot(oldHeart.right, Vector3.up)) != 0) {
					tgt.Set(tgt.x, myTransform.position.y, tgt.z);
				} else {
					tgt.Set(tgt.x, tgt.y, myTransform.position.z);
				}
			}

			return tgt;
		}
	}
}