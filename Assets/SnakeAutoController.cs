using UnityEngine;

public class SnakeAutoController : MovementController
{
	public SnakeAutoCharacter snakeAuto { get; private set; }

	private readonly Transform myTransform;


	public SnakeAutoController(SnakeAutoCharacter s) : base(s)
	{
		this.snakeAuto = s;
		this.myTransform = s.myTransform;
	}
}
