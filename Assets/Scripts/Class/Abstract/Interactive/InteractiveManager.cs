using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactive.Engine;

public class InteractiveManager : MonoSingleton<InteractiveManager>
{
	[Header("Interactive Engine")]
	[SerializeField] private InteractiveEngine engine = null;

	protected override void Awake()
	{
		base.Awake();

		this.engine.OnAwake();
	}
}
