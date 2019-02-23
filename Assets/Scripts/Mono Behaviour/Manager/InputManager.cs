using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	private static InputManager _instance = null;
	public static InputManager instance {
		get {
			if(_instance == null) {
				Debug.LogError("Instance is null, either you tried to access it from the Awake function or it has not been initialized in its own Awake function");
			}
			return _instance;
		}
		set {
			_instance = value;
		}
	}
	
	void Awake()
	{
		instance = this;
	}

	public bool AnyKey()
	{
		return Input.anyKey;
	}
	public bool AnyKeyDown()
	{
		return Input.anyKeyDown;
	}




	public bool GetSubmitDown()
	{
		return Input.GetButtonDown("Submit");
	}
	public bool GetSubmit()
	{
		return Input.GetButton("Submit");
	}
	public bool GetSubmitUp()
	{
		return Input.GetButtonUp("Submit");
	}


	public bool GetCancelDown()
	{
		return Input.GetButtonDown("Cancel");
	}
	public bool GetCancel()
	{
		return Input.GetButton("Cancel");
	}
	public bool GetCancelUp()
	{
		return Input.GetButtonUp("Cancel");
	}


	public bool GetPauseDown()
	{
		return Input.GetButtonDown("Pause");
	}
	public bool GetPause()
	{
		return Input.GetButton("Pause");
	}
	public bool GetPauseUp()
	{
		return Input.GetButtonUp("Pause");
	}


	public bool GetQuitDown()
	{
		return Input.GetButtonDown("Quit");
	}
	public bool GetQuit()
	{
		return Input.GetButton("Quit");
	}
	public bool GetQuitUp()
	{
		return Input.GetButtonUp("Quit");
	}


	public bool GetShiftDown()
	{
		return Input.GetButtonDown("Shift");
	}
	public bool GetShift()
	{
		return Input.GetButton("Shift");
	}
	public bool GetShiftUp()
	{
		return Input.GetButtonUp("Shift");
	}


	public bool AnyAxis()
	{
		return (GetVerticalAxisRaw() != 0 || GetHorizontalAxisRaw() != 0);
	}

	public int GetVerticalAxisRaw()
	{
		return (int)Input.GetAxisRaw("Vertical");
	}
	public float GetVerticalAxis()
	{
		return Input.GetAxis("Vertical");
	}

	public int GetHorizontalAxisRaw()
	{
		return (int)Input.GetAxisRaw("Horizontal");
	}
	public float GetHorizontalAxis()
	{
		return Input.GetAxis("Horizontal");
	}

	public bool GetHorizontalAxisDown()
	{
		return Input.GetButtonDown("Horizontal");
	}
	public bool GetHorizontalAxisHold()
	{
		return Input.GetButton("Horizontal");
	}
	public bool GetHorizontalAxisUp()
	{
		return Input.GetButtonUp("Horizontal");
	}

	public bool GetVerticalAxisDown()
	{
		return Input.GetButtonDown("Vertical");
	}
	public bool GetVerticalAxisHold()
	{
		return Input.GetButton("Vertical");
	}
	public bool GetVerticalAxisUp()
	{
		return Input.GetButtonUp("Vertical");
	}
}
