using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAIManager : MonoSingleton<UtilityAIManager>
{
	[Header("Utility AI Behaviours")]
	[SerializeField] private UtilityAIBehaviour[] behaviours = null;

	protected override void Awake()
	{
		base.Awake();
		
		foreach(UtilityAIBehaviour b in this.behaviours) {
			b.OnAwake();
		}
	}

	private void Start()
	{
		foreach(UtilityAIBehaviour b in this.behaviours) {
			b.OnStart();
		}
	}

	private void Update()
	{
		foreach(UtilityAIBehaviour b in this.behaviours) {
			b.OnUpdate();

			if(Time.time - b.lastUpdate < b.updateRate) {
				continue;
			}

			b.lastUpdate = Time.time;

			b.UpdateUtilityActions();
		}
	}
}
