using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

public abstract class ParticleEffect : Effect
{
	public ParticleSystem main { get; private set; }
	protected ParticleSystem[] all;

	protected override private void Awake()
	{
		base.Awake();

		this.main = this.GetComponentInChildrenWithName<ParticleSystem>("Effect");
		this.all = this.GetComponentsInChildren<ParticleSystem>();

		if(!this.main) {
			Debug.LogWarning("WARNING : This ParticleEffect doesn't have any Particle System named \"Effect\".");
		}
	}


	public override void SetColor(Color c)
	{
		base.SetColor(c);
		
		ParticleSystem.MainModule m;
		ParticleSystem.ColorBySpeedModule cs;
		ParticleSystem.ColorOverLifetimeModule cl;
		ParticleSystem.TrailModule t;

		foreach(ParticleSystem p in all) {

			m = p.main;
			m.startColor = new Color(c.r, c.g, c.b, m.startColor.color.a);

			cs = p.colorBySpeed;
			if(cs.enabled) {
				cs.color = this.GetAlphaMatchingColor(cs.color, c);
			}
			
			cl = p.colorOverLifetime;
			if(cl.enabled) {
				cl.color = this.GetAlphaMatchingColor(cl.color, c);
			}
			
			t = p.trails;
			if(t.enabled) {
				t.colorOverLifetime = this.GetAlphaMatchingColor(t.colorOverLifetime, c);
				t.colorOverTrail = this.GetAlphaMatchingColor(t.colorOverTrail, c);
			}
		}
	}
	public void SetSimulationSpeedScale(float value)
	{
		ParticleSystem.MainModule main;

		foreach(ParticleSystem p in this.all)
		{
			main = p.main;
			main.simulationSpeed *= value;
		}
	}
	
	public virtual void Initialize(Vector3 position, Vector3 direction, Color color, float simulationSpeedScale)
	{
		this.SetOrientation(position, direction);
		this.SetColor(color);
		this.SetSimulationSpeedScale(simulationSpeedScale);
	}



	public override void Launch()
	{
		base.Launch();

		this.main.Play();
	}

	public virtual void Stop(ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting)
	{
		this.main.Stop();

		PoolingManager.instance?.Stow(this);
	}

	public override void Reset()
	{
		this.main.Stop();

		base.Reset();
	}



	private ParticleSystem.MinMaxGradient GetAlphaMatchingColor(ParticleSystem.MinMaxGradient grad, Color c)
	{
		if(grad.mode == ParticleSystemGradientMode.Color) {
			grad.color = new Color(c.r, c.g, c.b, grad.color.a);
		} else if(grad.mode == ParticleSystemGradientMode.Gradient) {
			grad.gradient.SetKeys(new GradientColorKey[1] {new GradientColorKey(c, 0f)}, grad.gradient.alphaKeys);
		} else if(grad.mode == ParticleSystemGradientMode.TwoColors) {
			grad.colorMax = new Color(c.r, c.g, c.b, grad.colorMax.a);
			grad.colorMin = new Color(c.r, c.g, c.b, grad.colorMin.a);
		} else if(grad.mode == ParticleSystemGradientMode.TwoGradients) {
			grad.gradientMax.SetKeys(new GradientColorKey[1] {new GradientColorKey(c, 0f)}, grad.gradientMax.alphaKeys);
			grad.gradientMin.SetKeys(new GradientColorKey[1] {new GradientColorKey(c, 0f)}, grad.gradientMin.alphaKeys);
		} else {
			Debug.Log("To do");
		}

		return grad;
	}
}
