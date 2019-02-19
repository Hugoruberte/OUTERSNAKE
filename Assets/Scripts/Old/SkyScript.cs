using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform Belt;
	private Transform Biosphere;
	private Transform myCamera;
	private Transform Planets;
	private Transform Snake;
	private Transform TreeFolder;
	private Transform RabbitFolder;
	private Transform AppleFolder;
	private Transform NuclearSwitchFolder;
	private Transform RedRabbitFolder;
	private Transform RocketFolder;

	private Transform[] Polysuns;
	private Transform[] Polyrays;

	private PerformanceScript perfScript;
	private PlanetSetup planetSetup;

	private Renderer Dome_1;
	private Renderer Dome_2;

	public GameObject Clouds;
	public GameObject SpaceShip;
	public GameObject BlackHole;
	public GameObject SpaceRabbit;

	[HideInInspector]
	public bool LightOn;

	private Renderer SnakeRenderer;
	private Renderer SnakeHoodRenderer;
	private Renderer LooneyRenderer;
	private Renderer RibbonRenderer;

	private ParticleSystemRenderer SnakeParticleRenderer;

	[Range(0, 3)]
	public int Spread = 1;
	private float PolySpeed = 10.0f;
	private float dayColorValue = 1f;
	private int PolysunsLength = 0;
	
	private float timeofday = 0.0f;
	public float TimeOfDay
	{
		get
		{
			return timeofday;
		}
		set
		{
			timeofday = value;
			EditorTimeOfDay = value;
			SetTimeOfDay();
		}
	}
	[Range(0.0f, 1.0f)]
	public float EditorTimeOfDay = 0.0f;
	

	void Awake()
	{
		myTransform = transform;
		Belt = myTransform.Find("Belt");
		myCamera = GameObject.Find("MainCamera").transform;
		Planets = GameObject.Find("Planets").transform;
		Snake = GameObject.FindWithTag("Player").transform;

		perfScript = GameObject.Find("LevelManager").GetComponent<PerformanceScript>();
		planetSetup = Planets.GetComponent<PlanetSetup>();

		Dome_1 = myTransform.Find("SkyDome1").GetComponent<Renderer>();
		Dome_2 = myTransform.Find("SkyDome2").GetComponent<Renderer>();

		SnakeRenderer = Snake.GetComponent<Renderer>();
		SnakeHoodRenderer = Snake.Find("Hood").GetComponent<Renderer>();

		LooneyRenderer = GameObject.Find("Armchair/Body/Looney/Body").GetComponent<Renderer>();
		RibbonRenderer = GameObject.Find("Armchair/Body/Looney/Ribbon").GetComponent<Renderer>();
		
		SnakeParticleRenderer = Snake.Find("Explosion").GetComponent<ParticleSystemRenderer>();

		int count = 0;
		int length = myTransform.childCount;
		for(int i = 0; i < length; i++)
		{
			if(myTransform.GetChild(i).name == "PolySun")
				count ++;
		}
		Polysuns = new Transform[count];
		Polyrays = new Transform[count];
		count = 0;
		for(int i = 0; i < length; i++)
		{
			if(myTransform.GetChild(i).name == "PolySun")
			{
				Polysuns[count] = myTransform.GetChild(i).Find("PolyRay");
				Polyrays[count] = myTransform.GetChild(i).Find("PolyRay/Body");
				count ++;
			}
		}
		PolysunsLength = count;
	}

	void Start()
	{
		TreeFolder = GameObject.Find("ObjectPoolingStock/TreesPooling").transform;
		RabbitFolder = GameObject.Find("ObjectPoolingStock/RabbitsPooling").transform;
		AppleFolder = GameObject.Find("ObjectPoolingStock/ApplesPooling").transform;
		NuclearSwitchFolder = GameObject.Find("ObjectPoolingStock/NuclearSwitchsPooling").transform;
		RedRabbitFolder = GameObject.Find("ObjectPoolingStock/RedRabbitsPooling").transform;
		RocketFolder = GameObject.Find("ObjectPoolingStock/RocketsPooling").transform;

		SetupBiosphere();

		StartCoroutine(WaitForBegin());
	}

	private IEnumerator WaitForBegin()
	{
		yield return new WaitUntil(() => perfScript.Done);

		TimeOfDay = EditorTimeOfDay;
	}

	void Update()
	{
		Belt.Rotate(Vector3.forward * 1.0f * Time.deltaTime);

		for(int i = 0; i < PolysunsLength; i++)
		{
			Polysuns[i].LookAt(myCamera);
			Polyrays[i].Rotate(Vector3.right * Time.deltaTime * PolySpeed);
		}
	}

	private void SetTimeOfDay()
	{
		Dome_1.material.SetTextureOffset("_MainTex", new Vector2(timeofday/2.0f, 0));
		Dome_2.material.SetTextureOffset("_MainTex", new Vector2(timeofday/2.0f, 0));
		float aspect = dayColorValue - timeofday * dayColorValue;
		RenderSettings.ambientLight = new Color(aspect, aspect, aspect, 1.0f);

		LightOn = (timeofday > 0.3);

		float emission;

		emission = 0.25f * aspect;
		SnakeRenderer.material.SetColor("_EmissionColor", Color.white * emission);
		SnakeHoodRenderer.material = SnakeRenderer.material;
		SnakeParticleRenderer.material = SnakeRenderer.material;

		emission = -1.0f * aspect + 0.85f;
		LooneyRenderer.material.SetColor("_EmissionColor", Color.white * emission);

		emission = -0.75f * aspect + 0.7f;
		RibbonRenderer.material.SetColor("_EmissionColor", Color.red * emission);

		PlanetSetup();

		if(planetSetup.Trees)
		{
			TreeSetup();
		}
		if(planetSetup.Rabbit)
		{
			RabbitSetup();
		}
		if(planetSetup.Apple)
		{
			AppleSetup();
		}
		if(planetSetup.NuclearSwitch)
		{
			NuclearSwitchSetup();
		}
		if(planetSetup.RedRabbit)
		{
			RedRabbitSetup();
		}
		if(planetSetup.Rocket)
		{
			RocketSetup();
		}
	}

	private void PlanetSetup()
	{
		/*float aspect = dayColorValue - timeofday * dayColorValue;
		float emission = -0.83332f * aspect + 0.75f;
		int length = Planets.childCount;
		Color tmpColor;
		Renderer tmpRenderer;
		for(int i = 0; i < length; i++)
		{
			tmpRenderer = Planets.GetChild(i).Find("Body").GetComponent<Renderer>();
			tmpColor = tmpRenderer.material.color;
			tmpRenderer.material.SetColor("_EmissionColor", tmpColor * emission);
		}*/
	}

	private void TreeSetup()
	{
		float aspect = dayColorValue - timeofday * dayColorValue;
		Color tmpColor;
		Renderer tmpRenderer;
		int length;
		float emission;
		float intensity;
		float lens;
		float range;

		emission = -(1.0f/3.0f) * aspect + 0.4f;
		intensity = -1.6667f * aspect + 3.0f;
		lens = -0.6667f * aspect + 0.4f;
		range = -5.0f * aspect + 6.0f;
		
		length = TreeFolder.childCount;
		for(int i = 0; i < length; i++)
		{
			tmpRenderer = TreeFolder.GetChild(i).Find("Body").GetComponent<Renderer>();
			tmpColor = tmpRenderer.material.color;
			tmpRenderer.material.SetColor("_EmissionColor", tmpColor * emission);
			TreeFolder.GetChild(i).Find("Body/Light").GetComponent<Light>().range = range;
			TreeFolder.GetChild(i).Find("Body/Light").GetComponent<Light>().intensity = intensity;
			TreeFolder.GetChild(i).Find("Body/Light").GetComponent<LensFlare>().brightness = lens;
		}
	}

	private void RabbitSetup()
	{
		float aspect = dayColorValue - timeofday * dayColorValue;
		Color tmpColor;
		Renderer tmpRenderer;
		int length;
		float emission;
		float range;

		emission = -0.66667f * aspect + 0.6f;
		range = -5.0f * aspect + 3.0f;
		
		length = RabbitFolder.childCount;
		for(int i = 0; i < length; i++)
		{
			tmpRenderer = RabbitFolder.GetChild(i).Find("Body").GetComponent<Renderer>();
			tmpColor = tmpRenderer.material.color;
			tmpRenderer.material.SetColor("_EmissionColor", tmpColor * emission);
			RabbitFolder.GetChild(i).Find("Body/Part3").GetComponent<ParticleSystemRenderer>().material = tmpRenderer.material;
			RabbitFolder.GetChild(i).Find("Body").GetComponent<Light>().range = range;
		}
	}

	private void AppleSetup()
	{
		float aspect = dayColorValue - timeofday * dayColorValue;
		Color tmpColor;
		Renderer tmpRenderer;
		int length;
		float emission;
		float range;

		emission = -0.66667f * aspect + 0.6f;
		range = -3.33332f * aspect + 2.0f;
		
		length = AppleFolder.childCount;
		for(int i = 0; i < length; i++)
		{
			tmpRenderer = AppleFolder.GetChild(i).Find("Body").GetComponent<Renderer>();
			tmpColor = tmpRenderer.material.color;
			tmpRenderer.material.SetColor("_EmissionColor", tmpColor * emission);
			AppleFolder.GetChild(i).Find("Body/Explosion").GetComponent<ParticleSystemRenderer>().material = tmpRenderer.material;
			AppleFolder.GetChild(i).Find("Body").GetComponent<Light>().range = range;
		}
	}

	private void NuclearSwitchSetup()
	{
		float aspect = dayColorValue - timeofday * dayColorValue;
		Color tmpColor;
		Renderer tmpRenderer;
		int length;
		float emission;

		emission = -0.66667f * aspect + 0.6f;
			
		length = NuclearSwitchFolder.childCount;
		for(int i = 0; i < length; i++)
		{
			tmpRenderer = NuclearSwitchFolder.GetChild(i).Find("Trigger").GetComponent<Renderer>();
			tmpColor = tmpRenderer.material.color;
			tmpRenderer.material.SetColor("_EmissionColor", tmpColor * emission);
			if(NuclearSwitchFolder.GetChild(i).Find("Light").GetComponent<Light>().enabled == false && LightOn)
				NuclearSwitchFolder.GetChild(i).Find("Light").GetComponent<Light>().enabled = true;
			else if(NuclearSwitchFolder.GetChild(i).GetComponent<NuclearSwitchScript>().Activated == false && !LightOn)
				NuclearSwitchFolder.GetChild(i).Find("Light").GetComponent<Light>().enabled = false;
		}
	}

	private void RedRabbitSetup()
	{
		float aspect = dayColorValue - timeofday * dayColorValue;
		Color tmpColor;
		Renderer tmpRenderer;
		int length;
		float emission;

		emission = -0.85f * aspect + 0.75f;
			
		length = RedRabbitFolder.childCount;
		for(int i = 0; i < length; i++)
		{
			tmpRenderer = RedRabbitFolder.GetChild(i).Find("Rabbit/Body").GetComponent<Renderer>();
			tmpColor = tmpRenderer.material.color;
			tmpRenderer.material.SetColor("_EmissionColor", tmpColor * emission);
		}
	}

	private void RocketSetup()
	{
		float aspect = dayColorValue - timeofday * dayColorValue;
		Color tmpColor;
		Renderer tmpRenderer;
		int length;

		float white_emission = -0.6667f * aspect + 0.6f;
		float blue_emission = -0.9167f * aspect + 0.8f;
		float orange_emission = -0.75f * aspect + 0.6f;
		
		length = RocketFolder.childCount;
		for(int i = 0; i < length; i++)
		{
			tmpRenderer = RocketFolder.GetChild(i).Find("Axis/Body").GetComponent<Renderer>();

			tmpColor = tmpRenderer.materials[0].color;
			tmpRenderer.materials[0].SetColor("_EmissionColor", tmpColor * white_emission);
			tmpColor = tmpRenderer.materials[1].color;
			tmpRenderer.materials[1].SetColor("_EmissionColor", tmpColor * blue_emission);
			tmpColor = tmpRenderer.materials[2].color;
			tmpRenderer.materials[2].SetColor("_EmissionColor", tmpColor * orange_emission);
		}
	}




	private void SetupBiosphere()
	{
		GameObject obj = new GameObject();
		obj.name = "Biosphere";
		obj.transform.parent = myTransform;
		obj.transform.localPosition = Vector3.zero;
		Biosphere = obj.transform;

		int limit = (Spread - 1) * 2 + 1;	//doit etre impair !

		for(int j = 0; j <= limit; j++)
		{
			for(int k = 0; k <= limit; k++)
			{
				for(int l = 0; l <= limit; l++)
				{
					Vector3 zone = new Vector3(limit*50 - j*100, limit*50 - k*100, limit*50 - l*100);
					GameObject clone;
					float random = Random.Range(0.0f, 100.0f);

					if(random > 65f) // 100 -> 65 : 35%
					{
						clone = Instantiate(SpaceRabbit, Vector3.zero, Quaternion.identity);
						clone.name = "SpaceRabbit";
						clone.transform.parent = Biosphere;
						clone.transform.localPosition = zone;
						clone.GetComponent<SpaceRabbitScript>().startPosition = clone.transform.position;
					}
					else if(random > 25f) // 65 -> 25 : 40%
					{
						clone = Instantiate(Clouds, Vector3.zero, Random.rotation);
						clone.name = "Clouds";
						clone.transform.parent = Biosphere;
						clone.transform.localPosition = zone;
						clone.transform.localScale = new Vector3(Random.Range(0.75f,1.0f), Random.Range(0.75f,1.0f), Random.Range(0.75f,1.0f));
					}
					else if(random > 5f) // 25 -> 5 : 20%
					{
						clone = Instantiate(SpaceShip, Vector3.zero, Quaternion.identity);
						clone.name = "SpaceShip";
						clone.transform.parent = Biosphere;
						clone.transform.localPosition = zone + Random.insideUnitSphere * 0.3f;
					}
					else // 5 -> 0 : 5%
					{
						clone = Instantiate(BlackHole, Vector3.zero, Quaternion.identity);
						clone.name = "BlackHole";
						clone.transform.parent = Biosphere;
						clone.transform.localPosition = zone + Random.insideUnitSphere * 0.25f;
					}
				}
			}
		}	
	}
}
