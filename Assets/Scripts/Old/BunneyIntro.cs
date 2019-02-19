using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using Tools;
using System.Linq;

public class BunneyIntro : MonoBehaviour
{
	private GameObject Dialog;
	private GameObject Tip;
	private GameObject YesObject;
	private GameObject NoObject;
	private GameObject SuperYesObject;

	private WaitForSecondsRealtime waitforsecondsrealtime_01;
	private WaitForSecondsRealtime waitforsecondsrealtime_00325;

	private RectTransform DialogRect;
	private Transform TextTransform;
	private Text Skipping;
	private Text NoText;
	private Image WhiteParadise;

	private WaitForSeconds waitforseconds_1 = new WaitForSeconds(1.0f);
	private WaitForSeconds waitforseconds_10 = new WaitForSeconds(10.0f);
	private WaitForSeconds waitforseconds_3 = new WaitForSeconds(3.0f);
	private WaitForSeconds waitforseconds_05 = new WaitForSeconds(0.5f);
	private WaitForSeconds waitforseconds_075 = new WaitForSeconds(0.75f);

	public Transform Snake2D;

	public TextAsset myText;
	private string[] source;
	private string[] words;

	private Text[] Letter;
	private RectTransform[] Used;

	private string line;

	private Text YesColor;
	private Text SYesColor;
	private Text NoColor;

	private Color32 colorYes = new Color32(0, 255, 65, 255);
	private Color32 colorNo = new Color32(255, 60, 60, 255);
	//private Color32 colorYellow = new Color32(255, 255, 0, 255);

	private Vector2 Default;
	private Vector2 reference = Vector2.zero;

	private int compterTip = 0;
	private int NbLineRead = 0;
	private int compterNo = 0;
	private int index = 0;

	private float velocity = 0.0f;
	private float velocity2;

	private bool NextPanel = false;
	private bool Stop = false;
	private bool Talk = false;
	private bool Reading = false;
	private bool Yes = false;
	private bool No = false;
	private bool LoopChoice = true;
	private bool SkipIntro = false;
	private bool Exit = false;

	void Awake()
	{
		waitforsecondsrealtime_01 = new WaitForSecondsRealtime(0.1f);
		waitforsecondsrealtime_00325 = new WaitForSecondsRealtime(0.0325f);

		Dialog = GameObject.Find("Canvas/Dialog");
		Tip = Dialog.transform.Find("Tip").gameObject;

		NoText = GameObject.Find("Ground/No").GetComponent<Text>();

		YesObject = GameObject.Find("Ground/Yes");
		NoObject = GameObject.Find("Ground/No");
		SuperYesObject = GameObject.Find("Ground/SuperYes");

		NoColor = NoObject.GetComponent<Text>();
		YesColor = YesObject.GetComponent<Text>();
		SYesColor = SuperYesObject.GetComponent<Text>();

		DialogRect = Dialog.GetComponent<RectTransform>();
		Default = DialogRect.anchoredPosition;
		WhiteParadise = GameObject.Find("Canvas/White").GetComponent<Image>();

		TextTransform = Dialog.transform.Find("Text");
		Skipping = GameObject.Find("Canvas/Skip").GetComponent<Text>();

		int length = TextTransform.childCount;
		Letter = new Text[length];
		Used = new RectTransform[length];
		for(int i = 0; i < length; i++)
		{
			Letter[i] = TextTransform.GetChild(i).GetComponent<Text>();
			Used[i] = null;
		}
	}

	void Start()
	{
		Dialog.SetActive(false);
		source = myText.text.Split("\n"[0]);
		line = source[index];
		Clear();
	}

	void Update()
	{
		if(Input.GetKeyDown("return") && Talk)
			NextPanel = true;

		if(!Exit)
		{
			if(((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && PlayerPrefs.HasKey("Intro")) || SkipIntro)
			{
				WhiteParadise.SetColorA(Mathf.SmoothDamp(WhiteParadise.color.a, 1.0f, ref velocity, 0.4f));
				Skipping.SetColorA(Mathf.SmoothDamp(Skipping.color.a, 1.0f, ref velocity2, 0.4f));

				if(WhiteParadise.color.a > 0.99f)
				{
					PlayerPrefs.SetInt("Session", PlayerPrefs.GetInt("Session") + 1);
					SceneManager.LoadScene("Arcade");
				}
				else if(WhiteParadise.color.a > 0.75f && !SkipIntro)
				{
					SkipIntro = true;
					PlayerPrefs.SetInt("Intro", 0);
				}
			}
			else if(WhiteParadise.color.a > 0.0f + 0.01f)
			{
				WhiteParadise.SetColorA(Mathf.SmoothDamp(WhiteParadise.color.a, 0.0f, ref velocity, 0.5f));
				Skipping.SetColorA(Mathf.SmoothDamp(Skipping.color.a, 0.0f, ref velocity2, 0.3f));
			}
		}
	}



	//Texte

	public void Contact()
	{
		int session = PlayerPrefs.GetInt("Session");

		if(session == 0)		//premiere fois qu'on joue
		{
			Open();
			GoTo(1);
			StartCoroutine(Read(26, false, 0, true));
			StartCoroutine(Close(26));
		}
		else
		{
			Open();
			GoTo(8);
			StartCoroutine(Read(28, false, 0, true));
			StartCoroutine(Close(28));
		}
		
		StartCoroutine(TransitionC_B());
	}

	private IEnumerator TransitionC_B()
	{
		while(!(line.StartsWith("I'll leave you alone now.") && Input.GetKeyDown("return")))
			yield return null;

		yield return waitforseconds_3;
		Boring();
	}

	private void Boring()
	{
		Open();
		GoTo(2);
		StartCoroutine(Read(5, false, 0, true));
		
		StartCoroutine(TransitionB_F());
	}

	private IEnumerator TransitionB_F()
	{
		while(!(line.StartsWith("I'll just go play by myself ...") && Input.GetKeyDown("return")))
			yield return null;

		StartCoroutine(Following());
		yield return waitforseconds_10;
		StartCoroutine(Read(3, true, 1.0f, false));
	}

	private IEnumerator Following()
	{
		float time = 0.25f;

		for(int i = 0; i < 20; i++)
		{
			while(Vector2.Distance(DialogRect.anchoredPosition, new Vector2(150, -200)) > 1.0F)
			{
				DialogRect.anchoredPosition = Vector2.SmoothDamp(DialogRect.anchoredPosition, new Vector2(150, -200), ref reference, time, Mathf.Infinity, Time.deltaTime);
				yield return null;
			}

			while(Vector2.Distance(DialogRect.anchoredPosition, new Vector2(-150, 115)) > 1.0F)
			{
				DialogRect.anchoredPosition = Vector2.SmoothDamp(DialogRect.anchoredPosition, new Vector2(-150, 115), ref reference, time, Mathf.Infinity, Time.deltaTime);
				yield return null;
			}

			while(Vector2.Distance(DialogRect.anchoredPosition, new Vector2(225, 180)) > 1.0F)
			{
				DialogRect.anchoredPosition = Vector2.SmoothDamp(DialogRect.anchoredPosition, new Vector2(225, 180), ref reference, time, Mathf.Infinity, Time.deltaTime);
				yield return null;
			}

			if(time > 0.005f)
				time *= 0.75f;
		}

		yield return waitforseconds_05;

		while(Vector2.Distance(DialogRect.anchoredPosition, Default) > 0.1f)
		{
			DialogRect.anchoredPosition = Vector2.SmoothDamp(DialogRect.anchoredPosition, Default, ref reference, 0.5f, Mathf.Infinity, Time.deltaTime);
			yield return null;
		}
		DialogRect.anchoredPosition = Default;


		StartCoroutine(Read(9, false, 0, true));
		StartCoroutine(Close(9));
		StartCoroutine(TransitionF_C());
	}

	private IEnumerator TransitionF_C()
	{
		while(Dialog.activeInHierarchy)
			yield return null;

		StartCoroutine(Chosing());
	}

	private IEnumerator Chosing()
	{
		YesObject.SetActive(true);
		NoObject.SetActive(true);

		float clock = 0.0f;

		while(LoopChoice)
		{
			while(Dialog.activeInHierarchy)
				yield return null;

			No = false;
			Yes = false;

			bool first = false;
			bool second = false;
			bool third = false;

			while(!Yes && !No)
			{
				if(!Dialog.activeInHierarchy)
					clock += Time.deltaTime;
				else
					clock = 0.0f;
				
				if(clock > 30.0f && !first)
				{
					first = true;
					Open();
					GoTo(9);
					StartCoroutine(Read(6, false, 0, true));
					StartCoroutine(Close(6));

					YesColor.color = colorYes;
					SYesColor.color = colorYes;
					NoColor.color = colorNo;

					YesObject.GetComponent<Collider>().enabled = false;
					NoObject.GetComponent<Collider>().enabled = false;
					SuperYesObject.GetComponent<Collider>().enabled = false;

					while(Dialog.activeInHierarchy)
						yield return null;
					yield return waitforseconds_05;

					YesObject.GetComponent<Collider>().enabled = true;
					NoObject.GetComponent<Collider>().enabled = true;
					SuperYesObject.GetComponent<Collider>().enabled = true;
				}
				else if(clock > 30.0f && !second)
				{
					second = true;
					Open();
					GoTo(10);
					StartCoroutine(Read(1, false, 0, false));
					StartCoroutine(Close(1));

					YesColor.color = colorYes;
					SYesColor.color = colorYes;
					NoColor.color = colorNo;

					YesObject.GetComponent<Collider>().enabled = false;
					NoObject.GetComponent<Collider>().enabled = false;
					SuperYesObject.GetComponent<Collider>().enabled = false;

					while(Dialog.activeInHierarchy)
						yield return null;
					yield return waitforseconds_05;

					YesObject.GetComponent<Collider>().enabled = true;
					NoObject.GetComponent<Collider>().enabled = true;
					SuperYesObject.GetComponent<Collider>().enabled = true;
				}
				else if(clock > 30.0f && !third)
				{
					third = true;
					Open();
					GoTo(11);
					StartCoroutine(Read(2, false, 0, true));
					StartCoroutine(Close(2));

					YesColor.color = colorYes;
					SYesColor.color = colorYes;
					NoColor.color = colorNo;

					YesObject.GetComponent<Collider>().enabled = false;
					NoObject.GetComponent<Collider>().enabled = false;
					SuperYesObject.GetComponent<Collider>().enabled = false;

					while(Dialog.activeInHierarchy)
						yield return null;

					YesObject.SetActive(false);
					NoObject.SetActive(false);
					SuperYesObject.SetActive(false);

					break;
				}
				yield return null;
			}

			if(Yes)
			{
				Open();
				GoTo(3);
				StartCoroutine(Read(3, false, 0, false));

				YesObject.SetActive(false);
				NoObject.SetActive(false);
				SuperYesObject.SetActive(false);

				LoopChoice = false;
			}
			else if(No)
			{
				compterNo ++;

				if(compterNo == 1)
				{
					Open();
					GoTo(4);
					StartCoroutine(Read(2, false, 0, true));
					StartCoroutine(Close(2));
					NoText.text = "I like this place.";
					No = false;
				}
				else if(compterNo == 2)
				{
					Open();
					GoTo(5);
					StartCoroutine(Read(1, false, 0, true));
					StartCoroutine(Close(1));
					NoText.text = "Get outta here";
					No = false;
				}
				else if(compterNo == 3)
				{
					Open();
					GoTo(6);
					StartCoroutine(Read(1, false, 0, true));
					StartCoroutine(Close(1));
					No = false;

					while(Dialog.activeInHierarchy)
						yield return null;
					yield return waitforseconds_05;

					YesObject.SetActive(false);
					NoObject.SetActive(false);
					SuperYesObject.SetActive(true);
				}
			}
			else
			{
				Open();
				GoTo(12);
				StartCoroutine(Read(8, false, 0, false));

				LoopChoice = false;
			}
		}
		
		StartCoroutine(TransitionC_E());	
	}

	private IEnumerator TransitionC_E()
	{
		while(!line.StartsWith("ADDING A Z-AXIS !!!!"))
			yield return null;

		StartCoroutine(Ending());
	}

	private IEnumerator Ending()
	{
		Exit = true;

		float transp = 0.0f;
		float jerk = 0.0f;

		while(transp < 1.0f - 0.005f)
		{
			transp = Mathf.SmoothDamp(transp, 1.0f, ref jerk, 2.0f);
			WhiteParadise.SetColorA(transp);
			yield return null;
		}

		WhiteParadise.SetColorA(1.0f);
		Stop = true;
		Dialog.SetActive(false);
		Dialog.transform.SetAsLastSibling();

		yield return waitforseconds_1;

		Open();
		GoTo(7);
		StartCoroutine(Read(5, false, 0, true));
		StartCoroutine(Close(5));

		while(Dialog.activeInHierarchy)
			yield return null;

		yield return waitforseconds_075;

		PlayerPrefs.SetInt("Intro", 1);

		if(!PlayerPrefs.HasKey("Session"))
			PlayerPrefs.SetInt("Session", 1);
		else
			PlayerPrefs.SetInt("Session", PlayerPrefs.GetInt("Session") + 1);

		SceneManager.LoadScene("Arcade");
	}

	//Fin du Texte






	private IEnumerator Read(int nbline, bool auto, float delay, bool skip)
	{
		while(Reading && !Stop)
			yield return null;

		NbLineRead = 0;
		Reading = true;
		Stop = false;
		int key;
		int floor = 1;
		int length = 0;

		StartCoroutine(WaitForNextPanel());

		WaitForSecondsRealtime waitforsecondsrealtime_delay = new WaitForSecondsRealtime(delay);

		for(int i = 0; i < nbline; i++)
		{
			Talk = true;
			Clear();
			Letter[0].text = ">";
			key = 1;

			Tip.SetActive(compterTip ++ < 4);

			line = source[++index];

			if(line == null || Stop)
				break;

			words = line.Split(" "[0]);

			string last = words[words.Length - 1];
			string replace = System.String.Empty;
			string word;
			for(int m = 0; m < last.Length - 1; m ++)
				replace += last[m].ToString();
			words[words.Length - 1] = replace;

			int words_length = words.Length;
			for(int m = 0; m < words_length; m++)
			{
				word = words[m];
				length = word.Length;
				floor = key / 30 + 1;

				if(key % 30 != 0)
					key ++;

				if(key + length > floor * 30 && !word.Contains("!!!"))
				{
					key = floor * 30;
					floor ++;
				}

				int word_length = word.Length;
				for(int l = 0; l < word_length; l++)
				{
					if(key >= Letter.Length)
						break;

					Letter[key ++].text = word[l].ToString();

					if(!(NextPanel && skip) && !Stop)
						yield return waitforsecondsrealtime_00325;
				}
			}

			if(Stop)
				break;

			Talk = false;
			NextPanel = false;

			yield return waitforsecondsrealtime_01;

			if(auto)
				yield return waitforsecondsrealtime_delay;
			while(!auto && !Enter() && !Stop)
				yield return null;

			NbLineRead ++;
		}

		Reading = false;
		Stop = false;
	}

	private void Clear()
	{
		int length = Letter.Length;
		for(int i = 0; i < length; i++)
			Letter[i].GetComponent<Text>().text = System.String.Empty;
	}

	private IEnumerator WaitForNextPanel()
	{
		while(Reading && !Stop)
		{
			if(Enter() && Talk)
				NextPanel = true;
			yield return null;
		}
	}

	private void GoTo(int number)
	{
		index = 0;
		line = source[index];

		while(!line.StartsWith("**index** " + number.ToString()))
		{
			line = source[++index];
			if(line == null)
			{
				Debug.Log("Error : No index found !");
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
		while(NbLineRead != nbline)
			yield return null;

		Dialog.SetActive(false);
		Clear();
	}

	public void Choice(bool ch)
	{
		Yes = ch;
		No = !ch;
	}

	private bool Enter()
	{
		return(Input.GetKeyDown("return") || Input.GetKeyDown("space"));
	}
}