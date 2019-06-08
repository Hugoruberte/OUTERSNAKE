using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Shared
{
	public static List<Vector3> vector3List = new List<Vector3>();

	public static Collider[] colliderArray = new Collider[50];

	public static readonly Vector3 vector3Zero = Vector3.zero;
	public static readonly Vector3 vector3One = Vector3.one;
	public static readonly Vector3 vector3Right = Vector3.right;
	public static readonly Vector3 vector3Left = Vector3.left;
	public static readonly Vector3 vector3Up = Vector3.up;
	public static readonly Vector3 vector3Down = Vector3.down;
	public static readonly Vector3 vector3Forward = Vector3.forward;
	public static readonly Vector3 vector3Back = Vector3.back;
}
