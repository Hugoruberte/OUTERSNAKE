using UnityEngine;
using UnityEngine.Events;

namespace Snakes
{
	public class SnakeMovementEvents
	{
		public class StepDestinationEvent : UnityEvent<Vector3, Vector3> { }

		public StepDestinationEvent onStartStep;
		public UnityEvent onEndStep;

		public SnakeMovementEvents()
		{
			this.onStartStep = new StepDestinationEvent();
			this.onEndStep = new UnityEvent();
		}
	}
}