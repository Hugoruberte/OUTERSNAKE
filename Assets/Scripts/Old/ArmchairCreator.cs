using UnityEngine;
using System.Collections;
using System.Linq;
using Tools;

public class ArmchairCreator : MonoBehaviour
{
	private GameManagerV1 gameManager;
	private ArmchairScript looneyScript;
	private PlanetScript planetScript;

	public GameObject ArmchairPrefab;
	private Transform Poubelle;
	private Transform Planet;
	private Transform Heart;
	private Transform Armchair;

	private Vector3 position;
	private Quaternion rotation;

	private Vector3[] World = new Vector3[6] {Vector3.right, Vector3.up, Vector3.forward, -Vector3.right, -Vector3.up, -Vector3.forward};
	private Faces[] myFaces = new Faces[6] {Faces.FaceX1, Faces.FaceY1, Faces.FaceZ1, Faces.FaceX2, Faces.FaceY2, Faces.FaceZ2};

	[HideInInspector]
	public bool Done = false;

	void Awake()
	{
		GameObject armchairObject = GameObject.FindWithTag("Armchair");
		if(armchairObject == null)
			armchairObject = Instantiate(ArmchairPrefab, Vector3.zero, Quaternion.identity);

		Armchair = armchairObject.transform;
		looneyScript = Armchair.GetComponent<ArmchairScript>();
		
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
	}

	public void Create()
	{
		StartCoroutine(Setup());
	}

	private IEnumerator Setup()
	{
		Done = false;

		looneyScript.AboveHole = false;

		Planet = gameManager.MainPlanet;
		Heart = Planet.Find("Heart");

		do
		{
			Poubelle = GameObject.Find("Poubelle").transform;
			yield return null;
		}
		while(Poubelle == null);

		Transform folder = Armchair.Find("MugFolder");
		folder.gameObject.SetActive(false);
		folder.parent = Poubelle;
		folder.localPosition = Vector3.zero;

		position = Vector3Extension.RoundToInt(Planet.position + Heart.forward * 15.5f - Heart.right * 0.5f + Heart.up * 0.5f);
		rotation = Heart.AbsoluteRotation();

		Armchair.position = position;
		Armchair.rotation = rotation;

		yield return null;

		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();
		looneyScript.Face = FindOrientation();

		yield return null;

		if(planetScript.DestroyedFaces.Contains(looneyScript.Face))
		{
			looneyScript.SetHoledFace();
		}
		else
		{
			looneyScript.SetMugedFace();
		}

		Done = true;
	}

	private Faces FindOrientation()
	{
		for(int i = 0; i < 6; i++)
		{
			if(Mathf.RoundToInt(Vector3.Dot(World[i], Armchair.forward)) == 1)
				return myFaces[i];
		}

		Debug.LogError("Cannot find the face !");
		return Faces.FaceY2;
	}
}