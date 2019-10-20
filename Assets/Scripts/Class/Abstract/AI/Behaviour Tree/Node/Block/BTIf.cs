using BehaviourTreeAI;

public class BTIf : BTBlock
{
	protected System.Func<bool> fn;
	
	public BTIf(System.Func<bool> fn) : base()
	{
		this.fn = fn;
	}

	public sealed override BTState Tick()
	{
		return this.fn() ? BTState.Success : BTState.Failure;
	}
}