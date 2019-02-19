using UnityEngine;
using System.Collections;
using Tools;

public class RabbitControllerV0 : MonoBehaviour
{
	private Transform myTransform;
	private Transform Body;
	private Transform Snake;

	public int myCell = -1;
	private float jumpSpeed = 5.0f;

	private Faces myFace;

	public IEnumerator move_coroutine;
	private IEnumerator jumping_coroutine;
	private IEnumerator doublejumping_coroutine;

	[HideInInspector]
	public WaitForSeconds waitforseconds;

	private Vector3 targetHeight = new Vector3(0, 2, 0);
	private Vector3 doublejumpingHeight = new Vector3(0, 3, 0);
	private Vector3 targetScale = new Vector3(1, 1, 0.5f);
	private Vector3 targetLocalPosition = new Vector3(0, -0.25f, 0);
	private Vector3 targetPosition;
	[HideInInspector]
	public Vector3 Lign;
	[HideInInspector]
	public Vector3 Column;

	private bool Jumping = false;

	private GameManagerV1 gameManager;
	private SnakeControllerV3 snakeScript;
	private RabbitManagement rabbitManag;
	private PlanetScript planetScript;


	void Awake()
	{
		myTransform = transform;
		Body = myTransform.Find("Body");
		Snake = GameObject.FindWithTag("Player").transform;
		
		gameManager = GameObject.Find("LevelManager").GetComponent<GameManagerV1>();
		snakeScript = GameObject.FindWithTag("Player").GetComponent<SnakeControllerV3>();
		rabbitManag = Body.GetComponent<RabbitManagement>();

		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();

		float delay_median = -0.14f * gameManager.WorldSetting.RabbitEnergy + 1.5f;
		float delay = Random.Range(delay_median - 0.1f, delay_median + 0.1f);
		waitforseconds = new WaitForSeconds(delay);

		jumpSpeed = - (100f/7f) * delay + (185f/7f);
	}

	private IEnumerator Move()
	{
		Vector3[] Directions;
		Vector3 cell_position;
		int[] Cells = new int[4] {-1, -1, -1, -1};
		int[] TargetCells = new int[4] {-1, -1, -1, -1};
		int target_index;
		int choice;
		int cell;
		float min;
		float dist;

		while(true)
		{
			if(rabbitManag.State != RabbitState.Dead && snakeScript.State == SnakeState.Run && myCell >= 0)
			{
				target_index = 0;
				choice = 0;
				min = 100000.0f;
				dist = 0.0f;
				Directions = new Vector3[4] {myTransform.forward, myTransform.right, -myTransform.forward, -myTransform.right};

				for(int i = 0; i < 4; i++)
				{
					cell = CellExtension.PositionToCell(myTransform.AbsolutePosition() + Directions[i] * 2, gameManager.MainPlanet);
					Cells[i] = cell;

					if(cell >= 0 && (planetScript.Grid[cell] == CellEnum.Empty || planetScript.Grid[cell] == CellEnum.Burrow))
						TargetCells[target_index ++] = i;
				}

				if(target_index > 0)
				{
					if(rabbitManag.State == RabbitState.Bomb && myFace == snakeScript.Face)
					{
						for(int k = 0; k < target_index; k++)
						{
							cell_position = CellExtension.CellToPosition(Cells[TargetCells[k]], gameManager.MainPlanet);
							dist = Vector3.Distance(cell_position, Snake.position);
							if(dist < min)
							{
								min = dist;
								choice = TargetCells[k];
							}
						}
					}
					else
					{
						choice = TargetCells[Random.Range(0, target_index)];
					}

					myTransform.rotation *= Quaternion.Euler(0, choice * 90, 0);

					yield return null;

					targetPosition = myTransform.AbsolutePosition() + 2 * myTransform.forward;
					while(Vector3.Distance(Body.localScale, targetScale) > 0.05f)
					{
						Body.localScale = Vector3.MoveTowards(Body.localScale, targetScale, jumpSpeed * Time.deltaTime);
						Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetLocalPosition, jumpSpeed/2 * Time.deltaTime);
						yield return null;
					}
					jumping_coroutine = JumpCoroutine();
					StartCoroutine(jumping_coroutine);
					while(Vector3.Distance(myTransform.position, targetPosition) > 0.05f)
					{
						myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, 10.0f * Time.deltaTime);
						yield return null;
					}
					myTransform.position = targetPosition;

					if(planetScript.Grid[myCell] == CellEnum.Rabbit)
						planetScript.Grid[myCell] = CellEnum.Empty;

					myCell = Cells[choice];
					
					if(planetScript.Grid[myCell] == CellEnum.Empty)
						planetScript.Grid[myCell] = CellEnum.Rabbit;
				}

				do
					yield return null;
				while(Jumping);
			}

			yield return waitforseconds;
		}
	}

	private IEnumerator JumpCoroutine()
	{
		Jumping = true;

		while(Vector3.Distance(Body.localPosition, targetHeight) > 0.05f)
		{
			Body.localPosition = Vector3.MoveTowards(Body.localPosition, targetHeight, 20.0f * Time.deltaTime);
			Body.localScale = Vector3.MoveTowards(Body.localScale, Vector3.one, jumpSpeed * Time.deltaTime);
			yield return null;
		}
		while(Vector3.Distance(Body.localPosition, Vector3.zero) > 0.05f)
		{
			Body.localPosition = Vector3.MoveTowards(Body.localPosition, Vector3.zero, 20.0f * Time.deltaTime);
			yield return null;
		}
		Body.localPosition = Vector3.zero;
		Body.localScale = Vector3.one;

		Jumping = false;
	}

	public void DoubleJumping()
	{
		StopCoroutine(jumping_coroutine);

		if(doublejumping_coroutine != null)
			StopCoroutine(doublejumping_coroutine);
		doublejumping_coroutine = DoubleJumpCoroutine();
		StartCoroutine(doublejumping_coroutine);
	}

	private IEnumerator DoubleJumpCoroutine()
	{
		Jumping = true;

		while(Vector3.Distance(Body.localPosition, doublejumpingHeight) > 0.05f)
		{
			Body.localPosition = Vector3.MoveTowards(Body.localPosition, doublejumpingHeight, 15f * Time.deltaTime);
			yield return null;
		}
		while(Vector3.Distance(Body.localPosition, Vector3.zero) > 0.05f)
		{
			Body.localPosition = Vector3.MoveTowards(Body.localPosition, Vector3.zero, 15f * Time.deltaTime);
			yield return null;
		}
		Body.localPosition = Vector3.zero;
		Body.localScale = Vector3.one;

		Jumping = false;
	}

	/*private int PositionToCell(Vector3 position)
	{
		int cell = -1;
		int a;
		int b;
		int myface = (int)myFace;

		Vector3 vector;
		Vector3 zero;

		zero = gameManager.MainPlanet.position + 10.5f * Lign - 10.5f * Column;

		vector = Vector3.Scale((zero - position), Lign);
		if(Mathf.RoundToInt(vector.x) != 0)
			a = Mathf.RoundToInt(vector.x);
		else if(Mathf.RoundToInt(vector.y) != 0)
			a = Mathf.RoundToInt(vector.y);
		else
			a = Mathf.RoundToInt(vector.z);

		vector = Vector3.Scale((position - zero), Column);
		if(Mathf.RoundToInt(vector.x) != 0)
			b = Mathf.RoundToInt(vector.x);
		else if(Mathf.RoundToInt(vector.y) != 0)
			b = Mathf.RoundToInt(vector.y);
		else
			b = Mathf.RoundToInt(vector.z);

		if(a >= 0 && b >= 0 && a <= 21 && b <= 21)
			cell = myface*22*22 + a*22 + b;

		return cell;
	}*/

	public IEnumerator ReloadPosition(Transform myBurrow , int cell)
	{
		if(move_coroutine != null)
		{
			StopCoroutine(move_coroutine);
			move_coroutine = null;
		}

		planetScript = gameManager.MainPlanet.GetComponent<PlanetScript>();
		myCell = cell;
		myFace = (Faces)(myCell/(22*22));

		targetPosition = Vector3Extension.RoundToInt(myBurrow.position + myBurrow.forward * 0.25f);
		myTransform.position = targetPosition;

		yield return null;

		rabbitManag.SetFire(false);

		float delay_median = -0.14f * gameManager.WorldSetting.RabbitEnergy + 1.5f;
		float delay = Random.Range(delay_median - 0.15f, delay_median + 0.15f);
		waitforseconds = new WaitForSeconds(delay);

		jumpSpeed = - (100f/7f) * delay + (185f/7f);
		
		yield return null;

		move_coroutine = Move();
		StartCoroutine(move_coroutine);
	}
}