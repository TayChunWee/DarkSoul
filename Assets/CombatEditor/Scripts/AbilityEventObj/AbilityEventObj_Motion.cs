using UnityEngine;

 namespace CombatEditor {	
	[System.Serializable]
	public class MotionTarget
	{
	    public Vector3 Offset;
	    public GameObject CreateObject(CombatController controller)
	    {
	        var _obj = new GameObject("TargetPoint");
	        return _obj;
	    }
	}
	
	[AbilityEvent]
	[CreateAssetMenu(menuName = "AbilityEvents / Motion")]
	public class AbilityEventObj_Motion : AbilityEventObj
	{
	    public MotionTarget target;
	    [ReadOnly]
	    public float MotionTime;
	    [MyAnimationCurve]
	    public AnimationCurve TimeToDis;
	
	
	    public override EventTimeType GetEventTimeType()
	    {
	        return EventTimeType.EventRange;
	    }
	    public override AbilityEventEffect Initialize()
	    {
	        return new AbilityEventEffect_Motion(this);
	    }
	    public override AbilityEventPreview InitializePreview()
	    {
	        return new AbilityEventPreview_Motion(this);
	    }
	}
	public partial class AbilityEventEffect_Motion : AbilityEventEffect
	{
	    float CurrentSpeed;
	    public override void StartEffect()
	    {
	        base.StartEffect();
	    }
	    float LastFrameDistance = 0;
	    public override void EffectRunning(float CurrentTimePercentage)
	    {
	        base.EffectRunning(CurrentTimePercentage);
	
	        var timePercentage = (CurrentTimePercentage - eve.GetEventStartTime()) / (eve.GetEventEndTime() - eve.GetEventStartTime());
	
	        var TargetDistance = TargetObj.TimeToDis.Evaluate(timePercentage);
	
	        var DeltaDistance = TargetDistance - LastFrameDistance;
	
	        LastFrameDistance = TargetDistance;
	
	        _combatController.SimpleMoveRG(_combatController._animator.transform.rotation * TargetObj.target.Offset * DeltaDistance);
          
	    }
	    public override void EndEffect()
	    {
	        LastFrameDistance = 0;
	        base.EndEffect();
	    }
	}
	public partial class AbilityEventEffect_Motion : AbilityEventEffect
	{
	    AbilityEventObj_Motion TargetObj => (AbilityEventObj_Motion)_EventObj;
	    public AbilityEventEffect_Motion(AbilityEventObj InitObj) : base(InitObj)
	    {
	        _EventObj = InitObj;
	    }
	}
}
