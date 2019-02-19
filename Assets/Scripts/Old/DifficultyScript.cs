using UnityEngine;
using System.Collections;

public class DifficultyScript : MonoBehaviour
{
	[Header("Difficulty")]
	[Range(0, 100)]
	public int EditorDifficulty = 0;
	private int difficulty = 0;
	public int Difficulty
	{
		get
		{
			return difficulty;
		}
		set
		{
			difficulty = value;
			EditorDifficulty = value;
		}
	}

	private GameManagerV1 gameManager;
	private CasterBlasterCreator casterblasterCreator;
	private SawCreator sawCreator;
	private MeteoreCreator meteoreCreator;
	private SuperLazerCreator suplazerCreator;

	[Header("Threshold")]
	[Range(0, 100)]
	public int lazerThreshold = 0;
	[Range(0, 100)]
	public int sawThreshold = 0;
	[Range(0, 100)]
	public int rabbitBomberThreshold = 0;
	[Range(0, 100)]
	public int superLazerThreshold = 0;
	[Range(0, 100)]
	public int nuclearThreshold = 0;
	[Range(0, 100)]
	public int redRabbitThreshold = 0;
	[Range(0, 100)]
	public int casterBlasterThreshold = 0;
	[Range(0, 100)]
	public int meteoreThreshold = 0;


	void Awake()
	{
		gameManager = GetComponent<GameManagerV1>();

		Transform Creator = transform.Find("Creator");

		casterblasterCreator = Creator.GetComponent<CasterBlasterCreator>();
		sawCreator = Creator.GetComponent<SawCreator>();
		meteoreCreator = Creator.GetComponent<MeteoreCreator>();
		suplazerCreator = Creator.GetComponent<SuperLazerCreator>();
	}

	void Start()
	{
		Difficulty = EditorDifficulty;
	}

	private void DifficultySetting()
	{
		if(difficulty < 10)
		{
			if(suplazerCreator)
			{
				suplazerCreator.MinTime = 10.0f;
				suplazerCreator.MaxTime = 15.0f;
			}
			if(casterblasterCreator)
			{
				casterblasterCreator.MinTime = 3.0f;
				casterblasterCreator.MaxTime = 10.0f;
			}
			if(sawCreator)
			{
				sawCreator.MinTime = 2.5f;
				sawCreator.MaxTime = 10.0f;
			}
			if(meteoreCreator)
			{
				meteoreCreator.MinTime = 5.0f;
				meteoreCreator.MaxTime = 7.5f;
			}
			
			gameManager.WorldSetting.AppleAmount = 100;
			gameManager.WorldSetting.RottenApplePercent = 0.1f;
			gameManager.WorldSetting.RabbitAmount = 12;
			gameManager.WorldSetting.TreeAmountPerFace = 6;
			gameManager.WorldSetting.NuclearAmount = 2;
			gameManager.WorldSetting.RedRabbitAmount = 3;
			gameManager.WorldSetting.LazerDamagePerSecond = 1;
			gameManager.WorldSetting.LazerLoading = 2.5f;
			gameManager.WorldSetting.SawSpeed = 12;
			gameManager.WorldSetting.NuclearDelay = 5.0f;
			gameManager.WorldSetting.RedRabbitDelay = 2.0f;
			gameManager.WorldSetting.RedRabbitAccuracy = 0.2f;
			gameManager.WorldSetting.MissileDamage = 2;
			gameManager.WorldSetting.RabbitBombProb = 0;
			gameManager.WorldSetting.RabbitEnergy = 3;
			gameManager.WorldSetting.MeteoreSpeed = 25.0f;
			gameManager.WorldSetting.MeteoreHeight = 35;
			gameManager.WorldSetting.CasterBlasterDamage = 3;
			gameManager.WorldSetting.SuperLazerDuration = 2.0f;
			gameManager.WorldSetting.SuperLazerLoading = 2.5f;
		}
		else if(difficulty < 30)
		{
			if(suplazerCreator)
			{
				suplazerCreator.MinTime = 10.0f;
				suplazerCreator.MaxTime = 15.0f;
			}
			if(casterblasterCreator)
			{
				casterblasterCreator.MinTime = 3.0f;
				casterblasterCreator.MaxTime = 10.0f;
			}
			if(sawCreator)
			{
				sawCreator.MinTime = 4.0f;
				sawCreator.MaxTime = 10.0f;
			}
			if(meteoreCreator)
			{
				meteoreCreator.MinTime = 5.0f;
				meteoreCreator.MaxTime = 7.5f;
			}

			gameManager.WorldSetting.AppleAmount = 75;
			gameManager.WorldSetting.RottenApplePercent = 0.1f;
			gameManager.WorldSetting.RabbitAmount = 6;
			gameManager.WorldSetting.TreeAmountPerFace = 5;
			gameManager.WorldSetting.NuclearAmount = 2;
			gameManager.WorldSetting.RedRabbitAmount = 3;
			gameManager.WorldSetting.LazerDamagePerSecond = 1;
			gameManager.WorldSetting.LazerLoading = 2.5f;
			gameManager.WorldSetting.SawSpeed = 12;
			gameManager.WorldSetting.NuclearDelay = 4.0f;
			gameManager.WorldSetting.RedRabbitDelay = 2.0f;
			gameManager.WorldSetting.RedRabbitAccuracy = 0.25f;
			gameManager.WorldSetting.MissileDamage = 2;
			gameManager.WorldSetting.RabbitBombProb = 0;
			gameManager.WorldSetting.RabbitEnergy = 3;
			gameManager.WorldSetting.MeteoreSpeed = 25.0f;
			gameManager.WorldSetting.MeteoreHeight = 35;
			gameManager.WorldSetting.CasterBlasterDamage = 3;
			gameManager.WorldSetting.SuperLazerDuration = 1.5f;
			gameManager.WorldSetting.SuperLazerLoading = 2f;
		}
		else if(difficulty < 50)
		{
			if(suplazerCreator)
			{
				suplazerCreator.MinTime = 8.0f;
				suplazerCreator.MaxTime = 12.0f;
			}
			if(casterblasterCreator)
			{
				casterblasterCreator.MinTime = 3.0f;
				casterblasterCreator.MaxTime = 7.5f;
			}
			if(sawCreator)
			{
				sawCreator.MinTime = 2.5f;
				sawCreator.MaxTime = 7.0f;
			}
			if(meteoreCreator)
			{
				meteoreCreator.MinTime = 5.0f;
				meteoreCreator.MaxTime = 6.0f;
			}

			gameManager.WorldSetting.AppleAmount = 70;
			gameManager.WorldSetting.RottenApplePercent = 0.5f;
			gameManager.WorldSetting.RabbitAmount = 5;
			gameManager.WorldSetting.TreeAmountPerFace = 4;
			gameManager.WorldSetting.NuclearAmount = 3;
			gameManager.WorldSetting.RedRabbitAmount = 3;
			gameManager.WorldSetting.LazerDamagePerSecond = 1.5f;
			gameManager.WorldSetting.LazerLoading = 2f;
			gameManager.WorldSetting.SawSpeed = 13;
			gameManager.WorldSetting.NuclearDelay = 3.5f;
			gameManager.WorldSetting.RedRabbitDelay = 1.5f;
			gameManager.WorldSetting.RedRabbitAccuracy = 0.5f;
			gameManager.WorldSetting.MissileDamage = 2;
			gameManager.WorldSetting.RabbitBombProb = 2;
			gameManager.WorldSetting.RabbitEnergy = 4;
			gameManager.WorldSetting.MeteoreSpeed = 25.0f;
			gameManager.WorldSetting.MeteoreHeight = 35;
			gameManager.WorldSetting.CasterBlasterDamage = 3;
			gameManager.WorldSetting.SuperLazerDuration = 1f;
			gameManager.WorldSetting.SuperLazerLoading = 1.5f;
		}
		else if(difficulty < 60)
		{
			if(suplazerCreator)
			{
				suplazerCreator.MinTime = 7.0f;
				suplazerCreator.MaxTime = 10.0f;
			}
			if(casterblasterCreator)
			{
				casterblasterCreator.MinTime = 3.0f;
				casterblasterCreator.MaxTime = 5.0f;
			}
			if(sawCreator)
			{
				sawCreator.MinTime = 2.0f;
				sawCreator.MaxTime = 8.0f;
			}
			if(meteoreCreator)
			{
				meteoreCreator.MinTime = 4.0f;
				meteoreCreator.MaxTime = 7.0f;
			}

			gameManager.WorldSetting.AppleAmount = 50;
			gameManager.WorldSetting.RottenApplePercent = 0.75f;
			gameManager.WorldSetting.RabbitAmount = 4;
			gameManager.WorldSetting.TreeAmountPerFace = 3;
			gameManager.WorldSetting.NuclearAmount = 4;
			gameManager.WorldSetting.RedRabbitAmount = 4;
			gameManager.WorldSetting.LazerDamagePerSecond = 2f;
			gameManager.WorldSetting.LazerLoading = 2f;
			gameManager.WorldSetting.SawSpeed = 14;
			gameManager.WorldSetting.NuclearDelay = 3.0f;
			gameManager.WorldSetting.RedRabbitDelay = 1.0f;
			gameManager.WorldSetting.RedRabbitAccuracy = 0.75f;
			gameManager.WorldSetting.MissileDamage = 2;
			gameManager.WorldSetting.RabbitBombProb = 5;
			gameManager.WorldSetting.RabbitEnergy = 5;
			gameManager.WorldSetting.MeteoreSpeed = 25.0f;
			gameManager.WorldSetting.MeteoreHeight = 35;
			gameManager.WorldSetting.CasterBlasterDamage = 3;
			gameManager.WorldSetting.SuperLazerDuration = 0.75f;
			gameManager.WorldSetting.SuperLazerLoading = 1.25f;
		}
		else if(difficulty < 80)
		{
			if(suplazerCreator)
			{
				suplazerCreator.MinTime = 5.0f;
				suplazerCreator.MaxTime = 7.0f;
			}
			if(casterblasterCreator)
			{
				casterblasterCreator.MinTime = 1.5f;
				casterblasterCreator.MaxTime = 2.0f;
			}
			if(sawCreator)
			{
				sawCreator.MinTime = 2.5f;
				sawCreator.MaxTime = 5.0f;
			}
			if(meteoreCreator)
			{
				meteoreCreator.MinTime = 2.0f;
				meteoreCreator.MaxTime = 5.0f;
			}

			gameManager.WorldSetting.AppleAmount = 30;
			gameManager.WorldSetting.RottenApplePercent = 0.5f;
			gameManager.WorldSetting.RabbitAmount = 4;
			gameManager.WorldSetting.TreeAmountPerFace = 2;
			gameManager.WorldSetting.NuclearAmount = 4;
			gameManager.WorldSetting.RedRabbitAmount = 3;
			gameManager.WorldSetting.LazerDamagePerSecond = 3f;
			gameManager.WorldSetting.LazerLoading = 1f;
			gameManager.WorldSetting.SawSpeed = 15;
			gameManager.WorldSetting.NuclearDelay = 2.5f;
			gameManager.WorldSetting.RedRabbitDelay = 0.75f;
			gameManager.WorldSetting.RedRabbitAccuracy = 0.8f;
			gameManager.WorldSetting.MissileDamage = 2;
			gameManager.WorldSetting.RabbitBombProb = 6;
			gameManager.WorldSetting.RabbitEnergy = 6;
			gameManager.WorldSetting.MeteoreSpeed = 30.0f;
			gameManager.WorldSetting.MeteoreHeight = 35;
			gameManager.WorldSetting.CasterBlasterDamage = 4;
			gameManager.WorldSetting.SuperLazerDuration = 0.5f;
			gameManager.WorldSetting.SuperLazerLoading = 1f;
		}
		else
		{
			if(suplazerCreator)
			{
				suplazerCreator.MinTime = 5.0f;
				suplazerCreator.MaxTime = 6.0f;
			}
			if(casterblasterCreator)
			{
				casterblasterCreator.MinTime = 0.5f;
				casterblasterCreator.MaxTime = 1.0f;
			}
			if(sawCreator)
			{
				sawCreator.MinTime = 0.5f;
				sawCreator.MaxTime = 2.0f;
			}
			if(meteoreCreator)
			{
				meteoreCreator.MinTime = 1.0f;
				meteoreCreator.MaxTime = 2.5f;
			}

			gameManager.WorldSetting.AppleAmount = 40;
			gameManager.WorldSetting.RottenApplePercent = 1f;
			gameManager.WorldSetting.RabbitAmount = 3;
			gameManager.WorldSetting.TreeAmountPerFace = 2;
			gameManager.WorldSetting.NuclearAmount = 6;
			gameManager.WorldSetting.RedRabbitAmount = 6;
			gameManager.WorldSetting.LazerDamagePerSecond = 4f;
			gameManager.WorldSetting.LazerLoading = 0.5f;
			gameManager.WorldSetting.SawSpeed = 15;
			gameManager.WorldSetting.NuclearDelay = 1.5f;
			gameManager.WorldSetting.RedRabbitDelay = 0.5f;
			gameManager.WorldSetting.RedRabbitAccuracy = 1f;
			gameManager.WorldSetting.MissileDamage = 2;
			gameManager.WorldSetting.RabbitBombProb = 8;
			gameManager.WorldSetting.RabbitEnergy = 8;
			gameManager.WorldSetting.MeteoreSpeed = 35.0f;
			gameManager.WorldSetting.MeteoreHeight = 35;
			gameManager.WorldSetting.CasterBlasterDamage = 5;
			gameManager.WorldSetting.SuperLazerDuration = 0.25f;
			gameManager.WorldSetting.SuperLazerLoading = 0.75f;
		}
	}
}