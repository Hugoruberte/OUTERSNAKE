using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BunneyArcade : MonoBehaviour
{
	[Header("Text")]
	public TextAsset myText;
	[Header("Sprite")]
	public Sprite BunneySprite;
	[Header("Other")]

	private RectTransform DialogRect;

	private DialogScript dialogScript;
	private TitleScript titleScrit;
	private SaveScript saveScript;
	private GameManagerV1 gameManager;
	private DeathScript deathScript;

	private WaitForSeconds waitforseconds_1 = new WaitForSeconds(1.0f);
	private WaitForSeconds waitforseconds_025 = new WaitForSeconds(0.25f);
	private WaitForSeconds waitforseconds_0975 = new WaitForSeconds(0.975f);
	private WaitForSeconds waitforseconds_055 = new WaitForSeconds(0.55f);
	private WaitForSeconds waitforseconds_0625 = new WaitForSeconds(0.625f);
	private WaitForSeconds waitforseconds_06575 = new WaitForSeconds(0.6575f);
	private WaitForSecondsRealtime waitforsecondsrealtime_05;

	private string[] source;
	private string[] words;

	public string line;

	private Vector2 Default;
	private Vector2 reference = Vector2.zero;

	private int compterNoSave = 0;
	private float clock;

	private bool NoSave = false;

	void Awake()
	{
		dialogScript = GameObject.Find("Canvas/Dialog").GetComponent<DialogScript>();
		dialogScript.myText = myText;

		waitforsecondsrealtime_05 = new WaitForSecondsRealtime(0.5f);

		DialogRect = GameObject.Find("Canvas/Dialog").GetComponent<RectTransform>();
		Default = DialogRect.anchoredPosition;

		titleScrit = GameObject.Find("Title").GetComponent<TitleScript>();
		deathScript = GameObject.Find("LevelManager").GetComponent<DeathScript>();
		saveScript = GameObject.Find("LevelManager").GetComponent<SaveScript>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
	}

	void Start()
	{
		dialogScript.ClearUp();
		//dialogScript.Body(Styles.Undertale);
		dialogScript.FontDialogSetup();
	}


	//Intro

	public IEnumerator Surprise(bool skip)
	{
		int session = PlayerPrefs.GetInt("Session");

		if(session == 1)	//premiere fois qu'on voit le titre
		{
			dialogScript.Open();
			dialogScript.GoTo(1);
			dialogScript.Read(6, false);
			
			StartCoroutine(TransitionS_I());
		}
		else if(saveScript.playerData.Reboot)		//on revient du rebootsystem
		{
			saveScript.playerData.Reboot = false;

			dialogScript.Open();
			dialogScript.GoTo(26);
			dialogScript.Read(1, false);
			dialogScript.Close(1);

			yield return new WaitWhile(() => dialogScript.Active);

			titleScrit.Interact(true);
		}
		else if(!skip)		//ce n'est pas la premiere fois qu'on voit le titre mais on a revu l'intro
		{
			dialogScript.Open();
			dialogScript.GoTo(8);
			dialogScript.Read(4, false);
			dialogScript.Close(4);

			yield return new WaitWhile(() => dialogScript.Active);
			
			titleScrit.Interact(true);
		}
		else if(!NoSave)		//on a passé l'intro et tout est normal
		{
			titleScrit.Interact(true);
		}
	}

	private IEnumerator TransitionS_I()
	{
		while(!(line.StartsWith("...") && dialogScript.Enter()))
			yield return null;

		StartCoroutine(SimpleRead("Wait, I hide my beautiful title ...", 0.0325f));

		yield return waitforseconds_0975;

		StartCoroutine(InMyWay());
	}

	private IEnumerator InMyWay()
	{
		float time = 0.3f;
		Vector2 reference = Vector2.zero;

		while(Vector2.Distance(DialogRect.anchoredPosition, new Vector2(190, 150)) > 1)
		{
			DialogRect.anchoredPosition = Vector2.SmoothDamp(DialogRect.anchoredPosition, new Vector2(190, 150), ref reference, time, Mathf.Infinity, Time.deltaTime);
			yield return null;
		}

		dialogScript.ClearUp();
		StartCoroutine(SimpleRead("Better ?", 0.0325f));
		yield return waitforseconds_055;

		while(Vector2.Distance(DialogRect.anchoredPosition, new Vector2(-200, 200)) > 1)
		{
			DialogRect.anchoredPosition = Vector2.SmoothDamp(DialogRect.anchoredPosition, new Vector2(-200, 200), ref reference, time, Mathf.Infinity, Time.deltaTime);
			yield return null;
		}

		dialogScript.ClearUp();
		StartCoroutine(SimpleRead("And here ?", 0.0325f));
		yield return waitforseconds_0625;

		while(Vector2.Distance(DialogRect.anchoredPosition, new Vector2(0, -420)) > 1)
		{
			DialogRect.anchoredPosition = Vector2.SmoothDamp(DialogRect.anchoredPosition, new Vector2(0, -420), ref reference, time, Mathf.Infinity, Time.deltaTime);
			yield return null;
		}

		dialogScript.ClearUp();
		StartCoroutine(SimpleRead("And now ?", 0.0325f));
		yield return waitforseconds_06575;

		StartCoroutine(TransitionI_E());
	}

	private IEnumerator TransitionI_E()
	{
		while(!Input.anyKeyDown)
			yield return null;

		while(Vector2.Distance(DialogRect.anchoredPosition, Default) > 1)
		{
			DialogRect.anchoredPosition = Vector2.SmoothDamp(DialogRect.anchoredPosition, Default, ref reference, 0.175f, Mathf.Infinity, Time.deltaTime);
			yield return null;
		}

		dialogScript.GoTo(2);
		dialogScript.Read(4, false);
		dialogScript.Close(4);

		while(!(line.StartsWith("(Choose with the arrows") && dialogScript.Enter()))
			yield return null;

		titleScrit.Interact(true);
	}

	public IEnumerator Entering()
	{
		dialogScript.Open();
		dialogScript.GoTo(7);
		dialogScript.Wave(0.05f, 0.025f);
		dialogScript.Read(1, false);

		yield return new WaitForSeconds(1.5f);

		dialogScript.Close();
	}

	public IEnumerator NoSaving()
	{
		NoSave = true;
		int session = PlayerPrefs.GetInt("Session");

		if(session == 1)
		{
			titleScrit.Interact(false);

			dialogScript.Open();
			if(compterNoSave >= 3)
			{
				dialogScript.GoTo(6);
				dialogScript.Read(1, false);
				dialogScript.Close(1);
				compterNoSave ++;
			}
			else if(compterNoSave == 2)
			{
				dialogScript.GoTo(5);
				dialogScript.Read(6, false);
				dialogScript.Close(6);
				compterNoSave ++;
			}
			else if(compterNoSave == 1)
			{
				dialogScript.GoTo(4);
				dialogScript.Read(2, false);
				dialogScript.Close(2);
				compterNoSave ++;
			}
			else if(compterNoSave == 0)
			{
				dialogScript.GoTo(3);
				dialogScript.Read(4, false);
				dialogScript.Close(4);
				compterNoSave ++;
			}
			
			yield return new WaitWhile(() => dialogScript.Active);
			yield return waitforseconds_025;

			titleScrit.Interact(true);
		}
		else if(session > 1)
		{
			titleScrit.Interact(false);

			dialogScript.Open();
			if(compterNoSave >= 5)
			{
				dialogScript.GoTo(33);
				dialogScript.Read(1, false);
				dialogScript.Close(1);
				compterNoSave ++;

				yield return new WaitWhile(() => dialogScript.Active);

				PlayerPrefs.SetInt("HellCounter", 0);
				SceneManager.LoadScene("TrueHell");
			}
			else if(compterNoSave >= 3)
			{
				dialogScript.GoTo(6);
				dialogScript.Read(1, false);
				dialogScript.Close(1);
				compterNoSave ++;
			}
			else if(compterNoSave == 2)
			{
				dialogScript.GoTo(5);
				dialogScript.Read(6, false);
				dialogScript.Close(6);
				compterNoSave ++;
			}
			else if(compterNoSave == 1)
			{
				dialogScript.GoTo(4);
				dialogScript.Read(2, false);
				dialogScript.Close(2);
				compterNoSave ++;
			}
			else if(compterNoSave == 0)
			{
				dialogScript.GoTo(9);
				dialogScript.Read(4, false);
				dialogScript.Close(4);
				compterNoSave ++;
			}
			
			yield return new WaitWhile(() => dialogScript.Active);
			yield return waitforseconds_025;

			titleScrit.Interact(true);
		}
	}

	//Fin Intro


	//Tuto

	public IEnumerator NuclearContinue()
	{
		yield return waitforsecondsrealtime_05;
		if(gameManager.Safe || dialogScript.Active)
			yield break;

		dialogScript.Open();
		dialogScript.GoTo(25);
		dialogScript.Read(3, false);
		dialogScript.Close(3);

		yield return new WaitWhile(() => dialogScript.Active);

		deathScript.Interact();
	}

	public IEnumerator Hole()
	{
		yield return waitforsecondsrealtime_05;
		if(gameManager.Safe || dialogScript.Active)
			yield break;

		dialogScript.Open();
		dialogScript.GoTo(31);
		dialogScript.Read(3, false);
		dialogScript.Close(3);
	}

	public IEnumerator Death()
	{
		yield return waitforseconds_1;
		yield return new WaitWhile(() => dialogScript.Active);

		dialogScript.Open();

		if(saveScript.playerData.HasSaved || PlayerPrefs.GetInt("Death") == 1)
		{
			dialogScript.GoTo(34);
		}
		else
		{
			dialogScript.GoTo(35);
		}

		dialogScript.Read(11, false);
		dialogScript.Close(11);
		
		yield return new WaitWhile(() => dialogScript.Active);

		deathScript.Interact();
	}

	private IEnumerator SimpleRead(string line, float interval)
	{
		/*//interval = 0.05f;
		Letter[0].text = ">";
		Letter[1].text = System.String.Empty;

		WaitForSecondsRealtime waitforsecondsrealtime_interval = new WaitForSecondsRealtime(interval);

		for(int i = 0; i < line.Length; i++)
		{
			Letter[i + 2].text = line[i].ToString();
			if(line[i] != ' ')
				yield return waitforsecondsrealtime_interval;
			else
				yield return null;
		}*/
		Debug.LogError("Il faut virer cette fonction !");
		yield break;
	}
}