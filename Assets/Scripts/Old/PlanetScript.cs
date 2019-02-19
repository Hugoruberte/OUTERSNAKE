using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetScript : MonoBehaviour
{
	[Header("State")]
	public bool MainPlanet = true;

	private bool visible = false;
	private bool NeedToExploseFace = false;

	[HideInInspector]
	public CellEnum[] Grid;

	[Header("Creative Stuff")]
	public GameObject Border;
	public GameObject Hideo;
	public Mesh HoleAfterNuclear;
	public Material MaterialAfterNuclear;

	private IEnumerator rotationCoroutine = null;

	[HideInInspector]
	public List<Faces> DestroyedFaces = new List<Faces>();

	private string[] Conversion = new string[6] {"FaceX1", "FaceY1", "FaceZ1", "FaceX2", "FaceY2", "FaceZ2"};

	private WaitForSeconds waitforseconds_05 = new WaitForSeconds(0.5f);
	private WaitForSeconds waitforseconds_025 = new WaitForSeconds(0.25f);
	private WaitForSeconds waitforseconds_01 = new WaitForSeconds(0.1f);

	private Transform BorderParent;
	private Transform myTransform;
	private GameObject myLight;
	private Transform myCore;
	private GameObject Skin;
	private Transform Particles;
	private GameObject Snake;
	private Transform Heart;
	private Transform myFaces;
	private Transform DarkMatter;
	private Transform myCamera;

	private ParticleSystem Fire;

	private CameraScript cameraScript;
	private SnakeManagement snakeManag;
	private GameManagerV1 gameManager;
	private PlanetSetup planetSetup;

	[HideInInspector]
	public float Size = 0f;
	private float OmegaX;
	private float OmegaY;
	private float OmegaZ;
	private float freqX;
	private float freqY;
	private float freqZ;
	private float max;

	private float core_heartRate = 1.0f;
	private float core_maxSize = 0.5f;
	private float core_minSize = 0.1f;
	private float core_speed = 0.25f;
	private int core_omegaX = 200;
	private int core_omegaY = 200;
	private int core_omegaZ = 200;

	private Vector3 core_reference = Vector3.zero;

	private Mesh BasicMesh;
	private Material BasicMaterial;


	void Awake()
	{
		myTransform = transform;
		myFaces = myTransform.Find("Body/Faces");
		myLight = myTransform.Find("Light").gameObject;
		myCore = myTransform.Find("Body/Core");
		DarkMatter = myTransform.Find("Body/DarkMatter");
		BasicMesh = myFaces.Find("FaceX1").GetComponent<MeshFilter>().mesh;
		BasicMaterial = myFaces.Find("FaceX1").GetComponent<Renderer>().material;
		Heart = myTransform.Find("Heart");
		myCamera = GameObject.Find("MainCamera").transform;
		Size = myTransform.Find("Body").localScale.x;

		Particles = myTransform.Find("Particles");

		Fire = Particles.Find("Fire").GetComponent<ParticleSystem>();

		cameraScript = myCamera.GetComponent<CameraScript>();
		cameraScript.noise = GameObject.Find("Camera").GetComponent<UnityStandardAssets.ImageEffects.NoiseAndScratches>();
		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();

		Snake = GameObject.FindWithTag("Player");
		snakeManag = Snake.GetComponent<SnakeManagement>();

		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();

		freqX = Random.Range(-5.0f, 5.0f);
		freqY = Random.Range(-5.0f, 5.0f);
		freqZ = Random.Range(-5.0f, 5.0f);
		max = Random.Range(0.0f, 10.0f);

		Grid = new CellEnum[22*22*6];

		Skin = CreatePlanetSkin();
		Skin.SetActive(MainPlanet);
	}

	public void SetCell(int nb, CellEnum value)
	{
		if(Grid.Length > nb)
			Grid[nb] = value;
		else if(Grid.Length == 0)
			StartCoroutine(WaitForSetCell(nb, value));
		else
			Debug.LogError("Cannot set cell " + nb + " because Grid.Length = " + Grid.Length + ".");
	}

	private IEnumerator WaitForSetCell(int nb, CellEnum value)
	{
		float clock = 10f;
		while(Grid.Length <= nb && clock > 0f)
		{
			clock -= Time.deltaTime;
			yield return null;
		}
		if(clock <= 0f)
			Debug.LogError("Take too much time to set cell, might be an error.");
		else
			Grid[nb] = value;
	}

	public void MainSetup(bool main)
	{
		Skin.SetActive(main);
		myLight.SetActive(main);
		MainPlanet = main;
		SetRotation(!main);
	}

	public void LaunchingNuclearExplosion(Quaternion rot, Faces face)
	{
		StartCoroutine(LaunchingNuclearExplosionCoroutine(rot, face));
	}

	private IEnumerator LaunchingNuclearExplosionCoroutine(Quaternion rot, Faces face)
	{
		Particles.rotation = rot;

		planetSetup.DestroyedFacesName.Add(myTransform.name + "/Body/Faces/" + Conversion[(int)face]);
		DestroyedFaces.Add(face);

		NeedToExploseFace = true; // On part avec le principe qu'on va détruire la face

		if(MainPlanet)
		{
			if(Particles.rotation == Heart.rotation) // Snake est sur la face qui va exploser...
			{
				cameraScript.Shake(Shaketype.Nuclear);
				NeedToExploseFace = false;

				if(snakeManag.Health != SnakeHealth.Dead) // ...et est vivant...
				{
					if(!gameManager.Rocket) // ...et ne s'envole pas en fusée.
					{
						gameManager.KilledBy = Killer.NuclearBomb;
						snakeManag.DestroySnake();
					}
				}
				else // ...et n'est pas vivant...
				{
					gameManager.WeirdKilledByNuclear = true;
				}
			}
			else if(!gameManager.Safe) // Snake n'est pas sur la face qui va exploser et n'est pas en sécurité
			{
				cameraScript.Shake(Shaketype.Nuclear);
			}

			if(!gameManager.Safe)
			{
				cameraScript.noise.enabled = true;
				cameraScript.noise.grainIntensityMin = 1.0f;
				cameraScript.noise.grainIntensityMax = 3.0f;
				cameraScript.glitch.enabled = true;
				cameraScript.glitch.intensity = 2.0f;
			}
		}

		Fire.Play();

		if(NeedToExploseFace) // si Snake n'a pas été tué, on detruit la face sur laquelle la bombe a explosé
		{
			Explode(myFaces.GetChild((int)face));

			yield return waitforseconds_05;

			cameraScript.noise.enabled = false;
			cameraScript.glitch.enabled = false;
		}
		else
		{
			yield return waitforseconds_025;

			cameraScript.NuclearEffectSetup(true);
			HideoCreator();
		}
	}

	public void Explode(Transform face)
	{
		switch(face.name)
		{
			case "FaceX1":
				DestroyedFaces.Add(Faces.FaceX1);
				break;
			case "FaceX2":
				DestroyedFaces.Add(Faces.FaceX2);
				break;
			case "FaceY1":
				DestroyedFaces.Add(Faces.FaceY1);
				break;
			case "FaceY2":
				DestroyedFaces.Add(Faces.FaceY2);
				break;
			case "FaceZ1":
				DestroyedFaces.Add(Faces.FaceZ1);
				break;
			case "FaceZ2":
				DestroyedFaces.Add(Faces.FaceZ2);
				break;
		}

		face.GetComponent<MeshFilter>().mesh = HoleAfterNuclear;
		face.GetComponent<Renderer>().material = MaterialAfterNuclear;
		DarkMatter.Find(face.name).gameObject.SetActive(false);

		if(BorderParent == null)
		{
			GameObject borders = new GameObject();
			borders.name = "Borders";
			borders.transform.parent = myTransform;
			borders.transform.localPosition = Vector3.zero;
			BorderParent = borders.transform;
		}

		Vector3 position = myTransform.position + face.forward * ((Size + 1.0f) / 2.0f);
		Quaternion rotation = face.rotation;

		GameObject border = Instantiate(Border, position, rotation);
		border.name = "Border";
		border.transform.parent = BorderParent;

		StartCoroutine(FireZone(border.transform));

		CoreRotation(true);
	}

	public void Repair(Transform Face)
	{
		Face.GetComponent<MeshFilter>().mesh = BasicMesh;
		Face.GetComponent<Renderer>().material = BasicMaterial;

		for(int i = 0; i < BorderParent.childCount; i++)
			Destroy(BorderParent.GetChild(i).gameObject);

		CoreRotation(false);
	}

	private void HideoCreator() // Put a hideo image in front of the camera
	{
		GameObject screen = Instantiate(Hideo, Vector3.zero, Quaternion.identity);
		Transform screen_trans = screen.transform;
		screen.name = "Hideo";
		screen_trans.parent = myCamera.Find("Camera");
		screen_trans.localPosition = new Vector3(0, 0, 8.5f);
		screen_trans.localRotation = Quaternion.Euler(90, 180, 0);
		screen_trans.localScale = new Vector3(1.6f, 1, 0.9f);
	}

	private IEnumerator FireZone(Transform border)
	{
		yield return waitforseconds_01;
		Destroy(border.Find("FireZone").gameObject);
	}

	public void SetRotation(bool rotate)
	{
		if(rotate)
		{
			if(rotationCoroutine != null)
				StopCoroutine(rotationCoroutine);
			rotationCoroutine = AutoRotation();
			StartCoroutine(rotationCoroutine);
		}
		else
		{
			if(rotationCoroutine != null)
			{
				StopCoroutine(rotationCoroutine);
				rotationCoroutine = null;
			}

			transform.rotation = Quaternion.identity;
		}
	}

	private IEnumerator AutoRotation()
	{
		while(!MainPlanet)
		{
			if(visible)
			{
				OmegaX = Mathf.PingPong(freqX * Time.time, max) + 2.0f;
				OmegaY = Mathf.PingPong(freqY * Time.time, max) + 2.0f;
				OmegaZ = Mathf.PingPong(freqZ * Time.time, max) + 2.0f;
				
				myTransform.Rotate(Vector3.right * OmegaX * Time.deltaTime);
				myTransform.Rotate(Vector3.up * OmegaY * Time.deltaTime);
				myTransform.Rotate(Vector3.forward * OmegaZ * Time.deltaTime);
			}
			yield return null;
		}
	}

	private void CoreRotation(bool rotate)
	{
		if(rotate)
		{
			StartCoroutine(CoreRandomRotation());
			StartCoroutine(CoreMainRotation());
		}
		else
		{
			StopCoroutine(CoreRandomRotation());
			StopCoroutine(CoreMainRotation());
		}
	}
	private IEnumerator CoreMainRotation()
	{
		float core_size;

		while(true)
		{
			if(visible)
			{
				myCore.Rotate(Vector3.right * core_omegaX * Time.deltaTime);
				myCore.Rotate(Vector3.up * core_omegaY * Time.deltaTime);
				myCore.Rotate(Vector3.forward * core_omegaZ * Time.deltaTime);
				
				core_size = core_minSize + Mathf.PingPong(Time.time * core_heartRate, core_maxSize - core_minSize);

				if(float.IsNaN(core_size))
				{
					Debug.LogError("core_size is NaN !");
					core_size = core_minSize;
				}

				myCore.localScale = Vector3.SmoothDamp(myCore.localScale, new Vector3(core_size, core_size, core_size), ref core_reference, core_speed);
			}

			yield return null;
		}
	}
	private IEnumerator CoreRandomRotation()
	{
		while(true)
		{
			if(visible)
			{
				core_omegaX = 10 * Random.Range(0, 150);
				core_omegaY = 10 * Random.Range(0, 150);
				core_omegaZ = 10 * Random.Range(0, 150);

				yield return new WaitForSeconds(Random.Range(0.25f, 0.75f));

				core_omegaX = 0;
				core_omegaY = 0;
				core_omegaZ = 0;

				yield return new WaitForSeconds(Random.Range(0.2f, 0.25f));
			}

			yield return null;
		}
	}

	private GameObject CreatePlanetSkin()
	{
		GameObject parent = new GameObject();
		parent.name = "Skin";
		parent.transform.parent = myTransform;
		parent.transform.localPosition = Vector3.zero;
		parent.transform.localRotation = Quaternion.identity;

		GameObject skin = null;
		Transform skinTrans = null;

		string[] names = new string[6] {"X1", "X2", "Y1", "Y2", "Z1", "Z2"};
		Vector3[] dir = new Vector3[6] {Vector3.right, -Vector3.right, Vector3.up, -Vector3.up, Vector3.forward, -Vector3.forward};
		Quaternion[] quat = new Quaternion[6] {Quaternion.Euler(0,90,0), Quaternion.Euler(0,270,0), Quaternion.Euler(270,0,0), Quaternion.Euler(90,0,0), Quaternion.identity, Quaternion.Euler(0,180,0)};

		for(int i = 0; i < 6; i++)
		{
			skin = new GameObject();
			skinTrans = skin.transform;
			skin.name = names[i];
			skin.tag = "ZoneSnake";
			skinTrans.localScale = new Vector3(Size+2, Size+2 ,1);
			skin.AddComponent<BoxCollider>();
			BoxCollider coll = skin.GetComponent<BoxCollider>();
			coll.isTrigger = true;
			coll.center = new Vector3(0,0,-0.5f);
			skin.layer = 2;
			skinTrans.parent = parent.transform;

			skinTrans.position = myTransform.position + dir[i] * (Size/2f+0.5f);
			skinTrans.rotation = quat[i];
		}

		return parent;
	}

	void OnBecameVisible()
	{
		visible = true;
	}

	void OnBecameInvisible()
	{
		visible = false;
	}
}
