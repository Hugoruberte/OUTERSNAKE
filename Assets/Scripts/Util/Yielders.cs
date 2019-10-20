using System.Collections.Generic;
using UnityEngine;
// using static System.Func<bool>;

public static class Yielders
{
	private static Dictionary<float, WaitForSeconds> intervals = new Dictionary<float, WaitForSeconds>(100);
	
	public static readonly WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
	public static readonly WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
 


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

	public static WaitUntil Until(WaitUntil handle, System.Func<bool> predicate)
	{
		if(handle == null) {
			handle = new WaitUntil(predicate);
		}

		return handle;
	}

	public static WaitWhile Until(WaitWhile handle, System.Func<bool> predicate)
	{
		if(handle == null) {
			handle = new WaitWhile(predicate);
		}

		return handle;
	}
}
