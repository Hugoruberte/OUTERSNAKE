using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Tools;

public class RebootScript : MonoBehaviour
{
	private Transform fakeSnake;

	private GlitchEffect glitchScript;
	private SaveScript saveScript;

	private WaitForSeconds waitforseconds_04 = new WaitForSeconds(0.4f);
	private WaitForSeconds waitforseconds_005 = new WaitForSeconds(0.05f);

	private Vector3 targetPosition = Vector3.zero;
	private Vector3 reference = Vector3.zero;
	private Vector3 velocity = Vector3.zero;

	public bool Rabbit = true;

	private float jerk = 0.0f;
	private float jerk2;
	private float targetLight = 3.0f;
	private float glitchjerk = 0.0f;

	private Light redlight;

	private string[] List;


	void Awake()
	{
		List = new string[16] {"Error_", "Error", "Erro", "Err", "Er", "E", System.String.Empty, "R", "Re", "Reb", "Rebo", "Reboo", "Reboot", "Rebooti", "Rebootin", "Rebooting"};
		fakeSnake = transform.Find("Snake");

		redlight = transform.Find("Redlight").GetComponent<Light>();

		glitchScript = GameObject.Find("Main Camera").GetComponent<GlitchEffect>();
		saveScript = GameObject.Find("Matrix").GetComponent<SaveScript>();
	}


	void Start()
	{
		Transform Rabbits = GameObject.Find("Rabbits").transform;
		Transform Screens = transform.Find("Screens");

		int count = Rabbits.childCount;

		for(int i = 0; i < count; i++)
		{
			StartCoroutine(RabbitBehaviour(Rabbits.GetChild(i)));
		}

		for(int j = 0; j < 4; j++)
		{
			StartCoroutine(TextDisplay(Screens.GetChild(j).Find("Text").GetComponent<TextMesh>()));
		}
	}


	void Update()
	{
		targetPosition = new Vector3(0, Mathf.PingPong(Time.time * 0.75f, 4.0f) + 4.0f, 0);
		fakeSnake.localPosition = Vector3.SmoothDamp(fakeSnake.localPosition, targetPosition, ref reference, 0.75f);
		fakeSnake.Rotate(new Vector3(100, Mathf.PingPong(Time.time * 40.0f, 100.0f), 100) * Time.deltaTime);

		targetLight = 2 + Mathf.PingPong(Time.time * 4.0f, 4.0f);
		redlight.intensity = Mathf.SmoothDamp(redlight.intensity, targetLight, ref jerk2, 0.4f);
	}


	private IEnumerator RabbitBehaviour(Transform rabbitt)
	{
		float scale = 0;
		float pos = 0;

		while(Rabbit)
		{
			scale = 1 - Mathf.PingPong(Time.time * 0.15f, 0.2f);
			pos = 0.5f - Mathf.PingPong(Time.time * 0.15f, 0.1f);

			rabbitt.localScale = Vector3.SmoothDamp(rabbitt.localScale, new Vector3(1.0f, 1.0f, scale), ref velocity, 0.5f);
			rabbitt.SetLocalPositionY(Mathf.SmoothDamp(rabbitt.localPosition.y, pos, ref jerk, 0.5f));
			yield return null;
		}
	}


	private IEnumerator TextDisplay(TextMesh mytext)
	{
		mytext.text = "Error_";

		for(int i = 0; i < 6; i++)
		{
			mytext.text = (i % 2 == 0) ? "Error_" : "Error";

			yield return waitforseconds_04;
		}

		for(int j = 0; j < List.Length; j++)
		{
			mytext.text = List[j];

			yield return waitforseconds_005;
		}

		mytext.text = "Rebooting";

		while(glitchScript.intensity < 1.95f)
		{
			glitchScript.intensity = Mathf.SmoothDamp(glitchScript.intensity, 2.0f, ref glitchjerk, 1.0f);
			yield return null;
		}

		saveScript.playerData.Reboot = true;
		SceneManager.LoadScene("Arcade");
	}
}