using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{	
	public class MoveExecutor
	{
	    public CombatController _combatController;
        RootMotionReceiver receiver;
        public MoveExecutor(CombatController _controller)
        {
            _combatController = _controller;
            receiver = _combatController._animator.gameObject.AddComponent<RootMotionReceiver>();
        }
	    public void Execute()
	    {
	        
	    }
        /// <summary>
        /// Remember to change this to the physics you desire.
        /// </summary>
        /// <param name="DeltaMove"></param>
	    public void Move(Vector3 DeltaMove)
	    {
            _combatController.transform.Translate(DeltaMove, Space.World);
        }
	    public Vector3 GetCurrentRootMotion()
	    {
	        return receiver.CurrentRootMotion;
	    }
	
	}
}
