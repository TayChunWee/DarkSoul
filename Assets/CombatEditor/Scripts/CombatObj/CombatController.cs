using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace CombatEditor
{

    public class RecordedClip2Time
    {
        public AnimationClip _clip;
        public double time;
        public RecordedClip2Time(AnimationClip clip,double time)
        {
            _clip = clip;
            this.time = time;
        }
    }

    [System.Serializable]
	public class CombatGroup
	{
	    public bool IsFolded;
	    public string Label;
	    public List<AbilityScriptableObject> CombatObjs;
	    public List<AbilityObjWithEffect> eves = new List<AbilityObjWithEffect>();
	}
	public class AbilityObjWithEffect
	{
	    public AbilityScriptableObject Obj;
	    public int Index;
	    public List<AbilityEventEffect> EventEffects = new List<AbilityEventEffect>();
	}
	
	[System.Serializable]
	public class CharacterNode
	{
	    public enum NodeType { Animator, BottomCenter, BodyCenter, Head, Spine, Hand, RHand, LHand, Foot ,LFoot, RFoot, Weapon , WeaponBase, WeaponTip}
	    public NodeType type;
	    public Transform NodeTrans;
	}
	
	
	public class AbilityEventWithEffects
	{
	    public AbilityEvent eve;
	    public AbilityEventEffect effect;
	}



    public class CombatController : MonoBehaviour
    {
        public Animator _animator;
        public AbilityScriptableObject SelectedAbility;
        public List<CombatGroup> CombatDatas = new List<CombatGroup>();
        public AnimationClip clip;

        CombatEventReceiver receiver;


        public AnimSpeedExecutor _animSpeedExecutor;
        public MoveExecutor _moveExecutor;


        public List<CharacterNode> Nodes = new List<CharacterNode>();

        public List<RecordedClip2Time> _recordedSelfTransClips = new List<RecordedClip2Time>();

        public Dictionary<int, List<AbilityEventWithEffects>> ClipID_To_EventEffects;

        public List<AbilityEventEffect_States> RunningStates = new List<AbilityEventEffect_States>();

        private void Start()
        {

            ClipID_To_EventEffects = new Dictionary<int, List<AbilityEventWithEffects>>();
            ClearNullReference();
            InitClipsOnRunningLayers();
            InitAnimEffects();
            _animSpeedExecutor = new AnimSpeedExecutor(this);
            _moveExecutor = new MoveExecutor(this);
        }

        public void InitClipsOnRunningLayers()
        {
            LayerActiveClipIDs = new List<int[]>();
            for (int i = 0; i < _animator.layerCount; i++)
            {
                LayerActiveClipIDs.Add(null);
            }
        }
        public void ClearNullReference()
        {
            for (int i = 0; i < CombatDatas.Count; i++)
            {
                for (int j = 0; j < CombatDatas[i].CombatObjs.Count; j++)
                {
                    if (CombatDatas[i].CombatObjs[j] == null)
                    {
                        CombatDatas[i].CombatObjs.RemoveAt(j);
                        j--;
                        continue;
                    }
                    else
                    {
                        for (int k = 0; k < CombatDatas[i].CombatObjs[j].events.Count; k++)
                        {
                            if (CombatDatas[i].CombatObjs[j].events[k].Obj == null)
                            {
                                CombatDatas[i].CombatObjs[j].events.RemoveAt(k);
                                k--;
                                continue;
                            }
                        }

                    }
                }
            }
        }


        private void Update()
        {
         
            RunEffects(0);
            _animSpeedExecutor.Execute();
        }

        private void FixedUpdate()
        {
            RunEffects(1);
        }

        public Vector3 GetCurrentRootMotion()
        {
            return _moveExecutor.GetCurrentRootMotion();
        }
        
        public List<int[]> LayerActiveClipIDs = new List<int[]>();
        /// <summary>
        /// Fetch states and clips in animator.
        /// UpdateMode : 0:Update 1:FixedUpdate.
        /// </summary>
        public void RunEffects( int UpdateMode = 0 )
        {
            for (int i = 0; i < _animator.layerCount; i++)
            {
                var LayerIndex = i;
                if (!_animator.IsInTransition(LayerIndex))
                {
                    var CurrentAnimState = _animator.GetCurrentAnimatorStateInfo(LayerIndex);
                    var RunningClips = _animator.GetCurrentAnimatorClipInfo(LayerIndex);

                    int[] runningclipsID = new int[RunningClips.Length];
                    for (int j = 0; j < RunningClips.Length; j++)
                    {
                        runningclipsID[j] = RunningClips[j].clip.GetInstanceID();
                    }
                    UpdateLayerActiveClips(LayerIndex, runningclipsID);

                    for (int j = 0; j < RunningClips.Length; j++)
                    {
                        var CurrentClipID = RunningClips[j].clip.GetInstanceID();

                        if (!ClipID_To_EventEffects.ContainsKey(CurrentClipID)) { continue; }


                        RunningEventsOnClip(CurrentClipID, CurrentAnimState.normalizedTime, LayerIndex, UpdateMode);
                    }
                }

                if (_animator.IsInTransition(LayerIndex))
                {
                    var NextAnimState = _animator.GetNextAnimatorStateInfo(LayerIndex);
                    var NextRunningClips = _animator.GetNextAnimatorClipInfo(LayerIndex);

                    int[] runningClipsID = new int[NextRunningClips.Length];
                    for (int j = 0; j < NextRunningClips.Length; j++)
                    {
                        runningClipsID[j] = NextRunningClips[j].clip.GetInstanceID();
                    }
                    UpdateLayerActiveClips(LayerIndex, runningClipsID);

                    for (int j = 0; j < NextRunningClips.Length; j++)
                    {
                        var CurrentClip = NextRunningClips[j].clip.GetInstanceID();
                        if (!ClipID_To_EventEffects.ContainsKey(CurrentClip)) { continue; }
                        RunningEventsOnClip(CurrentClip, NextAnimState.normalizedTime, LayerIndex ,UpdateMode);
                    }
                }

            }
        }
        /// <summary>
        //  Running the target effects on animation clip.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="NormalizedTime"></param>
        /// <param name="LayerIndex"></param>
        /// <param name="UpdateMode"> 0 : Update 1:FixedUpdate </param>
        public void RunningEventsOnClip(int clipID, float NormalizedTime, int LayerIndex ,int UpdateMode = 0)
        {
            List<AbilityEventWithEffects> abilityEventWithEffects = ClipID_To_EventEffects[clipID];
            for (int j = 0; j < abilityEventWithEffects.Count; j++)
            {
                var eve = abilityEventWithEffects[j];
                var StartTime = eve.eve.GetEventStartTime();
                var EndTime = eve.eve.GetEventEndTime();
                var EveTimeType = eve.eve.GetEventTimeType();

                if (EveTimeType == AbilityEventObj.EventTimeType.EventTime && UpdateMode == 0)
                {
                    if (NormalizedTime >= StartTime)
                    {
                        if (eve.effect._EventObj.IsActive && !eve.effect.IsRunning)
                        {
                            eve.effect.StartEffect();
                        }
                        if (eve.effect._EventObj.IsActive && eve.effect.IsRunning)
                        {
                            eve.effect.EffectRunning();
                        }
                    }
                    //If self to self translation, events can't close itself cause clip is not change. So it need to close itself by percentage.
                    else
                    {
                        eve.effect.EndEffect();
                    }
                }
                if (EveTimeType == AbilityEventObj.EventTimeType.EventRange || EveTimeType == AbilityEventObj.EventTimeType.EventMultiRange)
                {
                    //Start Even if the start frame is jumpped 
                    //  StartTime < CurrentTime < EndTime
                    if (NormalizedTime < EndTime && NormalizedTime >= StartTime)
                    {
                        if (!eve.effect.IsRunning && eve.effect._EventObj.IsActive && UpdateMode == 0)
                        {
                            eve.effect.StartEffect();
                        }

                        if(eve.effect.IsRunning)
                        {
                            if (UpdateMode == 0)
                            {
                                eve.effect.EffectRunning();
                                eve.effect.EffectRunning(NormalizedTime);
                            }
                            if(UpdateMode == 1)
                            {
                                eve.effect.EffectRunningFixedUpdate(NormalizedTime);
                            }
                        }

                    }
                    //If self to self translation, events can't close itself cause clip is not change. So it need to close itself by percentage.

                    if (NormalizedTime >= EndTime || NormalizedTime < StartTime && UpdateMode == 0)
                    {
                        if (eve.effect._EventObj.IsActive)
                        {
                            eve.effect.EndEffect();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update Active Clips on target Layer, End last frame events if needed.
        /// </summary>
        /// <param name="LayerIndex"></param>
        /// <param name="clips"></param>
        public void UpdateLayerActiveClips(int LayerIndex, int[] clipsID)
        {
            bool RunningClipsChangedInLayer = false;
            if (LayerActiveClipIDs[LayerIndex] != null)
            {
                if (LayerActiveClipIDs[LayerIndex].Length != clipsID.Length)
                {
                    RunningClipsChangedInLayer = true;
                }
                else
                {
                    for (int i = 0; i < LayerActiveClipIDs[LayerIndex].Length; i++)
                    {
                        if (LayerActiveClipIDs[LayerIndex][i] != clipsID[i])
                        {
                            RunningClipsChangedInLayer = true;
                        }
                    }

                }
            }
            else RunningClipsChangedInLayer = true;

            if (RunningClipsChangedInLayer)
            {
                if(LayerActiveClipIDs[LayerIndex] != null)
                for (int i = 0; i < LayerActiveClipIDs[LayerIndex].Length; i++)
                {
                    var clip = LayerActiveClipIDs[LayerIndex][i];
                    if (ClipID_To_EventEffects.ContainsKey(clip))
                    {
                        for (int j = 0; j < ClipID_To_EventEffects[clip].Count; j++)
                        {
                                ClipID_To_EventEffects[clip][j].effect.EndEffect();
                        }
                    }
                }
                LayerActiveClipIDs[LayerIndex] = clipsID;
            }
        }
        
	    public Transform GetNodeTranform(CharacterNode.NodeType type)
	    {
	        if(type == CharacterNode.NodeType.Animator)
	        {
	            if (_animator != null)
	            {
	                return _animator.transform;
	            }
	            return transform;
	        }
	
	        for(int i  =0;i<Nodes.Count;i++)
	        {
	            if(Nodes[i].type == type)
	            {
	                if(Nodes[i].NodeTrans == null)
	                {
	                    return _animator.transform;
	                }
	                return Nodes[i].NodeTrans;
	            }
	        }
	        return _animator.transform;
	    }
	
	    public void SimpleMoveRG(Vector3 deltaMove)
	    {
            _moveExecutor.Move(deltaMove);
        }
	
	    public void InitAnimReceiver()
	    {
	        receiver = _animator.gameObject.AddComponent<CombatEventReceiver>();
	        receiver.controller = this;
	        receiver.CombatDatasID = new List<string>();
	        for (int i =0;i<CombatDatas.Count;i++)
	        {
	            var Group = CombatDatas[i];
	            for (int j = 0; j < Group.CombatObjs.Count; j++)
	            {
	                receiver.CombatDatasID.Add(Group.CombatObjs[j].GetInstanceID().ToString());
	            }
	        }
	    }
	    public void StartEvent(int GroupIndex, int ObjIndex, int EventIndex)
	    {
	        if (CombatDatas[GroupIndex].eves[ObjIndex].Obj.events[EventIndex].Obj.IsActive)
	        {
	            CombatDatas[GroupIndex].eves[ObjIndex].EventEffects[EventIndex].eve = CombatDatas[GroupIndex].CombatObjs[ObjIndex].events[EventIndex];
	            CombatDatas[GroupIndex].eves[ObjIndex].EventEffects[EventIndex].StartEffect();
	        }
	    }
	    public void EndEvent(int GroupIndex, int ObjIndex, int EventIndex)
	    {
	        if (CombatDatas[GroupIndex].eves[ObjIndex].Obj.events[EventIndex].Obj.IsActive)
	        {
	            CombatDatas[GroupIndex].eves[ObjIndex].EventEffects[EventIndex].EndEffect();
	        }
	    }
	
	
	
	    public void InitAnimEffects()
	    {
	        for (int i = 0; i < CombatDatas.Count; i++)
	        {
	            var Group = CombatDatas[i];
	            for (int j = 0; j < Group.CombatObjs.Count; j++)
	            {
	                //Caution: The Number of CombatObj and EventEffects must sync
	                var CombatObj = Group.CombatObjs[j];
	                AbilityObjWithEffect ae = new AbilityObjWithEffect();
	                ae.Obj = CombatObj;
	                for (int k = 0; k < CombatObj.events.Count; k++)
	                {
	                    var EventEffect = AddEventEffects( CombatObj.Clip.GetInstanceID(), CombatObj.events[k]);
	                    EventEffect.AnimObj = CombatObj;
	                    ae.EventEffects.Add(EventEffect);
	                }
	                Group.eves.Add(ae);
	            }
	        }
	    }
	
	    List<AbilityEventEffect> _abilityEventEffects = new List<AbilityEventEffect>();
	    public AbilityEventEffect AddEventEffects( int clipID, AbilityEvent eve)
	    {
	        AbilityEventObj EffectObj = eve.Obj;
	        AbilityEventEffect _abilityEventEffect = EffectObj.Initialize();
	        _abilityEventEffect.eve = eve;
	        _abilityEventEffect._combatController = this;
	        _abilityEventEffects.Add(_abilityEventEffect);
	
	        AbilityEventWithEffects eveWithEffects = new AbilityEventWithEffects();
	        eveWithEffects.eve = eve;
	        eveWithEffects.effect = _abilityEventEffect;
	
	        //Save all animationEvents to dictionary
	        if(ClipID_To_EventEffects.ContainsKey(clipID))
	        {
                ClipID_To_EventEffects[clipID].Add(eveWithEffects);
	        }
	        else
	        {
	            List<AbilityEventWithEffects> list = new List<AbilityEventWithEffects>();
	            list.Add(eveWithEffects);
                ClipID_To_EventEffects.Add(clipID, list);
	        }
	
	
	        return _abilityEventEffect;
	    }
	
	
        public bool IsInState(string Name)
        {
            for(int i =0;i<RunningStates.Count;i++)
            {
                if (RunningStates[i].CurrentStateName == Name)
                {
                    return true;
                }
            }
            return false;
        }
	
	
	}
}
