using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

[RequireComponent(typeof(Rigidbody))]
public class TargetEffect : Effect
{
	public float tmp = 10f;

	private struct GeometricShapeData {
		public readonly GeometricShapeController ctr;
		public readonly float baseWidth;
		public readonly float baseSize;

		public GeometricShapeData(GeometricShapeController gsc) {
			this.ctr = gsc;
			this.baseWidth = gsc.width;
			this.baseSize = gsc.size;
		}
	}

	private Rigidbody rb;
	private Renderer[] meshRenderers;
	private LineRenderer[] lineRenderers;
	private GeometricShapeData[] gsds;

	protected override private void Awake()
	{
		base.Awake();

		this.rb = GetComponent<Rigidbody>();
		this.meshRenderers = this.GetComponentsInChildren<MeshRenderer>(true);
		this.lineRenderers = this.GetComponentsInChildren<LineRenderer>(true);

		GeometricShapeController[] ctrs = this.GetComponentsInChildren<GeometricShapeController>(true);
		this.gsds = new GeometricShapeData[ctrs.Length];
		for(int i = 0; i < ctrs.Length; i++) {
			this.gsds[i] = new GeometricShapeData(ctrs[i]);
		}

		this.rb.maxAngularVelocity = 20f;
	}

	public void Launch(float smooth, float revealSpeed)
	{
		base.Launch();

		this.rb.drag = smooth;
		this.StartCoroutine(this.BehaviourCoroutine(revealSpeed));
	}

	public void HideAway(float hideSpeed)
	{
		this.StopAllCoroutines();
		this.StartCoroutine(this.HideAwayCoroutine(hideSpeed));
	}





	private IEnumerator BehaviourCoroutine(float revealSpeed)
	{
		float speed, step;
		float widthMult, widthMax;
		float sizeMult, sizeMax;

		this.StartCoroutine(this.LockCoroutine(false, revealSpeed));
		this.StartCoroutine(this.FadeCoroutine(false, revealSpeed));

		while(true) {

			step = 0f;
			speed = Random.Range(1f, 5f);
			widthMax = Random.Range(1.5f, 5f);
			sizeMax = Random.Range(0.25f, 1.25f);

			this.rb.angularVelocity = this.transform.forward * Random.Range(10f, 15f) * RandomExtension.randomSign;

			while(step < 1f) {

				widthMult = (Mathf.PingPong(step, 0.5f) * 2f) * (widthMax - 1f) + 1f;
				sizeMult = (Mathf.PingPong(step, 0.5f) * 2f) * (sizeMax - 1f) + 1f;
				foreach(GeometricShapeData gsd in this.gsds) {
					gsd.ctr.UpdateWidth(gsd.baseWidth * widthMult);
					gsd.ctr.UpdateSize(gsd.baseSize * sizeMult);
				}

				step += speed * Time.deltaTime;
				yield return null;
			}

			this.rb.angularVelocity = Shared.vector3Zero;

			yield return Yielders.Wait(1f);
		}
	}

	private IEnumerator HideAwayCoroutine(float hideSpeed)
	{
		Coroutine co = this.StartCoroutine(this.LockCoroutine(true, hideSpeed));
		yield return this.FadeCoroutine(true, hideSpeed);
		yield return co;

		yield return Yielders.Wait(0.25f);

		PoolingManager.instance.Stow(this);
	}

	private IEnumerator LockCoroutine(bool away, float animSpeed)
	{
		float step, from, to;
		float mult;

		if(away) {
			from = 1f;
			to = 2.5f;
		} else {
			from = 2.5f;
			to = 1f;
		}
		
		step = 0f;

		while(step < 1f) {

			mult = Mathf.Lerp(from, to, step);
			foreach(GeometricShapeData gsd in this.gsds) {
				gsd.ctr.UpdateSize(gsd.baseSize * mult);
			}

			step += animSpeed * Time.deltaTime;
			yield return null;
		}

		foreach(GeometricShapeData gsd in this.gsds) {
			gsd.ctr.UpdateSize(gsd.baseSize * to);
		}
	}

	private IEnumerator FadeCoroutine(bool away, float fadeSpeed)
	{
		Color c, from ,to;
		float step;

		from = this.lineRenderers[0].startColor;
		to = from;
		to.a = (away) ? 0f : 1f;
		step = 0f;

		while(step < 1f) {
			c = Color.Lerp(from, to, step);
			this.SetColor(c);
			step += fadeSpeed * Time.deltaTime;
			yield return null;
		}

		this.SetColor(to);
	}






	public override void SetColor(Color c)
	{
		foreach(MeshRenderer r in this.meshRenderers) {
			r.material.color = c;
		}

		foreach(LineRenderer r in this.lineRenderers) {
			r.startColor = c;
			r.endColor = c;
		}
	}

	public override void Reset()
	{
		base.Reset();

		this.rb.drag = 0f;
		this.rb.angularVelocity = Shared.vector3Zero;
		this.SetColor(new Color(1f,1f,1f,0f));

		foreach(GeometricShapeData gsd in this.gsds) {
			gsd.ctr.UpdateSize(gsd.baseSize);
			gsd.ctr.UpdateWidth(gsd.baseWidth);
		}
	}
}
