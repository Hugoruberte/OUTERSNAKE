using UnityEngine;
using UnityEngine.PostProcessing;
using System.Collections;
using Tools;

public enum CameraState {
	Idle = 0,
	Moving
}

public class CameraScript : MonoBehaviour
{
	[HideInInspector]
	public Transform Heart;
	private Transform Snake;
	private Transform targetCam;
	private GameObject liveText;
	private GameObject scoreText;

	private Transform myTransform;
	private Transform myCamera;

	[Header("Settings")]
	[Tooltip("Distance camera <-> snake")]
	public int height = 25;

	private IEnumerator shake_coroutine;
	private IEnumerator loop_coroutine;

	private float dist;
	private float diff;
	private float smooth = 0.5f;
	public float omega = 1.5f;

	private float targetColorR;
	private float targetColorG;
	private float targetColorB;

	private float referenceR;
	private float referenceG;
	private float referenceB;

	private float current_intensity = -1.0f;

	private Vector3 targetPosition;
	private Vector3 velocity = Vector3.zero;

	private Quaternion targetRotation;
	private Quaternion fromRotation;

	public CameraState State = CameraState.Idle;

	private bool Slerpt = false;

	[HideInInspector]
	public UnityStandardAssets.ImageEffects.NoiseAndScratches noise;
	[HideInInspector]
	public GlitchEffect glitch;
	[HideInInspector]
	public UnityStandardAssets.ImageEffects.BloomOptimized bloom;
	[HideInInspector]
	public UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration vignette;
	[HideInInspector]
	public UnityStandardAssets.ImageEffects.Fisheye fisheye;
	[HideInInspector]
	public UnityStandardAssets.ImageEffects.SunShafts shaft;
	[HideInInspector]
	public PostProcessingBehaviour postProcess;

	private GameManagerV1 gameManager;
	private SnakeControllerV3 snakeScript;



	void Awake()
	{
		myTransform = transform;
		myCamera = myTransform.Find("Camera");

		liveText = GameObject.Find("Canvas/InGame/Life");
		scoreText = GameObject.Find("Canvas/InGame/Score");

		noise = myCamera.GetComponent<UnityStandardAssets.ImageEffects.NoiseAndScratches>();
		glitch = myCamera.GetComponent<GlitchEffect>();
		bloom = myCamera.GetComponent<UnityStandardAssets.ImageEffects.BloomOptimized>();
		vignette = myCamera.GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>();
		fisheye = myCamera.GetComponent<UnityStandardAssets.ImageEffects.Fisheye>();
		shaft = myCamera.GetComponent<UnityStandardAssets.ImageEffects.SunShafts>();
		postProcess = myCamera.GetComponent<PostProcessingBehaviour>();
		
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();

		Heart = gameManager.MainPlanet.Find("Heart");
		fisheye.enabled = true;
		
		Snake = GameObject.FindWithTag("Player").transform;
		snakeScript = Snake.GetComponent<SnakeControllerV3>();
		
		targetRotation = Heart.rotation;
	}

	void Update()
	{
		if(State != CameraState.Idle)
		{
			myTransform.position = Vector3.SmoothDamp(myTransform.position, targetPosition, ref velocity, smooth);

			if(Slerpt)
			{
				dist = Vector3.Distance(myTransform.position, targetPosition);
				myTransform.rotation = Quaternion.Slerp(fromRotation, targetRotation, 1.0f - (dist / diff));
			}
			else
			{
				myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, omega * Time.deltaTime);
			}
		}
	}

	public void SetEffects(bool active)
	{
		noise.enabled = active;
		glitch.enabled = active;
		bloom.enabled = active;
		vignette.enabled = active;
		fisheye.enabled = active;
		shaft.enabled = active;
		postProcess.profile.depthOfField.enabled = active;
		postProcess.profile.colorGrading.enabled = active;
	}
	public void StandardEffectSetup()
	{
		noise.enabled = false;
		glitch.enabled = false;
		bloom.enabled = true;
		vignette.enabled = true;
		fisheye.enabled = true;
		shaft.enabled = true;
		postProcess.profile.depthOfField.enabled = false;
		postProcess.profile.colorGrading.enabled = false;

		bloom.threshold = 0.25f;
		bloom.intensity = 0.4f;
		bloom.blurSize = 3f;
		bloom.blurIterations = 2;

		vignette.intensity = 0.03f;
		vignette.blur = 0f;
		vignette.chromaticAberration = -5f;

		fisheye.strengthX = 0.025f;
		fisheye.strengthY = 0.025f;

		shaft.sunShaftBlurRadius = 2.5f;
		shaft.radialBlurIterations = 2;
		shaft.sunShaftIntensity = 1f;
	}

	public void Shake(float intensity)
	{
		if(shake_coroutine != null)
		{
			if(intensity > current_intensity)
				StopCoroutine(shake_coroutine);
			else
				return;
		}
		current_intensity = intensity;
		shake_coroutine = ShakeCoroutine(intensity);
		StartCoroutine(shake_coroutine);
	}
	public void Shake(Shaketype type)
	{
		switch(type)
		{
			case Shaketype.Nuclear:
				Shake(20.0f);
				break;

			case Shaketype.Rocket:
				Shake(2.5f);
				break;

			case Shaketype.Bomb:
				Shake(3.0f);
				break;

			case Shaketype.Gentle:
				Shake(0.1f);
				break;

			default:
				Debug.LogError("La configuration '" + name + "' n'existe pas !");
				break;
		}
	}
	private IEnumerator ShakeCoroutine(float intensity)
	{
		float current = intensity;
		float speed;
		Vector3 shakePosition;

		while(current > 0.055f)
		{
			shakePosition = Random.insideUnitSphere * current;
			speed = Vector3.Distance(myCamera.localPosition, shakePosition) / 0.05f;
			while(Vector3.Distance(myCamera.localPosition, shakePosition) > 0.01f)
			{
				myCamera.localPosition = Vector3.MoveTowards(myCamera.localPosition, shakePosition, speed * Time.deltaTime);
				yield return null;
			}
			current /= 1.75f;	//?
		}
		speed = Vector3.Distance(myCamera.localPosition, Vector3.zero) / 0.05f;
		while(Vector3.Distance(myCamera.localPosition, Vector3.zero) > 0.01f)
		{
			myCamera.localPosition = Vector3.MoveTowards(myCamera.localPosition, Vector3.zero, speed * Time.deltaTime);
			yield return null;
		}

		current_intensity = -1.0f;
		myCamera.localPosition = Vector3.zero;
	}

	public void NuclearEffectSetup(bool boom)
	{
		Transform hideo = myTransform.Find("Hideo");
		if(!boom && hideo)
			Destroy(hideo.gameObject);

		liveText.SetActive(!boom);
		scoreText.SetActive(!boom);

		if(boom)
		{
			fisheye.strengthX = 0.08f;
			fisheye.strengthY = 0.08f;
			targetPosition = Vector3.zero;
			myTransform.position = Vector3.zero;
		}
		else
		{
			fisheye.strengthX = 0.025f;
			fisheye.strengthY = 0.025f;
		}
	}















	public void SafeSetup(Transform tar)
	{
		targetCam = tar;

		targetPosition = targetCam.position;
		targetRotation = targetCam.rotation;
		smooth = 0.0f;

		myTransform.position = targetPosition;
		myTransform.rotation = targetRotation;

		if(loop_coroutine != null)
			StopCoroutine(loop_coroutine);
		loop_coroutine = SafeCoroutine();
		StartCoroutine(loop_coroutine);
	}
	private IEnumerator SafeCoroutine()
	{
		while(true)
		{
			targetPosition = targetCam.position;
			targetRotation = targetCam.rotation;
			yield return null;
		}
	}

	public void ExitSafeSetup()
	{
		if(loop_coroutine != null)
			StopCoroutine(loop_coroutine);
		loop_coroutine = ExitSafeCoroutine();
		StartCoroutine(loop_coroutine);
	}
	private IEnumerator ExitSafeCoroutine()
	{
		Slerpt = true;
		targetPosition = Snake.position - Snake.forward * height;
		fromRotation = myTransform.rotation;
		targetRotation = Heart.rotation;
		diff = Vector3.Distance(myTransform.position, targetPosition);
		dist = diff;
		smooth = 0.25f;

		while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
			yield return null;

		Slerpt = false;

		NormalSetup();
	}

	public void RocketSetup(Transform tar)
	{
		targetCam = tar;

		targetPosition = targetCam.position;
		targetRotation = targetCam.rotation;
		smooth = 0.25f;
		omega = 5.0f;

		if(loop_coroutine != null)
			StopCoroutine(loop_coroutine);
		loop_coroutine = RocketCoroutine();
		StartCoroutine(loop_coroutine);
	}
	public void RocketSetup(Transform tar, float ptime, float pomega)
	{
		targetCam = tar;

		targetPosition = targetCam.position;
		targetRotation = targetCam.rotation;
		smooth = ptime;
		omega = pomega;

		if(loop_coroutine != null)
			StopCoroutine(loop_coroutine);
		loop_coroutine = RocketCoroutine();
		StartCoroutine(loop_coroutine);
	}
	private IEnumerator RocketCoroutine()
	{
		while(true)
		{
			targetPosition = targetCam.position;
			targetRotation = targetCam.rotation;
			yield return null;
		}
	}

	public void TeleporterSetup()
	{
		targetPosition = Snake.position - Snake.forward * height;
		targetRotation = Snake.rotation;
		smooth = 0.175f;
		omega = 2f;

		if(loop_coroutine != null)
			StopCoroutine(loop_coroutine);
		loop_coroutine = TeleporterCoroutine();
		StartCoroutine(loop_coroutine);
	}
	private IEnumerator TeleporterCoroutine()
	{
		while(true)
		{
			targetPosition = Snake.position - Snake.forward * height;
			targetRotation = Snake.rotation;
			yield return null;
		}
	}

	public void TargetSetup(Transform tar, float ftime, float fomega)
	{
		targetCam = tar;

		targetPosition = targetCam.position;
		targetRotation = targetCam.rotation;
		smooth = ftime;
		omega = fomega;

		if(loop_coroutine != null)
			StopCoroutine(loop_coroutine);
		loop_coroutine = TargetCoroutine();
		StartCoroutine(loop_coroutine);
	}
	public void TargetSetup(Transform tar, float ftime)
	{
		if(ftime < 0.005f)
		{
			if(loop_coroutine != null)
				StopCoroutine(loop_coroutine);

			targetPosition = targetCam.position;
			targetRotation = targetCam.rotation;
			myTransform.position = targetPosition;
			myTransform.rotation = targetRotation;
			return;
		}
		
		Slerpt = true;

		targetCam = tar;
		targetPosition = targetCam.position;
		targetRotation = targetCam.rotation;

		diff = Vector3.Distance(myTransform.position, targetPosition);
		dist = diff;
		smooth = ftime;

		if(loop_coroutine != null)
			StopCoroutine(loop_coroutine);
		loop_coroutine = TargetSlerpCoroutine();
		StartCoroutine(loop_coroutine);
	}
	public void TargetSetup(Vector3 pos, Quaternion quat, float ftime)
	{
		if(ftime < 0.005f)
		{
			if(loop_coroutine != null)
				StopCoroutine(loop_coroutine);

			targetPosition = pos;
			targetRotation = quat;
			myTransform.position = targetPosition;
			myTransform.rotation = targetRotation;
			return;
		}

		Slerpt = true;

		targetPosition = pos;
		fromRotation = myTransform.rotation;
		targetRotation = quat;

		diff = Vector3.Distance(myTransform.position, targetPosition);
		dist = diff;
		smooth = ftime;

		if(loop_coroutine != null)
			StopCoroutine(loop_coroutine);
		loop_coroutine = TargetSlerpCoroutine();
		StartCoroutine(loop_coroutine);
	}
	public void TargetSetup(Vector3 pos, Quaternion quat, float ftime, float fomega)
	{
		targetPosition = pos;
		targetRotation = quat;
		smooth = ftime;
		omega = fomega;

		if(loop_coroutine != null)
			StopCoroutine(loop_coroutine);
		loop_coroutine = TargetCoroutine();
		StartCoroutine(loop_coroutine);
	}
	private IEnumerator TargetSlerpCoroutine()
	{
		while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
			yield return null;

		Slerpt = false;
	}
	private IEnumerator TargetCoroutine()
	{
		while(true)
		{
			targetPosition = targetCam.position;
			targetRotation = targetCam.rotation;
			yield return null;
		}
	}

	public void NormalSetup()
	{
		targetPosition = Snake.position - Snake.forward * height;
		targetRotation = Heart.rotation;

		if(loop_coroutine != null)
			StopCoroutine(loop_coroutine);
		loop_coroutine = NormalCoroutine();
		StartCoroutine(loop_coroutine);
	}
	public void NormalSetup(float ptime, float pomega)	//Hell - > smooth = 0.3f, omega = 2.0f
	{
		targetPosition = Snake.position - Snake.forward * height;
		targetRotation = Heart.rotation;

		smooth = ptime;
		omega = pomega;

		if(loop_coroutine != null)
			StopCoroutine(loop_coroutine);
		loop_coroutine = NormalCoroutine();
		StartCoroutine(loop_coroutine);
	}
	private IEnumerator NormalCoroutine()
	{
		while(true)
		{
			float speed = snakeScript.Speed;
			smooth = (-9f/800f)*speed + (49f/80f);
			//omega = (17f/80f)*speed + (-5f/8f);
			omega = 1383140f + (1.762732f - 1383140f)/(1f + Mathf.Pow(speed/924.1536f, 4.124743f));
			
			targetPosition = Snake.position - Snake.forward * height;
			targetRotation = Heart.rotation;
			yield return null;
		}
	}

	public void InstantSnakeSetup()
	{
		targetPosition = Snake.position - Snake.forward * height;
		targetRotation = Heart.rotation;
		myTransform.position = targetPosition;
		myTransform.rotation = targetRotation;
	}
}
