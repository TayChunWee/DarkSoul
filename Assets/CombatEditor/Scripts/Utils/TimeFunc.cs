using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{	
	public class TimeFunc : MonoBehaviour
	{
	    public void StartInvoke2(Action a1, Action a2, float Time)
	    {
	        StartCoroutine(StartInvoke2IE(a1,a2,Time));
	    }
	    public IEnumerator StartInvoke2IE( Action a1,Action a2, float Time)
	    {
	        a1.Invoke();
	        yield return new WaitForSeconds(Time);
	        a2.Invoke();
	    }
	}
}
