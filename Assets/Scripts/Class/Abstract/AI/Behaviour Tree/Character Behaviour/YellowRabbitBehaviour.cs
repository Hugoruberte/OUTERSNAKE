using BehaviourTreeAI;

public class YellowRabbitBehaviour : BehaviourTree
{
	public YellowRabbitBehaviour(YellowRabbit yr) : base()
	{
		// base is done before this
		this.root = Behaviour(yr);
	}

	private BTNode Behaviour(YellowRabbit yr)
	{
		BTNode root = Selector();

		return root;
	}
}