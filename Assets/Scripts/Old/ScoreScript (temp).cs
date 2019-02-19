using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreScript_temp : MonoBehaviour
{
	private Text scoreText;
	private Text detailText;

	private WaitForSeconds waitforseconds_1 = new WaitForSeconds(1.0f);

	public int score = 0;
	private int old = 0;
	public int log = 100;
	private float reference = 0.0f;
	private float secure;

	private Multiplicator_temp multiScript;
	private GameManagerV1 gameManager;


	void Awake()
	{
		scoreText = GameObject.Find("Canvas/InGame/Score").GetComponent<Text>();
		scoreText.text = "0";
		detailText = GameObject.Find("Canvas/InGame/Score/Detail").GetComponent<Text>();
		detailText.text = System.String.Empty;

		multiScript = GameObject.Find("LevelManager").GetComponent<Multiplicator_temp>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
	}

	public void UpdateScore(int plus)
	{
		if(gameManager.State == Scenes.Hell)
			return;

		int addition = 0;

		if(multiScript.multiplier > 1 && plus > 0)
			addition = plus * log * multiScript.multiplier;
		else if(score + plus * log > 0)
			addition = plus * log;
		else
			score = 0;

		score += addition;
		StartCoroutine(UpdateDetail(addition));
		StartCoroutine(MovingScore((int)Mathf.Sign(plus)));
	}

	private IEnumerator MovingScore(int sign)
	{
		while(old != score)
		{
			old = (int)Mathf.SmoothDamp(old, score + 5 * sign, ref reference, 0.075f, Mathf.Infinity, Time.unscaledDeltaTime);
			scoreText.text = old.ToString();

			secure += Time.deltaTime;
			if(secure > 0.25f)
				break;

			yield return null;
		}
		secure = 0.0f;
		old = score;
		scoreText.text = score.ToString();
	}

	private IEnumerator UpdateDetail(int plus)
	{
		string sign;

		if(Mathf.Sign(plus) > 0)
		{
			sign = "+";
			detailText.color = new Color(0.196f, 0.196f, 0.196f, 1.0f);
		}
		else
		{
			sign = System.String.Empty;
			detailText.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		}

		detailText.text = sign + plus.ToString();

		yield return waitforseconds_1;

		detailText.text = System.String.Empty;
	}
}