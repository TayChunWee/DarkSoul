using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace CombatEditor
{
    public partial class CombatEditor
    {

        private void Update()
        {
            if ((Time.realtimeSinceStartup - LastTickTime) >= (1 / 60f))
            {
                Tick();
            }
            if (Time.realtimeSinceStartup - StartTime >= 1)
            {
                StartTime = Mathf.Infinity;
            }
        }

        public void PaintL2()
        {
            if (SelectedController == null) return;
            if (SelectedController._animator == null) return;
            if (SelectedAbilityObj == null) return;
            if (SelectedAbilityObj.Clip == null) return;
            PaintPlayButtons();
            PaintTrackLabels();
            HandleDraggingEvents();

            if (DragHandleRequired)
            {
                PaintL2DragIndicator();
            }
            DragHandleRequired = false;
        }
        private static GUIStyle ToggleButtonStyleNormal = null;
        private static GUIStyle ToggleButtonStyleToggled = null;

        public void PaintPlayButtons()
        {

            ToggleButtonStyleNormal = "Button";
            ToggleButtonStyleToggled = new GUIStyle("Button");
            ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.onFocused.background;

            float PlayButtonLength = 35;
            
            Rect PlayAnimRect = new Rect(L2Rect.x, L2Rect.y, PlayButtonLength, PlayButtonLength);
            Rect LoopAnimRect = new Rect(L2Rect.x + PlayButtonLength, L2Rect.y, PlayButtonLength, PlayButtonLength);
            Rect StopAnimRect = new Rect(L2Rect.x + PlayButtonLength * 2, L2Rect.y, PlayButtonLength, PlayButtonLength);
            //Rect PauseRect = new Rect(L2Rect.x + PlayButtonLength * 3, L2Rect.y, PlayButtonLength, PlayButtonLength);
            Rect ClearPreviewRect = new Rect(L2Rect.x + PlayButtonLength * 3, L2Rect.y, PlayButtonLength, PlayButtonLength);

            
            var PlayToggle = GUI.Toggle(PlayAnimRect, IsPlaying, "", "ButtonLeft");
            if(PlayToggle!= IsPlaying)
            {
                IsPlaying = PlayToggle;
                if (IsPlaying)
                {
                    OnStartPlay();
                }
                else
                {
                    OnStartPlay();
                    //OnPausePlay();
                }
            }
            if (!IsPlaying)
            {
                CombatEditorUtility.DrawEditorTextureOnRect(PlayAnimRect, 0.6f, "PlayButton@2x");
            }
            else
            {
                //CombatEditorUtility.DrawEditorTextureOnRect(PlayAnimRect, 0.6f, "d_PauseButton@2x");
                CombatEditorUtility.DrawEditorTextureOnRect(PlayAnimRect, 0.6f, "PlayButton@2x");
            }
            var LoopToggle = GUI.Toggle(LoopAnimRect, IsLooping, "", "ButtonMid");
            if (LoopToggle != IsLooping)
            {
                IsLooping = LoopToggle;
                if (IsLooping)
                {
                    OnStartLoop();
                }
                else
                {
                    OnPausePlay();
                }
            }
            if (IsLooping)
            {
                CombatEditorUtility.DrawEditorTextureOnRect(LoopAnimRect, 0.6f, "d_PauseButton@2x");
            }
            else
            {
                CombatEditorUtility.DrawEditorTextureOnRect(LoopAnimRect, 0.7f, "d_preAudioLoopOff@2x");
            }
            if (GUI.Button(StopAnimRect, "" ,"ButtonMid"))
            {
                OnStopPlayAnimation();
            }
            CombatEditorUtility.DrawEditorTextureOnRect(StopAnimRect, 0.6f, "beginButton");

            //if (GUI.Button(PauseRect, "", "ButtonMid"))
            //{
            //    OnStopPlayAnimation();
            //}
            //CombatEditorUtility.DrawEditorTextureOnRect(PauseRect, 0.6f, "d_PauseButton@2x");


            GUIStyle style = new GUIStyle("ButtonRight");
            style.margin = new RectOffset(0, 0, 0, 0);
            style.fontSize = 20;
            style.padding = new RectOffset(0, 0, 0, 0);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.contentOffset = new Vector2(0, 0);
            if (GUI.Button(ClearPreviewRect, "T", style))
            {
                OnStopPlayAnimation();
                OnEndPreview();
            }

            //CombatEditorUtility.DrawEditorTextureOnRect(TimeMulIcon, 1, "d_AnimationClip Icon");

            //CombatEditorUtility.DrawEditorTextureOnRect(LoopWaitIcon, 0.6f, "beginButton");

        }

        public void PaintTrackLabels()
        {

            #region ConfigAnimRange
            Rect AnimConfigRect = new Rect(L2Rect.x, L3TrackAvailableRect.y, L2Rect.width, LineHeight);

            TrackRect = new Rect(L2Rect.x, L2Rect.y + Height_Top, L2Rect.width, L2Rect.height - Height_Top);
            Scroll_Fields = GUI.BeginScrollView(TrackRect, Scroll_Fields, new Rect(TrackRect.x, TrackRect.y, TrackRect.width, (SelectedAbilityObj.events.Count + 2) * LineHeight), GUIStyle.none, GUIStyle.none);
            Scroll_Track = new Vector2(Scroll_Track.x, Scroll_Fields.y);

            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.alignment = GUI.skin.label.alignment;

            Color DefaultColor = GUI.backgroundColor;
            if (CurrentInspectedType == InspectedType.PreviewConfig) GUI.backgroundColor = Color.green;
            if (GUI.Button(AnimConfigRect, "PreviewRange", style))
            {
                ChangeInspectedType(InspectedType.PreviewConfig);
            }
            GUI.backgroundColor = DefaultColor;
            #endregion
            List<AbilityEvent> eves = SelectedAbilityObj.events;

            for (int i = 0; i < eves.Count; i++)
            {
                if (eves[i].Obj == null)
                {
                    eves.RemoveAt(i);
                    i = 0;
                    AssetDatabase.SaveAssets();
                    continue;
                }

                Rect LabelRect = new Rect(L2Rect.x + 2 * LineHeight, L3TrackAvailableRect.y + (i + 1) * LineHeight, Width_TrackLabel - 3 * LineHeight - SplitterIntervalDistance, LineHeight);
                Rect PreviewToggle = new Rect(L2Rect.x + LineHeight, L3TrackAvailableRect.y + (i + 1) * LineHeight, LineHeight, LineHeight);
                Rect ToggleRect = new Rect(L2Rect.x, L3TrackAvailableRect.y + (i + 1) * LineHeight, LineHeight, LineHeight);
                var StartX = Width_Ability + Width_TrackLabel;
                Rect DeleteRect = new Rect(StartX - LineHeight - SplitterIntervalDistance , Height_Top + (i + 1) * LineHeight, LineHeight, LineHeight);

                AbilityEventObj obj = eves[i].Obj;
                Event e = Event.current;
                if (e.isKey && e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.F2 && LabelRect.Contains(e.mousePosition))
                    {
                        StartPaintRenameField(LabelRect, obj.name, () => { obj.name = NameOfRename; AssetDatabase.SaveAssets(); });
                        Debug.Log("UseEvent?");
                        e.Use();
                    }
                }

                int controlID = GUIUtility.GetControlID(FocusType.Passive);
                if (GUIUtility.hotControl == controlID && !LabelRect.Contains(e.mousePosition))
                {
                    DragHandleRequired = true;
                }

                switch (e.type)
                {
                    case (EventType.MouseDown):
                        if (LabelRect.Contains(e.mousePosition))
                        {
                            GUIUtility.hotControl = controlID;
                            OnClickFields(i, eves[i]);
                            //Debug.Log("UseEvent?");
                            e.Use();
                            StartDragElement(new Vector2(LabelRect.x + LabelRect.width * 0.5f, LabelRect.y + LabelRect.height * 0.5f));
                            L2DragEndIndex = i;
                            DraggingFieldStartIndex = i;

                        }
                        break;
                    case (EventType.MouseUp):
                        {
                            if (GUIUtility.hotControl != controlID) break;
                            EndDrag();
                            OnSwapAnimEvents(DraggingFieldStartIndex, L2DragEndIndex);
                        }
                        break;
                    case (EventType.Ignore):
                        {
                            if (GUIUtility.hotControl != controlID) break;
                            EndDrag();
                            OnSwapAnimEvents(DraggingFieldStartIndex, L2DragEndIndex);
                        }
                        break;
                }

                if (!eves[i].Obj.IsActive)
                {
                    GUI.backgroundColor = Color.red;
                }
                if (SelectedTrackIndex == i + 1)
                {
                    HighlightBGIfInspectType(InspectedType.Track);
                }
                //ToggleOnAndOff
                if (GUI.Button(ToggleRect, GUIContent.none))
                {
                    OnClickToggleActive(eves[i].Obj);
                }

                if (eves[i].Obj.IsActive)
                {
                    CombatEditorUtility.DrawEditorTextureOnRect(ToggleRect, 0.5f, "FilterSelectedOnly@2x");
                }
                else
                {
                    CombatEditorUtility.DrawEditorTextureOnRect(ToggleRect, 0.7f, "scenevis_hidden@2x");
                }
                //Label

                if (GUI.Button(LabelRect, eves[i].Obj.name, style))
                {
                    //OnClickFields(i, eves[i]);
                }
                #region TogglePreviewButton
                GUI.backgroundColor = DefaultColor;
                if (eves[i].Previewable)
                {
                    GUI.backgroundColor = Color.green;
                }
                int PreviewControlID = GUIUtility.GetControlID(FocusType.Passive);
                if (GUI.Button(PreviewToggle, GUIContent.none))
                {
                    OnTogglePreview(eves[i]);
                }
                if (eves[i].Previewable)
                {
                    CombatEditorUtility.DrawEditorTextureOnRect(PreviewToggle, 0.6f, "d_Record On@2x");
                }
                else
                {
                    CombatEditorUtility.DrawEditorTextureOnRect(PreviewToggle, 0.6f, "d_Record Off@2x");
                }

                GUI.backgroundColor = DefaultColor;

                #endregion
                if (GUI.Button(DeleteRect, "-", MyDeleteButtonStyle))
                {
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(SelectedAbilityObj));
                    if (assets.Contains(SelectedAbilityObj.events[i].Obj))
                    {
                        Undo.DestroyObjectImmediate(SelectedAbilityObj.events[i].Obj);
                        AssetDatabase.SaveAssets();
                    }
                    SelectedAbilityObj.events.RemoveAt(i);

                    OnAnimEventChanges();

                }
            }

            for (int i = 0; i < eves.Count; i++)
            {

            }
            PaintAddAbilityButton();
            GUI.EndScrollView();
        
        }
        
        public void PaintAddAbilityButton()
        {
            #region AddObjButton
            List<AbilityEvent> eves = SelectedAbilityObj.events;
            Rect AddButtonRect = new Rect(L2Rect.x, L3TrackAvailableRect.y + (eves.Count + 1) * LineHeight, L2Rect.width , LineHeight);
            if (GUI.Button(AddButtonRect, "+", MyDeleteButtonStyle))
            {
                CreatAddTrackMenu();
            }
            #endregion
        }

        public void PaintDragLabelDragger()
        {
            Debug.Log(DragEndRect);
            EditorGUI.DrawRect(DragEndRect, Color.red);
        }

        public void PaintL2DragIndicator()
        {
            EditorGUI.DrawRect(L2DragRect, Color.green);
            if (LastIndex != L2DragEndIndex)
            {
                Repaint();
            }
            LastIndex = L2DragEndIndex;
        }

    }
}
