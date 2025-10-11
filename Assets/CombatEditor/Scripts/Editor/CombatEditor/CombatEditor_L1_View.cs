using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CombatEditor
{
    public partial class CombatEditor
    {
        public void PaintL1()
        {
            float DefaultWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 80;
            PaintHeader();
            PaintControllerSelectorPopup();
            if (!CharacterExist()) return;
            InitStyleAndAbilities();
            PaintAbilities();
            PaintL1DragTargetRec();
            HandleL1Drag();

            ElementCount = HeightCounter;
            GUI.EndScrollView();
            EditorGUIUtility.labelWidth = DefaultWidth;
            //IsDraggingL1ThisFrame = false;
        }


        //DrawGroup and CombatData.
        public void PaintHeader()
        {
            AbilityRect = new Rect(0, Height_Top, Width_Ability, position.height);
            Rect HeadRect = new Rect(0, 0, Width_Ability, Height_Top);
            InitHeaderStyle();
            GUI.Box(HeadRect, "Animations", HeaderStyle);
        }
      
        public void PaintControllerSelectorPopup()
        {
            if (SelectedController == null)
            {
                TrySetControllerByName();
            }
            if (PopUpStyle == null)
            {
                PopUpStyle = new GUIStyle(EditorStyles.popup);
                PopUpStyle.fixedHeight = LineHeight;
            }
            CharacterConfigRect = new Rect(AbilityRect.x, AbilityRect.y, AbilityRect.width - Width_Scroll, LineHeight);
            float width = SelectedController == null ? CharacterConfigRect.width : CharacterConfigRect.width - LineHeight;
            Rect CharacterSelectRect = new Rect(CharacterConfigRect.x, CharacterConfigRect.y, width, CharacterConfigRect.height);
            LastSelectedControllerName = "SelectController";
            if (SelectedController != null)
            {
                LastSelectedControllerName = SelectedController.name;
            }
            else
            {
                SetL2L3Target(null);
            }
            if (GUI.Button(CharacterSelectRect, new GUIContent(LastSelectedControllerName)))
            {
                PaintControllerSelectMenu();
            }
            if (SelectedController != null)
            {
                PaintConfigIcon();
            }
        }

        public void PaintControllerSelectMenu()
        {
            var ControllersInScene = GameObject.FindObjectsOfType<CombatController>();

            string[] CharacterName = new string[ControllersInScene.Length];
            for (int i = 0; i < ControllersInScene.Length; i++)
            {
                CharacterName[i] = ControllersInScene[i].name;
            }
            GenericMenu Menu = new GenericMenu();
            for (int i = 0; i < CharacterName.Length; i++)
            {
                Menu.AddItem(new GUIContent(CharacterName[i]), false, OnSelectCharacterInMenu, ControllersInScene[i]);
            }
            Menu.ShowAsContext();
        }


        public void PaintConfigIcon()
        {
            Rect ConfigRect = new Rect(AbilityRect.width - Width_Scroll - LineHeight, CharacterConfigRect.y, LineHeight, LineHeight);

            var color = GUI.backgroundColor;
            if (CurrentInspectedType == InspectedType.CombatConfig)
            {
                GUI.backgroundColor = OnInspectedColor;
            }
            if (GUI.Button(ConfigRect, ""))
            {
                OnSelectCharacterConfig();
            }

            CombatEditorUtility.DrawEditorTextureOnRect(ConfigRect, 0.9f, "_Popup@2x");

            GUI.backgroundColor = color;

        }

        public void PaintL1DragTargetRec()
        {
            //Debug.Log(IsDraggingL1ThisFrame);
            if (IsDraggingL1)
            {
                EditorGUI.DrawRect(L1DraggingTargetRect, Color.green);
                if(TargetChangeThisFrame)
                {
                    TargetChangeThisFrame = false;
                    Repaint();
                }
            }
        }

        public void PaintAbilities()
        {
            HeightCounter = 1;
            if (SelectedController == null)
            {
                return;
            }
            for (int i = 0; i < SelectedController.CombatDatas.Count; i++)
            {
                var DataGroup = SelectedController.CombatDatas[i];
                Rect GroupHeaderRect = new Rect(AbilityRect.x, AbilityRect.y + HeightCounter * LineHeight, AbilityRect.width - Width_Scroll, LineHeight);

                HeightCounter += 1;
                PaintGroupHeader(i, DataGroup, GroupHeaderRect);

                if (DataGroup.IsFolded)
                {
                    for (int j = 0; j < DataGroup.CombatObjs.Count; j++)
                    {
                        var data = DataGroup.CombatObjs[j];
                        Rect DataSelectRect = new Rect(AbilityRect.x, AbilityRect.y + HeightCounter * LineHeight, AbilityRect.width - LineHeight - Width_Scroll, LineHeight);
                        Rect DataDeleteRect = new Rect(AbilityRect.width - LineHeight - Width_Scroll, AbilityRect.y + HeightCounter * LineHeight, LineHeight, LineHeight);
                        HeightCounter += 1;

                        //If Selected, Update GUI color.
                        Color defaultColor = GUI.backgroundColor;

                        if (CurrentGroupIndex == i && CurrentAbilityIndexInGroup == j)
                        {
                            HighlightBGIfInspectType(InspectedType.AnimationConfig);
                        }

                        HandleSwapEvents(data, DataSelectRect,i,j);

                        var ClipName = "Null";
                        if (data != null) ClipName = data.Clip ? data.Clip.name : "Empty";
                        if (GUI.Button(DataSelectRect, ClipName))
                        {
                            OnSelectAbilityObj(data, i, j);
                        }
                        GUI.backgroundColor = defaultColor;

                        if (GUI.Button(DataDeleteRect, "-", MyDeleteButtonStyle))
                        {
                            OnDeleteAbilityObj(i, j);
                        }
                    }
                }
            }
            Rect AddGroupRect = new Rect(AbilityRect.x, AbilityRect.y + HeightCounter * LineHeight, AbilityRect.width - Width_Scroll, LineHeight);
            PaintAddGroupButton(AddGroupRect);
            HeightCounter += 2;
        }
        
        public bool PaintGroupHeader(int GroupIndex, CombatGroup DataGroup, Rect rect)
        {
            Rect HeaderRect = new Rect(rect.x, rect.y, rect.width - 2 * LineHeight, LineHeight);
            Rect GroupAddRect = new Rect(rect.width - 2 * LineHeight, rect.y, LineHeight, LineHeight);
            Rect GroupDeleteRect = new Rect(rect.width - LineHeight, rect.y, LineHeight, LineHeight);
            DataGroup.IsFolded = EditorGUI.Foldout(HeaderRect, DataGroup.IsFolded, new GUIContent(DataGroup.Label), true);
            F2ToRename(HeaderRect, GroupIndex);
            if (GUI.Button(GroupAddRect, "+", MyDeleteButtonStyle))
            {

                //SerializedProperty TargetObj = ObjsProperty.GetArrayElementAtIndex(ObjsProperty.arraySize - 1);
                OnAddCombat(GroupAddRect, GroupIndex);
            }
            if (GUI.Button(GroupDeleteRect, "-", MyDeleteButtonStyle))
            {
                SerializedObject so = new SerializedObject(SelectedController);
                so.Update();
                SerializedProperty combatDatas = so.FindProperty("CombatDatas");
                combatDatas.DeleteArrayElementAtIndex(GroupIndex);
                so.ApplyModifiedProperties();
            }
            EditorGUI.HelpBox(HeaderRect, "", MessageType.None);
            return DataGroup.IsFolded;
        }

        public void PaintAddGroupButton(Rect rect)
        {
            if (GUI.Button(rect, "+Group"))
            {
                SerializedObject so = new SerializedObject(SelectedController);
                so.Update();
                SerializedProperty combatDatas = so.FindProperty("CombatDatas");
                combatDatas.InsertArrayElementAtIndex(combatDatas.arraySize);
                combatDatas.GetArrayElementAtIndex(combatDatas.arraySize - 1).FindPropertyRelative("CombatObjs").arraySize = 0;
                combatDatas.GetArrayElementAtIndex(combatDatas.arraySize - 1).FindPropertyRelative("Label").stringValue = "F2_Rename";

                so.ApplyModifiedProperties();
            }
        }
    }
}
