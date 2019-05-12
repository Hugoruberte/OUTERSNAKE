using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snakes;
using Interactive.Engine;
using My.Tools;


public class SnakePartCharacter : SnakeEntity
{
	private BoxCollider myCollider;
	private IEnumerator behaviourCoroutine = null;
	private ParticleSystem explosion;

	private static float BOXCOLLIDER_STANDARD_SIZE_VALUE = 1f;
	
	public SnakePartState snakePartState { get; private set; }



	protected override void Awake()
	{
		base.Awake();

		this.snakePartState = SnakePartState.Alive;
		this.myCollider = GetComponent<BoxCollider>();
		this.explosion = GetComponentInChildren<ParticleSystem>();

		BOXCOLLIDER_STANDARD_SIZE_VALUE = this.myCollider.size.x;
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
		this.myTransform.SetAsFirstSibling();

		this.body.gameObject.SetActive(true);
		this.body.localScale = Vector3Extension.ONE;
		this.myCollider.enabled = true;
		this.myCollider.size = Vector3Extension.ONE * BOXCOLLIDER_STANDARD_SIZE_VALUE;

		this.snakePartState = SnakePartState.Alive;
	}

	public void SetupSnakePart()
	{
		this.StopAllCoroutines();

		this.InitializeSnakePart();
		
		this.cellable.UpdateCurrentCell(this.myTransform.position, this.myTransform.up);
	}

	public void ReduceSnakePart(float speed)
	{
		if(this.snakePartState != SnakePartState.Alive) {
			return;
		}

		this.snakePartState = SnakePartState.Reduce;

		this.StopAllCoroutines();
		this.StartAndStopCoroutine(ref this.behaviourCoroutine, this.ReduceSnakePartCoroutine(speed));
	}

	private IEnumerator ReduceSnakePartCoroutine(float speed)
	{
		Vector3 from, to;
		Vector3 cfrom;
		float step;

		from = this.body.localScale;
		cfrom = this.myCollider.size;
		to = Vector3Extension.ZERO;
		step = 0f;

		while(step < 0.75f)
		{
			this.body.localScale = Vector3.Lerp(from, to, step);
			this.myCollider.size = Vector3.Lerp(cfrom, to, step);
			step += speed * Time.deltaTime;
			yield return null;
		}

		this.myCollider.enabled = false;

		while(step < 1f)
		{
			this.body.localScale = Vector3.Lerp(from, to, step);
			step += speed * Time.deltaTime;
			yield return null;
		}

		this.body.localScale = to;
		this.snakePartState = SnakePartState.Reusable;
		this.behaviourCoroutine = null;
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
		if(this.snakePartState != SnakePartState.Alive && this.snakePartState != SnakePartState.Reduce) {
			return;
		}

		this.snakePartState = SnakePartState.Explode;
		this.StopAllCoroutines();
		SnakeBodyManager.instance.ExplodeFromSnakePart(this.myTransform);
	}

	// Only called from SnakeBodyController, do not called it from anywhere else !
	public void Explosion()
	{
		if(this.snakePartState != SnakePartState.Reusable && this.snakePartState != SnakePartState.Dead)
		{
			this.snakePartState = SnakePartState.Explode;

			this.body.gameObject.SetActive(false);
			this.myCollider.enabled = false;
			this.explosion.Play();

			this.StopAllCoroutines();
			this.StartAndStopCoroutine(ref this.behaviourCoroutine, this.ExplosionCoroutine());
		}
		else
		{
			this.Death();
		}
	}

	private IEnumerator ExplosionCoroutine()
	{
		yield return new WaitWhile(() => this.explosion.IsAlive());

		this.Death();
	}

	protected override void Death()
	{
		base.Death();

		this.snakePartState = SnakePartState.Dead;
		GarbageManager.instance.PutInGarbage(gameObject);
	}
}


