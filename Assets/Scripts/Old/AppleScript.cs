using UnityEngine;
using System.Collections;

public enum AppleType
{
	Red,
	Rotten,
	Dung
};

public class AppleScript : MonoBehaviour
{
	private Transform myTransform;
	private Transform myChild;

	private ParticleSystem myExplosion;
	private ParticleSystem myFire;
	private ParticleSystemRenderer myExplosionRenderer;

	private WaitForSeconds waitforseconds_1 = new WaitForSeconds(1.0f);

	private Renderer myRenderer;
	private Collider myCollider;
	private Light myLight;

	Color FireColor = new Color32(255, 165, 0, 255);
	Color RedColor = new Color32(255, 0, 0, 255);

	private WaitForSeconds waitforseconds = new WaitForSeconds(1.0f);

	private float Height;
	public int myCell;

	public AppleType State = AppleType.Red;

	private bool visible = true;
	private bool Growth = false;

	private Vector3 TargetScale = new Vector3(0.5f,0.5f,0.5f);

	private ArmchairScript looneyScript;
	private PlanetScript planetScript;
	private GameManagerV1 gameManager;
	private TutorielManager tutoScript;

	private Animator anim;


	void Awake()
	{
		myTransform = transform;
		myChild = myTransform.Find("Body");

		myLight = myChild.GetComponent<Light>();

		myRenderer = myChild.GetComponent<Renderer>();
		myCollider = myTransform.GetComponent<Collider>();

		myExplosion = myChild.Find("Explosion").GetComponent<ParticleSystem>();
		myFire = myChild.Find("Fire").GetComponent<ParticleSystem>();
		myExplosionRenderer = myChild.Find("Explosion").GetComponent<ParticleSystemRenderer>();
		
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		tutoScript = GameObject.Find("LevelManager").GetComponent<TutorielManager>();

		looneyScript = GameObject.Find("Armchair").GetComponent<ArmchairScript>();
		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();
		
		Height = (gameManager.MainPlanet.Find("Body").localScale.x/2) + 0.25f;

		anim = myChild.GetComponent<Animator>();
	}

	void OnEnable()
	{
		Color col = (State == AppleType.Red) ? new Color32(255, 0, 0, 255) : new Color32(30, 40, 0, 255);
		myExplosionRenderer.material.color = col;
		myRenderer.material.color = col;

		if(State != AppleType.Red)
			myLight.enabled = false;

		myChild.localScale = TargetScale;

		StartCoroutine(Jump());
	}

	private IEnumerator Explosion()
	{
		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();
		planetScript.Grid[myCell] = CellEnum.Empty;

		myRenderer.enabled = false;
		myCollider.enabled = false;
		myLight.enabled = false;

		myExplosion.Play();
		AppleBurnt(false);

		yield return waitforseconds_1;

		AppleSetGrid();
	}

	public IEnumerator Scale()
	{
		Growth = true;
		myChild.localScale = Vector3.zero;
		while(Vector3.Distance(myChild.localScale, TargetScale) > 0.01f)
		{
			myChild.localScale = Vector3.MoveTowards(myChild.localScale, TargetScale, 1.0f * Time.deltaTime);
			yield return null;
		}
		myChild.localScale = TargetScale;
		Growth = false;
	}

	private void AppleSetGrid()
	{
		int cell;
		int looney_face = (int)looneyScript.Face;
		int inc;

		inc = 0;
		do
		{
			if(looney_face == 0)
				cell = Random.Range(22*22, 22*22*6);
			else if(looney_face == 5)
				cell = Random.Range(0, 22*22*5);
			else
				cell = (Random.Range(0, 5) < looney_face) ? Random.Range(0, looney_face*22*22) : Random.Range((looney_face+1)*22*22, 22*22*6);
		}
		while(planetScript.Grid[cell] != CellEnum.Empty && inc ++ < 100);

		myTransform.rotation = CellToRotation(cell);
		myTransform.position = CellToPosition(cell);

		myRenderer.enabled = true;
		myCollider.enabled = true;
		myLight.enabled = true;

		if(!Growth)
			StartCoroutine(Scale());
		else
			myChild.localScale = Vector3.zero;

		planetScript.Grid[cell] = CellEnum.Apple;
		myCell = cell;
	}

	private Vector3 CellToPosition(int nb)
	{
		int face = nb /(22*22);
		Vector3[] ligns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
		Vector3[] columns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
		Vector3[] pointeurs = new Vector3[6] {Vector3.right, Vector3.up, Vector3.forward, -Vector3.right, -Vector3.up, -Vector3.forward};

		Vector3 lign;
		Vector3 column;
		Vector3 pointeur;

		if(face > 2)	//Reverse
		{
			lign = ligns[(face + 2) % 3];
			column = columns[(face + 1) % 3];
		}
		else
		{
			lign = ligns[(face + 1) % 3];
			column = columns[(face + 2) % 3];
		}

		pointeur = pointeurs[face];

		return gameManager.MainPlanet.position + pointeur*Height + lign*10.5f - column*10.5f + column*(nb%22) - lign*((nb-22*22*face)/22);
	}

	private Quaternion CellToRotation(int nb)
	{
		int face = nb/(22*22);
		Quaternion rot = Quaternion.identity;

		switch(face)
		{
			//X
			case 0:
			case 3:
				rot = (face == 0) ? Quaternion.Euler(0,0,270) : Quaternion.Euler(0,0,90);
			break;

			//Y
			case 1:
			case 4:
				rot = (face == 1) ? Quaternion.identity : Quaternion.Euler(180,0,0);
			break;

			//Z
			case 2:
			case 5:
				rot = (face == 2) ? Quaternion.Euler(90,0,0) : Quaternion.Euler(270,0,0);
			break;

			default:
				Debug.LogError("Grid too big : cell " + nb);
			break;
		}

		return rot;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			if(tutoScript && !tutoScript.Phase_2)
			{
				myRenderer.enabled = false;
				myCollider.enabled = false;
				myLight.enabled = false;

				myExplosion.Play();
			}
			else
			{
				StartCoroutine(Explosion());
			}
		}
		else if(other.CompareTag("Bounds"))
		{
			myTransform.localPosition = Vector3.zero;
			gameObject.SetActive(false);
		}
		else if(other.CompareTag("Fire") && myFire.isStopped)
		{
			AppleBurnt(true);
			planetScript.Grid[myCell] = CellEnum.AppleBurn;
		}
		else if(other.CompareTag("Saw"))
		{
			StartCoroutine(Explosion());
		}
	}

	public void AppleBurnt(bool burn)
	{
		if(burn)
		{
			myFire.Play();
			myLight.color = FireColor;
		}
		else if(myFire.isPlaying)
		{
			myFire.Stop();
			if(State == AppleType.Red)
				myLight.color = RedColor;
		}
	}

	private IEnumerator Jump()
	{
		while(State != AppleType.Dung)
		{
			if(visible)
			{
				yield return new WaitForSeconds(Random.Range(5.0f, 15.0f));

				anim.enabled = true;
				anim.SetBool("play", true);
				yield return null;
				anim.SetBool("play", false);

				yield return waitforseconds;

				anim.enabled = false;
			}
			else
			{
				yield return null;
			}
		}
	}

	void OnBecameVisible()
	{
		visible = true;
	}

	void OnBecameInvisible()
	{
		visible = false;
	}
}