using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerTrapController : MovementController
{
	protected LazerTrap lazer;

	public LazerTrapController(LazerTrap l)
	{
		this.lazer = l;
	}
}
