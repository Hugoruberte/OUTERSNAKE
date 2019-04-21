using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class ParticleInstantController : ParticleEffect
{
	private WaitWhile wait = null;
	private WaitForSeconds delay = Yielders.Wait(0.5f);
	private IEnumerator lifeCoroutine = null;


	public override void Launch()
	{
		base.Launch();

		this.StartAndStopCoroutine(ref this.lifeCoroutine, this.LifetimeCoroutine());
	}

	private IEnumerator LifetimeCoroutine()
	{
		if(wait == null) {
			wait = new WaitWhile(() => this.main.IsAlive(true));
		}

		yield return wait;
		yield return delay;

		// stow
		this.poolingManager.Stow(this);
	}
}
