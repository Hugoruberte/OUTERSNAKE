using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tools;

public class IntroManager : MonoBehaviour
{
	private Camera myCam;
	private Light myLight;

	public GameObject Snake;
	public GameObject Instruction;
	private GameObject Score;

	private WaitForSeconds waitforseconds_1 = new WaitForSeconds(1.0f);
	private WaitForSeconds waitforseconds_05 = new WaitForSeconds(0.5f);

	private Text Kcolor;

	private float reference = 0.0f;
	private int CheatCodeIndex;

	private KeyCode[] CheatCode;

	private bool Instruct = true;
	private bool Skip;
	private bool K;

	private AudioSource YourePrettyGood;

	void Awake()
	{
		myCam = GameObject.Find("MainCamera").GetComponent<Camera>();
		myCam.orthographicSize = 200f;
		myLight = GameObject.Find("Ground/Light").GetComponent<Light>();
		myLight.intensity = 0.0f;
		Snake.SetActive(false);
		Score = GameObject.Find("Canvas/Score").gameObject;
		Kcolor = GameObject.Find("K").GetComponent<Text>();
		YourePrettyGood = GetComponent<AudioSource>();

		CheatCode = new KeyCode[11] {KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.B, KeyCode.A, KeyCode.Return};
		
	}

	void Start()
	{
		//Score.gameObject.SetActive(false);

		if(PlayerPrefs.GetInt("Session") == 0)
		{
			Instruction.SetActive(true);
			Instruct = true;
			StartCoroutine(Timer());
		}
		else
		{
			Instruction.SetActive(false);
			Instruct = false;
			Snake.SetActive(true);
			Launch();
		}
	}

	private void Launch()
	{
		StartCoroutine(Zoom());
		StartCoroutine(LightUp());
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F4))
			Screen.fullScreen = !Screen.fullScreen;

		if(Instruct)
		{
			if(Input.GetKeyDown(KeyCode.Return) && Skip)
			{
				Instruct = false;
				Instruction.SetActive(false);
				Snake.SetActive(true);
				Launch();
			}
			if(Input.GetKeyDown(KeyCode.K))
			{
				StartCoroutine(Yellow_K());
			}
			if(CheatCodeIndex < CheatCode.Length)
			{
				if(CheatCodeIndex > 0 && Input.anyKeyDown && !Input.GetKeyDown(CheatCode[CheatCodeIndex]))
				{
					CheatCodeIndex = 0;
				}
				else if(Input.GetKeyDown(CheatCode[CheatCodeIndex]) && ++CheatCodeIndex == 11)
				{
					YourePrettyGood.Play();
				}
			}
		}
	}

	private IEnumerator Yellow_K()
	{
		K = false;
		yield return null;
		K = true;

		Kcolor.SetColorA(1.0f);
		float color = 1.0f;
		float jerk = 0.0f;

		while(K && color > 0.005f)
		{
			color = Mathf.SmoothDamp(color, 0.0F, ref jerk, 0.5F);
			Kcolor.SetColorA(color);
			yield return null;
		}
	}

	private IEnumerator Timer()
	{
		Skip = false;
		yield return waitforseconds_1;
		Skip = true;
	}

	private IEnumerator Zoom()
	{
		while(myCam.orthographicSize > 17.01f)
		{
			myCam.orthographicSize = Mathf.MoveTowards(myCam.orthographicSize, 17F, 50.0f * Time.deltaTime);
			yield return null;
		}

		myCam.orthographicSize = 17.0f;
		Score.SetActive(true);
	}

	private IEnumerator LightUp()
	{
		yield return waitforseconds_05;
		while(myLight.intensity < 1.09f)
		{
			myLight.intensity = Mathf.SmoothDamp(myLight.intensity, 1.1F, ref reference, 2.0F);
			yield return null;
		}

		myLight.intensity = 1.1f;
	}

	public static void FindResolution()
	{
		for(int i = 300; i < 721; i++)
		{
			float num = (float)i;
			float num2 = num * 1.77777779f;
			if(Mathf.Approximately(num2, Mathf.Round(num2)))
			{
				Debug.Log(num + " : " + num2);
			}
		}
	}
}

