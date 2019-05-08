using UnityEngine;
using My.Tools;

namespace My.Events
{
	public delegate void ActionEvent();

	public delegate void IntEvent(int current);
	public delegate void IntIntEvent(int previous, int current);

	public delegate void FloatEvent(float current);
	public delegate void FloatFloatEvent(float previous, float current);

	public delegate void BoolEvent(bool current);
	public delegate void BoolBoolEvent(bool previous, bool current);

	public delegate void StringEvent(string current);
	public delegate void StringStringEvent(string previous, string current);

	public delegate void Vector3Event(Vector3 current);
	public delegate void Vector3Vector3Event(Vector3 previous, Vector3 current);

	public delegate void QuaternionEvent(Quaternion current);
	public delegate void QuaternionQuaternionEvent(Quaternion previous, Quaternion current);

	public delegate void _TransformEvent(_Transform current);
	public delegate void _Transform_TransformEvent(_Transform previous, _Transform current);
}