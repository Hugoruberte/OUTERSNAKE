using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{
	private LineRenderer Line;

	[Range(2, 30)]
	public int Width = 4;
	private float fwidth;
	private int index = 0;

	void Awake()
	{
		Line = GetComponent<LineRenderer>();
		fwidth = (float)Width;
	}

	void Start ()
	{
		transform.localScale = new Vector3(Width, 1.0f, Width);

		Vector3[] positions = new Vector3[6 * Width - 1];

		positions[index ++] = new Vector3(0.0f, 0.0f, 0.0f);
		positions[index ++] = new Vector3(0.0f, 0.0f, 1.0f);
		positions[index ++] = new Vector3(1.0f, 0.0f, 1.0f);

		int i;

		for(i = Width - 1; i > 0; i--)
		{
			positions[index ++] = new Vector3(1.0f, 0.0f, i/fwidth);
			positions[index ++] = new Vector3(0.0f, 0.0f, i/fwidth);
			positions[index ++] = new Vector3(1.0f, 0.0f, i/fwidth);
		}

		positions[index ++] = new Vector3(1.0f, 0.0f, 0.0f);

		for(i = Width - 1; i > 0; i--)
		{
			positions[index ++] = new Vector3(i/fwidth, 0.0f, 0.0f);
			positions[index ++] = new Vector3(i/fwidth, 0.0f, 1.0f);
			positions[index ++] = new Vector3(i/fwidth, 0.0f, 0.0f);
		}

		positions[index ++] = new Vector3(0.0f, 0.0f, 0.0f);

		for(i = 0; i < positions.Length; i++)
		{
			positions[i] -= new Vector3(0.5f, 0.0f, 0.5f);
		}

		Line.positionCount = positions.Length;
		Line.SetPositions(positions);
	}
}
