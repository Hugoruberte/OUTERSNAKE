using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScore : MonoBehaviour
{
	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;
	private ScoreScript scoreScript;

	void Awake()
	{
		snakeScript = GetComponent<SnakeControllerV3>();
		snakeManag = GetComponent<SnakeManagement>();
		scoreScript = GameObject.Find("LevelManager").GetComponent<ScoreScript>();
	}

	void OnTriggerEnter(Collider other)
	{
		if(snakeScript.State == SnakeState.Run)
		{
			if(other.CompareTag("Apple"))
			{
				AppleType apple_type = other.transform.GetComponent<AppleScript>().State;
				switch(apple_type)
				{
					case AppleType.Red:
						scoreScript.Score += 100;
						break;

					case AppleType.Rotten:
						break;

					case AppleType.Dung:
						break;
				}
			}
			else if(other.CompareTag("Rabbit"))
			{
				RabbitState rabbit_type = other.GetComponent<RabbitManagement>().State;
				if(rabbit_type == RabbitState.Yellow)
				{
					scoreScript.Score += 500;
				}
			}
			else if(other.CompareTag("SnakeBody") && snakeManag.Health != SnakeHealth.Invincible 
				&& other.GetComponent<SnakeFollow>() && other.GetComponent<SnakeFollow>().Ready)
			{
				
			}
			else if(other.CompareTag("Meteore") && snakeManag.Health != SnakeHealth.Invincible)
			{
				
			}
			else if(other.CompareTag("Saw") && snakeManag.Health != SnakeHealth.Invincible)
			{
				
			}
			else if(other.CompareTag("Bounds"))
			{
				
			}
			else if(other.name == "Trunk" && snakeManag.Health != SnakeHealth.Invincible)
			{
				
			}
			else if(other.CompareTag("Fire") && snakeManag.Health != SnakeHealth.Invincible)
			{
				
			}
			else if(other.CompareTag("Rocket"))
			{
				scoreScript.Score += 10000;
			}
		}
	}
	
}
