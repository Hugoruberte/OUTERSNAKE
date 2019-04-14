using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public abstract class ParticleEffect : PoolableEntity
{
	protected ParticleSystem main;
	protected ParticleSystem[] all;

	protected override void Awake()
	{
		base.Awake();

		this.main = this.GetComponentInChildrenWithName<ParticleSystem>("Effect");
		this.all = this.GetComponentsInChildren<ParticleSystem>();
	}

	public virtual void Initialize(Vector3 position, Vector3 direction)
	{
		this.SetOrientation(position, direction);
	}
	public virtual void Initialize(Vector3 position, Vector3 direction, Color color)
	{
		this.Initialize(position, direction);
		this.SetColor(color);
	}

	public override void Launch()
	{
		base.Launch();

		this.main.Play();
	}

	public virtual void Stop()
	{
		this.main.Stop();
	}

	public override void Reset()
	{
		this.Stop();

		base.Reset();
	}






	private void SetOrientation(Vector3 position, Vector3 direction)
	{
		this.transform.position = position;
		this.transform.rotation = Quaternion.LookRotation(direction);
	}

	private void SetColor(Color c)
	{
		ParticleSystem.MainModule m;
		ParticleSystem.ColorBySpeedModule cs;
		ParticleSystem.ColorOverLifetimeModule cl;
		ParticleSystem.TrailModule t;

		Gradient grad = new Gradient();
		grad.SetKeys(new GradientColorKey[]{new GradientColorKey(c, 0.0f), new GradientColorKey(c, 1.0f)},
					 new GradientAlphaKey[]{new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f)});

		foreach(ParticleSystem p in all) {

			m = p.main;
			m.startColor = new Color(c.r, c.g, c.b, m.startColor.color.a);

			cs = p.colorBySpeed;
			cs.color = grad;

			cl = p.colorOverLifetime;
			cl.color = grad;

			t = p.trails;
			t.colorOverLifetime = grad;
			t.colorOverTrail = grad;
		}
	}

	private void MatchGradientWithAlpha(Gradient grad, float alpha)
	{
		GradientAlphaKey key;

		for(int i = 0; i < grad.alphaKeys.Length; i++) {
			key = grad.alphaKeys[i];
			key.alpha = alpha;
			grad.alphaKeys[i] = key;
		}
	}
}
