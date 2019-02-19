using UnityEngine;
using System.Collections;
using Tools;

public class SnakeTutorial : MonoBehaviour 
{
	private Transform Heart;

	private GameManagerV1 gameManager;
	private TutorielManager tutoScript;
	private SnakeManagement snakeManag;
	private TeleporterStationScript teleporterScript;


	void Awake()
	{
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		if(gameManager.State != Scenes.Tutoriel)
			Debug.LogError("Ce script <SnakeTutoriel> n'a rien à faire là !", this);
			
		Heart = GameObject.Find("Planets/Planet_1/Heart").transform;
		
		snakeManag = GetComponent<SnakeManagement>();
		
		tutoScript = GameObject.Find("LevelManager").GetComponent<TutorielManager>();
		teleporterScript = GameObject.Find("Teleporters/TeleporterA").GetComponent<TeleporterStationScript>();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Apple") && !tutoScript.Phase_2)
		{
			snakeManag.BodyNumber -= 1;
		}
		else if(other.CompareTag("Rabbit"))
		{
			tutoScript.RabbitKilled ++;
		}
		else if(other.CompareTag("Teleporter"))
		{
			SetBaliseRotation();
		}
	}

	private void SetBaliseRotation()
	{
		int len = teleporterScript.balisesRotation.Length;
		Transform ReceptorHeart = GameObject.Find("Planets/Planet_2/Heart").transform;
		ReceptorHeart.rotation = Quaternion.Euler(90, 180, 0);
		teleporterScript.balisesRotation[0] = Heart.rotation;
		teleporterScript.balisesRotation[len - 1] = ReceptorHeart.rotation;
	}
}