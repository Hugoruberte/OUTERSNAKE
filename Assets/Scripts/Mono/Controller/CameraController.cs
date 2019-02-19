using UnityEngine;
using UnityEngine.PostProcessing;
using System.Collections;
using Tools;

namespace Cameras
{
	public enum CameraMoveState
	{
		Idle = 0,
		Move
	}

	public class CameraController : MonoBehaviour
	{
		public Transform heart;
		public Transform snake;
		private Transform myTransform;
		private _Transform cacheHeart = new _Transform();
		private _Transform oldHeart = new _Transform();

		[Header("Settings")]
		[SerializeField, Tooltip("Distance camera <-> snake"), Range(10, 75)]
		private int height = 25;

		[Range(0.01f, 0.5f), SerializeField]
		private float smooth = 0.2f;
		private const float SMOOTH_OMEGA = 3f;

		private Vector3 targetPosition;
		private Vector3 velocity = Vector3.zero;

		private Quaternion targetRotation;
		private Quaternion previousTargetRotation = Quaternion.identity;

		[Header("State")]
		public CameraMoveState state = CameraMoveState.Idle;

		private IEnumerator smoothRotationCoroutine = null;


		void Awake()
		{
			myTransform = transform;
		}

		void Start()
		{
			targetRotation = heart.rotation * Quaternion.Euler(90, 0, 0);
			previousTargetRotation = targetRotation;

			cacheHeart.Copy(heart);
			oldHeart.Copy(heart);
		}

		void Update()
		{
			if(state == CameraMoveState.Idle)
			{
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
			targetPosition = snake.position + snake.up * height;
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
			// (at this point snake has already rotated to the new face)
			if(targetRotation != previousTargetRotation)
			{
				previousTargetRotation = targetRotation;
				// technique of the ancient, very mystical, much dangerous
				oldHeart.Copy(cacheHeart);
				cacheHeart.Copy(heart);

				if(smoothRotationCoroutine != null)
					StopCoroutine(smoothRotationCoroutine);
				smoothRotationCoroutine = SmoothRotationCoroutine(targetRotation);
				StartCoroutine(smoothRotationCoroutine);
			}
		}

		private IEnumerator SmoothRotationCoroutine(Quaternion targetHeart)
		{
			Vector3 lookUp;
			Vector3 target;
			int rightDot;
			int forwardDot;
			float previousAngle;
			float angle;


			rightDot = Mathf.RoundToInt(Vector3.Dot(snake.right, heart.right));
			forwardDot = Mathf.RoundToInt(Vector3.Dot(oldHeart.up, -rightDot * heart.forward));
			lookUp = heart.forward + rightDot * forwardDot * heart.up;


			// Camera looks at snake during face rotation
			do {
				target = GetSnakeTargetLockAxis(rightDot);
				myTransform.LookAt(target, lookUp);
				angle = Quaternion.Angle(myTransform.rotation, targetHeart);

				yield return null;
			}
			while(angle > 10f);

			// Camera continues to look at snake until it reaches best angle
			do {
				target = GetSnakeTargetLockAxis(rightDot);
				myTransform.LookAt(target, lookUp);
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

		private Vector3 GetSnakeTargetLockAxis(int dot)
		{
			Vector3 target = snake.position;

			// dot == 0 -> heart rotate around its z axis
			// dot != 0 -> heart rotate around its x axis

			// We use old heart rotation for the calculus

			if(dot == 0) {
				if(Mathf.RoundToInt(Vector3.Dot(oldHeart.forward, Vector3.right)) != 0)
					target.Set(myTransform.position.x, target.y, target.z);
				else if(Mathf.RoundToInt(Vector3.Dot(oldHeart.forward, Vector3.up)) != 0)
					target.Set(target.x, myTransform.position.y, target.z);
				else
					target.Set(target.x, target.y, myTransform.position.z);
			} else {
				if(Mathf.RoundToInt(Vector3.Dot(oldHeart.right, Vector3.right)) != 0)
					target.Set(myTransform.position.x, target.y, target.z);
				else if(Mathf.RoundToInt(Vector3.Dot(oldHeart.right, Vector3.up)) != 0)
					target.Set(target.x, myTransform.position.y, target.z);
				else
					target.Set(target.x, target.y, myTransform.position.z);
			}

			return target;
		}
	}
}