using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

 namespace CombatEditor
{
	public class EventReceiver : MonoBehaviour
	{
	    public string eventString;
	    public UnityEvent e;
	    private void OnEnable()
	    {
	        EventManager.StartListening(eventString,invoke);
	    }
	    public void invoke()
	    {
	        e.Invoke();
	    }
	
	}
}
