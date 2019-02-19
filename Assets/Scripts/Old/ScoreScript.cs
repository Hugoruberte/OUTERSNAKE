using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
	private GameObject ScoreGameObject;

	private Text ScoreText;

	private SnakeManagement snakeManag;

	private int score = 0;
	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			score = (value > 0) ? (value - score) * snakeManag.BodyNumber + score : 0;
			ScoreText.text = Convertor(score);
		}
	}

	void Awake()
	{
		snakeManag = GameObject.FindWithTag("Player").GetComponent<SnakeManagement>();
		ScoreGameObject = GameObject.Find("Canvas/InGame/Score");
		ScoreText = ScoreGameObject.GetComponent<Text>();
	}

	private string Convertor(int value)
	{
		string e = value.ToString("n2");
		string s = System.String.Empty;

		int len = e.Length;

		for(int i = 0; i < len - 3; i++)
			s = string.Concat(s, e[i]);

		return s;
	}
}
