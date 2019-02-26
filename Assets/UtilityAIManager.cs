using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAIManager : Singleton<UtilityAIManager>
{
	public UtilityAIBehaviour[] behaviours;

	void Awake()
	{
		instance = this;

		foreach(UtilityAIBehaviour b in this.behaviours) {
			b.Initialize();
		}
	}

	void Start()
	{
		foreach(UtilityAIBehaviour b in this.behaviours) {
			b.Start();
		}
	}

	void Update()
	{
		foreach(UtilityAIBehaviour b in this.behaviours) {

			if(Time.time - b.lastUpdate < b.updateRate) {
				continue;
			}

			b.lastUpdate = Time.time;

			b.UpdateUtilityActions(this);
		}
	}
}
