using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tools;

public class GarageSnakeCreatorScript : MonoBehaviour
{
	private Transform MainStructure;
	private Transform Shapes;
	private Transform Rail;
	private Transform Core;
	private Transform Unavailables;

	private RectTransform SelectionImage;
	private RectTransform NameStructure;
	private RectTransform Datas;
	private RectTransform Approval;
	private RectTransform[] Jauges = new RectTransform[4];

	private Image flashImage;

	private Text YesText;
	private Text NoText;

	private Camera myCamera;

	public GameObject[] Shapes0 = new GameObject[3];
	public GameObject[] Shapes1 = new GameObject[3];
	public GameObject[] Shapes2 = new GameObject[3];
	private GameObject[][] Shapeses;

	private CameraShakeScript shakeScript;

	private WaitForSeconds waitforseconds_005 = new WaitForSeconds(0.05f);

	private IEnumerator scrollCoroutine = null;
	private IEnumerator jaugeCoroutine = null;
	private List<IEnumerator> unavailableCoroutines = new List<IEnumerator>();

	private Vector3 targetPosition;
	private float[] cacheValues = new float[4];
	private float[,] randomValues = new float[9,4];

	private int firstChoice = 0;
	public int scrollChoice = 1;
	public int scrollIndex = 0;
	private float canvasSizeX = 0f;
	private float canvasSizeY = 0f;

	void Awake()
	{
		MainStructure = GameObject.Find("Main").transform;
		Shapes = GameObject.Find("Shapes").transform;
		Core = MainStructure.Find("Core").transform;
		Rail = Shapes.Find("Rail");
		Unavailables = GameObject.Find("Canvas/Menu/Unavailables").transform;

		NameStructure = GameObject.Find("Canvas/Menu/Name").GetComponent<RectTransform>();
		Datas = GameObject.Find("Canvas/Menu/Datas").GetComponent<RectTransform>();
		SelectionImage = GameObject.Find("Canvas/Menu/Selection").GetComponent<RectTransform>();
		Approval = GameObject.Find("Canvas/Menu/Approval").GetComponent<RectTransform>();

		flashImage = GameObject.Find("Canvas/Menu/Flash").GetComponent<Image>();

		YesText = GameObject.Find("Canvas/Menu/Approval/Yes").GetComponent<Text>();
		NoText = GameObject.Find("Canvas/Menu/Approval/No").GetComponent<Text>();

		Transform JaugesP = GameObject.Find("Canvas/Menu/Datas/Jauges").transform;
		for(int i = 0; i < 4; i++)
			Jauges[i] = JaugesP.GetChild(i).GetComponent<RectTransform>();

		RectTransform canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
		canvasSizeX = canvas.sizeDelta.x;
		canvasSizeY = canvas.sizeDelta.y;

		myCamera = GameObject.Find("MainCamera/Camera").GetComponent<Camera>();
		shakeScript = GameObject.Find("MainCamera").GetComponent<CameraShakeScript>();

		Shapeses = new GameObject[][]{Shapes0, Shapes1, Shapes2};
	}

	void Start()
	{
		targetPosition = Rail.localPosition;
		StartCoroutine(InputCoroutine());
		scrollCoroutine = ScrollCoroutine();
		StartCoroutine(scrollCoroutine);

		for(int i = 0; i < 9; i++){
			for(int j = 0; j < 4; j++)
				randomValues[i,j] = Random.Range(-30f, -500f);
		}

		SetShapes(0);

		if(jaugeCoroutine != null)
			StopCoroutine(jaugeCoroutine);
		jaugeCoroutine = JaugeCoroutine(scrollIndex);
		StartCoroutine(jaugeCoroutine);
	}

	private IEnumerator InputCoroutine()
	{
		bool back = false;
		bool itIsANo = false;

		int horizontal = 0;

		Color selectedColor = new Color32(255, 150, 0, 255);

		Vector3 targetMainFront = new Vector3(556.5f, 320.4f, -656f);
		Vector2 targetSelectionFront = new Vector2(0, 830);
		Vector2 targetNameFront = new Vector2(-30, -10);
		Vector3 targetShapesFront = new Vector3(555, 325, -655);
		Vector2 targetDatasFront = new Vector2(-75, 200);
		Vector2 targetApprovalFront = new Vector2(-75, 10);

		Vector3 targetMainBack = new Vector3(557f, 319.4f, -655f);
		Vector2 targetSelectionBack = new Vector2(0, 85);
		Vector2 targetNameBack = new Vector2(-30, -100);
		Vector3 targetShapesBack = new Vector3(555, 320, -655);
		Vector2 targetDatasBack = new Vector2(-75, -15);
		Vector2 targetApprovalBack = new Vector2(-75, -250);

		Vector3 mreference = Vector3.zero;
		Vector3 spreference = Vector3.zero;
		Vector2 sreference = Vector2.zero;
		Vector2 nreference = Vector2.zero;
		Vector2 dreference = Vector2.zero;
		Vector2 areference = Vector2.zero;

		while(true)
		{
			// Select shape
			while(true)
			{
				if(Input.GetKeyDown(KeyCode.Return))
				{
					if(scrollIndex == 0)
					{
						firstChoice = scrollChoice;
						for(int j = 0; j < 4; j++)
						{
							cacheValues[j] = randomValues[6 + firstChoice,j];
							randomValues[6 + firstChoice,j] = -340f;
						}
					}

					if(scrollChoice == firstChoice)
					{
						shakeScript.Shake(0.5f);
						AddShapeToStructure(scrollIndex ++);

						if(scrollIndex < 3)
						{
							if(jaugeCoroutine != null)
								StopCoroutine(jaugeCoroutine);
							jaugeCoroutine = JaugeCoroutine(scrollIndex);
							StartCoroutine(jaugeCoroutine);
						}
						else
						{
							break;
						}

						RemoveShapes();
						SetShapes(scrollIndex);
					}
					else
					{
						shakeScript.Shake(0.1f);
					}

					yield return waitforseconds_005;
				}
				else if(Input.GetKeyDown(KeyCode.Backspace))
				{
					shakeScript.Shake(0.1f);

					if(scrollIndex > 0)
					{
						RemoveShapes();
						Destroy(Core.GetChild(scrollIndex --).gameObject);
						SetShapes(scrollIndex);

						if(jaugeCoroutine != null)
							StopCoroutine(jaugeCoroutine);
						jaugeCoroutine = JaugeCoroutine(scrollIndex);
						StartCoroutine(jaugeCoroutine);

						if(scrollIndex == 0)
						{
							foreach(IEnumerator co in unavailableCoroutines)
								StopCoroutine(co);
							unavailableCoroutines.Clear();
							for(int i = 1; i < Unavailables.childCount; i++)
								Destroy(Unavailables.GetChild(i).gameObject);
							for(int i = 0; i < 4; i++)
								randomValues[6 + firstChoice, i] = cacheValues[i];					
						}
					}

					yield return waitforseconds_005;
				}

				yield return null;
			}

			StopCoroutine(scrollCoroutine);
			back = false;
			itIsANo = false;
			YesText.color = selectedColor;
			NoText.color = Color.black;

			// Show selection
			while(!back && Vector3.Distance(MainStructure.position, targetMainFront) > 0.01f)
			{
				MainStructure.position = Vector3.SmoothDamp(MainStructure.position, targetMainFront, ref mreference, 0.2f);
				SelectionImage.anchoredPosition = Vector2.SmoothDamp(SelectionImage.anchoredPosition, targetSelectionFront, ref sreference, 0.2f, Mathf.Infinity, Time.deltaTime);
				NameStructure.anchoredPosition = Vector2.SmoothDamp(NameStructure.anchoredPosition, targetNameFront, ref nreference, 0.2f, Mathf.Infinity, Time.deltaTime);
				Datas.anchoredPosition = Vector2.SmoothDamp(Datas.anchoredPosition, targetDatasFront, ref dreference, 0.2f, Mathf.Infinity, Time.deltaTime);
				Approval.anchoredPosition = Vector2.SmoothDamp(Approval.anchoredPosition, targetApprovalFront, ref areference, 0.2f, Mathf.Infinity, Time.deltaTime);
				Shapes.position = Vector3.SmoothDamp(Shapes.position, targetShapesFront, ref spreference, 0.2f);

				back = Input.GetKeyDown(KeyCode.Backspace);
				horizontal = (int)Input.GetAxisRaw("Horizontal");
				if(horizontal != 0)
				{
					if(horizontal == 1)
					{
						itIsANo = false;
						YesText.color = selectedColor;
						NoText.color = Color.black;
					}
					else
					{
						itIsANo = true;
						YesText.color = Color.black;
						NoText.color = selectedColor;
					}
				}

				yield return null;
			}

			// Valid selection
			while(!back)
			{
				horizontal = (int)Input.GetAxisRaw("Horizontal");

				if(Input.GetKeyDown(KeyCode.Return))
				{
					back = itIsANo;
					yield return waitforseconds_005;
					break;
				}
				else if(Input.GetKeyDown(KeyCode.Backspace))
				{
					back = true;
					yield return waitforseconds_005;
					break;
				}
				else if(horizontal != 0)
				{
					if(horizontal == 1)
					{
						itIsANo = false;
						YesText.color = selectedColor;
						NoText.color = Color.black;
					}
					else
					{
						itIsANo = true;
						YesText.color = Color.black;
						NoText.color = selectedColor;
					}
				}

				yield return null;
			}

			shakeScript.Shake(0.5f);

			// If back
			if(back)
			{
				StartCoroutine(scrollCoroutine);
				Destroy(Core.GetChild(scrollIndex --).gameObject);

				while(Vector3.Distance(MainStructure.position, targetMainBack) > 0.01f)
				{
					MainStructure.position = Vector3.SmoothDamp(MainStructure.position, targetMainBack, ref mreference, 0.1f);
					SelectionImage.anchoredPosition = Vector2.SmoothDamp(SelectionImage.anchoredPosition, targetSelectionBack, ref sreference, 0.1f, Mathf.Infinity, Time.deltaTime);
					NameStructure.anchoredPosition = Vector2.SmoothDamp(NameStructure.anchoredPosition, targetNameBack, ref nreference, 0.1f, Mathf.Infinity, Time.deltaTime);
					Datas.anchoredPosition = Vector2.SmoothDamp(Datas.anchoredPosition, targetDatasBack, ref dreference, 0.1f, Mathf.Infinity, Time.deltaTime);
					Approval.anchoredPosition = Vector2.SmoothDamp(Approval.anchoredPosition, targetApprovalBack, ref areference, 0.1f, Mathf.Infinity, Time.deltaTime);
					Shapes.position = Vector3.SmoothDamp(Shapes.position, targetShapesBack, ref spreference, 0.1f);
					yield return null;
				}
			}
			else
			{
				break;
			}
		}

		// Clean coroutine
		foreach(IEnumerator co in unavailableCoroutines)
			StopCoroutine(co);
		unavailableCoroutines.Clear();

		float flashValue = 1f;
		flashImage.SetColorA(flashValue);
		while(flashValue > 0.01f)
		{
			flashValue -= 2.5f * Time.deltaTime;
			flashImage.SetColorA(flashValue);
			yield return null;
		}
		flashImage.SetColorA(0f);

		Debug.Log("DONE.");
	}

	private IEnumerator ScrollCoroutine()
	{
		int horizontal = 0;
		float smooth = 0.1f;
		Vector3 reference = Vector3.zero;
		float clock = 0f;

		while(true)
		{
			do
			{
				horizontal = -(int)Input.GetAxisRaw("Horizontal");
				yield return null;
			}
			while(horizontal == 0);
			smooth = (Time.time-clock < 0.425f) ? 0.05f : 0.1f;
			clock = Time.time;

			targetPosition = targetPosition + Rail.right * 2.55f * horizontal;

			scrollChoice = ((scrollChoice - horizontal) % 3 == -1) ? 2 : (scrollChoice - horizontal) % 3;

			if(jaugeCoroutine != null)
				StopCoroutine(jaugeCoroutine);
			jaugeCoroutine = JaugeCoroutine(scrollIndex);
			StartCoroutine(jaugeCoroutine);
			
			while(Vector3.Distance(targetPosition, Rail.localPosition) > 0.01f)
			{
				Rail.localPosition = Vector3.SmoothDamp(Rail.localPosition, targetPosition, ref reference, smooth);
				yield return null;
			}

			Rail.localPosition = targetPosition;

			if(Rail.localPosition.x >= 7.645f || Rail.localPosition.x <= -7.645f)
			{
				Rail.localPosition = Vector3.zero;
				targetPosition = Vector3.zero;
			}
		}
	}

	private void RemoveShapes()
	{
		Transform child;

		for(int i = 0; i < 3; i++)
		{
			child = Rail.GetChild(i);

			for(int j = 0; j < 3; j++)
			{
				Destroy(child.GetChild(j).GetChild(0).GetChild(0).gameObject);
			}
		}
	}

	private void SetShapes(int index)
	{
		Transform child;
		Transform cube;
		Vector3 shapeScale = new Vector3(0.25f, 0.25f, 0.25f);
		Color unavailableColor = new Color32(140, 140, 140, 255);
		IEnumerator co = null;

		for(int i = 0; i < 3; i++)
		{
			child = Rail.GetChild(i);

			for(int j = 0; j < 3; j++)
			{
				cube = child.GetChild(j).GetChild(0);
				GameObject sh = Instantiate(Shapeses[i][index], cube.position, cube.rotation);
				sh.transform.localScale = shapeScale;
				sh.transform.parent = cube;
				sh.transform.localPosition = Vector3.zero;

				if(index > 0 && firstChoice != i)
				{
					sh.GetComponent<Renderer>().SetFlatColor(unavailableColor);

					if(index == 1 && unavailableCoroutines.Count < 6)
					{
						co = UnavailableCoroutine(cube);
						StartCoroutine(co);
						unavailableCoroutines.Add(co);
					}
				}
			}
		}
	}

	private void AddShapeToStructure(int index)
	{
		GameObject sh = Instantiate(Shapeses[scrollChoice][index], Core.position, Core.rotation);
		sh.transform.parent = Core;
		sh.transform.localScale = Vector3.one;
	}

	private IEnumerator UnavailableCoroutine(Transform tr)
	{
		Vector3 worldScreenPos;
		Vector2 screenPos;
		GameObject unv = Instantiate(Unavailables.GetChild(0).gameObject, Vector3.zero, Quaternion.identity);
		unv.SetActive(true);
		unv.transform.SetPositionZ(-120);
		unv.transform.SetParent(Unavailables, false);
		RectTransform unvRect = unv.GetComponent<RectTransform>();

		while(true)
		{
			worldScreenPos = myCamera.WorldToViewportPoint(tr.position);
			screenPos = new Vector2(canvasSizeX * (worldScreenPos.x - 0.5f), canvasSizeY * (worldScreenPos.y - 0.5f));
			unvRect.anchoredPosition = screenPos;
			yield return null;
		}
	}

	private IEnumerator JaugeCoroutine(int index)
	{
		float[] values = new float[4];
		for(int i = 0; i < 4; i++)
			values[i] = randomValues[index * 3 + scrollChoice, i];
		float[] reference = new float[]{0f, 0f, 0f, 0f};
		bool cond = true;

		while(cond)
		{
			cond = false;
			for(int i = 0; i < 4; i++)
			{
				Jauges[i].SetOffsetMaxX(Mathf.SmoothDamp(Jauges[i].offsetMax.x, values[i], ref reference[i], 0.2f));
				if(Mathf.Abs(Jauges[i].offsetMax.x - values[i]) > 0.005f)
					cond = true;
			}

			yield return null;
		}
	}
}
