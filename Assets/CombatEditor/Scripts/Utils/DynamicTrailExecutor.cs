using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{	
	
	public class DynamicTrailExecutor : MonoBehaviour
	{
	    public DynamicTrailGenerator trail;
	    public bool TrailEnabled = false;
	    public void StartTrail()
	    {
	        TrailEnabled = true;
	    }
	    public void StopTrail()
	    {
	        TrailEnabled = false;
	    }
	    private void Update()
	    {
	        if (TrailEnabled)
	        {
	            trail.UpdateTrailOnCurrentFrame();
	        }
	        else
	        {
	            trail.StopTrailSmoothly();
	            if (trail.QuadCount == 0)
	            {
	                Destroy(gameObject);
	            }
	        }
	    }
	}
	
}
