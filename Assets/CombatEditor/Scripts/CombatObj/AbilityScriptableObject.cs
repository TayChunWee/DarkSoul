using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

 namespace CombatEditor
{
    [System.Serializable]
    public class AbilityEvent
    {

        public float EventTime;
        public Vector2 EventRange = new Vector2(0, 1);
        public float[] EventMultiRange = new float[4]{0.2f,0.4f,0.6f,0.8f}; 
	    public bool Previewable;
        public AbilityEventObj Obj;

        public void ResetAbilityEvent()
	    {
	        if (Obj != null)
	        {
	   
	        }
	    }
	    public float GetEventStartTime()
	    {
	        if(Obj == null)
	        {
	            return 0;
	        }
	        if (Obj.GetEventTimeType() == AbilityEventObj.EventTimeType.Null)
	        {
	            return 0;
	        }
	        if (Obj.GetEventTimeType() == AbilityEventObj.EventTimeType.EventTime)
	        {
	            return EventTime;
	        }
	        if (Obj.GetEventTimeType() == AbilityEventObj.EventTimeType.EventRange)
	        {
	            return EventRange.x;
	        }
            if(Obj.GetEventTimeType() == AbilityEventObj.EventTimeType.EventMultiRange)
            {
                return EventRange.x;
            }

	        return 0;
	    }
	    public float GetEventEndTime()
	    {
	        if(!Obj)
	        {
	            return 0;
	        }
	        if (Obj.GetEventTimeType() == AbilityEventObj.EventTimeType.EventRange)
	        {
	            return EventRange.y;
	        }
            if (Obj.GetEventTimeType() == AbilityEventObj.EventTimeType.EventMultiRange)
            {
                return EventRange.y;
            }
            return 1;
	    }
        public AbilityEventObj.EventTimeType GetEventTimeType()
        {
            return Obj.GetEventTimeType();
        }

    }
	
	[CreateAssetMenu(menuName =("AbilityObj"))]
	public class AbilityScriptableObject : ScriptableObject
	{
	    public AbilityTypes AbilityType;
	      
	    public enum AbilityTypes { OneShot, Loop , BlendingTree_1D, BlendingTree_2D }
	    public AnimationClip Clip;
	    [HideInInspector]
	    public Vector2 PreviewPercentageRange = new Vector2(0, 1);
	    //public float Speed = 1;
	    public float loopCount = 0;
	    
	    public List<AbilityEvent> events = new List<AbilityEvent>();
	
	    public void ResetEvent()
	    {
	        EventManager.TriggerEvent("ChangeAbilityEvent");
	    }
	
	
	}
}
