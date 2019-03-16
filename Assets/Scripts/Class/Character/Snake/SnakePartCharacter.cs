using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snakes;
using Interactive.Engine;


public class SnakePartCharacter : SnakeEntity
{
	private SnakeBodyManager snakeBodyManager;

	private IEnumerator behaviourCoroutine = null;
	private ParticleSystem explosion;
	
	public SnakePartState snakePartState { get; private set; }



	protected override void Awake()
	{
		base.Awake();

		snakePartState = SnakePartState.Alive;

		explosion = GetComponentInChildren<ParticleSystem>();
	}

	protected override void Start()
	{
		base.Start();

		snakeBodyManager = SnakeBodyManager.instance;
	}







	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ------------------------------------ INTERACT FUNCTIONS -------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public override void InteractWith(InteractiveStatus s, PhysicalInteractionEntity i)
	{
		// to do
	}












	

	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ----------------------------- NORMAL BIRTH AND REUSE FUNCTIONS ------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public void InitializeSnakePart()
	{
		myTransform.SetAsFirstSibling();

		body.gameObject.SetActive(true);
		body.localScale = Vector3.one;

		snakePartState = SnakePartState.Alive;
	}

	public void SetupSnakePart()
	{
		StopAllCoroutines();

		InitializeSnakePart();
		
		this.cellable.UpdateCurrentCell(myTransform.position, myTransform.up);
	}

	public void ReduceSnakePart(float speed)
	{
		if(snakePartState != SnakePartState.Alive) {
			return;
		}

		snakePartState = SnakePartState.Reduce;

		StopAllCoroutines();
		behaviourCoroutine = ReduceSnakePartCoroutine(speed);
		StartCoroutine(behaviourCoroutine);
	}

	private IEnumerator ReduceSnakePartCoroutine(float speed)
	{
		Vector3 from = body.localScale;
		Vector3 to = Vector3.zero;
		float step = 0f;

		while(step < 1f)
		{
			body.localScale = Vector3.Lerp(from, to, step);
			step += speed * Time.deltaTime;
			yield return null;
		}

		body.localScale = to;
		snakePartState = SnakePartState.Reusable;
		behaviourCoroutine = null;
	}








	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ------------------------------------ EXPLOSION FUNCTION -------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	// Use this function to make a snake part explode and so explode the snake part behind it
	private void MakeThisSnakePartExplode()
	{
		// Can explode only if in Alive snakePartState or Reduce snakePartState
		if(snakePartState != SnakePartState.Alive && snakePartState != SnakePartState.Reduce) {
			return;
		}

		snakePartState = SnakePartState.Explode;
		StopAllCoroutines();
		snakeBodyManager.ExplodeFromSnakePart(myTransform);
	}

	// Only called from SnakeBodyController, do not called it from anywhere else !
	public void Explosion()
	{
		if(snakePartState != SnakePartState.Reusable && snakePartState != SnakePartState.Dead)
		{
			snakePartState = SnakePartState.Explode;

			body.gameObject.SetActive(false);
			myCollider.enabled = false;
			explosion.Play();

			StopAllCoroutines();
			behaviourCoroutine = ExplosionCoroutine();
			StartCoroutine(behaviourCoroutine);
		}
		else
		{
			this.Death();
		}
	}

	private IEnumerator ExplosionCoroutine()
	{
		yield return new WaitWhile(() => explosion.IsAlive());

		this.Death();
	}

	protected override void Death()
	{
		base.Death();

		snakePartState = SnakePartState.Dead;
		GameManager.PutInGarbage(gameObject);
	}
}


