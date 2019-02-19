using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Tools;

public class DeathScript : MonoBehaviour
{
	private Transform SelectionAfterDeath;
	private GameObject Body;

	private WaitForSeconds waitforseconds_02 = new WaitForSeconds(0.2f);

	private GameManagerV1 gameManager;
	private SaveScript saveScript;
	private DialogScript dialogScript;

	private RectTransform SelectRect;
	private RectTransform Cursor;

	private Text TextComponent;
	private Text QuestionText;
	private Text CursorText;
	private Text RightText;
	private Text LeftText;
	private Image Image_2;

	enum DieOrRetry
	{
		Liveley,
		Looney,
		Yes,
		No,
		None
	};

	private DieOrRetry TheChoice = DieOrRetry.Liveley;

	private Vector2 SelectionScale;

	private Color32 yellowColor = new Color32(255, 240, 0, 255);
	private Color32 greyColor = new Color32(50, 50, 50, 255);


	void Awake()
	{
		SelectionAfterDeath = GameObject.Find("Canvas/InGame/ChoiceAfterDeath").transform;
		Body = SelectionAfterDeath.Find("Body").gameObject;
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		saveScript = GameObject.Find("LevelManager").GetComponent<SaveScript>();
		dialogScript = GameObject.Find("Canvas/Dialog").GetComponent<DialogScript>();

		SelectRect = SelectionAfterDeath.GetComponent<RectTransform>();
		Cursor = SelectionAfterDeath.Find("Body/Cursor").GetComponent<RectTransform>();

		SelectionScale = SelectionAfterDeath.GetComponent<RectTransform>().sizeDelta;

		CursorText = SelectionAfterDeath.Find("Body/Cursor").GetComponent<Text>();
		TextComponent = SelectionAfterDeath.Find("Body/Text").GetComponent<Text>();
		QuestionText = SelectionAfterDeath.Find("Body/Question").GetComponent<Text>();
		RightText = SelectionAfterDeath.Find("Body/Choices/Looney").GetComponent<Text>();
		LeftText = SelectionAfterDeath.Find("Body/Choices/Liveley").GetComponent<Text>();
		Image_2 = SelectionAfterDeath.Find("Image2").GetComponent<Image>();
	}

	public void Interact()
	{
		StartCoroutine(Choice());
		string kill = gameManager.KilledByConvertor();

		if(PlayerPrefs.GetInt("Death") == 1)
		{
			TextComponent.text = "> Ouch! Seems like you just died, killed by <color=red>" + kill.ToUpper() + "</color> ...";
		}
		else
		{
			TextComponent.text = "> Killed by <color=red>" + kill.ToUpper() + "</color> ... Shit happens.";
			PlayerPrefs.SetInt("Death", 1);
		}

		QuestionText.text = "> Whaddya want ?";
		RightText.text = "Go back to\nLooney !";
		LeftText.text = "Deal with\nLiveley ...";
	}

	private IEnumerator Choice()
	{
		while(dialogScript.Active)
			yield return null;
		yield return waitforseconds_02;

		SelectionAfterDeath.gameObject.SetActive(true);
		
		SelectRect.sizeDelta = new Vector2(100.0f, 0.0f);

		Body.SetActive(false);
		Image_2.SetColorA(1.0f);

		while(SelectRect.sizeDelta.y != SelectionScale.y)
		{
			SelectRect.SetSizeDeltaY(Mathf.MoveTowards(SelectRect.sizeDelta.y, SelectionScale.y, 2500.0f * Time.deltaTime));
			yield return null;
		}

		while(SelectRect.sizeDelta.x != SelectionScale.x)
		{
			SelectRect.SetSizeDeltaX(Mathf.MoveTowards(SelectRect.sizeDelta.x, SelectionScale.x, 2500.0f * Time.deltaTime));
			yield return null;
		}

		Body.SetActive(true);
		Image_2.SetColorA(0.0f);


		TheChoice = DieOrRetry.Liveley;
		RightText.color = Color.white;
		LeftText.color = yellowColor;
		CursorText.color = yellowColor;
		Cursor.SetAnchoredPositionX(-270);

		bool doce = false;

		while(true)
		{
			if(Input.GetKeyDown("return"))
			{
				if(TheChoice == DieOrRetry.Looney && saveScript.playerData.HasSaved)
				{
					doce = true;
					TheChoice = DieOrRetry.Yes;

					TextComponent.text = "> Back to Looney's ?";
					QuestionText.text = "> Is that what you want ?";
					RightText.text = "Yes !\n";
					LeftText.text = "No ...\n";
				}
				else if(TheChoice == DieOrRetry.Yes)
				{
					SelectionAfterDeath.gameObject.SetActive(false);
					gameManager.PlayLooney();
					break;
				}
				else if(TheChoice == DieOrRetry.No)
				{
					doce = false;
					TextComponent.text = "> Killed by <color=red>" + gameManager.KilledByConvertor().ToUpper() + "</color> ... Shit happens.";
					QuestionText.text = "> Whaddya want ?";
					RightText.text = "Go back to\nLooney !";
					LeftText.text = "Deal with\nLiveley ...";
				}
				else if(TheChoice == DieOrRetry.Liveley)
				{
					SelectionAfterDeath.gameObject.SetActive(false);
					gameManager.PlayLiveley();
					break;
				}
			}

			if(Input.GetAxisRaw("Horizontal") == 1.0f)
			{
				TheChoice = (doce == true) ? DieOrRetry.Yes : DieOrRetry.Looney;

				RightText.color = (saveScript.playerData.HasSaved) ? yellowColor : greyColor;
				LeftText.color = Color.white;

				CursorText.color = (saveScript.playerData.HasSaved) ? yellowColor : greyColor;
				Cursor.SetAnchoredPositionX(55);
			}
			else if(Input.GetAxisRaw("Horizontal") == -1.0f)
			{
				TheChoice = (doce == true) ? DieOrRetry.No : DieOrRetry.Liveley;

				RightText.color = Color.white;
				LeftText.color = yellowColor;

				CursorText.color = yellowColor;
				Cursor.SetAnchoredPositionX(-270);
			}
			yield return null;
		}
	}
}