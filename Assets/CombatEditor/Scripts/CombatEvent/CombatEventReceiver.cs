using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{	
	public class CombatEventReceiver : MonoBehaviour
	{
	    public CombatController controller;
	
	    public List<string> CombatDatasID;
	
	    //public string CombatDatasID;
	    public void AnimEvent(string animData)
	    {
	        string ControllerID = animData.Split('_')[0];
	        if(controller.GetInstanceID().ToString() == ControllerID)
	        {
	            var AnimEventInfo = animData.Split('_');
	            int ReceivedControllerID = -1;
	            int.TryParse(AnimEventInfo[1],out ReceivedControllerID);
	            int ReceivedGroupID = -1;
	            int.TryParse(AnimEventInfo[2], out ReceivedGroupID);
	            int ReceivedDataID = -1;
	            int.TryParse(AnimEventInfo[4], out ReceivedDataID);
	            controller.StartEvent(ReceivedControllerID,ReceivedGroupID,ReceivedDataID);
	        }
	    }
	    public void EndAnimEvent(string animData)
	    {
	        string ControllerID = animData.Split('_')[0];
	        if (controller.GetInstanceID().ToString() == ControllerID)
	        {
	            var AnimEventInfo = animData.Split('_');
	            int ReceivedControllerID = -1;
	            int.TryParse(AnimEventInfo[1], out ReceivedControllerID);
	            int ReceivedGroupID = -1;
	            int.TryParse(AnimEventInfo[2], out ReceivedGroupID);
	            int ReceivedDataID = -1;
	            int.TryParse(AnimEventInfo[4], out ReceivedDataID);
	            controller.EndEvent(ReceivedControllerID, ReceivedGroupID, ReceivedDataID);
	        }
	    }
	}
}
