using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SaveGameFree;
using UnityEngine.SceneManagement;
using Tools;

public class SaveScript : MonoBehaviour
{
	public string playerFileName = "playerData";
	private readonly string gameFileName = "gameData";

	private Transform Snake;
	private Transform Planets;
	private Transform MainPlanet;
	private Transform OldPlanet;
	private Transform myCamera;
	private Transform Armchair;

	[HideInInspector]
	public PlayerData playerData;
	[HideInInspector]
	public GameData gameData;

	private SnakeControllerV3 snakeScript;
	private SnakeManagement snakeManag;
	private GameManagerV1 gameManager;
	private PlanetScript planetScript;
	private CameraScript cameraScript;
	private ArmchairScript looneyScript;
	private PlanetSetup planetSetup;

	

	void Awake()
	{
		//playerData = new PlayerData();			// Initialize game data
		Saver.InitializeDefault();			// Initialize the Saver with the default configurations
		playerData = Saver.LoadPlayer(playerFileName);	// Load game data after initialization
		gameData = Saver.LoadGame(gameFileName);

		gameManager = GetComponent<GameManagerV1>();

		Armchair = GameObject.FindWithTag("Armchair").transform;
		GameObject Snake_GameObject = GameObject.FindWithTag("Player");
		Snake = Snake_GameObject.transform;

		myCamera = GameObject.Find("MainCamera").transform;

		snakeScript = Snake_GameObject.GetComponent<SnakeControllerV3>();
		snakeManag = Snake_GameObject.GetComponent<SnakeManagement>();
		cameraScript = myCamera.GetComponent<CameraScript>();
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		Planets = GameObject.Find("Planets").transform;
		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();
		looneyScript = Armchair.GetComponent<ArmchairScript>();
		planetSetup = Planets.GetComponent<PlanetSetup>();
	}

	public void Save()
	{
		playerData.HasSaved = true;

		switch(gameManager.State)
		{
			case Scenes.Tutoriel:
			case Scenes.Arcade:
				playerData.Position = Snake.AbsolutePosition();
				playerData.Rotation = Snake.AbsoluteRotation();
				playerData.myFace = (int)snakeScript.Face;
				playerData.BodyNumber = snakeManag.BodyNumber;

				playerData.Lives = gameManager.Lives;

				playerData.ArmchairPosition = looneyScript.initialPosition;
				playerData.ArmchairRotation = looneyScript.initialRotation;

				playerData.MainPlanet = gameManager.MainPlanet.name;
				playerData.OldPlanet = gameManager.OldPlanet.name;
				playerData.HeartLocalRotation = TransformExtension.AbsoluteRotation(gameManager.MainPlanet.Find("Heart").localRotation);

				playerData.NumberOfFaceDestroyed = planetSetup.DestroyedFacesName.Count;
				playerData.NameOfFaceDestroyed = ArrayExtension.DeepCopy(planetSetup.DestroyedFacesName);

				playerData.LooneyFace = (int)looneyScript.Face;

				int PlanetCount = Planets.childCount;
				playerData.Grids = new int[PlanetCount * 22*22*6];

				for(int i = 0; i < PlanetCount; i++)
				{
					CellEnum[] mygrid = ArrayExtension.DeepCopy(Planets.GetChild(i).GetComponent<PlanetScript>().Grid);
					for(int j = 0; j < 22*22*6; j++)
					{
						playerData.Grids[i*22*22*6 + j] = (int)mygrid[j];
					}
				}
				break;

			default:
				Debug.LogError("La sauvegarde n'a pas encore été gérée dans cette scene !", this);
				break;
		}

		Saver.Save(playerData, playerFileName);
		Debug.Log("Saved !");
	}

	public void Load()
	{
		playerData = Saver.LoadPlayer(playerFileName);
		StartCoroutine(Rebuild());
	}

	private IEnumerator Rebuild()
	{
		switch(gameManager.State)
		{
			case Scenes.Arcade:
				
				Transform ObjectPooling = GameObject.Find("ObjectPoolingStock").transform;
				Transform Folder;
				int length = ObjectPooling.childCount;
				int sub_length;
				for(int i = 0; i < length; i++)
				{
					Folder = ObjectPooling.GetChild(i);
					sub_length = Folder.childCount;
					for(int j = 0; j < sub_length; j++)
					{
						Folder.GetChild(j).gameObject.SetActive(false);
					}
				}

				snakeManag.Health = SnakeHealth.Alive;

				snakeScript.Face = (Faces)playerData.myFace;

				int PlanetCount = Planets.childCount;
				for(int i = 0; i < PlanetCount; i++)
				{
					CellEnum[] mygrid = new CellEnum[22*22*6];

					for(int j = 0; j < 22*22*6; j++)
					{
						mygrid[j] = (CellEnum)playerData.Grids[i*22*22*6 + j];
					}
					Planets.GetChild(i).GetComponent<PlanetScript>().Grid = ArrayExtension.DeepCopy(mygrid);
				}

				yield return null;

				MainPlanet = FindPlanet(playerData.MainPlanet);
				OldPlanet = FindPlanet(playerData.OldPlanet);
				gameManager.MainPlanet = MainPlanet;
				gameManager.OldPlanet = OldPlanet;

				gameManager.MainPlanet.Find("Skin").gameObject.SetActive(true);

				snakeScript.Heart = MainPlanet.Find("Heart");
				snakeManag.BodyNumber = playerData.BodyNumber;

				gameManager.Lives = playerData.Lives;

				MainPlanet.Find("Heart").localRotation = playerData.HeartLocalRotation;

				planetScript = MainPlanet.GetComponent<PlanetScript>();

				planetScript.MainPlanet = true;
				planetScript.SetRotation(false);

				cameraScript.Heart = MainPlanet.Find("Heart");

				if(gameManager.MainPlanet != gameManager.OldPlanet)
				{
					OldPlanet.GetComponent<PlanetScript>().MainPlanet = false;
					OldPlanet.GetComponent<PlanetScript>().SetRotation(true);
				}

				looneyScript.Face = (Faces)playerData.LooneyFace;

				planetSetup.DestroyedFacesName = new List<string>(playerData.NameOfFaceDestroyed);
				planetSetup.MainPlanetSetObjectsWithoutArmchair();
				planetSetup.DestroyPlanetFaces();

				yield return null;
				
				Armchair.position = playerData.ArmchairPosition;
				Armchair.rotation = playerData.ArmchairRotation;

				looneyScript.StartCoroutine(looneyScript.EnterSetup(true));
				break;
		}

		Debug.Log("Loaded !");
	}

	public void Reset()
	{
		PlayerPrefs.DeleteAll();

		playerData.HasSaved = false;

		int PlanetCount = Planets.childCount;
		playerData.Grids = new int[PlanetCount * 22*22*6];
		
		playerData.BodyNumber = 0;
		playerData.Thoughts = -1;

		playerData.Position = Vector3.zero;
		playerData.Rotation = Quaternion.identity;
		playerData.HeartLocalRotation = Quaternion.identity;

		playerData.myFace = (int)Faces.FaceX1;
		playerData.MainPlanet = System.String.Empty;
		playerData.OldPlanet = System.String.Empty;
		
		playerData.NumberOfFaceDestroyed = 0;
		playerData.NameOfFaceDestroyed = new string[72];

		Saver.Save(playerData, playerFileName);
	}

	private Transform FindPlanet(string name)
	{
		int PlanetCount = Planets.childCount;
		for(int i = 0; i < PlanetCount; i++)
		{
			if(Planets.GetChild(i).name.Contains(name))
			{
				return Planets.GetChild(i).transform;
			}
		}

		return null;
	}
}