using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{	
	public class AbilityEventPreview_Motion : AbilityEventPreview
	{
	    public AbilityEventObj_Motion Obj => (AbilityEventObj_Motion)_EventObj;
	    public AbilityEventPreview_Motion(AbilityEventObj Obj) :base(Obj)
	    {
	        _EventObj = Obj;
	    }
	
#if UNITY_EDITOR
	    public override void InitPreview()
	    {
	        base.InitPreview();
	        CreateMotionTarget();
	        CreateMotionHandles();
	    }
	    public override bool NeedStartFrameValue()
	    {
	        return true;
	    }
	    public GameObject PreviewTarget;
	    public void CreateMotionTarget()
	    {
	        PreviewTarget = new GameObject("Preview_Motion");
	        PreviewTarget.transform.SetParent(previewGroup.transform);
	    }
	    PreviewMotionHandle handle;
	    public void CreateMotionHandles()
	    {
	        handle = previewGroup.AddComponent<PreviewMotionHandle>();
	        handle.StartPosition = _combatController.transform.position;
	
	        handle.TargetTrans = PreviewTarget.transform;
	        //handle = PreviewTarget.AddComponent<PreviewMotionHandle>();
	        handle.Init();
	        handle.target = Obj.target;
	        handle._combatController = _combatController;
	        handle._preview = this;
	        //AddMotionHandles
	    }
	    public override void PreviewRunning(float CurrentTimePercentage)
	    {
	        base.PreviewRunning(CurrentTimePercentage);
	        Obj.MotionTime = (EndTimePercentage - StartTimePercentage) * AnimObj.Clip.length;
	        if (PreviewInRange(CurrentTimePercentage) || CurrentTimePercentage > EndTimePercentage)
	        {
	            _combatController.transform.position = CombatGlobalEditorValue.CharacterTransPosBeforePreview + GetOffsetAtCurrentFrame(CurrentTimePercentage);
	            
	        }
	        else
	        {
	            _combatController.transform.position = CombatGlobalEditorValue.CharacterTransPosBeforePreview;
	            //_combatController.transform.position = Vector3.zero;
	        }
	
	        //if (PreviewInRange(CurrentTimePercentage) || CurrentTimePercentage > EndTimePercentage)
	        //{
	        //    var TimePercentage = (CurrentTimePercentage - StartTimePercentage) / (EndTimePercentage - StartTimePercentage);
	        //    if (TimePercentage >= 1) TimePercentage = 1;
	
	
	            //    float DistancePercentage = 0;
	            //    if (Obj.MotionCurve != null)
	            //    {
	            //        DistancePercentage = Obj.MotionCurve.Evaluate(TimePercentage);
	            //    }
	            //    else
	            //    {
	            //        Debug.Log("Please Config the MotionCurve.");
	            //    }
	            //    _combatController.transform.position = handle.StartPosition + _combatController._animator.transform.rotation *
	            //        (DistancePercentage * Obj.target.Offset);
	            //}
	            //else
	            //{
	            //    _combatController.transform.position = Vector3.zero;
	            //}
	    }
	
	    public Vector3 GetOffsetAtCurrentFrame(float CurrentTimePercentage)
	    {
	        Obj.MotionTime = (EndTimePercentage - StartTimePercentage) * AnimObj.Clip.length;
	
	
	        if (PreviewInRange(CurrentTimePercentage) || CurrentTimePercentage > EndTimePercentage)
	        {
	            var TimePercentage = (CurrentTimePercentage - StartTimePercentage) / (EndTimePercentage - StartTimePercentage);
	            if (TimePercentage >= 1) TimePercentage = 1;
	
	            float DistancePercentage = 0;
	            if (Obj.TimeToDis != null)
	            {
	                DistancePercentage = Obj.TimeToDis.Evaluate(TimePercentage);
	            }
	            else
	            {
	                Obj.TimeToDis = new AnimationCurve();
	                Obj.TimeToDis.AddKey(0, 0);
	                Obj.TimeToDis.AddKey(1, 1);
	                //Debug.Log("Please Config the MotionCurve.");
	            }
	            var offset = _combatController._animator.transform.rotation * (DistancePercentage * Obj.target.Offset);
	            return offset;
	        }
	        return Vector3.zero;
	    }
	
	
	    public override void DestroyPreview()
	    {
	        //_combatController.transform.position = handle.StartPosition;
	        _combatController.transform.position = CombatGlobalEditorValue.CharacterTransPosBeforePreview;
	        base.DestroyPreview();
	    }
	    public override void GetStartFrameDataBeforePreview()
	    {
	        base.GetStartFrameDataBeforePreview();
	        FetchDataAtStartFrame();
	    }
	    public Vector3 NodePosAtStartFrame = Vector3.zero;
	    public Quaternion NodeRotAtStartFrame = Quaternion.identity;
	    public Quaternion AnimatorRotAtStartFrame = Quaternion.identity;
	    public Vector3 ControllerStartPosition;
	
	    public void FetchDataAtStartFrame()
	    {
	        ControllerStartPosition = handle.StartPosition + _combatController._animator.transform.rotation * CombatGlobalEditorValue.CurrentMotionTAtGround;
	        AnimatorRotAtStartFrame = _combatController.GetNodeTranform(CharacterNode.NodeType.Animator).rotation;
	
	        NodePosAtStartFrame = ControllerStartPosition;
	        NodeRotAtStartFrame = AnimatorRotAtStartFrame;
	
	
	        //Debug.Log(handle.StartPosition +":"+ _combatController._animator.transform.rotation * CombatGlobalEditorValue.CurrentMotionTAtGround);
	        //Debug.Log(NodePosAtStartFrame);
	
	        handle.SetStartFramePos(NodePosAtStartFrame, NodeRotAtStartFrame, AnimatorRotAtStartFrame);
	    }
#endif
	}
}
