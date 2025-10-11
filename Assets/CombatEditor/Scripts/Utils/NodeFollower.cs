using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{	
	public class NodeFollower : MonoBehaviour
	{
	    public Transform NodeTrans;
	    public bool FollowPos;
	    public bool FollowRotation;
	    public Vector3 PosOffset;
	    public Quaternion RotOverNode;
	    public CombatController _controller;
	    public void Init(Transform trans , Vector3 Offset , Quaternion Rot, bool followPos, bool followRot,CombatController controller)
	    {
	        NodeTrans = trans;
	        PosOffset = Offset;
	        RotOverNode = Rot;
	        FollowPos = followPos;
	        FollowRotation = followRot;
	        _controller = controller;
	
	        transform.position = NodeTrans.position + NodeTrans.rotation * PosOffset;
	        if (FollowRotation)
	        {
	            transform.rotation = NodeTrans.rotation * RotOverNode;
	        }
	        else
	        {
	            transform.rotation =  _controller.GetNodeTranform(CharacterNode.NodeType.Animator).rotation * RotOverNode;
	        }
	    }
	    public void SetTransform()
	    {
	        if (FollowPos)
	        {
	            transform.position = NodeTrans.position + NodeTrans.rotation * PosOffset;
	        }
	        if (FollowRotation && FollowPos)
	        {
	            transform.rotation = NodeTrans.rotation * RotOverNode;
	        }
	    }
	
	    private void Update()
	    {
	        SetTransform();
	    }
	}
}
