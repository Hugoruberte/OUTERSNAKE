using BehaviourTreeAI;

public class WhiteRabbitBehaviour : BehaviourTree
{
	// public WhiteRabbitBehaviour(WhiteRabbit wr) : base()
	// {
	// 	// IMPORTANT : base is done before this
	// 	this.root = Behaviour(wr);
	// }

	// /*private BTNode Behaviour(WhiteRabbit wr)
	// {
	// 	BTNode root = Selector(

	// 		// if player is nearby
	// 		If(wr.SnakeIsNearby).Selector(
	// 			// and if rabbit is scared
	// 			If(wr.RabbitIsScared).Sequence(
	// 				// then
	// 				Do(wr.RunAway)
	// 			),

	// 			// else
	// 			Wander(wr)
	// 		),

	// 		// else if rabbit is hungry
	// 		If(wr.RabbitIsHungry).Sequence(
	// 			Eat(wr)
	// 		),

	// 		// else
	// 		Wander(wr)
	// 	);

	// 	return root;
	// }*/

	// private BTNode Behaviour(WhiteRabbit wr)
	// {
	// 	BTNode root = Selector(

	// 		// if rabbit is scared
	// 		If(wr.RabbitIsScared).Selector(
	// 			// and if player is nearby
	// 			If(wr.SnakeIsNearby).Sequence(
	// 				// then
	// 				Do(wr.RunAway)
	// 			),

	// 			// else
	// 			Wander(wr)
	// 		),

	// 		// else if rabbit is hungry
	// 		If(wr.RabbitIsHungry).Sequence(
	// 			Eat(wr)
	// 		),

	// 		// else
	// 		Wander(wr)
	// 	);

	// 	return root;
	// }


	// private BTNode Wander(WhiteRabbit wr)
	// {
	// 	BTNode root = Sequence(

	// 		Do(wr.Wander),
	// 		// this allow to cancel the temporization
	// 		// if the rabbit is in danger
	// 		If(wr.RabbitIsOkay).Sequence(Do(wr.Temporize))
	// 	);

	// 	return root;
	// }

	// private BTNode Eat(WhiteRabbit wr)
	// {
	// 	BTNode root = Sequence(

	// 		Test(wr.SearchForNearestFood),
	// 		Do(wr.StepToFood),
	// 		Do(wr.EatFood)
	// 	);

	// 	return root;
	// }
}