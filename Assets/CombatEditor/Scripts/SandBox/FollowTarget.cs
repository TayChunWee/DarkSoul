using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{	
	public class FollowTarget : MonoBehaviour
	{
	    Transform target;
	    Vector3 offset;
	    public void SetTarget( Transform Target ,Vector3 Offset)
	    {
	        target = Target;
	        offset = Offset;
	    }
	    private void Update()
	    {
	        if (target != null)
	        {
	            transform.position = target.position + offset;
	        }
	    }
	}
}
