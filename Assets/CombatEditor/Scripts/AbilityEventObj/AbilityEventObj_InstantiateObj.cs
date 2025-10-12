using UnityEngine;
namespace CombatEditor
{
    [AbilityEvent]
    [CreateAssetMenu(menuName = "AbilityEvents / CreateObjWithHandle")]
    public class AbilityEventObj_CreateObjWithHandle : AbilityEventObj
    {
        public InsedObject ObjData = new InsedObject();
        public override EventTimeType GetEventTimeType()
        {
            return EventTimeType.EventTime;
        }
        public override AbilityEventEffect Initialize()
        {
            return new AbilityEventEffect_CreateObjWithHandle(this);
        }
#if UNITY_EDITOR
        public override AbilityEventPreview InitializePreview()
        {
            return new AbilityEventPreview_CreateObjWithHandle(this);
        }
#endif

    }
    public partial class AbilityEventEffect_CreateObjWithHandle : AbilityEventEffect
    {
        public GameObject InstantiatedObj;

        public override void StartEffect()
        {
            base.StartEffect();

            InstantiatedObj = EventObj.ObjData.CreateObject(_combatController);
        }
        public override void EffectRunning()
        {
            base.EffectRunning();
        }
        public override void EndEffect()
        {
            base.EndEffect();
        }
    }
    public partial class AbilityEventEffect_CreateObjWithHandle : AbilityEventEffect
    {
        AbilityEventObj_CreateObjWithHandle EventObj => (AbilityEventObj_CreateObjWithHandle)_EventObj;
        public AbilityEventEffect_CreateObjWithHandle(AbilityEventObj InitObj) : base(InitObj)
        {
            _EventObj = InitObj;
        }
    }
}
