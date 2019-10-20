using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SnakeController), typeof(SnakeAutoCharacter))]
public class SnakeAutoWanderController : MonoBehaviour
{
	private SnakeController ctr;

	[Header("Parameters")]
	[SerializeField, Range(0.25f, 10f)] private float interval = 1f;
	[SerializeField, Range(0f, 2f)] private float intervalOffset = 0f;

	private void Awake()
	{
		this.ctr = this.GetComponent<SnakeController>();
	}

	private void Start()
	{
		this.StartCoroutine(this.AutoWanderCoroutine());
	}

	private IEnumerator AutoWanderCoroutine()
	{
		yield return Yielders.Wait(1f);

		while(true)
		{
			this.ctr.Turn(Random.value > 0.5f);

			yield return Yielders.Wait(this.interval + Random.Range(-this.intervalOffset, this.intervalOffset));
		}
	}
}
