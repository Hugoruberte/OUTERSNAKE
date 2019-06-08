using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

public class GroundImpactEffect : PoolableEntity
{
	[Header("Camera")]
	[SerializeField] private Transform cameraTransform = null;

	[Header("Projectors")]
	[SerializeField] private Projector impactProjector = null;


	private ParticleSystem particle;
	private Camera cam;
	
	private WaitWhile wait = null;
	private WaitForSeconds delay = Yielders.Wait(0.5f);
	private RenderTexture renderTexture;
	private Material mat;


	private void Awake()
	{
		this.particle = this.GetComponentInChildrenWithName<ParticleSystem>("Effect");
		this.cam = GetComponentInChildren<Camera>();
		this.mat = new Material(this.impactProjector.material);

		this.impactProjector.material = this.mat;
	}

	public void Initialize(Vector3 position, Vector3 direction)
	{
		transform.position = position - direction.normalized * 1.5f;
		transform.rotation = Quaternion.LookRotation(direction);

		this.renderTexture = new RenderTexture(48, 48, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		// this.renderTexture.useMipMap = true;

		this.cam.targetTexture = this.renderTexture;
		this.impactProjector.material.SetTexture("_ShadowTex", this.renderTexture);
	}

	public override void Launch()
	{
		base.Launch();

		this.cameraTransform.position = FarAwayManager.instance.GetFarAwayPosition(gameObject);
		this.particle.Play();

		this.StartCoroutine(this.LifetimeCoroutine());
	}

	private IEnumerator LifetimeCoroutine()
	{
		if(wait == null) {
			wait = new WaitWhile(() => this.particle.IsAlive(true));
		}

		yield return wait;
		yield return delay;

		// stow
		PoolingManager.instance.Stow(this);
	}

	public override void Reset()
	{
		FarAwayManager.instance?.ReleaseFarAwayPosition(gameObject);

		this.cameraTransform.localPosition = Shared.vector3Zero;
		this.renderTexture?.Release();

		base.Reset();
	}
}
