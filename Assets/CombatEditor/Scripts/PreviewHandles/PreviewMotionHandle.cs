using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
	
 namespace CombatEditor {	
	public class PreviewMotionHandle : PreviewerOnObject
	{
	    public Transform TargetTrans;
	    public  MotionTarget target;
	    public Vector3 StartPosition;
	    public override void SelfDestroy()
	    {
	        if(_combatController!=null)
	        {
	            _combatController.transform.position = StartPosition;
	        }
	        base.SelfDestroy();
	    }
	
	    public override void PaintHandle()
	    {
	        if (!_preview.eve.Previewable)
	        {
	            UpdateSelfTransByData();
	            return;
	        }
	        if (Selection.activeObject == gameObject)
	        {
	            Tools.hidden = true;
	        }
	        else
	        {
	            Tools.hidden = false;
	        }
	
	        EditorGUI.BeginChangeCheck();
	        var PosAfterMove = Handles.PositionHandle(TargetTrans.position, TargetTrans.rotation);
	        if (EditorGUI.EndChangeCheck())
	        {
	            TargetTrans.position = PosAfterMove;
	
	            SetTransformDataToEventObj(TargetTrans);
	        }
	
	        UpdateSelfTransByData();
	
	        Handles.SphereHandleCap(0, StartFramePos, Quaternion.identity, 0.1f, EventType.Repaint);
	        Handles.DrawLine(StartFramePos, TargetTrans.position);
	
	    }
	
	    public void SetTransformDataToEventObj(Transform PreviewTransform)
	    {
	        var StartFrameOffset = PreviewTransform.position - StartFramePos;
	        target.Offset = Quaternion.Inverse(StartFrameRot) * StartFrameOffset;
	    }
	
	
	    Vector3 StartFramePos;
	    Quaternion StartFrameRot;
	    Quaternion StartAnimatorRot;
	    public void SetStartFramePos(Vector3 pos, Quaternion rot, Quaternion AnimatorRot)
	    {
	        StartFramePos = pos;
	        StartFrameRot = rot;
	        StartAnimatorRot = AnimatorRot;
	    }
	    public void UpdateSelfTransByData()
	    {
	        Vector3 TargetPos = Vector3.zero;
	        Quaternion TargetRot = Quaternion.identity;
	        TargetPos = StartFramePos + StartFrameRot * target.Offset;
	        TargetRot = StartAnimatorRot;
	
	        TargetTrans.position = TargetPos;
	        TargetTrans.rotation = StartAnimatorRot;
	    }
	
	}
}
#endif
