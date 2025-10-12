using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{	
	[System.Serializable]
	public class TweenCurve
	{
	    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
	    public float StartValue = 1;
	    public float EndValue = 0;
	
	
	    public float GetCurveValue(float StartTime,float EndTime,float CurrentTime)
	    {
	       if(CurrentTime< StartTime)
	        {
	            return StartValue;
	        }
	        if (CurrentTime > EndTime)
	        {
	            return EndValue;
	        }
	        return StartValue + (EndValue - StartValue) *  curve.Evaluate( (CurrentTime - StartTime) / ( EndTime - StartTime ));
	    }
	}
}
