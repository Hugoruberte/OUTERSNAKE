using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class TeleporterStationScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform Parent;
	[HideInInspector]
	public Transform Receptor;
	[HideInInspector]
	public Transform Planet;
	private Transform BaliseEditor;
	private Transform Snake;
	private Transform Decoy;
	private Transform Heart;
	private Transform ReceptorHeart;
	private Transform Orb;

	private TeleporterScript teleScript;
	private SnakeControllerV3 snakeScript;
	private GameManagerV1 gameManager;
	private TeleporterStationScript receptorScript;
	private CameraScript cameraScript;
	private PlanetSetup planetSetup;

	public int myCell = -1;
	public Faces Face = Faces.FaceY1;

	[HideInInspector]
	public float OrbSpeed = 10.0f;
	[HideInInspector]
	public float Parcours = 0.0f;

	private ParticleSystem myParticle;

	private Collider myColl;
	private Collider ReceptorColl;
	private Collider SnakeColl;

	private Renderer SnakeRend;
	private Renderer SnakeHoodRend;

	private Vector3 targetPosition = Vector3.zero;
	private Quaternion targetOrbRotation = Quaternion.identity;

	[HideInInspector]
	public Vector3[] balisesPosition;
	[HideInInspector]
	public Quaternion[] balisesRotation;

	void Awake()
	{
		myTransform = transform;
		Parent = myTransform.parent;
		Snake = GameObject.FindWithTag("Player").transform;

		snakeScript = Snake.GetComponent<SnakeControllerV3>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		teleScript = Parent.GetComponent<TeleporterScript>();
		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();

		BaliseEditor = Parent.Find("Balises");
		Decoy = myTransform.Find("Decoy");
		Orb = myTransform.Find("Body/Orb");

		SnakeRend = Snake.GetComponent<Renderer>();
		SnakeHoodRend = Snake.Find("Hood").GetComponent<Renderer>();
		SnakeColl = Snake.GetComponent<Collider>();
		
		myParticle = Decoy.GetComponent<ParticleSystem>();
	}

	void Start()
	{
		Heart = Planet.Find("Heart");

		receptorScript = Receptor.GetComponent<TeleporterStationScript>();
		ReceptorColl = Receptor.GetComponent<Collider>();

		myCell = myTransform.PositionToCell(Planet, Face);

		targetPosition = myTransform.position;
		
		if(myTransform.name.Contains("A"))
		{
			if(!BaliseEditor || BaliseEditor.childCount == 0)
			{
				balisesPosition = new Vector3[2] {myTransform.position, Receptor.position};
				balisesRotation = new Quaternion[2] {Heart.rotation, Heart.rotation};
				Parcours = Vector3.Distance(myTransform.position, Receptor.position);
			}
			else if(BaliseEditor && BaliseEditor.childCount > 0)
			{
				int count = BaliseEditor.childCount;

				balisesPosition = new Vector3[count];
				balisesRotation = new Quaternion[count];
				for(int i = 0; i < count; i++)
				{
					balisesPosition[i] = BaliseEditor.GetChild(i).transform.position;
					balisesRotation[i] = BaliseEditor.GetChild(i).transform.rotation;
					if(i > 0)
						Parcours += Vector3.Distance(balisesPosition[i-1], balisesPosition[i]);
				}

				Destroy(BaliseEditor.gameObject);
			}

			Vector3[] tmppos = ArrayExtension.DeepCopy(balisesPosition);
			System.Array.Reverse(tmppos);
			receptorScript.balisesPosition = tmppos;
			Quaternion[] tmpquat = ArrayExtension.DeepCopy(balisesRotation);
			System.Array.Reverse(tmpquat);
			receptorScript.balisesRotation = tmpquat;

			receptorScript.Parcours = Parcours;
		}
	}

	void Update()
	{
		if(Quaternion.Angle(Orb.rotation, targetOrbRotation) > 3)
			Orb.rotation = Quaternion.Slerp(Orb.rotation, targetOrbRotation, OrbSpeed * Time.deltaTime);
		else
			targetOrbRotation = Random.rotation;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") && snakeScript.State == SnakeState.Run)
		{
			ReceptorColl.enabled = false;

			if(teleScript.Type == TeleporterType.Planet)
			{
				receptorScript.Planet.GetComponent<PlanetScript>().SetRotation(false);
				ReceptorHeart = receptorScript.Planet.Find("Heart");
				Quaternion q = myTransform.rotation * Receptor.rotation;
				ReceptorHeart.rotation = Heart.rotation * q;
				snakeScript.Heart = ReceptorHeart;
				snakeScript.Face = receptorScript.Face;
			}

			receptorScript.OrbSpeed = 50.0f;
			OrbSpeed = 50.0f;

			//SnakeColl.enabled = false;
			SnakeRend.enabled = false;
			SnakeHoodRend.enabled = false;
			snakeScript.State = SnakeState.Stopped;

			myParticle.Play();

			cameraScript.TeleporterSetup();

			if(teleScript.Mode == TeleporterMode.Transfert)
			{
				StartCoroutine(Transfert());
				StartCoroutine(DecoyTransfert());
			}
			else
			{
				StartCoroutine(Direct());
			}
		}
	}

	private IEnumerator Transfert()
	{
		int length = balisesPosition.Length;
		float dist;
		float diff;
		Vector3 targetBalisePosition;
		Quaternion targetBaliseRotation;
		Quaternion fromRotation;

		Decoy.rotation = balisesRotation[0];

		bool settle = false;

		for(int i = 1; i < length; i++)
		{
			targetBalisePosition = balisesPosition[i];
			targetBaliseRotation = balisesRotation[i];

			fromRotation = Decoy.rotation;
			dist = Vector3.Distance(targetPosition, targetBalisePosition);
			diff = dist;

			if(teleScript.Type == TeleporterType.Planet && i >= length/4f && !settle)
			{
				settle = true;

				snakeScript.upStored = 1;
				snakeScript.rightStored = 0;

				receptorScript.Planet.GetComponent<PlanetScript>().MainSetup(true);
				gameManager.SetMainPlanet(receptorScript.Planet);
				planetSetup.MainPlanetSetObjects();
				Planet.GetComponent<PlanetScript>().MainSetup(false);
				cameraScript.Heart = ReceptorHeart;
			}

			float Speed = Parcours / teleScript.Duration;

			while(dist > 0.1f)
			{
				dist = Vector3.Distance(targetPosition, targetBalisePosition);
				targetPosition = Vector3.MoveTowards(targetPosition, targetBalisePosition, Speed * Time.deltaTime);
				Decoy.rotation = Quaternion.Slerp(fromRotation, targetBaliseRotation, 1f - (dist/diff));
				yield return null;
			}
		}
	}

	private IEnumerator DecoyTransfert()
	{
		Vector3 reference = Vector3.zero;
		Quaternion snakeInitialRotation = Snake.AbsoluteRotation();

		while(Vector3.Distance(Decoy.position, Receptor.position) > 0.25f)
		{
			Decoy.position = Vector3.SmoothDamp(Decoy.position, targetPosition, ref reference, 0.4f);
			Snake.position = Decoy.position;
			Snake.rotation = Decoy.rotation;
			yield return null;
		}

		receptorScript.OrbSpeed = 10.0f;
		OrbSpeed = 10.0f;

		targetPosition = balisesPosition[0];
		Decoy.position = targetPosition;
		Decoy.rotation = balisesRotation[0];

		myParticle.Stop();

		Snake.position = Receptor.position;
		snakeScript.targetPosition = Snake.position;

		SnakeColl.enabled = true;
		SnakeRend.enabled = true;
		SnakeHoodRend.enabled = true;

		gameManager.StartCoroutine(gameManager.InvincibleState());

		if(receptorScript.Face != snakeScript.Face)
		{
			Snake.rotation = Heart.rotation;
			snakeScript.targetRotation = Snake.rotation;
			snakeScript.upStored = 1;
			snakeScript.rightStored = 0;
		}
		else
		{
			Snake.rotation = snakeInitialRotation;
			snakeScript.targetRotation = snakeInitialRotation;
		}

		snakeScript.State = SnakeState.Run;
		cameraScript.NormalSetup();

		while(Vector3.Distance(Snake.position, Receptor.position) < 1.5f)
			yield return null;

		ReceptorColl.enabled = true;
	}

	private IEnumerator Direct()
	{
		Quaternion snakeInitialRotation = Snake.AbsoluteRotation();

		receptorScript.OrbSpeed = 10.0f;
		OrbSpeed = 10.0f;

		Snake.position = Receptor.position;
		snakeScript.targetPosition = Snake.position;

		SnakeColl.enabled = true;
		SnakeRend.enabled = true;
		SnakeHoodRend.enabled = true;

		yield return null;

		if(receptorScript.Face != snakeScript.Face)
		{
			Snake.rotation = Heart.rotation;
			snakeScript.targetRotation = Snake.rotation;
			snakeScript.upStored = 1;
			snakeScript.rightStored = 0;
		}
		else
		{
			Snake.rotation = snakeInitialRotation;
			snakeScript.targetRotation = snakeInitialRotation;
		}

		snakeScript.State = SnakeState.Run;
		cameraScript.NormalSetup();

		yield return new WaitUntil(() => Vector3.Distance(Snake.position, Receptor.position) > 1.5f);

		ReceptorColl.enabled = true;
	}
}
