using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

[ExecuteInEditMode]
public class ShowParentRelativePositionUnscaled : MonoBehaviour
{
	public bool button = false;

	private void OnValidate()
	{
		Debug.Log(transform.parent.InverseTransformPointUnscaled(transform.position));
	}
}
