﻿using UnityEngine;

public class UtilityAIManager : MonoSingleton<UtilityAIManager>
{
	[Header("Utility AI Behaviours")]
	[SerializeField] private UtilityAIBehaviour[] behaviours = null;

	protected override void Awake()
	{
		base.Awake();
		
		foreach(UtilityAIBehaviour b in this.behaviours)
		{
			b.OnAwake();
		}
	}

	private void Start()
	{
		foreach(UtilityAIBehaviour b in this.behaviours)
		{
			b.OnStart();
		}
	}

	private void Update()
	{
		foreach(UtilityAIBehaviour b in this.behaviours)
		{
			b.OnUpdate();

			if(Time.time - b.lastUpdate < b.updateRate) {
				continue;
			}

			b.lastUpdate = Time.time;

			b.UpdateUtilityActions();
		}
	}

	public T Get<T>() where T : class
	{
		UtilityAIBehaviour b;

		for(int i = 0; i < this.behaviours.Length; ++i)
		{
			b = this.behaviours[i];

			if(b is T) {
				return b as T;
			}
		}

		return null;
	}
}
