using UnityEngine;
using System.Collections;
using Tools;

public class SawScript : MonoBehaviour
{
	/*	C'est le script qui fait tourner la scie circulaire	*/
	/*	Ce script est directement sur la scie et pas sur le Parent pour le OnTriggerEnter */

	private Transform myTransform;
	private Transform Body;
	private Transform Parent;
	private Transform Armchair;

	private float omega = 1500.0f;
	private float speed = 12.0f;

	private WaitForSeconds waitforseconds = new WaitForSeconds(0.5f);

	private Vector3 targetPosition;

	private Collider myColl;

	private bool Rotating = false;

	private Vector3 targetScale = new Vector3(0.25f, 1, 1);

	private GameManagerV1 gameManager;
	private SawCreator sawCreator;
	private CameraScript cameraScript;
	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;
	private ArmchairScript armchairScript;

	private ParticleSystem myParticle;


	void Awake()
	{
		myTransform = transform;
		Armchair = GameObject.FindWithTag("Armchair").transform;

		myColl = GetComponent<Collider>();

		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		sawCreator = GameObject.Find("LevelManager/Creator").GetComponent<SawCreator>();
		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		snakeManag = GameObject.FindWithTag("Player").GetComponent<SnakeManagement>();
		armchairScript = Armchair.GetComponent<ArmchairScript>();

		Body = myTransform.Find("Body");
		Parent = myTransform.parent;

		myParticle = myTransform.Find("Spark").GetComponent<ParticleSystem>();
	}

	void Start()
	{
		speed = gameManager.WorldSetting.SawSpeed;
		myColl.enabled = false;
	}

	void Update()
	{
		if(Rotating)
		{
			Body.Rotate(Vector3.right * omega * Time.deltaTime);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if((other.CompareTag("SnakeBody") || other.CompareTag("Player")) && snakeScript.State == SnakeState.Run && snakeManag.Health != SnakeHealth.Invincible)
		{
			myParticle.Play();
			gameManager.StartCoroutine(gameManager.Blink());
			cameraScript.Shake(1.0f);
		}
	}

	public IEnumerator Launch()
	{
		int index = FindOrientation();

		myTransform.localScale = Vector3.zero;

		while(Parent.localScale.z != 1.0f)
		{
			Parent.SetLocalScaleZ(Mathf.MoveTowards(Parent.localScale.z, 1.0f, 4.0f * Time.deltaTime));
			yield return null;
		}

		myColl.enabled = true;
		Rotating = true;

		while(Vector3.Distance(myTransform.localScale, targetScale) > 0.1)
		{
			myTransform.localScale = Vector3.MoveTowards(myTransform.localScale, targetScale, 3.0f * Time.deltaTime);
			yield return null;
		}

		int getchild = 0;
		int tour = 0;
		int nb_tour = (index == -1) ? 4 : 6;
		int direction = 1;
		
		while(tour < nb_tour)
		{
			if(getchild % 4 == index)
			{
				direction *= -1;
				if(getchild + direction < 0)
					getchild = 3;
				else if(getchild + direction > 3)
					getchild = 0;
				else
					getchild += direction;
			}

			targetPosition = Parent.GetChild(getchild % 4).position + Parent.GetChild(getchild % 4).right * direction * 15;
			while(Vector3.Distance(myTransform.position, targetPosition) > 0.1)
			{
				myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, speed * Time.deltaTime);
				yield return null;
			}

			if(getchild % 4 != index)
			{
				if(getchild + direction < 0)
					getchild = 3;
				else if(getchild + direction > 3)
					getchild = 0;
				else
					getchild += direction;
			}

			tour ++;

			if(!sawCreator.SawActivated)
				break;
		}

		yield return waitforseconds;

		while(Vector3.Distance(myTransform.localScale, Vector3.zero) > 0.1)
		{
			myTransform.localScale = Vector3.MoveTowards(myTransform.localScale, Vector3.zero, 1.5f * Time.deltaTime);
			if(myTransform.localScale.z < 0.4f && myColl.enabled)
				myColl.enabled = false;
			yield return null;
		}

		Rotating = false;

		while(Parent.localScale.z != 0.0f)
		{
			Parent.SetLocalScaleZ(Mathf.MoveTowards(Parent.localScale.z, 0.0f, 4.0f * Time.deltaTime));
			yield return null;
		}

		if(index > -1)
			Parent.GetChild(index).gameObject.SetActive(true);
		Parent.localPosition = Vector3.zero;				//on remet le piege à sa place dans AxePathsPooling
		Parent.rotation = Quaternion.identity;
		Parent.gameObject.SetActive(false);
	}

	private int FindOrientation()
	{
		if(armchairScript.Face != Faces.None)
		{
			Transform child;
			for(int i = 0; i < 4; i++)
			{
				child = Parent.GetChild(i);
				if(Mathf.RoundToInt(Vector3.Dot(child.up, Armchair.forward)) == 1)
				{
					child.gameObject.SetActive(false);
					return i;
				}
			}
		}

		return -1;
	}
}