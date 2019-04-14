using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Yielders
{
	private static Dictionary<float, WaitForSeconds> intervals = new Dictionary<float, WaitForSeconds>(100);
	
	private static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
	public static WaitForEndOfFrame endOfFrame {
		get { return _endOfFrame;}
	}
 
	private static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
	public static WaitForFixedUpdate fixedUpdate {
		get { return _fixedUpdate; }
	}
 


	public static WaitForSeconds Wait(float duration)
	{
		WaitForSeconds res;

		if(!intervals.ContainsKey(duration)) {
			res = new WaitForSeconds(duration);
			intervals.Add(duration, res);
		} else {
			res = intervals[duration];
		}

		return res;
	}
}
