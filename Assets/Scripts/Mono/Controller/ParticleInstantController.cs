using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Tools;

public class ParticleInstantController : ParticleEffect
{
	private WaitWhile wait = null;
	private WaitForSeconds delay = Yielders.Wait(0.5f);
	private IEnumerator lifeCoroutine = null;

	[Header("Events")]
	public UnityEvent onAwake = new UnityEvent();
	public UnityEvent onPlay = new UnityEvent();
	public UnityEvent onEnd = new UnityEvent();


	public override void Launch()
	{
		this.onAwake.Invoke();

		base.Launch();

		this.onPlay.Invoke();

		this.StartAndStopCoroutine(ref this.lifeCoroutine, this.LifetimeCoroutine());
	}

	private IEnumerator LifetimeCoroutine()
	{
		if(wait == null) {
			wait = new WaitWhile(() => this.main.IsAlive(true));
		}

		yield return wait;
		yield return delay;

		this.onEnd.Invoke();

		// stow
		this.poolingManager.Stow(this);
	}
}
