using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CombatEditor
{
    public partial class CombatEditor
    {
        public void InitStyleAndAbilities()
        {
            AbilityScroll = GUI.BeginScrollView(
           new Rect(AbilityRect.x, AbilityRect.y + LineHeight, AbilityRect.width, AbilityRect.height),
           AbilityScroll,
           new Rect(AbilityRect.x, AbilityRect.y + LineHeight, Width_Ability, LineHeight * (ElementCount + 1)));
            if (AbilityButtonStyle == null)
            {
                AbilityButtonStyle = new GUIStyle(GUI.skin.button);
                AbilityButtonStyle.alignment = TextAnchor.MiddleLeft;
                AbilityButtonStyle.padding = new RectOffset(GUI.skin.button.padding.left, GUI.skin.button.padding.left, 0, 0);
            }
        }
        public bool CharacterExist()
        {
            if (SelectedController == null) return false;
            if (SelectedController._animator == null) return false;
            return true;
        }

    
        public void HandleSwapEvents(AbilityScriptableObject _data, Rect rect, int groupIndex, int arrayIndex)
        {
            var DataSelectRect = rect;
            var data = _data;
            var i = groupIndex;
            var j = arrayIndex;
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (e.type)
            {
                case (EventType.MouseDown):
                    if (DataSelectRect.Contains(e.mousePosition))
                    {
                        ClearL1SwapRect();
                        GUIUtility.hotControl = controlID;
                        OnSelectAbilityObj(data, i, j);
                        e.Use();
                        StartDragElement(new Vector2(DataSelectRect.x + DataSelectRect.width * 0.5f, DataSelectRect.y + DataSelectRect.height * 0.5f));
                        SwapGroupIndexBefore = i;
                        SwapArrayIndexBefore = j;
                        IsDraggingL1 = true;
                        SwapRequired = true;
                    }
                    break;
                case (EventType.MouseDrag):
                    {
                        //Cant drag in itself!
                        if (GUIUtility.hotControl == controlID) return;
                      
                        var VerticalDistance = e.mousePosition.y - DataSelectRect.y;
                        if (VerticalDistance < LineHeight && VerticalDistance >= 0)
                        {
                            if (VerticalDistance < LineHeight / 2)
                            {
                                L1DraggingTargetRect = new Rect(DataSelectRect.x, DataSelectRect.y - DragRectHeight * 0.5f, DataSelectRect.width, DragRectHeight);

                                if (SwapGroupIndexAfter != i || SwapArrayIndexAfter != j) TargetChangeThisFrame = true;
                                SwapGroupIndexAfter = i;
                                SwapArrayIndexAfter = j;
                            }
                            else
                            {
                                L1DraggingTargetRect = new Rect(DataSelectRect.x, DataSelectRect.y + LineHeight - DragRectHeight * 0.5f, DataSelectRect.width, DragRectHeight);
                                if (SwapGroupIndexAfter != i || SwapArrayIndexAfter != j + 1) TargetChangeThisFrame = true;
                                SwapGroupIndexAfter = i;
                                SwapArrayIndexAfter = j + 1;
                            }
                        }
                    }
                    break;
                case (EventType.MouseUp):
                    {
                        if (IsDraggingL1)
                        {
                            if(DataSelectRect.Contains(e.mousePosition))
                            {
                                if (GUIUtility.hotControl == controlID)
                                {
                                    SwapRequired = false;
                                }
                            }
                        }
                    }
                    break;
           
            }
        }
        public void HandleL1Drag()
        {
            Event e = Event.current;
            if(e.type == EventType.MouseUp || e.type == EventType.Ignore)
            {
                if(SwapRequired)
                {
                    OnSwapAbilityObj();
                }
                IsDraggingL1 = false;
                ClearL1SwapRect();
                EndDrag();
            }
        }

        public void ClearL1SwapRect()
        {

            L1DraggingTargetRect = new Rect(0, 0, 0, 0);
        }
       

        public void F2ToRename(Rect FoldOutRect, int index)
        {
            SerializedObject so = new SerializedObject(SelectedController);
            so.Update();
            SerializedProperty combatDatas = so.FindProperty("CombatDatas");
            Event e = Event.current;
            if (e.isKey && e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.F2 && FoldOutRect.Contains(e.mousePosition))
                {
                    StartPaintRenameField(FoldOutRect, combatDatas.GetArrayElementAtIndex(index).FindPropertyRelative("Label").stringValue, () =>
                    {
                        combatDatas.GetArrayElementAtIndex(index).FindPropertyRelative("Label").stringValue = NameOfRename;
                        so.ApplyModifiedProperties();
                    });
                    e.Use();
                }
            }
            //so.ApplyModifiedProperties();
        }
        public void OnSetCurrentController(CombatController controller)
        {
            SelectedController = controller;
            if (SelectedController != null)
            {
                LastSelectedControllerName = SelectedController.gameObject.name;
            }
        }
        public void OnSelectCharacterConfig()
        {
            CombatInspector.GetInspector().SelectCombatConfig();
        }
        public void OnAddCombat(Rect buttonRect, int GroupIndex)
        {

            AssetDatabase.SaveAssets();
            SerializedObject so = new SerializedObject(SelectedController);
            SerializedProperty combatDatas = so.FindProperty("CombatDatas");
            SerializedProperty ObjsProperty = combatDatas.GetArrayElementAtIndex(GroupIndex).FindPropertyRelative("CombatObjs");
            ObjsProperty.arraySize++;
            SerializedProperty TargetObj = ObjsProperty.GetArrayElementAtIndex(ObjsProperty.arraySize - 1);
            TargetObj.objectReferenceValue = null;
            //InsObj.PreviewPercentageRange = new Vector2(0, 1);
            //TargetObj.objectReferenceValue = InsObj;
            so.ApplyModifiedProperties();
        }
        /// <summary>
        /// Clear, and ReInit prevciews. Useful when some datas that need to be instantiated are changed
        /// </summary>
        void OnHardResetPreviewObj()
        {
            if (_previewer == null)
            {
                _previewer = new CombatPreviewController();
            }
            _previewer.SetPreviewTarget(SelectedController, SelectedAbilityObj);
            _previewer.FlushAndInsAllPreviews();
        }
        public void OnEndPreview()
        {
            if (_previewer != null)
            {
                _previewer.OnDestroyPreview();
            }
            _previewer = null;
        }
        public void TrySetControllerByName()
        {
            var obj = GameObject.Find(LastSelectedControllerName);
            if (obj != null)
            {
                var controller = obj.GetComponent<CombatController>();
                if (controller != null)
                {
                    OnSetCurrentController(controller);
                }
            }
        }
        public void ClearCombatController()
        {
            LastSelectedControllerName = "";
            SelectedController = null;
        }
        void OnSelectCharacterInMenu(object controller)
        {
            CurrentGroupIndex = -1;
            CurrentAbilityIndexInGroup = -1;

            CurrentSelectedAbilityIndex = -1;
            OnSetCurrentController(controller as CombatController);
        }
        public void OnSwapAbilityObj()
        {
            SerializedObject so = new SerializedObject(SelectedController);
            so.Update();
            SerializedProperty combatDatas = so.FindProperty("CombatDatas");
            SerializedProperty BeforeListProperty = combatDatas.GetArrayElementAtIndex(SwapGroupIndexBefore).FindPropertyRelative("CombatObjs");

            SerializedProperty AfterListProperty = combatDatas.GetArrayElementAtIndex(SwapGroupIndexAfter).FindPropertyRelative("CombatObjs");

            var Obj = BeforeListProperty.GetArrayElementAtIndex(SwapArrayIndexBefore).objectReferenceValue;

            //Debug.Log();
            AfterListProperty.InsertArrayElementAtIndex(SwapArrayIndexAfter);
            AfterListProperty.GetArrayElementAtIndex(SwapArrayIndexAfter).objectReferenceValue = Obj;

            CurrentGroupIndex = SwapGroupIndexAfter;
            CurrentAbilityIndexInGroup = SwapArrayIndexAfter;

            if (SwapGroupIndexBefore == SwapGroupIndexAfter)
            {
                if (SwapArrayIndexBefore < SwapArrayIndexAfter)
                {
                    AfterListProperty.DeleteArrayElementAtIndex(SwapArrayIndexBefore);
                    CurrentAbilityIndexInGroup -= 1;
                }
                else
                {
                    AfterListProperty.DeleteArrayElementAtIndex(SwapArrayIndexBefore + 1);
                }
            }
            else
            {
                BeforeListProperty.DeleteArrayElementAtIndex(SwapArrayIndexBefore);
            }
            so.ApplyModifiedProperties();
            Repaint();
            //SerializedObject so = new SerializedObject(SelectedController);
        }
        public void OnDeleteAbilityObj(int i1, int i2)
        {
            SerializedObject so = new SerializedObject(SelectedController);
            so.Update();
            SerializedProperty combatDatas = so.FindProperty("CombatDatas");
            SerializedProperty ObjsProperty = combatDatas.GetArrayElementAtIndex(i1).FindPropertyRelative("CombatObjs");
            ObjsProperty.DeleteArrayElementAtIndex(i2);
            so.ApplyModifiedProperties();
        }
        /// <summary>
        /// Click select ability
        /// </summary>
        /// <param name="data"> abilityObj </param>
        /// <param name="groupIndex"> the groupindex </param>
        /// <param name="abilityIndex"> the abilityIndex </param>
        public void OnSelectAbilityObj(AbilityScriptableObject data, int groupIndex, int abilityIndex)
        {
            SelectedAbilityObj = data;
            CurrentGroupIndex = groupIndex;
            CurrentAbilityIndexInGroup = abilityIndex;

            //FlushAndInsPreviewToFrame0();
            OnEndPreview();

            LoadL3();


            ChangeInspectedType( InspectedType.AnimationConfig);
        }
        public void ChangeInspectedType(InspectedType targetType)
        {
            CurrentInspectedType = targetType;
            if (CombatInspector.GetInspector() != null)
            {
                CombatInspector.GetInspector().Repaint();
            }
        }

    }
}
