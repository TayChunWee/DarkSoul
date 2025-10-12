using UnityEngine;
//Replace the "States" with the event you want to create
//If you want to create a object with handle in preview, please inherit the "AbilityEventObj" with "AbilityEventObj_CreateObjWithHandle"

namespace CombatEditor
{

    [AbilityEvent]
    [CreateAssetMenu(menuName = "AbilityEvents / States")]
    public class AbilityEventObj_States : AbilityEventObj
    {
        [Range(1,5)]
        public int DivideCount = 3;
        public string[] States = new string[5];
        

        //Write the data you need here.
        public override EventTimeType GetEventTimeType()
        {
            return EventTimeType.EventMultiRange;
        }
        public override int GetMultiRangeCount()
        {
            return DivideCount;
        }
        public override AbilityEventEffect Initialize()
        {
            return new AbilityEventEffect_States(this);
        }
    }
    //Write you logic here
    public partial class AbilityEventEffect_States : AbilityEventEffect
    {
        public string CurrentStateName;
        /// <summary>
        /// If you want to check if character is in a state, please use CombatEditor.IsInState(string StateName.);
        /// </summary>
        public override void StartEffect()
        {
            _combatController.RunningStates.Add(this);
            base.StartEffect();
        }
        public override void EffectRunning(float CurrentTimePercentage)
        {
            base.EffectRunning(CurrentTimePercentage);
            UpdateState(CurrentTimePercentage);
        }
        public override void EndEffect()
        {
            if (IsRunning)
            {
                _combatController.RunningStates.Remove(this);
            }
            CurrentStateName = "";
            base.EndEffect();
        }
        public void UpdateState(float percentageTime)
        {
          
            float[] Ranges = new float[EventObj.DivideCount + 1];
            Ranges[0] = 0;
            Ranges[Ranges.Length - 1] = 1;


            for(int i =0;i< EventObj.DivideCount - 1; i++)
            {
                Ranges[i + 1] = eve.EventMultiRange[i];
            }

            if (percentageTime > 1) percentageTime = 1;
            for(int i =0;i<Ranges.Length - 1;i++)
            {
                if(percentageTime > Ranges[i])
                {
                    if(percentageTime <= Ranges[i+1])
                    {
                        CurrentStateName = EventObj.States[i];
                        return;
                    }
                }
            }
            CurrentStateName = "";
        }
    }

    public partial class AbilityEventEffect_States : AbilityEventEffect
    {
        AbilityEventObj_States EventObj => (AbilityEventObj_States)_EventObj;
        public AbilityEventEffect_States(AbilityEventObj InitObj) : base(InitObj)
        {
            _EventObj = InitObj;
        }
    }
}