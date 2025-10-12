using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{	
	[System.Serializable]
	public class InsedObject
	{
	    public GameObject TargetObj;
	    public PreviewTransformHandle.ControlTypeEnum controlType;
	    public Vector3 Offset;
	    public Quaternion Rot;
	    public CharacterNode.NodeType TargetNode;
	    public bool FollowNode = true;
	    public bool RotateByNode;
	
	    public GameObject CreateObject( CombatController controller)
	    {
	        GameObject _obj = null;
	        if (TargetObj != null)
	        {
	            _obj = Object.Instantiate(TargetObj);
	
	            var follower = _obj.AddComponent<NodeFollower>();
	            follower.Init(
	                controller.GetNodeTranform(TargetNode),
	                Offset,
	                Rot,
	                FollowNode,
	                RotateByNode,
	                controller
	                );
	        }
	        return _obj;
	    }
	
	}
	
	[AbilityEvent]
	[CreateAssetMenu(menuName = "AbilityEvents / Particles")]
	public class AbilityEventObj_Particles : AbilityEventObj_CreateObjWithHandle
	{
	    //public InsedObject ParticleData = new InsedObject();
	    public EventTimeType TimeType = EventTimeType.EventTime;
	    public override EventTimeType GetEventTimeType()
	    {
	        return TimeType;
	    }
	
	    public override AbilityEventEffect Initialize()
	    {
	        return new AbilityEventEffect_Particles(this);
	    }
	
# if UNITY_EDITOR
	    public override AbilityEventPreview InitializePreview()
	    {
	        return new AbilityEventPreview_Particles(this);
	    }
#endif
	}
	
	
	public class AbilityEventEffect_Particles : AbilityEventEffect
	{
	    AbilityEventObj_Particles Obj => (AbilityEventObj_Particles)_EventObj;
	    //Vector3 TargetPos => _combatController.transform.position + _combatController._animator.transform.rotation * Obj.Offset;
	
	    GameObject InsedParticle;
	    public AbilityEventEffect_Particles(AbilityEventObj Obj) : base(Obj)
	    {
	        _EventObj = Obj;
	    }
	
	    public override void EndEffect()
	    {
	        base.EndEffect();
	        if (Obj.GetEventTimeType() == AbilityEventObj.EventTimeType.EventRange)
	        {
	            if (InsedParticle != null)
	            {
	                Object.Destroy(InsedParticle);
	            }
	        }
	    }
	    public override void StartEffect()
	    {
	        base.StartEffect();
	        InsedParticle = Obj.ObjData.CreateObject(_combatController);
	    }
	
	}
}
