using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SnakeAutoAI", menuName = "Scriptable Object/AI/SnakeAutoAI", order = 3)]
public class SnakeAutoAI : UtilityAIBehaviour<SnakeAutoAI>
{
	public UtilityAIBehaviour Launch(SnakeAutoCharacter snakeAuto)
	{
		SnakeAutoController sacr = new SnakeAutoController(snakeAuto);
		this.AddController(sacr);
	
		return this;
	}
}
