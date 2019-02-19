using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{
	private Transform myCamera;

	private PerformanceScript perfScript;
	private CameraScript cameraScript;
	private SnakeControllerV3 snakeScript;
	private PlanetSetup planetSetup;

	public float omega = 22.5f;
	public float taux = 2f;

	public bool Performance = true;

	void Awake()
	{
		myCamera = GameObject.FindWithTag("MainCamera").transform;

		perfScript = GetComponent<PerformanceScript>();
		cameraScript = myCamera.GetComponent<CameraScript>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();
	}

	void Start()
	{
		if(Performance)
		{
			snakeScript.State = SnakeState.Stopped;
			StartCoroutine(WaitBeforePlacingObject());
		}
		else
		{
			myCamera.position = new Vector3(600, 541, 601);
			cameraScript.StandardEffectSetup();
			cameraScript.State = CameraState.Moving;
			cameraScript.NormalSetup();
			snakeScript.State = SnakeState.Waiting;
		}
	}

	private IEnumerator WaitBeforePlacingObject()
	{
		perfScript.ArcadePreloading();

		yield return new WaitUntil(() => perfScript.Done);

		planetSetup.MainPlanetSetObjectsWithoutArmchair();

		yield return new WaitForSeconds(1f);

		myCamera.position = new Vector3(600, 541, 601);
		cameraScript.StandardEffectSetup();
		cameraScript.State = CameraState.Moving;
		cameraScript.NormalSetup();
		snakeScript.State = SnakeState.Waiting;
	}
}
