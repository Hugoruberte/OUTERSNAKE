using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

public class HaloEffect : Effect
{
	public enum HaloEffectMode
	{
		Shrink = 0,
		Expand,
		Still
	}

	private Transform mask;
	private Transform body;
	private const float HALO_MIN_SIZE = 0f;
	private const float HALO_WIDTH_EXPAND_SPEED = 10f;
	private float haloSpeed = 1f;
	private Renderer[] renderers;

	[Header("Editor")]
	[SerializeField, Range(0f, 10f)] private float editorRadius = 1f;
	[SerializeField, Range(HALO_MIN_SIZE, 10f)] private float editorWidth = 0.1f;
	

	protected override private void Awake()
	{
		base.Awake();

		this.body = transform.Find("Body");
		this.mask = transform.Find("Mask");
		this.renderers = GetComponentsInChildren<Renderer>();
	}

	public void Launch(float radius, float width, HaloEffectMode mode = HaloEffectMode.Shrink)
	{
		this.Launch();

		this.UpdateWidth(ref width, radius);

		if(mode == HaloEffectMode.Still) {
			return;
		}

		this.StartCoroutine(this.HaloCoroutine(radius, width, this.haloSpeed, mode));
		if(mode == HaloEffectMode.Shrink) {
			this.StartCoroutine(this.FadeCoroutine(this.haloSpeed * 1.75f, false));
		}
	}

	public void SetSpeed(float value) => this.haloSpeed = value;

	public void SetDuration(float value) => this.haloSpeed = 1f / value;





	private IEnumerator HaloCoroutine(float radius, float width, float speed, HaloEffectMode mode)
	{
		float step, from, to;

		if(mode == HaloEffectMode.Shrink) {
			from = radius;
			to = 0f;
		} else {
			from = 0f;
			to = radius;
		}

		step = 0f;
		
		while(step < 1f) {
			radius = Mathf.Lerp(from, to, step);
			this.body.localScale = Shared.vector3One * (radius * 2f);
			this.UpdateWidth(ref width, radius);

			step += speed * Time.deltaTime;
			yield return null;
		}

		if(mode == HaloEffectMode.Expand)
		{
			this.StartCoroutine(this.FadeCoroutine(speed * 1.1f, true));

			from = width;
			to = HALO_MIN_SIZE;
			step = 0f;

			while(step < 1f) {
				radius += speed * Time.deltaTime;
				this.body.localScale = Shared.vector3One * (radius * 2f);

				width = Mathf.Lerp(from, to, step);
				this.UpdateWidth(ref width, radius);

				step += HALO_WIDTH_EXPAND_SPEED * Time.deltaTime;
				yield return null;
			}
		}

		PoolingManager.instance.Stow(this);
	}

	private IEnumerator FadeCoroutine(float fadeSpeed, bool fadeOut)
	{
		float step;
		Color from, to;

		step = 0f;
		from = to = this.renderers[0].material.color;
		if(fadeOut) {
			from.a = 1f;
			to.a = 0f;
		} else {
			from.a = 0f;
			to.a = 1f;
		}

		while(step < 1f) {

			for(int i = 0; i < this.renderers.Length; i++) {
				this.renderers[i].material.color = Color.Lerp(from, to, step);
			}

			step += fadeSpeed * Time.deltaTime;
			yield return null;
		}

		for(int i = 0; i < this.renderers.Length; i++) {
			this.renderers[i].material.color = to;
		}
	}



	private void UpdateWidth(ref float w, float r)
	{
		w = Mathf.Max(HALO_MIN_SIZE, Mathf.Min(r, w));
		this.mask.localScale = Shared.vector3One * ((r - w) * 2f);
	}

	private void OnValidate()
	{
		if(!this.mask) {
			this.mask = transform.Find("Mask");
		}
		if(!body) {
			this.body = transform.Find("Body");
		}

		this.body.localScale = Shared.vector3One * (this.editorRadius * 2f);
		this.UpdateWidth(ref this.editorWidth, this.editorRadius);
	}
}
