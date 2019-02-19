using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class SnakeCreator : MonoBehaviour
{
	private Transform RedRabbitPooling;
	private Transform LazerPooling;

	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;

	private DifficultyScript hardScript;
	private SuperLazerCreator suplazerCreator;
	private CasterBlasterCreator casterblasterCreator;
	private SawCreator sawCreator;
	private MeteoreCreator meteoreCreator;


	void Awake()
	{
		snakeScript = GetComponent<SnakeControllerV3>();
		snakeManag = GetComponent<SnakeManagement>();

		suplazerCreator = GameObject.Find("LevelManager/Creator").GetComponent<SuperLazerCreator>();
		casterblasterCreator = GameObject.Find("LevelManager/Creator").GetComponent<CasterBlasterCreator>();
		sawCreator = GameObject.Find("LevelManager/Creator").GetComponent<SawCreator>();
		meteoreCreator = GameObject.Find("LevelManager/Creator").GetComponent<MeteoreCreator>();
		hardScript = GameObject.Find("LevelManager").GetComponent<DifficultyScript>();
	}

	void Start()
	{
		RedRabbitPooling = GameObject.Find("ObjectPoolingStock/RedRabbitsPooling").transform;
		LazerPooling = GameObject.Find("ObjectPoolingStock/LazersPooling").transform;
	}

	void Update()
	{		
		if(snakeScript.State == SnakeState.Waiting && (snakeScript.up != 0 || snakeScript.right != 0 || snakeScript.upStored != 0 || snakeScript.rightStored != 0))
			ActivateCreator();

		if(snakeManag.Health == SnakeHealth.Dead)
			DesactivateCreator();
	}

	void OnTriggerEnter(Collider other)
	{
		if(snakeScript.State == SnakeState.Run)
		{
			if(other.CompareTag("Armchair"))
			{
				DesactivateCreator();
			}
			else if(other.CompareTag("Rocket"))
			{
				DesactivateCreator();
			}
		}
	}

	public void ActivateCreator()
	{
		if(suplazerCreator && !suplazerCreator.SuperLazerActivated && hardScript.superLazerThreshold <= hardScript.Difficulty)
			suplazerCreator.LaunchSuperLazer();
		if(sawCreator && !sawCreator.SawActivated && hardScript.sawThreshold <= hardScript.Difficulty)
			sawCreator.LaunchSaw();
		if(casterblasterCreator && !casterblasterCreator.CasterBlasterActivated && hardScript.casterBlasterThreshold <= hardScript.Difficulty)
			casterblasterCreator.StartCoroutine(casterblasterCreator.LaunchCasterBlaster());
		if(meteoreCreator && !meteoreCreator.MeteoreActivated && hardScript.meteoreThreshold <= hardScript.Difficulty)
			meteoreCreator.StartCoroutine(meteoreCreator.LaunchMeteore());

		int length = RedRabbitPooling.childCount;
		Transform child;
		for(int i = 0; i < length; i++)
		{
			child = RedRabbitPooling.GetChild(i);
			if(child.gameObject.activeInHierarchy)
				child.GetComponent<RedRabbitController>().RedRabbitManager(true);
		}
	}
	public void DesactivateCreator()
	{
		if(sawCreator)
			sawCreator.SawActivated = false;
		if(suplazerCreator)
			suplazerCreator.SuperLazerActivated = false;
		if(casterblasterCreator)
			casterblasterCreator.CasterBlasterActivated = false;
		if(meteoreCreator)
			meteoreCreator.MeteoreActivated = false;

		int length;
		Transform child;
		length = LazerPooling.childCount;
		for(int i = 0; i < length; i++)
		{
			child = LazerPooling.GetChild(i);
			if(child.gameObject.activeInHierarchy)
				child.GetComponent<LazerScript>().SnakeExit();
		}

		length = RedRabbitPooling.childCount;
		for(int i = 0; i < length; i++)
		{
			child = RedRabbitPooling.GetChild(i);
			if(child.gameObject.activeInHierarchy)
				child.GetComponent<RedRabbitController>().RedRabbitManager(false);
		}
	}
}
