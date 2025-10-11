using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{
    
	//DisaplayHandles and control Object
	public class PreviewTransformHandle : PreviewerOnObject
	{
	    public InsedObject InsObjData;
	    public enum ControlTypeEnum { Translation, Rotation, Scale };
	    public ControlTypeEnum ControlType;
	
	    public Transform TargetTrans;
	
#if UNITY_EDITOR
	
	    public Action<Transform> ModifyTrans;
	
	    Vector3 StartFramePos;
	    Quaternion StartFrameRot;
	    Quaternion StartAnimatorRot;
	
	    public bool Previewable = false;
	
	    public void SetStartFramePos(Vector3 pos, Quaternion rot , Quaternion AnimatorRot)
	    {
	        StartFramePos = pos;
	        StartFrameRot = rot;
	        StartAnimatorRot = AnimatorRot;
	    }
	    public override void UpdateHiddenHandle()
	    {
	        UpdateSelfTransByData();
	    }
	    public override void PaintHandle()
	    {
	        if (Selection.activeObject == gameObject)
	        {
	            Tools.hidden = true;
	        }
	        else
	        {
	            Tools.hidden = false;
	        }
	        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
	
	        Vector3 controllerPos = Vector3.zero;    //Handles.SphereHandleCap(0, controllerPos, Quaternion.identity, 0.3f, EventType.Repaint);
	        //Handles.CircleHandleCap(0, controllerPos, Quaternion.Euler(90, 0, 0), 1f, EventType.Repaint);
	        Handles.Label(TargetTrans.position, TargetTrans.name);
	
	        if (Tools.current == Tool.Move)
	        {
	            EditorGUI.BeginChangeCheck();
	            var PosAfterMove = Handles.PositionHandle(TargetTrans.position, TargetTrans.rotation);
	            if (EditorGUI.EndChangeCheck())
	            {
	                TargetTrans.position = PosAfterMove;
	
	                SetTransformDataToEventObj(TargetTrans);
	            }
	        }
	
	        Handles.color = Color.green;
	        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
	        if (Tools.current == Tool.Rotate)
	        {
	            EditorGUI.BeginChangeCheck();
	            Quaternion rot = Handles.RotationHandle(TargetTrans.rotation, TargetTrans.position);
	            if (EditorGUI.EndChangeCheck())
	            {
	                TargetTrans.rotation = rot;
	                Undo.RecordObject(TargetTrans, "Rotated RotateAt Point");
	
	                SetTransformDataToEventObj(TargetTrans);
	                //ModifyTrans.Invoke(TargetTrans);
	            }
	        }
	        if (Tools.current == Tool.Scale)
	        {
	            //EditorGUI.BeginChangeCheck();
	            //Vector3 localScale = Handles.ScaleHandle(TargetTrans.localScale, TargetTrans.position, TargetTrans.rotation);
	            //if (EditorGUI.EndChangeCheck())
	            //{
	            //    TargetTrans.localScale = localScale;
	            //    Undo.RecordObject(TargetTrans, "Rotated RotateAt Point");
	
	            //    SetTransformDataToEventObj(TargetTrans);
	            //}
	        }
	
	    }
	    public override void UpdateTransformData()
	    {
	        UpdateSelfTransByData();
	    }
	
	
	    public void UpdateSelfTransByData()
	    {
	
	        Vector3 TargetPos = Vector3.zero;
	        Quaternion TargetRot = Quaternion.identity;
	
	
	        //If static, position and rotation is based on StartFrame.
	        //Is !FollowRot, rot by AnimatorFront, else rot by joint rotation.
	        if (!InsObjData.FollowNode)
	        {
	            TargetPos = StartFramePos + StartFrameRot * InsObjData.Offset;
	            if (InsObjData.RotateByNode)
	            {
	                TargetRot = StartFrameRot * InsObjData.Rot;
	            }
	            if (!InsObjData.RotateByNode)
	            {
	                TargetRot = StartAnimatorRot * InsObjData.Rot;
	            }
	        }
	        //If not static, position and rotation is based on currentFrame.
	        //Is !FollowRot, rot by AnimatorFront, else rot by joint rotation.
	        else
	        {
	            //Need To Add RootMotion cause root motion dont move the animator in editor mode
	            Transform trans = _combatController.GetNodeTranform(InsObjData.TargetNode);
	            Vector3 NodePos = trans.position;
	            TargetPos = NodePos + trans.rotation * InsObjData.Offset;
	            if (InsObjData.TargetNode == CharacterNode.NodeType.Animator)
	            {
	                TargetPos += trans.rotation * CombatGlobalEditorValue.CurrentMotionTAtGround;
	            }
	
	            if (InsObjData.RotateByNode)
	            {
	                TargetRot = trans.rotation * InsObjData.Rot;
	            }
	
	            if (!InsObjData.RotateByNode)
	            {
	                TargetRot = _combatController.GetNodeTranform(CharacterNode.NodeType.Animator).rotation * InsObjData.Rot;
	            }
	        }
	        TargetTrans.position = TargetPos;
	        TargetTrans.rotation = TargetRot;
	    }
	
	
	    public void SetTransformDataToEventObj(Transform PreviewTransform)
	    {
	        SetOffset(PreviewTransform);
	        SetRot(PreviewTransform);
	        SetScale(PreviewTransform);
	    }
	
	    public void SetOffset(Transform PreviewTransform)
	    {
	        if (!InsObjData.FollowNode)
	        {
	            var StartFrameOffset = PreviewTransform.position - StartFramePos;
	            InsObjData.Offset = Quaternion.Inverse(StartFrameRot) * StartFrameOffset;
	        }
	        else
	        {
	            Transform trans = _combatController.GetNodeTranform(InsObjData.TargetNode);
	            Vector3 OffsetWithRotation = PreviewTransform.position - trans.position;
	            if (InsObjData.TargetNode == CharacterNode.NodeType.Animator)
	            {
	                OffsetWithRotation -= trans.rotation * CombatGlobalEditorValue.CurrentMotionTAtGround;
	            }
	            InsObjData.Offset = Quaternion.Inverse(trans.rotation) * OffsetWithRotation;
	        }
	    }
	    public void SetRot(Transform PreviewTransform)
	    {
	        var AnimatorRot = _combatController._animator.transform.rotation;
	
	
	        if (!InsObjData.FollowNode)
	        {
	
	            if (InsObjData.RotateByNode)
	            {
	                InsObjData.Rot = Quaternion.Inverse(StartFrameRot) * PreviewTransform.rotation;
	            }
	            if (!InsObjData.RotateByNode)
	            {
	                InsObjData.Rot = Quaternion.Inverse(StartAnimatorRot) * PreviewTransform.rotation;
	            }
	        }
	        else
	        {
	            Transform trans = _combatController.GetNodeTranform(InsObjData.TargetNode);
	            if (InsObjData.RotateByNode)
	            {
	                InsObjData.Rot = Quaternion.Inverse(trans.rotation) * PreviewTransform.rotation;
	            }
	            if (!InsObjData.RotateByNode)
	            {
	                InsObjData.Rot = Quaternion.Inverse(AnimatorRot) * PreviewTransform.rotation;
	            }
	        }
	        TargetTrans.localScale = PreviewTransform.localScale;
	    }
	
	    public void SetScale(Transform PreviewTransform)
	    {
	        TargetTrans.localScale = PreviewTransform.localScale;
	    }
	
	    //public Action UpdateTransfrom(Transform trans)
	    //{
	
	    //}
	
#endif
	}
	
}
