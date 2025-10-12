using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

 namespace CombatEditor
{	
#if UNITY_EDITOR
	public class AbilityEventPreview_CreateObjWithHandle : AbilityEventPreview
	{
	    public AbilityEventObj_CreateObjWithHandle EventObj => (AbilityEventObj_CreateObjWithHandle)_EventObj;
	    public GameObject InstantiatedObj;
	    public Vector3 ControllerStartPosition;
	
	    public Vector3 NodePosAtStartFrame = Vector3.zero;
	    public Quaternion NodeRotAtStartFrame = Quaternion.identity;
	    public Quaternion AnimatorRotAtStartFrame = Quaternion.identity;
	
	    PreviewTransformHandle handle;
	    public AbilityEventPreview_CreateObjWithHandle(AbilityEventObj Obj) : base(Obj)
	    {
	        _EventObj = Obj;
	    }
	
	    public override void InitPreview()
	    {
	        base.InitPreview();
	        if (EventObj.ObjData == null) EventObj.ObjData = new InsedObject();
	        if(EventObj.ObjData.TargetObj == null)
	        {
	            return;
	        }
	        CreateObj();
	        CreateHandle();
	    }
	    public override void PreviewUpdateFrame(float CurrentTimePercentage)
	    {
	        base.PreviewUpdateFrame(CurrentTimePercentage);
	        if(handle!=null)
	        {
	            handle.UpdateTransformData();
	        }
	    }
	
	
	
	    public void CreateObj()
	    {
	        InstantiatedObj = (GameObject)PrefabUtility.InstantiatePrefab(EventObj.ObjData.TargetObj);
	        InstantiatedObj.transform.rotation = _combatController._animator.transform.rotation * EventObj.ObjData.Rot;
	        InstantiatedObj.transform.SetParent(previewGroup.transform);
	    }
	    public void CreateHandle()
	    {
	        handle = GameObject.Find(CombatGlobalEditorValue.PreviewGroupName).AddComponent<PreviewTransformHandle>();
	        handle.TargetTrans = InstantiatedObj.transform;
	        handle.InsObjData = EventObj.ObjData;
	        handle.Init();
	        handle._combatController = _combatController;
	        handle._preview = this;
	    }
	    public override void DestroyPreview()
	    {
	        if (InstantiatedObj != null)
	        {
	            Object.DestroyImmediate(InstantiatedObj);
	        }
	        base.DestroyPreview();
	    }
	
	
	    //If the object is static, it requires datas at start frame.
	    public override bool NeedStartFrameValue()
	    {
	        return true;
	    }
	    // If the object is static, give the handle the position and rot at start frame.
	    public override void GetStartFrameDataBeforePreview()
	    {
	        base.GetStartFrameDataBeforePreview();
	        if (InstantiatedObj == null)
	        {
	            return;
	        }
	        DataToHandleAtStartFrame();
	    }
	    public void DataToHandleAtStartFrame()
	    {
	        ControllerStartPosition = CombatGlobalEditorValue.CharacterRootCenterAtCurrentFrame;
	
	        AnimatorRotAtStartFrame = _combatController.GetNodeTranform(CharacterNode.NodeType.Animator).rotation;
	        if (EventObj.ObjData.TargetNode == CharacterNode.NodeType.Animator)
	        {
	            NodePosAtStartFrame = ControllerStartPosition;
	            NodeRotAtStartFrame = AnimatorRotAtStartFrame;
	        }
	        else
	        {
	            Transform trans = _combatController.GetNodeTranform(EventObj.ObjData.TargetNode);
	            if (trans != null)
	            {
	                NodePosAtStartFrame = CombatGlobalEditorValue.CharacterRootCenterAtCurrentFrame + (trans.position - _combatController._animator.transform.position - CombatGlobalEditorValue.CurrentRootMotionOffset);
	                NodeRotAtStartFrame = trans.rotation;
	            }
	        }
	        if (handle != null)
	        {
	            handle.SetStartFramePos(NodePosAtStartFrame, NodeRotAtStartFrame, AnimatorRotAtStartFrame);
	        }
	    }
	}
#endif
}
