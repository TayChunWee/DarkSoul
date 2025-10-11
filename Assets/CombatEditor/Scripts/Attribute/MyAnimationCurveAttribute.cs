using System;
using UnityEngine;
namespace CombatEditor
{	
	public class MyAnimationCurveAttribute:Attribute
	{
	   public MyAnimationCurveAttribute()
	    {
	        
	    }
	}
	[Serializable]
	public class MyAnimationCurve
	{
	    
	    public float Evaluate(float time)
	    {
	       return curve.Evaluate(time);
	    }
	    public AnimationCurve curve;
	    public float Scale;
	}
}
