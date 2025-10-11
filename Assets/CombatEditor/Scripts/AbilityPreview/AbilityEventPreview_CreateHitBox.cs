using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{	
#if UNITY_EDITOR
	public class AbilityEventPreview_CreateHitBox : AbilityEventPreview_CreateObjWithHandle
	{
	    public AbilityEventObj_CreateHitBox Obj => (AbilityEventObj_CreateHitBox)_EventObj;
	    public AbilityEventPreview_CreateHitBox(AbilityEventObj Obj) : base(Obj)
	    {
	        _EventObj = Obj;
	    }
	
	    public bool PreviewActive()
	    {
	        return eve.Previewable;
	    }
	
	    public override void InitPreview()
	    {
	        base.InitPreview();
	
	        if (Obj.ObjData.TargetObj == null)
	        {
	            return;
	        }
	        //AddControlScript.
	        CreateHitBoxHandles();
	    }
	    PreviewTransformHandle TransformHandle;
	    ColliderPreviewHandle ColliderHandle;
	    public void CreateHitBoxHandles()
	    {
	
	        ColliderHandle = InstantiatedObj.AddComponent<ColliderPreviewHandle>();
	
	        ColliderHandle._combatController = _combatController;
	        ColliderHandle._preview = this;
	        ColliderHandle.colliderPreview = this;
	        ColliderHandle.Init();
	    }
	    //SetCurrentParticleTime;
	    public override void PreviewRunning(float CurrentTime)
	    {
	        //Set Preview Position and Rotation
	        base.PreviewRunning(CurrentTime);
	    }
	
	    //Destroy Particles.
	    public override void DestroyPreview()
	    {
	        if (InstantiatedObj != null)
	        {
	            Object.DestroyImmediate(InstantiatedObj);
	        }
	        base.DestroyPreview();
	    }
	    
	}
#endif
}
