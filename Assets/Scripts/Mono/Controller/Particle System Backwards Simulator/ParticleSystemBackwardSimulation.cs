using System.Collections;
using UnityEngine;
using My.Tools;

public class ParticleSystemBackwardSimulation : MonoBehaviour
{
	private ParticleSystem particle;
	private IEnumerator playCoroutine = null;


	[HideInInspector] public bool startFromEnd = true;
	[HideInInspector] public float startTime = 2.0f;
	[HideInInspector] public float simulationSpeedScale = 1.0f;


	private void Awake()
	{
		this.particle = GetComponentInChildren<ParticleSystem>();

		if(!this.particle.proceduralSimulationSupported) {
			Debug.LogWarning("WARNING : This particle does not support procedural simulation and thus its rewinding will have weird behaviour.");
		}
	}

	public void PlayBackwards()
	{
		float st = (this.startFromEnd) ? this.particle.GetRealDuration() : this.startTime;

		// Fast-forwards to 'st' then pauses particle system
		this.particle.Simulate(st, true, true, true);

		this.StartAndStopCoroutine(ref this.playCoroutine, this.PlayBackwardsCoroutine(st));
	}

	public void Stop()
	{
		this.TryStopCoroutine(ref this.playCoroutine);
		this.particle.Stop(true);
	}






	

	private IEnumerator PlayBackwardsCoroutine(float st)
	{
		ParticleSystem.MainModule main;
		float clock;

		main = this.particle.main;
		clock = st;

		while(clock > 0f)
		{
			// Clear every particles
			this.particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

			// Unpause
			this.particle.Play(true);

			// Calculate next simulation time + simulate (without children)
			clock -= this.GetUsedDeltaTime(main) * main.simulationSpeed * this.simulationSpeedScale;
			this.particle.Simulate(clock, true, false, true);

			yield return Yielders.fixedUpdate;
		}
		
		this.particle.Play(true);
		this.particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	private float GetUsedDeltaTime(ParticleSystem.MainModule pm) => (pm.useUnscaledTime) ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
}