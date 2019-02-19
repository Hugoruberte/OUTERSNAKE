using UnityEngine;
using System.Collections;

public class ObjectPooling : MonoBehaviour
{
	public GameObject ApplePrefab;
	public GameObject BurrowPrefab;
	public GameObject RabbitPrefab;
	public GameObject LazerPrefab;
	public GameObject SawPathPrefab;
	public GameObject RocketPrefab;
	public GameObject RedRabbitPrefab;
	public GameObject NuclearSwitchPrefab;
	public GameObject CasterBlasterPrefab;
	public GameObject TreePrefab;

	[Header("Instantiate all prefab")]
	public bool Instancing = true;
	[HideInInspector]
	public bool Done = false;

	private PerformanceScript perfScript;

	private int appleQt = 500;
	private int rabbitQt = 30;
	private int redRabbitQt = 6;
	private int lazerQt = 30;
	private int sawQt = 10;
	private int rocketQt = 5;
	private int nuclearQt = 7;
	private int treeQt = 90;
	private int casterQt = 75;


	void Awake()
	{
		int index = 0;
		int i;


		GameObject Stock = new GameObject();
		Stock.name = "ObjectPoolingStock";
		Stock.transform.position = Vector3.zero;


		GameObject nApple = new GameObject();
		nApple.name = "ApplesPooling";
		nApple.transform.parent = Stock.transform;
		nApple.transform.localPosition = Vector3.zero;

		GameObject nBurrow = new GameObject();
		nBurrow.name = "BurrowsPooling";
		nBurrow.transform.parent = Stock.transform;
		nBurrow.transform.localPosition = Vector3.zero;

		GameObject nRabbit = new GameObject();
		nRabbit.name = "RabbitsPooling";
		nRabbit.transform.parent = Stock.transform;
		nRabbit.transform.localPosition = Vector3.zero;

		GameObject nRedRabbit = new GameObject();
		nRedRabbit.name = "RedRabbitsPooling";
		nRedRabbit.transform.parent = Stock.transform;
		nRedRabbit.transform.localPosition = Vector3.zero;

		GameObject fire = new GameObject();
		fire.name = "FiresPooling";
		fire.transform.localPosition = Vector3.zero;

		fire.transform.parent = Stock.transform;
		GameObject nLazer = new GameObject();
		nLazer.name = "LazersPooling";
		nLazer.transform.parent = Stock.transform;
		nLazer.transform.localPosition = Vector3.zero;

		GameObject nSawPath = new GameObject();
		nSawPath.name = "SawPathsPooling";
		nSawPath.transform.parent = Stock.transform;
		nSawPath.transform.localPosition = Vector3.zero;

		GameObject nRocket = new GameObject();
		nRocket.name = "RocketsPooling";
		nRocket.transform.parent = Stock.transform;
		nRocket.transform.localPosition = Vector3.zero;

		GameObject nNuclear = new GameObject();
		nNuclear.name = "NuclearSwitchsPooling";
		nNuclear.transform.parent = Stock.transform;
		nNuclear.transform.localPosition = Vector3.zero;

		GameObject nTree = new GameObject();
		nTree.name = "TreesPooling";
		nTree.transform.parent = Stock.transform;
		nTree.transform.localPosition = Vector3.zero;
		
		GameObject nCasterBlaster = new GameObject();
		nCasterBlaster.name = "CasterBlastersPooling";
		nCasterBlaster.transform.parent = Stock.transform;
		nCasterBlaster.transform.localPosition = Vector3.zero;


		if(!Instancing)
			return;


		perfScript = GameObject.Find("LevelManager").GetComponent<PerformanceScript>();
		perfScript.myRendered = new GameObject[appleQt + 2*rabbitQt + redRabbitQt + lazerQt + sawQt + rocketQt + nuclearQt + treeQt + casterQt];


		
		
		for(i = 0; i < appleQt; i++)
		{
			GameObject Anapple = Instantiate(ApplePrefab, Vector3.zero, Quaternion.identity);
			Anapple.name = "RedApple";
			Anapple.tag = "Apple";
			Anapple.GetComponent<AppleScript>().State = AppleType.Red;
			perfScript.myRendered[index ++] = Anapple;
			Anapple.transform.parent = nApple.transform;
		}
		
		
		
		
		
		
		
		for(i = 0; i < rabbitQt; i++)
		{
			GameObject ABurrow = Instantiate(BurrowPrefab, Vector3.zero, Quaternion.identity);
			ABurrow.name = "Burrow";
			perfScript.myRendered[index ++] = ABurrow;
			ABurrow.transform.parent = nBurrow.transform;
		}


		
		
		
		for(i = 0; i < rabbitQt; i++)
		{
			GameObject ARabbit = Instantiate(RabbitPrefab, Vector3.zero, Quaternion.identity);
			ARabbit.name = "Rabbit";
			perfScript.myRendered[index ++] = ARabbit;
			ARabbit.transform.parent = nRabbit.transform;

			nBurrow.transform.GetChild(i).GetComponent<BurrowScript>().myRabbit = ARabbit.transform;
		}




		
		
		
		for(i = 0; i < redRabbitQt; i++)
		{
			GameObject ARedRabbit = Instantiate(RedRabbitPrefab, Vector3.zero, Quaternion.identity);
			ARedRabbit.name = "RedRabbit";
			perfScript.myRendered[index ++] = ARedRabbit;
			ARedRabbit.transform.parent = nRedRabbit.transform;
		}
		
		
		
		
		
		
		
		for(i = 0; i < lazerQt; i++)
		{
			GameObject ALazer = Instantiate(LazerPrefab, Vector3.zero, Quaternion.identity);
			ALazer.name = "Lazer";
			perfScript.myRendered[index ++] = ALazer;
			ALazer.transform.parent = nLazer.transform;
		}
		
		
		
		
		
		
		
		for(i = 0; i < sawQt; i++)				//je crée arbitrairement 10 path pour avoir un peu de marge en ne surchargeant pas la scene
		{
			GameObject AnSawPath = Instantiate(SawPathPrefab, Vector3.zero, Quaternion.identity);
			AnSawPath.name = "SawPath";
			perfScript.myRendered[index ++] = AnSawPath;
			AnSawPath.transform.parent = nSawPath.transform;
		}

		
		
		
		
		
		
		
		
		for(i = 0; i < rocketQt; i++)
		{
			GameObject ARocket = Instantiate(RocketPrefab, Vector3.zero, Quaternion.identity);
			ARocket.name = "Rocket";
			perfScript.myRendered[index ++] = ARocket;
			ARocket.transform.parent = nRocket.transform;
		}






		
		
		
		for(i = 0; i < nuclearQt; i++)
		{
			GameObject ANuclear = Instantiate(NuclearSwitchPrefab, Vector3.zero, Quaternion.identity);
			ANuclear.name = "NuclearSwitch";
			perfScript.myRendered[index ++] = ANuclear;
			ANuclear.transform.parent = nNuclear.transform;
		}





		
		
		
		for(i = 0; i < treeQt; i++)
		{
			GameObject ATree = Instantiate(TreePrefab, Vector3.zero, Quaternion.identity);
			ATree.name = "Tree";
			float scale = Random.Range(2.0f, 2.5f);
			ATree.transform.Find("Body").localScale = new Vector3(scale, Random.Range(2.5f, 3.5f), scale);
			perfScript.myRendered[index ++] = ATree;
			ATree.transform.parent = nTree.transform;
		}





		
		
		
		for(i = 0; i < casterQt; i++)
		{
			GameObject ACasterBlaster = Instantiate(CasterBlasterPrefab, Vector3.zero, Quaternion.identity);
			ACasterBlaster.name = "CasterBlaster";
			perfScript.myRendered[index ++] = ACasterBlaster;
			ACasterBlaster.transform.parent = nCasterBlaster.transform;
		}

		Done = true;
	}
}