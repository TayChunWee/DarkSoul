using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CombatEditor
{
    public partial class CombatEditor :EditorWindow 
    {

        public void InitData()
        {
            var StartX = Width_Ability + Width_TrackLabel;
            var Height = Height_Top;
            L3SurfaceRect = new Rect(StartX, Height_Top, position.width - Width_Inspector - StartX, position.height - Height_Top);
            L3ViewRect = new Rect(StartX, Height, AnimFrameCount * FrameIntervalDistance, (AnimEventTracks.Count + 2) * LineHeight);
            MaxWidth = L3SurfaceRect.width > L3ViewRect.width ? L3SurfaceRect.width : L3ViewRect.width;
            Scroll_Track = GUI.BeginScrollView(L3SurfaceRect, Scroll_Track, L3ViewRect);
            Scroll_Fields = new Vector2(Scroll_Fields.x, Scroll_Track.y);
            AnimTrackRect = new Rect(L3TrackAvailableRect.x, L3TrackAvailableRect.y, (AnimFrameCount) * FrameIntervalDistance, LineHeight);
        }
        public void InitBeforePaintTrack()
        {
            if (AnimClipHelper == null)
            {
                AnimClipHelper = new TimeLineHelper(this);
            }
            if (AnimEventTracks == null)
            {
                LoadL3();
            }
        }
        public bool ConfigNullCheck()
        {
            if (SelectedController == null) return false;
            if (SelectedController._animator == null) return false;
            if (SelectedAbilityObj == null) return false;
            if (SelectedAbilityObj.Clip == null) return false;
            return true;
        }
        public void OnPreviewAnimationAtPercentage(float Percentage, bool Reset = false)
        {
            if (_previewer == null || Reset)
            {
                OnHardResetPreviewObj();
            }
            OnPreviewCheckResetNeeded();

            _previewer.ShowPreviewAtPercentage(Percentage);
        }
        public void OnPreviewAnimationAtFrame(int Frame)
        {
            if (_previewer == null)
            {
                OnHardResetPreviewObj();
            }
            OnPreviewCheckResetNeeded();

            if (SelectedAbilityObj != null)
            {
                if (SelectedAbilityObj.Clip != null)
                {
                    _previewer.ShowPreviewAtPercentage(Frame * (1 / 60f) / SelectedAbilityObj.Clip.length);
                }
            }
        }
        public void OnSetPointerOnTrack(int Frame)
        {
            CurrentFrame = Frame;
        }
        public void OnPreviewCheckResetNeeded()
        {
            if (PreviewNeedReload)
            {
                OnHardResetPreviewObj();
                PreviewNeedReload = false;
            }
        }
        public void OnDragRuler()
        {
            //Stop preview from playing.
            ResetPlayStates();
            OnPreviewAnimationAtFrame(CurrentFrame);
        }
        public void OnDragEventTimePoint()
        {
            UpdateAsset(SelectedAbilityObj);
            Repaint();
            //OnPreviewAnimationAtFrame(CurrentFrame);
        }
        public void OnResetMultiStatesCount(AbilityEventObj Obj)
        {
            for (int i =0;i< SelectedAbilityObj.events.Count;i++)
            {
                var eve = SelectedAbilityObj.events[i];
                if(eve.Obj == Obj)
                {
                    var TimePointCount = eve.Obj.GetMultiRangeCount() - 1;
                    for(int j = 0; j < TimePointCount ; j++)
                    {
                        eve.EventMultiRange[j] = (j+1) * 1f / (float)(TimePointCount+1);
                    }
                }
            }
            Repaint();
        }

    }
}
