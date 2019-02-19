using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Tools;

public class Multiplicator_temp : MonoBehaviour
{
	private Transform Canvas;
	private GameObject Combo;
	private Transform Text;
	private Transform Arrow;
	private Transform Times;
	private Transform Int;

	public int multiplier = 0;

	private float clock = 0.0f;
	private float w = 0.0f;
	private float jerk = 0.0f;
	public int height = 60;

	private bool setup = true;

	private RectTransform ComboRect;
	private RectTransform TextRect;
	private Vector2 reference = Vector2.zero;
	private Vector2 reference2;

	private GameManagerV1 gameManager;
	private SnakeManagement snakeManag;


	void Awake()
	{
		Canvas = GameObject.Find("Canvas").transform;
		
		Combo = Canvas.Find("InGame/Combo").gameObject;
		ComboRect = Combo.GetComponent<RectTransform>();
		ComboRect.anchoredPosition = new Vector2(-900, height);

		Arrow = Combo.transform.Find("Arrow");
		Text = Combo.transform.Find("Text");
		TextRect = Text.GetComponent<RectTransform>();
		Times = Text.Find("Times");
		Int = Text.Find("Int");

		Combo.SetActive(false);
		
		gameManager = GetComponent<GameManagerV1>();
		snakeManag = GameObject.FindWithTag("Player").GetComponent<SnakeManagement>();
	}


	void Update()
	{
		if(multiplier > 1 && !gameManager.GameOver && !gameManager.Rocket && !gameManager.Safe)
		{
			if(setup)
				_Setup(true);

			ComboRect.anchoredPosition = Vector2.SmoothDamp(ComboRect.anchoredPosition, new Vector2(-570, height), ref reference, 0.1f, Mathf.Infinity, Time.deltaTime);
			TextRect.anchoredPosition = Vector2.SmoothDamp(TextRect.anchoredPosition, new Vector2(-440, height + 50), ref reference2, 0.15f, Mathf.Infinity, Time.deltaTime);
			Int.GetComponent<Text>().text = multiplier.ToString();

			w = Mathf.SmoothDamp(w, 1.0f, ref jerk, 0.1f);

			Combo.GetComponent<Image>().SetColorA(w);
			Times.GetComponent<Image>().SetColorA(w);
			Arrow.GetComponent<Image>().SetColorA(w);
			Text.GetComponent<Text>().SetColorA(w);
			Int.GetComponent<Text>().SetColorA(w);
		}
		else if(Combo.activeSelf)
		{
			if(!setup)
				_Setup(false);

			ComboRect.anchoredPosition = Vector2.SmoothDamp(ComboRect.anchoredPosition, new Vector2(-900, height), ref reference, 0.1f, Mathf.Infinity, Time.deltaTime);

			w = Mathf.SmoothDamp(w, 0.0f, ref jerk, 0.05f);

			Combo.GetComponent<Image>().SetColorA(w);
			Times.GetComponent<Image>().SetColorA(w);
			Arrow.GetComponent<Image>().SetColorA(w);
			Text.GetComponent<Text>().SetColorA(w);
			Int.GetComponent<Text>().SetColorA(w);
			
			if(Vector2.Distance(ComboRect.anchoredPosition, new Vector2(-900, height)) < 0.01f)
			{
				Combo.SetActive(false);
				enabled = false;
			}
		}


		if(multiplier > 0 && (Time.time > clock + 3.0f || snakeManag.Health == SnakeHealth.Dead))
		{
			multiplier = 0;
		}
	}


	private void _Setup(bool open)
	{
		
	setup = !setup;

		if(open)
		{
			Combo.SetActive(true);
			Text.SetParent(Canvas.Find("InGame"), true);
		}
		else
		{
			TextRect.anchoredPosition = new Vector2(-420, height + 50);
			Text.SetParent(Combo.transform, true);
		}
	}


	public void AddCombo(int plus)
	{
		enabled = true;
		multiplier += plus;
		clock = Time.time;
	}
}
