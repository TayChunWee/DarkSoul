using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace CombatEditor
{
    public abstract class AbilityEventObj : ScriptableObject
    {
        [HideInInspector]
        public bool IsActive = true;
#if UNITY_EDITOR
        [ContextMenu("Delete")]
        public void Delete()
        {
            UnityEditor.Undo.DestroyObjectImmediate(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif

        public enum EventTimeType { Null, EventTime, EventRange,EventMultiRange }
        public void A()
        {
            EventManager.TriggerEvent("ChangeAbilityEvent");
        }
        public virtual EventTimeType GetEventTimeType()
        {
            return EventTimeType.Null;
        }
        public virtual int GetMultiRangeCount()
        {
            return 0;
        }

        public abstract AbilityEventEffect Initialize();

        public virtual AbilityEventPreview InitializePreview()
        {
            return null;
        }
        //if preview exist and no preview is instantiate then instantiate it
        public virtual bool PreviewExist()
        {
            return false;
        }


    }
    public class AbilityEventPreview
    {
        public AbilityEventObj _EventObj;
        public AbilityEvent eve;
        //public CombatEditorPreviewer _previwer;
        public AbilityScriptableObject AnimObj;
        public CombatController _combatController;

        public float StartTimePercentage;
        public float EndTimePercentage;

        //If ScaleTimeExist, this is the true time percentage after calculated.
        public float StartTimeScaledPercentage;
        public float EndTimeScaledPercentage;

        int EventStartFrame => Mathf.RoundToInt(StartTimePercentage * AnimObj.Clip.length * 60);
        int CurrentFrame;
        int EndFrame => Mathf.RoundToInt(EndTimePercentage * AnimObj.Clip.length * 60);
        public GameObject previewGroup => GameObject.Find(CombatGlobalEditorValue.PreviewGroupName);
        public float AnimLength;
        public int LastFrame = -1;
        public bool IsOnStartFrame;
        public float DeltaTime;
        public bool IsPreviewingForward;


        public AbilityEventPreview(AbilityEventObj Obj)
        {
            _EventObj = Obj;
        }
        // Init Preview happens when select current ability, not on this frame
        public virtual void InitPreview()
        {
#if UNITY_EDITOR
            //if (!EnterPlayEventRegistered)
            //{
            //    UnityEditor.EditorApplication.playModeStateChanged += PlaymodeStateChanged;
            //}
#endif
            InitFrame();
        }

        public void InitFrame()
        {
            CurrentFrame = 0;
            LastFrame = -1;
        }

        // called when stop the animation
        public virtual void BackToStart()
        {
        }
        //Called when shift animation/ close editor/ save prefab or scene 
        public virtual void DestroyPreview()
        {
        }
        // Before render current preview, this func need to be called.
        // for example , if preview a particle that not move with node, than we need to fetch the data at start frame 
        // every time we render the preview. 
        public virtual void GetStartFrameDataBeforePreview()
        {

        }

        //For e.g., Particles need to know the node position of the start frame if it dont follow the node,
        //but this requires the animation to render start frame, so have to manually config that.
        public virtual bool NeedStartFrameValue()
        {
            return false;
        }


        public void FetchCurrentValues()
        {
            StartTimePercentage = eve.GetEventStartTime();
            EndTimePercentage = eve.GetEventEndTime();
            AnimLength = AnimObj.Clip.length;
        }



        public virtual void PreviewUpdateFrame(float CurrentTimePercentage)
        {

        }
        // Preview running is on every frame, not in event range
        public virtual void PreviewRunning(float CurrentTimePercentage)
        {
            CurrentFrame = Mathf.RoundToInt(CurrentTimePercentage * AnimObj.Clip.length * 60);

            if (CurrentFrame != LastFrame)
            {
                PreviewUpdateFrame(CurrentTimePercentage);
            }

            if (CurrentFrame > LastFrame)
            {
                IsPreviewingForward = true;
                //DeltaTime = (CurrentFrame - LastFrame) * AnimObj.Clip.length * 60;
            }
            //If animation run slow, CurrentFrame may be the same of LastFrame.
            if (CurrentFrame == LastFrame)
            {
                return;
            }

            if (CurrentFrame == EventStartFrame)
            {
                if (LastFrame != EventStartFrame)
                {
                    IsOnStartFrame = true;
                    PassStartFrame();
                }
                else
                {
                    IsOnStartFrame = false;
                }
            }
            //If animation is too fast, Current Frame may jump over the start frame.
            else if (CurrentFrame > EventStartFrame)
            {
                if (LastFrame < EventStartFrame)
                {
                    IsOnStartFrame = true;
                    PassStartFrame();
                }
                else
                {
                    IsOnStartFrame = false;
                }
            }
            //else if(CurrentFrame < EventStartFrame)
            //{
            //    if (LastFrame > EventStartFrame && (CombatGlobalEditorValue.IsPlaying||CombatGlobalEditorValue.IsLooping))
            //    {
            //        IsOnStartFrame = true;
            //        OnStartFrame();
            //    }
            //    else
            //    {
            //        IsOnStartFrame = false;
            //    }
            //}

            else
            {
                IsOnStartFrame = false;
            }

            LastFrame = CurrentFrame;

        }

        public float LastTimeInScale;

        public bool CurrentInScaledRange;

        public virtual void PreviewRunningInScale(float ScaledPercentage)
        {
            var CurrentTimeInScale = ScaledPercentage * AnimObj.Clip.length * 60;

            if (ScaledPercentage > StartTimeScaledPercentage && ScaledPercentage < EndTimeScaledPercentage)
            {
                CurrentInScaledRange = true;
            }
            else
            {
                CurrentInScaledRange = false;
            }

            DeltaTime = (CurrentTimeInScale - LastTimeInScale) * AnimObj.Clip.length;
            if (DeltaTime < 0)
            {
                DeltaTime = 0;
            }
            LastTimeInScale = CurrentTimeInScale;

        }


        //Sometimes we have to know if the preview is on start frame .For example, playing the sfx
        public virtual void PassStartFrame()
        {

        }

        public virtual bool PreviewInRange(float CurrentTimePercentage)
        {
            if (CurrentTimePercentage >= StartTimePercentage && CurrentTimePercentage <= EndTimePercentage)
            {
                return true;
            }
            return false;
        }

    }



    [System.Serializable]
    public class AbilityEventEffect
    {
        public string ID;
        public bool IsRunning;
        public AbilityScriptableObject AnimObj;
        public void UpdateEventPosition()
        {
            Debug.Log("UpdateEventPosition");
        }
        public AbilityEventEffect(AbilityEventObj Obj)
        {
            _EventObj = Obj;
        }
        [HideInInspector]

        public CombatController _combatController;
        [HideInInspector]
        public AbilityEventObj _EventObj;

        public AbilityEvent eve;
        float CurrentTimePer;


        public virtual void StartEffect()
        {
            if (IsRunning)
            {
                EndEffect();
            }
            IsRunning = true;
        }

        public virtual void EffectRunning() { }
        public virtual void EffectRunning(float CurrentTimePercentage)
        {
            CurrentTimePer = CurrentTimePercentage;
        }
        public virtual void EffectRunningFixedUpdate(float CurrentTimePercentage)
        {
            CurrentTimePer = CurrentTimePercentage;
        }

        public virtual void EffectLateRunning() { }



        //public virtual void TryEnd() { if (IsRunning) EndEffect(); }
        public virtual void EndEffect() { IsRunning = false; }

        public virtual void DrawIndicator() { }
    }


}
