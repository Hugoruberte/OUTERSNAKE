using UnityEngine;
using System.Collections;
using Tools;

public class TutoFinishScript : MonoBehaviour
{
	private Transform Snake;

	private TutorielManager tutoScript;
	private SnakeControllerV3 snakeScript;

	void Awake()
	{
		Snake = GameObject.FindWithTag("Player").transform;

		tutoScript = GameObject.Find("LevelManager").GetComponent<TutorielManager>();
		snakeScript = Snake.GetComponent<SnakeControllerV3>();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.transform == Snake && !tutoScript.Racedone)
		{
			StartCoroutine(Ending());
		}
	}

	private IEnumerator Ending()
	{
		tutoScript.Racedone = true;

		yield return new WaitForSeconds(0.4f);

		snakeScript.State = SnakeState.Stopped;
		Snake.position = Snake.AbsolutePosition();
		snakeScript.targetPosition = Snake.position;
	}
}