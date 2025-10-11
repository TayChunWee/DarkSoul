using UnityEngine;
 namespace CombatEditor
{	
	[AbilityEvent]
	[CreateAssetMenu(menuName = "AbilityEvents / DynamicTrail")]
	public class AbilityEventObj_DynamicTrail : AbilityEventObj
	{
	    public CharacterNode.NodeType BaseNode;
	    public CharacterNode.NodeType TipNode;
	    public Material TrailMat;
	    public int MaxFrame = 50;
	    public int StopMultiplier = 4;
	    [Range(2,8)]
	    public int TrailSubs = 2;
	    [HideInInspector]
	    public int NUM_VERTICES = 12;
	
	    [System.Serializable]
	    public enum TrailBehavior { FlowUV, StaticUV }
	    //[SerializeField]
	    //public TrailBehavior uvMethod;
	
	    //Write the data you need here.
	    public override EventTimeType GetEventTimeType()
	    {
	        return EventTimeType.EventRange;
	    }
	    public override AbilityEventEffect Initialize()
	    {
	        return new AbilityEventEffect_DynamicTrail(this);
	    }
#if UNITY_EDITOR
	    public override AbilityEventPreview InitializePreview()
	    {
	        return new AbilityEventPreview_DynamicTrail(this);
	    }
#endif
	}
	//Write you logic here
	public partial class AbilityEventEffect_DynamicTrail : AbilityEventEffect
	{
	    DynamicTrailGenerator trail;
	    Transform _base;
	    Transform _tip;
	    DynamicTrailExecutor executor;
	    public override void StartEffect()
	    {
	        base.StartEffect();
	
	
	        _base = _combatController.GetNodeTranform(EventObj.BaseNode);
	        _tip = _combatController.GetNodeTranform(EventObj.TipNode);
	        if (_base == null || _tip == null)
	        {
	            return;
	        }
	        trail = new DynamicTrailGenerator(_base, _tip, EventObj.MaxFrame, EventObj.TrailSubs, EventObj.StopMultiplier, EventObj.TrailMat, AbilityEventObj_DynamicTrail.TrailBehavior.FlowUV);
	        trail.InitTrailMesh();
	        executor = trail._trailMeshObj.AddComponent<DynamicTrailExecutor>();
	        executor.trail = trail;
	        executor.StartTrail();
	
	    }
	    public override void EffectRunning()
	    {
	        base.EffectRunning();
	    }
	    public override void EndEffect()
	    {
	        if (executor != null)
	        {
	            executor.StopTrail();
	        }
	        base.EndEffect();
	    }
	}
	
	public partial class AbilityEventEffect_DynamicTrail : AbilityEventEffect
	{
	    AbilityEventObj_DynamicTrail EventObj => (AbilityEventObj_DynamicTrail)_EventObj;
	    public AbilityEventEffect_DynamicTrail(AbilityEventObj InitObj) : base(InitObj)
	    {
	        _EventObj = InitObj;
	    }
	}
}
