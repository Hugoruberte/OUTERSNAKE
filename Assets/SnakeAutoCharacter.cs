﻿public class SnakeAutoCharacter : SnakeCharacter
{
	private SnakeController ctr;

	protected override void Awake()
	{
		base.Awake();

		this.ctr = this.GetComponent<SnakeController>();
	}

	protected override void Start()
	{
		base.Start();

		this.data.onCancelInput += this.OnCancelInput;
		this.ctr.SetCancelInput(true);

		// Initialize AI behaviour (this will launch the AI)
		// this.behaviour = SnakeAutoAI.instance.Launch(this);
	}

	private void OnCancelInput(bool value)
	{
		if(!value) {
			this.ctr.SetCancelInput(true);
		}
	}
}
