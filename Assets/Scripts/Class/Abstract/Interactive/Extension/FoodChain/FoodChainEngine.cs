

namespace Interactive.Engine
{
	public class FoodChainEngine : InteractiveExtensionEngine
	{
		public override void InteractionBetween(InteractiveEntity main, InteractiveEntity other)
		{
			if(main is IFoodChainEntity m && other is IFoodChainEntity o) {

				float value = 0;

				// if my rank > other rank -> eat it : add its food chain value
				// if my rank < other rank -> get eat : remove its food chain value
				if(m.foodChainRank > o.foodChainRank) {
					value = o.foodChainValue;
				} else if(m.foodChainRank < o.foodChainRank) {
					value = -o.foodChainValue;
				}

				m.FoodChainInteraction(value);
			}
		}
	}
}

