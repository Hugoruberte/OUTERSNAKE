using UnityEngine;
using My.Events;

namespace Snakes
{
	public class SnakeMovementEvents
	{
		public ActionEvent onStartStep;
		public ActionEvent onEndStep;
		public Vector3Vector3Event onStartStepTo;
		public Vector3Event onEndStepTo;
	}
}