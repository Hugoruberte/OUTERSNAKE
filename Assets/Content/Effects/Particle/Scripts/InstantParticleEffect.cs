using System.Collections;
using UnityEngine;
using My.Tools;
using My.Events;

public class InstantParticleEffect : ParticleEffect
{
	private WaitWhile wait = null;
	private WaitForSeconds delay = Yielders.Wait(0.5f);
	private IEnumerator lifeCoroutine = null;
	private float durationScale = 1f;
	private bool isDurationDirty = false;

	public ActionEvent onAwake;
	public ActionEvent onPlay;
	public ActionEvent onEnd;

	[HideInInspector] public bool isDone;


	public override void Launch()
	{
		this.isDone = false;
		this.onAwake?.Invoke();

		base.Launch();

		this.onPlay?.Invoke();

		this.StartAndStopCoroutine(ref this.lifeCoroutine, this.LifetimeCoroutine());
	}

	private IEnumerator LifetimeCoroutine()
	{
		yield return Yielders.Until(this.wait, () => this.main.IsAlive(true));
		yield return delay;

		this.onEnd?.Invoke();
		this.isDone = true;

		// stow
		PoolingManager.instance.Stow(this);
	}

	public void SetDuration(float value)
	{
		this.durationScale = this.main.GetRealDuration() / value;

		this.ApplySimulationScale(this.durationScale);
		this.isDurationDirty = true;
	}

	public override void Reset()
	{
		if(this.isDurationDirty) {
			this.ApplySimulationScale(1f / this.durationScale);
			this.isDurationDirty = false;
		}

		base.Reset();
	}

	private void ApplySimulationScale(float value)
	{
		ParticleSystem.MainModule module;

		foreach(ParticleSystem p in this.all) {
			module = p.main;
			module.simulationSpeed *= value;
		}
	}
}
