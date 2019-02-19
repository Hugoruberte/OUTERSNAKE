using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RedRabbitCreator : MonoBehaviour
{
	private Transform RedRabbitsPooling;

	private float Height;
	private Vector3 posi;

	private PlanetScript planetScript;
	private GameManagerV1 gameManager;
	private ArmchairScript looneyScript;

	private bool[] DestroyedFacesBoolean;


	void Awake()
	{
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		looneyScript = GameObject.Find("Armchair").GetComponent<ArmchairScript>();
	}

	public void RedRabbits(Transform myPlanet)
	{
		if(RedRabbitsPooling == null)
			RedRabbitsPooling = GameObject.Find("RedRabbitsPooling").transform;

		planetScript = myPlanet.GetComponent<PlanetScript>();
		posi = myPlanet.position;
		Height = (myPlanet.Find("Body").localScale.x/2) + 5.0f;

		List<Faces> theList = new List<Faces>(planetScript.DestroyedFaces);
		DestroyedFacesBoolean = new bool[6] {false, false, false, false, false, false};
		
		for(int i = 0; i < theList.Count; i ++)
		{
			if((int)theList[i] != -1)
				DestroyedFacesBoolean[i] = true;
		}

		Vector3 Position = Vector3.zero;
		Quaternion Rotation = Quaternion.identity;

		int count = gameManager.WorldSetting.RedRabbitAmount;
		int looney_face = (int)looneyScript.Face;
		int face;
		int[] UsedFace = new int[6] {-1, -1, -1, -1, -1, -1};
		int used_index = 0;
		int inc;

		for(int i = 0; i < count; i++)
		{
			inc = 0;
			do
			{
				face = Random.Range(0,6);
			}
			while((UsedFace.Contains(face) || face == looney_face || DestroyedFacesBoolean[face]) && inc ++ < 50);

			if(inc >= 50)
			{
				Debug.LogWarning("Pas de face possible pour le placement de ce RedRabbit ! Looney Face = " + face);
				break;
			}

			UsedFace[used_index ++] = face;

			switch(face)
			{
				case 0:
					Position = new Vector3(posi.x + Height, posi.y, posi.z);
					Rotation = Quaternion.Euler(0,0,270);
				break;

				case 1:
					Position = new Vector3(posi.x, posi.y + Height, posi.z);
					Rotation = Quaternion.identity;
				break;

				case 2:
					Position = new Vector3(posi.x, posi.y, posi.z + Height);
					Rotation = Quaternion.Euler(90,0,0);
				break;

				case 3:
					Position = new Vector3(posi.x - Height, posi.y, posi.z);
					Rotation = Quaternion.Euler(0,0,90);
				break;

				case 4:
					Position = new Vector3(posi.x, posi.y - Height, posi.z);
					Rotation = Quaternion.Euler(180,0,0);
				break;

				case 5:
					Position = new Vector3(posi.x, posi.y, posi.z - Height);
					Rotation = Quaternion.Euler(270,0,0);
				break;
			}

			Transform rabbitH = RedRabbitsPooling.GetChild(i);
			rabbitH.gameObject.SetActive(true);
			rabbitH.GetComponent<RedRabbitController>().Setup(Position, Rotation);
			rabbitH.GetComponent<RedRabbitController>().Face = (Faces)face;
		}
	}
}