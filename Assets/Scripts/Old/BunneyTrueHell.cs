using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class BunneyTrueHell : MonoBehaviour
{
	public GameObject Dialog;
	private GameObject Tip;

	private Text TextComponent;

	private WaitForSecondsRealtime waitforsecondsrealtime_1;
	private WaitForSecondsRealtime waitforsecondsrealtime_025;
	private WaitForSecondsRealtime waitforsecondsrealtime_00325;
	private WaitForSecondsRealtime waitforsecondsrealtime_01;

	private TrueHellManager truehellManager;
	private SnakeControllerV3 snakeScript;

	public TextAsset myText;
	private string[] source;

	private string line;

	private int compterTip = 10;
	private int NbLineRead = 0;
	private int index = 0;
	private float velocity = 0.0f;

	private bool Next = false;
	private bool Stop = false;
	private bool Talk = false;
	private bool Reading = false;



	/* /!\ IL FAUT METTRE EN PLACE LES NOUVELLES FONCTIONS DE LECTURE, DE BUNNEY ARCADE OU BUNNEY INTRO /!\ */



	void Awake()
	{
		waitforsecondsrealtime_1 = new WaitForSecondsRealtime(1.0f);
		waitforsecondsrealtime_025 = new WaitForSecondsRealtime(0.25f);
		waitforsecondsrealtime_00325 = new WaitForSecondsRealtime(0.0325f);
		waitforsecondsrealtime_01 = new WaitForSecondsRealtime(0.1f);

		Tip = Dialog.transform.Find("Tip").gameObject;
		TextComponent = Dialog.transform.Find("Text").GetComponent<Text>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
	}

	void Start()
	{
		Dialog.SetActive(false);
		source = myText.text.Split("\n"[0]);
		line = source[index];
	}




	public IEnumerator Welcome()
	{
		StartCoroutine(SlowMo(true));
		Open();
		Goto(1);
		StartCoroutine(Read(6, true, 0.75f, true));
		StartCoroutine(Close(6));

		while(Dialog.activeInHierarchy)
			yield return null;

		StartCoroutine(SlowMo(false));
	}

	public IEnumerator NoPause()
	{
		if(!PlayerPrefs.HasKey("HellPause"))
		{
			PlayerPrefs.SetInt("HellPause", 1);
			StartCoroutine(SlowMo(true));

			yield return waitforsecondsrealtime_1;

			snakeScript.State = SnakeState.Stopped;

			Open();
			Goto(2);
			StartCoroutine(Read(2, true, 0.75f, true));
			StartCoroutine(Close(2));

			while(Dialog.activeInHierarchy)
				yield return null;

			StartCoroutine(SlowMo(false));

			snakeScript.State = SnakeState.Run;
		}
		else
		{
			Open();
			StartCoroutine(SimpleRead("Nope.", 0.03f));

			yield return waitforsecondsrealtime_025;
			StartCoroutine(Close(0));
		}
	}

	public IEnumerator Escape()
	{
		StartCoroutine(SlowMo(true));
		Open();
		Goto(3);
		StartCoroutine(Read(9, true, 0.75f, true));
		StartCoroutine(Close(9));

		while(Dialog.activeInHierarchy)
			yield return null;

		StartCoroutine(SlowMo(false));
	}




	//Fonctions outils

	private IEnumerator SlowMo(bool slow)
	{
		
	if(slow)
		{
			Time.timeScale = 0.005f;
		}
		else
		{
			while(Time.timeScale < 1.0f - 0.01f)
			{
				Time.timeScale = Mathf.SmoothDamp(Time.timeScale, 1.0f, ref velocity, 0.5f, Mathf.Infinity, Time.unscaledDeltaTime);
				yield return null;
			}
			Time.timeScale = 1.0f;
		}
	}

	private IEnumerator WaitForNext()
	{
		while(Reading && !Stop)
		{
			if((Input.GetKeyDown("return") || Input.GetKeyDown("mouse 0")) && Talk)
			{
				Next = true;
			}
			yield return null;
		}
	}

	private IEnumerator Read(int nbline, bool auto, float delay, bool skip)
	{
		while(Reading)
		{
			if(Stop)
				break;
			yield return null;
		}

		NbLineRead = 0;
		Reading = true;
		Stop = false;

		StartCoroutine(WaitForNext());

		WaitForSecondsRealtime waitforsecondsrealtime_delay = new WaitForSecondsRealtime(delay);

		for(int i = 0; i < nbline; i++)
		{
			Talk = true;
			TextComponent.text = "> ";

			line = source[++index];
			if(line == null || Stop)
				break;

			foreach(char letter in line)
			{
				TextComponent.text += letter;
				if((Next && skip) || Stop)
				{
					TextComponent.text = "> " + line;
					Next = false;
					break;
				}
				if(letter != ' ')
					yield return waitforsecondsrealtime_00325;
				else
					yield return null;
			}

			if(Stop)
				break;

			TextComponent.text = "> " + line;
			Talk = false;
			Next = false;

			yield return waitforsecondsrealtime_01;

			if(compterTip < 4)
			{
				Tip.SetActive(true);
				compterTip++;
			}
			if(auto)
			{
				yield return waitforsecondsrealtime_delay;
			}
			else
			{
				while(!Enter())
				{
					if(Stop)
						break;
					yield return null;
				}
			}

			NbLineRead ++;

			if(Tip.activeInHierarchy)
				Tip.SetActive(false);
		}

		Reading = false;
		Stop = false;
	}

	private void Goto(int number)
	{
		index = 0;
		line = source[index];

		while(!line.StartsWith("**index** " + number.ToString()))
		{
			line = source[++index];
			if(line == null)
			{
				Debug.LogError("Error : No index found ! number = " + number.ToString());
				break;
			}
		}

		for(int i = 0; i < 1; i++)
		{
			line = source[++index];
		}
	}

	private void Open()
	{
		Dialog.SetActive(true);
		//yeah, this function only does that, but it's way more elegant !
	}

	private IEnumerator Close(int nbline)
	{
		if(nbline != 0)
		{
			while(NbLineRead != nbline)
				yield return null;
		}

		Dialog.SetActive(false);
		Stop = true;
	}

	private IEnumerator SimpleRead(string line, float interval)
	{
		//interval = 0.05f;
		TextComponent.text = "> ";

		WaitForSecondsRealtime waitforsecondsrealtime_interval = new WaitForSecondsRealtime(interval);

		foreach(char letter in line)
		{
			TextComponent.text += letter;
			if(letter != ' ')
				yield return waitforsecondsrealtime_interval;
			else
				yield return null;
		}

		TextComponent.text = "> " + line;
	}

	private bool Enter()
	{
		
	return(Input.GetKeyDown("return") || Input.GetKeyDown("space"));
	}
}