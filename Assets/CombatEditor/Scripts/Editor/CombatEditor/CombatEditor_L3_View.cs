using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CombatEditor
{
    public partial class CombatEditor : EditorWindow
    {
        public void PaintL3()
        {
            if (!ConfigNullCheck()) return;
            PaintRuler();
            PaintTrack();
            PaintTimeLineScaler();
        }
        public void PaintTrack()
        {
            InitBeforePaintTrack();
            InitRect();
            PaintPreviewRange();
            InitData();

            for (int i = 0; i < AnimEventTracks.Count; i++)
            {
                PaintEventBG(i);
                PaintTrackBG(i);
                //Draggable
                if (AnimEventTracks[i].eve.Obj.GetEventTimeType() == AbilityEventObj.EventTimeType.EventRange)
                {
                    PaintRangeRect(i);
                }

                else if (AnimEventTracks[i].eve.Obj.GetEventTimeType() == AbilityEventObj.EventTimeType.EventTime)
                {
                    PaintPointRect(i);
                }

                else if (AnimEventTracks[i].eve.Obj.GetEventTimeType() == AbilityEventObj.EventTimeType.EventMultiRange)
                {
                    PaintMultiRangeRect(i);
                }
            }

            GUI.EndScrollView();
            PaintCurrentIndicator();
            PaintEndLine();
        }
        public void PaintPreviewRange()
        {
            GUI.Box(new Rect(AnimTrackRect.x, AnimTrackRect.y, L3SurfaceRect.width, AnimTrackRect.height), "", "AnimationEventBackground");

            //DrawSelected
            if (SelectedTrackIndex == 0)
            {
                EditorGUI.DrawRect(AnimTrackRect, SelectedTrackColor);
            }
            int AnimStartFrame = Mathf.RoundToInt(SelectedAbilityObj.PreviewPercentageRange.x * AnimFrameCount);
            int AnimEndFrame = Mathf.RoundToInt(SelectedAbilityObj.PreviewPercentageRange.y * AnimFrameCount);
            int[] AnimTimeRange =
                AnimClipHelper.DrawHorizontalDraggableRange(
                    AnimStartFrame,
                    AnimEndFrame,
                    AnimFrameCount,
                    AnimTrackRect,
                    Color.white,
                    "flow node 4",
                    5,
                    () =>
                    {
                        UpdateAsset(SelectedAbilityObj);
                        HardResetPreviewToCurrentFrame();
                    });
            SelectedAbilityObj.PreviewPercentageRange.x = (float)AnimTimeRange[0] / (float)AnimFrameCount;
            SelectedAbilityObj.PreviewPercentageRange.y = (float)AnimTimeRange[1] / (float)AnimFrameCount;
        }


        public void PaintPointRect(int i)
        {
            Rect AvilableTrackRect = new Rect(L3TrackAvailableRect.x, L3TrackAvailableRect.y + (i + 1) * LineHeight, L3TrackAvailableRect.width, LineHeight);
            Rect OutTrackRect = new Rect(L3TrackAvailableRect.x, L3TrackAvailableRect.y + (i + 1) * LineHeight, MaxWidth, LineHeight);

            int StartFrame = Mathf.RoundToInt((AnimEventTracks[i].eve.EventTime * AnimFrameCount));
            StartFrame = AnimEventTracks[i].helper.DrawHorizontalDraggablePoint(
                  StartFrame,
                  AnimFrameCount,
                  AvilableTrackRect,
                   Color.white,
                    "flow node 3",
                  TimePointWidth,
                  true,
                  true,false,null
                  , () =>
                  {
                      OnDragEventTimePoint();
                  }
                      );
            if (AnimFrameCount != 0)
            {
                AnimEventTracks[i].eve.EventTime = (float)StartFrame / (float)AnimFrameCount;
            }


        }
        public void PaintRangeRect(int i)
        {

            Rect AvilableTrackRect = new Rect(L3TrackAvailableRect.x, L3TrackAvailableRect.y + (i + 1) * LineHeight, L3TrackAvailableRect.width, LineHeight);
            Rect OutTrackRect = new Rect(L3TrackAvailableRect.x, L3TrackAvailableRect.y + (i + 1) * LineHeight, MaxWidth, LineHeight);

            int StartFrame = Mathf.RoundToInt(AnimEventTracks[i].eve.EventRange.x * AnimFrameCount);
            int EndFrame = Mathf.RoundToInt(AnimEventTracks[i].eve.EventRange.y * AnimFrameCount);
            int[] TimeRange =
                AnimEventTracks[i].helper.DrawHorizontalDraggableRange(
                    StartFrame,
                    EndFrame,
                    AnimFrameCount,
                    AvilableTrackRect,
                    Color.white,
                    "flow node 3",
                    5,
                      () =>
                      {
                          OnDragEventTimePoint();
                      }
                      );

            if (AnimFrameCount != 0)
            {
                AnimEventTracks[i].eve.EventRange.x = (float)TimeRange[0] / (float)AnimFrameCount;
                AnimEventTracks[i].eve.EventRange.y = (float)TimeRange[1] / (float)AnimFrameCount;
            }
        }

        public void PaintMultiRangeRect(int i)
        {
            Rect AvilableTrackRect = new Rect(L3TrackAvailableRect.x, L3TrackAvailableRect.y + (i + 1) * LineHeight, L3TrackAvailableRect.width, LineHeight);
            Rect OutTrackRect = new Rect(L3TrackAvailableRect.x, L3TrackAvailableRect.y + (i + 1) * LineHeight, MaxWidth, LineHeight);
            //Get Paintable Range
            int[] TargetFrames = new int[AnimEventTracks[i].eve.Obj.GetMultiRangeCount() - 1];
            for (int j = 0; j < TargetFrames.Length; j++)
            {
                TargetFrames[j] = Mathf.RoundToInt(AnimEventTracks[i].eve.EventMultiRange[j] * AnimFrameCount);
            }
            for (int k = 0; k < TargetFrames.Length; k++)
            {
            }
            string[] names = (AnimEventTracks[i].eve.Obj as AbilityEventObj_States).States;
            int[] Targets = AnimEventTracks[i].helper.DrawHorizontalMultiDraggable(TargetFrames, names, AnimFrameCount, AvilableTrackRect,Color.white, TimePointWidth,OnDragEventTimePoint);
            for(int j =0; j < Targets.Length; j++)
            {
                AnimEventTracks[i].eve.EventMultiRange[j] = (float)Targets[j] / (float)AnimFrameCount;
            }
        }


        public void PaintEventBG(int i)
        {
            Rect OutTrackRect = new Rect(L3TrackAvailableRect.x, L3TrackAvailableRect.y + (i + 1) * LineHeight, MaxWidth, LineHeight);
            GUI.Box(OutTrackRect, "", "AnimationEventBackground");
        }
        public void PaintTrackBG(int i)
        {
            Rect AvilableTrackRect = new Rect(L3TrackAvailableRect.x, L3TrackAvailableRect.y + (i + 1) * LineHeight, L3TrackAvailableRect.width, LineHeight);
            if (SelectedTrackIndex == i + 1)
                EditorGUI.DrawRect(AvilableTrackRect, SelectedTrackColor);
        }
        public void PaintCurrentIndicator()
        {
            if (CurrentFrame == 0) { return; }
            Vector3 CurrentTimeLineP1 = new Vector3(L3TrackAvailableRect.x + ((float)CurrentFrame / AnimFrameCount) * L3TrackAvailableRect.width, 0, 0);
            Vector3 CurrentTimeLineP2 = CurrentTimeLineP1 + new Vector3(0, position.height, 0);
            DrawVerticalLine(CurrentTimeLineP1, CurrentTimeLineP2, Color.white, 1);
        }
        public void PaintEndLine()
        {
            var EndLineTop = new Vector2(L3ViewRect.x + L3ViewRect.width, Height_Top);
            var EndLineBottom = EndLineTop + new Vector2(0, LineHeight * (AnimEventTracks.Count + 1));
            DrawVerticalLine(EndLineTop, EndLineBottom, new Color(1, 1, 1, 0.3f), 1);
        }
        public void PaintRuler()
        {
            #region NullCheck
            if (SelectedAbilityObj == null)
            {
                return;
            }
            if (SelectedAbilityObj.Clip == null)
            {
                return;
            }
            if (TopFrameThumbHelper == null)
            {
                TopFrameThumbHelper = new TimeLineHelper(this);
            }

            #endregion
            #region InitValues


            #endregion
            //UpdateAnimation
            #region UpdateAnimation
            CurrentFrame = TopFrameThumbHelper.DrawHorizontalDraggablePoint(CurrentFrame, AnimFrameCount, new Rect(L3TrackAvailableRect.x, 0, L3TrackAvailableRect.width, Height_Top), Color.white, GUIStyle.none, 12, true, true,false,
                (Percentage) =>
                {
                    OnDragRuler();
                },
                ()=>{ Repaint(); }
                );

            #endregion
            PaintScaleIndicators();


        }
        public void PaintScaleIndicators()
        {
            var StartX = Width_Ability + Width_TrackLabel;
            Rect L3HeadOutRect = new Rect(StartX, 0, position.width - Width_Inspector - StartX, Height_Top);
            Rect L3HeadInnerRect = new Rect(StartX, 0, L3TrackAvailableRect.width, Height_Top);
            Scroll_Ruler = GUI.BeginScrollView(L3HeadOutRect, Scroll_Ruler, L3HeadInnerRect, GUIStyle.none, GUIStyle.none);
            Scroll_Ruler = new Vector2(Scroll_Track.x, 0);
            var ViewableFrameCount = MaxWidth / FrameIntervalDistance;
            for (int i = 0; i < ViewableFrameCount; i += FrameIntervalCount)
            {
                Vector3 StartPoint = new Vector3(L3TrackAvailableRect.x + i * FrameIntervalDistance, L3TrackAvailableRect.y, 0);

                if (i <= AnimFrameCount)
                {
                    DrawVerticalLine(StartPoint, StartPoint - new Vector3(0, 10, 0), Color.white, 1);
                    GUI.Label(new Rect(StartPoint.x, StartPoint.y - 20, 35, 20), i.ToString());
                }
                else
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.normal.textColor = new Color(1, 1, 1, 0.3f);

                    DrawVerticalLine(StartPoint, StartPoint - new Vector3(0, 10, 0), new Color(1, 1, 1, 0.3f), 1);
                    GUI.Label(new Rect(StartPoint.x, StartPoint.y - 20, 35, 20), i.ToString(), style);

                }

            }
            //���Ŀ̶�
            Vector3 FrameEndStartPoint = new Vector3(L3TrackAvailableRect.x + AnimFrameCount * FrameIntervalDistance, L3TrackAvailableRect.y, 0);
            DrawVerticalLine(FrameEndStartPoint, FrameEndStartPoint - new Vector3(0, 10, 0), Color.white, 1);
            GUI.Label(new Rect(FrameEndStartPoint.x, FrameEndStartPoint.y - 20, 35, 20), AnimFrameCount.ToString());
            GUI.EndScrollView();
        }

        bool ShowScaler = true;
        public void PaintTimeLineScaler()
        {
            var width = 100;
            var height = 20;
            var Offset = 10;

            Rect rect;
            if (ShowScaler)
            {
                rect = new Rect(position.width - width - Offset, position.height - height - Offset - 3, width - 3, height);
            }
            else
            {
                rect  = new Rect(position.width - Offset - height, position.height - height - Offset - 3, height, height);
            }
            GUI.Box(rect,new GUIContent(""), EditorStyles.helpBox);
            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("S"), GUILayout.Width(height)))
            {
                ShowScaler = !ShowScaler;
            }
            if (ShowScaler)
            {
                TimeLineScaler = GUILayout.HorizontalSlider(TimeLineScaler, 0.4f, 1f);
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
