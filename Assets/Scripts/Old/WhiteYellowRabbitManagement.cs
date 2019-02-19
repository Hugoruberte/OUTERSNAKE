using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class WhiteYellowRabbitManagement : MonoBehaviour
{
	private Transform myTransform;
	private Transform Snake;

	private WhiteYellowRabbitController whiteScript;
	private SnakeControllerV3 snakeScript;
	private BunneyTutoriel bunneyScript;
	private TutorielManager tutoManager;

	private Collider myColl;

	[HideInInspector]
	public bool AfterTalk = false;

	void Awake()
	{
		myTransform = transform.parent;
		Snake = GameObject.FindWithTag("Player").transform;

		whiteScript = transform.parent.GetComponent<WhiteYellowRabbitController>();
		snakeScript = Snake.GetComponent<SnakeControllerV3>();
		bunneyScript = GameObject.Find("LevelManager").GetComponent<BunneyTutoriel>();
		tutoManager = GameObject.Find("LevelManager").GetComponent<TutorielManager>();

		myColl = GetComponent<Collider>();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			if((Vector3.Distance(Snake.AbsolutePosition(), myTransform.AbsolutePosition()) > 0.1f && !AfterTalk)
				|| tutoManager.Repetition == 9)
			{
				AfterTalk = true;
				myColl.enabled = false;
				whiteScript.StartCoroutine(whiteScript.Encounter());
			}
			else if(AfterTalk)
			{
				tutoManager.Repetition ++;

				if(tutoManager.Repetition > 2)
					tutoManager.Troll = true;

				snakeScript.State = SnakeState.Stopped;
				Snake.rotation = snakeScript.targetRotation;
				Snake.position = Snake.AbsolutePosition();
				snakeScript.targetPosition = Snake.position;
				snakeScript.up = 0;
				snakeScript.right = 0;

				bunneyScript.StartCoroutine(bunneyScript.RepetitionUnlessDisappear());
			}
		}
	}
}
