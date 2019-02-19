using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PerformanceScript : MonoBehaviour
{
	private Transform myCamera;

	[Header("Variants")]
	public ShaderVariantCollection myShaderVariant;

	[Header("Instantiation")]
	public GameObject[] myInstanced;
	[HideInInspector]
	public GameObject[] myRendered;

	[Header("Loading")]
	public bool Done = false;

	private CameraScript cameraScript;
	private PlanetSetup planetSetup;

	void Awake()
	{
		myCamera = GameObject.Find("MainCamera").transform;
		cameraScript = myCamera.GetComponent<CameraScript>();

		myShaderVariant.WarmUp();
	}

	public void ArcadePreloading()
	{
		planetSetup = GameObject.Find("Planets").GetComponent<PlanetSetup>();
		StartCoroutine(ArcadePreloadingCoroutine());
	}

	private IEnumerator ArcadePreloadingCoroutine() // Charge les meshes et les textures en mémoire (?)
	{
		if(myCamera == null)
			yield return new WaitUntil(() => myCamera != null);

		Vector3 initialPos = myCamera.position;
		Quaternion initialQuat = myCamera.rotation;

		// Place all objects
		planetSetup.MainPlanetSetObjectsAll();

		// Wait for object to be placed
		yield return new WaitUntil(() => planetSetup.DonePlacingObject);

		// Place camera & active effects -> Global
		myCamera.position = new Vector3(600, 650, 600);
		myCamera.rotation = Quaternion.Euler(90, 0, 0);
		cameraScript.SetEffects(true);

		// Create specials objects
		int len = myInstanced.Length;
		Vector3 target = myCamera.position + myCamera.forward * 15f;
		GameObject[] objs = new GameObject[len];
		for(int i = 0; i < len; i++)
			objs[i] = Instantiate(myInstanced[i], target, Quaternion.identity);

		// Rendering frame
		yield return new WaitForEndOfFrame();

		// Place camera -> Planet
		myCamera.position = new Vector3(600, 550, 600);

		// Rendering frame
		yield return new WaitForEndOfFrame();

		// Deactivate all object
		foreach(GameObject go in myRendered)
			go.SetActive(false);

		// Delete specials objects
		for(int i = 0; i < len; i++)
			Destroy(objs[i]);

		yield return null;

		myRendered = null;
		myCamera.position = initialPos;
		myCamera.rotation = initialQuat;
		cameraScript.SetEffects(false);

		// Prerendering done !
		Done = true;
	}
}
