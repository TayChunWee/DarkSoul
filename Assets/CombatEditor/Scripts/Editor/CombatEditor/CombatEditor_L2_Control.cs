using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CombatEditor
{
    public partial class CombatEditor
    {
        /// <summary>
        /// HandlePosition to SwapPosition;
        /// </summary>
        /// 
        public void HandleDraggingEvents()
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDrag)
            {
                var index = Mathf.RoundToInt(((e.mousePosition.y - L3TrackAvailableRect.y) - LineHeight) / LineHeight);
                index = Mathf.Clamp(index, 0, SelectedAbilityObj.events.Count);

                L2DragEndIndex = index;

                var rect = new Rect(L2Rect.x, L3TrackAvailableRect.y + (index + 1) * LineHeight - DragIndicatorHeight / 2, L2Rect.width, DragIndicatorHeight);
                L2DragRect = rect;
            }
        }
        public void OnSwapAnimEvents(int indexBefore, int indexAfter)
        {
            SelectedAbilityObj.events = SwapList<AbilityEvent>(SelectedAbilityObj.events, indexBefore, indexAfter);

            if (indexBefore < indexAfter)
            {
                SelectedTrackIndex = indexAfter;
            }
            else
            {
                SelectedTrackIndex = indexAfter + 1;
            }
            LoadL3();
            Repaint();
        }

        public List<T> SwapList<T>(List<T> list, int indexBefore, int indexAfter)
        {
            var obj = list[indexBefore];
            list.Insert(indexAfter, obj);
            if (indexBefore < indexAfter)
            {
                list.RemoveAt(indexBefore);
            }
            else
            {
                list.RemoveAt(indexBefore + 1);
            }
            return list;
        }


        /// <summary>
        /// Click Fields triggers Select, not Toggle
        /// </summary>
        /// <param name="i"></param>
        /// <param name="abilityEvent"></param>
        /// 
        public void OnClickFields(int i, AbilityEvent abilityEvent)
        {
            SelectedTrackIndex = i + 1;
            FocusOnEvent(abilityEvent);
        }

        public void FocusOnEvent(AbilityEvent abilityEvent)
        {
            CombatInspector.GetInspector().CreateInspectedObj(abilityEvent.Obj);
            CurrentInspectedType = InspectedType.Track;
        }

        public void OnClickToggleActive(AbilityEventObj obj)
        {
            obj.IsActive = !obj.IsActive;
            if (IsPlaying || IsLooping)
            {
                OnStopPlayAnimation();
                FlushAndInsPreviewToFrame0();
            }
            else
            {
                OnStopPlayAnimation();
                HardResetPreviewToCurrentFrame();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eve"></param>
        public void OnTogglePreview(AbilityEvent eve)
        {
            eve.Previewable = !eve.Previewable;
            SceneView.RepaintAll();
            if (IsPlaying || IsLooping)
            {
                OnStopPlayAnimation();
                FlushAndInsPreviewToFrame0();
            }
            else
            {
                //OnStopPlayAnimation();

                HardResetPreviewToCurrentFrame();
            }
        }

        public void CreatAddTrackMenu()
        {
            System.Type[] typesToDisplay = TypeCache.GetTypesWithAttribute<AbilityEventAttribute>().OrderBy(m => m.Name).ToArray();
            AnimEventSearchProvider provider = ScriptableObject.CreateInstance("AnimEventSearchProvider") as AnimEventSearchProvider;
            provider.types = typesToDisplay;
            provider.OnSetIndexCallBack = (type) =>
            {
                AbilityEventObj obj = ScriptableObject.CreateInstance(type) as AbilityEventObj;
                obj.name = type.Name.Replace("AbilityEventObj_", "");
                AbilityEvent e = new AbilityEvent();
                e.Obj = obj;
                AssetDatabase.AddObjectToAsset(obj, SelectedAbilityObj);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(SelectedAbilityObj));

                SelectedAbilityObj.events.Add(e);

                EditorUtility.SetDirty(SelectedAbilityObj);
                OnAnimEventChanges();

            DestroyImmediate(provider);
            };
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition + new Vector2(200, 0))), provider);
        }




        public bool IsDragging = false;
        public void StartDragElement(Vector2 StartPos)
        {
            DragStartPosition = StartPos;
            //StartDraggingInRect = true;
            //IsDragging = false;
        }
        public void DraggingElements(Vector2 CurrentPos, Rect EndIndicator)
        {
            DragEndRect = EndIndicator;
        }
        public void EndDrag()
        {
            IsDragging = false;
        }



        public void Tick()
        {
            LastTickTime = Time.realtimeSinceStartup;
            if (IsPlaying || IsLooping)
            {

                float CurrentSpeedModifier = 1;
                for (int i = 0; i < SpeedModifiers.Length; i++)
                {
                    CurrentSpeedModifier *= SpeedModifiers[i].CurrentAnimSpeedModifier;
                }
                CurrentSpeedModifier *= PlayTimeMultiplier;

                //Need the information on frame 0
                if (CurrentPlayTime < 0)
                {
                    CurrentPlayTime = 0;
                }
                else
                {
                    CurrentPlayTime += (1 / 60f) * CurrentSpeedModifier;
                }

                IterateFrame();
            }
        }


        public bool InPrefabMode()
        {
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                return true;
            }
            return false;
        }

        public void OnStartPlay()
        {
            OnPreparePlay();
            IsPlaying = true;
            CombatGlobalEditorValue.IsPlaying = true;
        }
        public void OnStartLoop()
        {
            OnPreparePlay();
            IsLooping = true;
            CombatGlobalEditorValue.IsLooping = true;
        }
        public void OnPreparePlay()
        {
            if (InPrefabMode())
            {
                return;
            }
            OnStopPlayAnimation();
            LastTickTime = -1;
            CurrentPlayTime = -1;
        }

        public void OnPausePlay()
        {
            ResetPlayStates();
            //ResetAnimation();
        }
        public void OnStopPlayAnimation()
        {
            ResetPlayStates();
            AnimationBackToStart();
            PreviewBackToStart();
        }



        public void ResetPlayStates()
        {
            IsPlaying = false;
            IsLooping = false;
            CombatGlobalEditorValue.IsLooping = false;
            CombatGlobalEditorValue.IsPlaying = false;
        }


        public void AnimationBackToStart()
        {
            //When Assembly Reload, this func calls, but when playmode is about to start, this func also calls.
            //So need to return if this is going to playmode.
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            StartTime = Time.realtimeSinceStartup;

            OnSetPointerOnTrack(0);
            OnPreviewAnimationAtPercentage(0);

            SpeedModifiers = new PreviewObject_AnimSpeed[0];
            SpeedModifiers = FindObjectsOfType<PreviewObject_AnimSpeed>();
        }

        public void IterateFrame()
        {
            //In Time Range, Preview.
            if (CurrentPlayTime <= (SelectedAbilityObj.PreviewPercentageRange.y - SelectedAbilityObj.PreviewPercentageRange.x) * SelectedAbilityObj.Clip.length + LoopWaitTime)
            {
                var CurrentPercentage = CurrentPlayTime / SelectedAbilityObj.Clip.length + SelectedAbilityObj.PreviewPercentageRange.x;
                var CurrentRealFrame = Mathf.RoundToInt(CurrentPercentage * SelectedAbilityObj.Clip.length * 60);
                var CurrentMaxFrame = Mathf.RoundToInt(SelectedAbilityObj.PreviewPercentageRange.y * SelectedAbilityObj.Clip.length * 60);
                int CurrentFrame = CurrentRealFrame < CurrentMaxFrame ? CurrentRealFrame : CurrentMaxFrame;

                //Debug.Log(CurrentMaxFrame);
                //Debug.Log(CurrentFrame);
                OnSetPointerOnTrack(CurrentFrame);

                OnPreviewAnimationAtPercentage(CurrentPercentage);
            }
            else if (IsPlaying)
            {
                OnSetPointerOnTrack(Mathf.RoundToInt(SelectedAbilityObj.PreviewPercentageRange.y * SelectedAbilityObj.Clip.length * 60));
                IsPlaying = false;
            }
            else if (IsLooping)
            {
                CurrentPlayTime = 0;
            }

            this.Repaint();
            //EditorWindow view = EditorWindow.GetWindow<SceneView>();
            //view.Repaint();
        }

        public void OnAnimEventChanges()
        {
            HardResetPreviewToCurrentFrame();
            //PreviewAnimationAtPercentage(CurrentPlayTime, true);
            LoadL3();
        }
        public void HardResetPreviewToCurrentFrame()
        {
            OnHardResetPreviewObj();
            OnPreviewAnimationAtFrame(CurrentFrame);
            SceneView.RepaintAll();
        }

        public void PreviewBackToStart()
        {
            if (_previewer != null)
            {
                _previewer.OnPreviewBackToStart();
            }
        }
        public void FlushAndInsPreviewToFrame0()
        {
            OnHardResetPreviewObj();
            OnPreviewAnimationAtFrame(0);
            SceneView.RepaintAll();
        }


        public void SetL2L3Target(AbilityScriptableObject obj)
        {
            SelectedAbilityObj = obj;
            LoadL3();
        }

    }

}

