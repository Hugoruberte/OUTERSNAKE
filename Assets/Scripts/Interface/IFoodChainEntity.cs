


public interface IFoodChainEntity
{
	int foodChainRank { get; }
	float foodChainValue { get; }

	void FoodChainInteraction(float value);
}
