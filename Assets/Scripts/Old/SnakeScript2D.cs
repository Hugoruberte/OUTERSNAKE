using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Tools;

public class SnakeScript2D : MonoBehaviour
{
	private Vector3 dir = -Vector3.right;
	private Vector3 bor = Vector3.zero;

	public float Speed = 0.2f;
	private float Clock = 0.0f;
	private int Score = 0;
	private int Death = 0;

	private WaitForSeconds waitforseconds_speed;

	private Transform myTransform;

	private GameObject Dialog;

	private AppleScript2D appleScript;
	private BunneyIntro bunneyScript;

	private Text Yes;
	private Text No;
	private Text SuperYes;

	private Color colorYes;
	private Color colorNo;
	private Color colorYellow = new Color(1.0f, 1.0f, 0.0f, 1.0f);

	private Text ScoreT;

	bool ate = false;
	bool border = false;
	bool alive = true;
	bool loopey = true;
	bool contact = false;

	public GameObject SnakePartPrefab;
	private GameObject SnakePartStock;

	List<Transform> tail = new List<Transform>();

	void Awake()
	{
		Dialog = GameObject.Find("Canvas/Dialog").gameObject;
		ScoreT = GameObject.Find("Canvas/Score").GetComponent<Text>();
		ScoreT.text = "Score : 0";

		waitforseconds_speed = new WaitForSeconds(Speed);
	}

	void Start()
	{
		myTransform = transform;
		SnakePartStock = new GameObject();
		SnakePartStock.name = "SnakePartStock";

		Yes = GameObject.Find("Ground/Yes").GetComponent<Text>();
		No = GameObject.Find("Ground/No").GetComponent<Text>();
		SuperYes = GameObject.Find("Ground/SuperYes").GetComponent<Text>();

		colorYes = Yes.color;
		colorNo = No.color;
		colorYes.SetColorA(1.0f);
		colorNo.SetColorA(1.0f);

		Yes.gameObject.SetActive(false);
		No.gameObject.SetActive(false);
		SuperYes.gameObject.SetActive(false);

		Yes.color = colorYes;
		No.color = colorNo;
		SuperYes.color = colorYes;

		appleScript = GameObject.Find("Ground").GetComponent<AppleScript2D>();
		bunneyScript = GameObject.Find("SceneManager").GetComponent<BunneyIntro>();

		GameObject snakePart = (GameObject)Instantiate(SnakePartPrefab, myTransform.position + myTransform.right, Quaternion.identity);
		snakePart.name = "SnakePart " + Time.time.ToString();
		snakePart.transform.parent = SnakePartStock.transform;
		tail.Insert(0, snakePart.transform);

		snakePart = (GameObject)Instantiate(SnakePartPrefab, myTransform.position + myTransform.right, Quaternion.identity);
		snakePart.name = "SnakePart " + Time.time.ToString();
		snakePart.transform.parent = SnakePartStock.transform;
		tail.Insert(0, snakePart.transform);
		
		StartCoroutine(Move());
	}

	IEnumerator Move()
	{
		while(alive)
		{
			loopey = true;

			Vector3 pos = myTransform.position;

			if(!border)
			{
				transform.Translate(dir);
			}
			else
			{
				transform.Translate(bor * 28f);
				border = false;
			}

			if(ate)
			{
				GameObject snakePart = (GameObject)Instantiate(SnakePartPrefab, pos, Quaternion.identity);
				snakePart.name = "SnakePart " + Time.time.ToString();
				snakePart.transform.parent = SnakePartStock.transform;
				tail.Insert(0, snakePart.transform);
				ate = false;
			}
			else if(tail.Count > 0)
			{
				tail.Last().position = pos;
				tail.Insert(0, tail.Last());
				tail.RemoveAt(tail.Count-1);
			}

			yield return waitforseconds_speed;

			loopey = false;
		}
	}

	void OnTriggerEnter(Collider coll)
	{
		if(coll.name.StartsWith("Apple"))
		{
			ate = true;
			appleScript.Clock = 0.0f;	// peut susciter un bug au début !
			appleScript.Create();
			Destroy(coll.gameObject);
			ScoreT.text = "Score : " +(++Score).ToString();
		}
		else if(coll.name == "Border")
		{
			border = true;
			bor = coll.transform.right;
		}
		else if(coll.name.StartsWith("SnakePart") && coll.name != tail.First().name)
		{
			alive = false;
			Death ++;
			ScoreT.text = "Score : 0";
			Score = 0;
			StartCoroutine(Collapse());
		}
	}

	void OnTriggerStay(Collider coll)
	{
		if(coll.name == "Yes")
			Yes.color = colorYellow;
		else if(coll.name == "No")
			No.color = colorYellow;
		else if(SuperYes && coll.name == "SuperYes")
			SuperYes.color = colorYellow;
	}

	void OnTriggerExit(Collider coll)
	{
		if(coll.name == "Yes")
			Yes.color = colorYes;
		else if(coll.name == "No")
			No.color = colorNo;
		else if(coll.name == "SuperYes")
			SuperYes.color = colorYes;
	}

	IEnumerator Collapse()
	{
		while(loopey)
  		{
  			yield return null;
  		}

  		tail.Clear();
  		Destroy(SnakePartStock);
  		myTransform.position = new Vector3(8, 0, 1);

  		SnakePartStock = new GameObject();
		SnakePartStock.name = "SnakePartStock";

		GameObject snakePart = (GameObject)Instantiate(SnakePartPrefab, myTransform.position + myTransform.right, Quaternion.identity);
		snakePart.name = "SnakePart";
		snakePart.transform.parent = SnakePartStock.transform;
		tail.Insert(0, snakePart.transform);

		snakePart = (GameObject)Instantiate(SnakePartPrefab, myTransform.position + myTransform.right, Quaternion.identity);
		snakePart.name = "SnakePart";
		snakePart.transform.parent = SnakePartStock.transform;
		tail.Insert(0, snakePart.transform);

		dir = -Vector3.right;

		alive = true;
		StartCoroutine(Move());
	}

	void Update()
	{
		if(Input.GetKeyDown("right") && dir != -Vector3.right)
			dir = Vector3.right;
		else if(Input.GetKeyDown("down") && dir != Vector3.forward)
			dir = -Vector3.forward;
		else if(Input.GetKeyDown("left") && dir != Vector3.right)
			dir = -Vector3.right;
		else if(Input.GetKeyDown("up") && dir != -Vector3.forward)
			dir = Vector3.forward;


		if(Select())
		{
			if((Vector4.Distance(SuperYes.color, colorYellow) == 0 || Vector4.Distance(Yes.color, colorYellow) == 0) && !Dialog.activeInHierarchy)
	
	 			bunneyScript.Choice(true);
	
	 		else if(Vector4.Distance(No.color, colorYellow) == 0 && !Dialog.activeInHierarchy)
	
	 			bunneyScript.Choice(false);
		}
		

		Clock += Time.deltaTime;

		if(!contact &&(Death > 4 || Clock > 15.0 || Score > 9))
		{
			bunneyScript.Contact();
			contact = true;
		}
	}

	bool Select()
	{
		return(Input.GetKeyDown("return") || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl));
	}
}

