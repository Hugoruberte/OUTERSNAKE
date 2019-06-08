using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

public class SnakeAutoController : MovementController
{
	public SnakeAutoCharacter snakeAuto { get; private set; }

	private Transform myTransform;


	public SnakeAutoController(SnakeAutoCharacter s) : base(s)
	{
		this.snakeAuto = s;
		this.myTransform = s.myTransform;
	}
}
