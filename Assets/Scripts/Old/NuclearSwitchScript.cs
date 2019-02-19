using UnityEngine;
using System.Collections;

public class NuclearSwitchScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform Parent;

	private Renderer myRenderer;
	private Light myLight;

	private Color32 RedColor = new Color32(255, 65, 65, 255);
	private Color32 GreenColor = new Color32(10, 240, 0, 255);

	[HideInInspector]
	public bool Activated = false;
	private bool Greened = false;

	private GameManagerV1 gameManager;
	private SnakeControllerV3 snakeScript;
	private SkyScript skyScript;

	private float NuclearDelay;


	void Awake()
	{
		myTransform = transform;

		myLight = myTransform.Find("Light").GetComponent<Light>();
		myRenderer = myTransform.Find("Trigger").GetComponent<Renderer>();
		myLight.enabled = false;

		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		skyScript = GameObject.Find("Sky").GetComponent<SkyScript>();

		NuclearDelay = gameManager.WorldSetting.NuclearDelay;
	}

	void Start()
	{
		Parent = GameObject.Find("NuclearSwitchsPooling").transform;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") && !Greened && !Activated && snakeScript.State == SnakeState.Run)
		{
			Setup();
		}
	}

	private void Setup()
	{
		Activated = true;
		myTransform.SetAsLastSibling();
		StartCoroutine(MakeOthersColorGreen(true));
		StartCoroutine(Blink());
	}

	public void SetColorGreen(bool safe)
	{
		Greened = safe;

		if(safe)
		{
			myRenderer.material.color = GreenColor;
			myLight.color = GreenColor;
		}
		else
		{
			myRenderer.material.color = RedColor;
			myLight.color = RedColor;
		}

		if(!skyScript.LightOn)
			myLight.enabled = safe;
	}

	private IEnumerator MakeOthersColorGreen(bool safe)		//true -> Met tous les autres interrupteurs vert
	{
		Transform other;
		for(int i = 0; i < 6; i++)
		{
			other = Parent.GetChild(i);
			if(other.gameObject.activeInHierarchy)
			{
				other.GetComponent<NuclearSwitchScript>().SetColorGreen(safe);
				yield return null;
			}
		}

		if(!safe) // This switch will be destroyed
		{
			myTransform.localPosition = Vector3.zero;
			myLight.enabled = false;
			Activated = false;
			gameObject.SetActive(false);
		}
	}

	private IEnumerator Blink()
	{
		PlanetScript script = gameManager.MainPlanet.GetComponent<PlanetScript>();

		myLight.color = RedColor;
		myLight.enabled = true;

		Faces face = snakeScript.Face;
		Quaternion rot = gameManager.MainPlanet.Find("Heart").rotation;

		float length;
		float chrono = NuclearDelay;
		float latence = 0.75f;

		float fmax = 0.125f;
		float fmin = 0.035f;

		while(chrono > 0.0f)
		{
			length = (chrono > latence) ? fmax : fmin;
			while(length > 0f)
			{
				chrono -= Time.deltaTime;
				length -= Time.deltaTime;
				yield return null;
			}

			myLight.enabled = !myLight.enabled;
		}

		StartCoroutine(MakeOthersColorGreen(false));
		script.LaunchingNuclearExplosion(rot, face);
	}
}