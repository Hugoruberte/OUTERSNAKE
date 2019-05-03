using UnityEngine;
using UnityEngine.Events;

namespace Snakes
{
	public class SnakeMovementEvents
	{
		public class StepDestinationEvent : UnityEvent<Vector3, Vector3> { }
		public class StepDepartureEvent : UnityEvent<Vector3> { }

		public UnityEvent onStartStep;
		public UnityEvent onEndStep;
		public StepDestinationEvent onStartStepTo;
		public StepDepartureEvent onEndStepTo;

		public SnakeMovementEvents()
		{
			this.onStartStep = new UnityEvent();
			this.onEndStep = new UnityEvent();

			this.onStartStepTo = new StepDestinationEvent();
			this.onEndStepTo = new StepDepartureEvent();
		}
	}
}