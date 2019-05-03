using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactive.Engine;
using Tools;

public abstract class RabbitEntity : CharacterEntity
{
	public RabbitData rabbitData;

	private Collider myCollider;
	private GameObject death;
	private GameObject fire;
	private IEnumerator fireLightCoroutine = null;
	private Light fireLight;

	protected override void Awake()
	{
		base.Awake();

		// variable
		this.foodChainRank = 20;
		this.foodChainValue = 100f;

		// effect
		this.fire = this.myTransform.DeepFind("Fire").gameObject;
		this.fireLight = this.fire.GetComponentInChildren<Light>();

		this.myCollider = GetComponent<Collider>();
		this.death = this.myTransform.DeepFind("Death").gameObject;
	}

	public override void InteractWith(InteractiveStatus s, PhysicalInteractionEntity i)
	{
		if(this.currentSetOnElement != null) {
			this.currentSetOnElement(false);
			this.currentSetOnElement = null;
		}
		
		switch(s.element.type) {

			case ChemicalElement.Fire:
				this.currentSetOnElement += this.SetOnFire;
				break;

			default:
				this.currentSetOnElement = null;
				break;
		}

		if(this.currentSetOnElement != null) {
			this.currentSetOnElement(true);
		}
	}

	protected override void Death()
	{
		base.Death();

		// will not interact in future
		this.myCollider.enabled = false;
		this.behaviour.Remove(this);

		// disappear
		this.body.gameObject.SetActive(false);
		this.death.SetActive(true);

		// get destroyed
		Destroy(gameObject, 4f);
	}

	private void SetOnFire(bool active)
	{
		if(fire.activeInHierarchy && active) {
			return;
		}

		fire.SetActive(active);
		if(active) {
			fireLightCoroutine = FireLightCoroutine(fireLight);
			StartCoroutine(fireLightCoroutine);
		} else {
			StopCoroutine(fireLightCoroutine);
			fireLightCoroutine = null;
		}
	}

	private IEnumerator FireLightCoroutine(Light l)
	{
		while(true) {
			l.intensity = Random.Range(2f, 4f);
			yield return Yielders.Wait(0.1f);
		}
	}
}
