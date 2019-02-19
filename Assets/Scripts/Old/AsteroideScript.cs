using UnityEngine;
using System.Collections;

public class AsteroideScript : MonoBehaviour
{
	/*private ParticleSystem myParticle;

	private Transform myTransform;
	private Transform myChild;

	[Range(100.0f, 200.0f)]
	public float Omega = 150.0f;

	private CameraScript cameraScript;


	void Awake()
	{
		myTransform = transform;
		myChild = myTransform.GetChild(0);

		cameraScript = GameObject.Find("MainCamera").GetComponent<CameraScript>();

		myParticle = myTransform.Find("SecondAsteroide/AsteroideGenerator").GetComponent<ParticleSystem>();
		myParticle.Stop();
	}


	void Update()
	{
		if(cameraScript.Asteroide)
		{
			myTransform.Rotate(0, 0, Omega * Time.deltaTime);
			Vector3 temp = myChild.localEulerAngles;
			temp = new Vector3(15.0f + Mathf.PingPong(15.0f * Time.time, 25.0f), 0, 0);
			myChild.localEulerAngles = temp;
		}
	}*/
}