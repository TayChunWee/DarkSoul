using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

 namespace CombatEditor
{	
	public class CombatInspector : EditorWindow
	{
	
	    float Height_Top = 40;
	    CombatEditor combatEditor;
	
	    //ReorderableList NodeList;
	
	    [MenuItem("Tools/CombatInspector")]
	    static void Init()
	    {
	        // Get existing open window or if none, make a new one:
	        CombatInspector.CreateWindow();
	        //window.InitWindow();
	    }
	
	    public static CombatInspector CreateWindow()
	    {
	        CombatInspector window = (CombatInspector)EditorWindow.GetWindow(typeof(CombatInspector));
	        window.Show();
	        return window;
	    }
	    private void OnEnable()
	    {
	    }
	    public void ResetInspector()
	    {
	        CombatControllerSO = null;
	        NodeList = null;
	    }
	
	
	    private void OnGUI()
	    {
	        PaintInspector();
	    }
	    
	    Editor InspectedEditor;
	    Vector2 InspectorScrollPos;
	
	    int SelectedClipIndex;
	
	    Vector2 Scroll;
	
	    int CurrentGroupIndex = -1;
	    int CurrentAbilityIndex = -1;
	    public void PaintInspector()
	    {
	        if (!CombatEditorUtility.EditorExist())
	        {
	            return;
	        }
	        combatEditor = CombatEditorUtility.GetCurrentEditor();
	        if(combatEditor.SelectedController == null)
	        {
	            return;
	        }
	
        #region Init
	        var HeaderStyle = combatEditor.HeaderStyle;
	        var inspectedType = combatEditor.CurrentInspectedType;
	
	        var  CurrentAbilityObj = combatEditor.SelectedAbilityObj;
	
	        CurrentGroupIndex = combatEditor.CurrentGroupIndex;
	        CurrentAbilityIndex = combatEditor.CurrentAbilityIndexInGroup;
	
	        var SelectedTrackIndex = combatEditor.SelectedTrackIndex;
	        var TrackHeight = CombatEditor.LineHeight;
        #endregion
	        GUILayout.Box("Inspector", HeaderStyle, GUILayout.Height(Height_Top));
	        Rect InspectorRect = new Rect(new Rect(0, Height_Top, position.width, position.height));
	        if (inspectedType == CombatEditor.InspectedType.Null)
	        {
	            return;
	        }
	        EditorGUILayout.BeginScrollView(Scroll);
            if(inspectedType == CombatEditor.InspectedType.PreviewConfig)
            {
                combatEditor.PlayTimeMultiplier = EditorGUILayout.FloatField(new GUIContent("PlaySpeed"),combatEditor.PlayTimeMultiplier);
                combatEditor.LoopWaitTime = EditorGUILayout.FloatField(new GUIContent("LoopInterval"),combatEditor.LoopWaitTime);
            }
	        if (inspectedType == CombatEditor.InspectedType.AnimationConfig)
	        {
	            float DefaultWidth = EditorGUIUtility.labelWidth;
	            EditorGUIUtility.labelWidth = 80;
	            SerializedObject so = new SerializedObject(combatEditor.SelectedController);
	            SerializedProperty combatDatas = so.FindProperty("CombatDatas");
	            so.Update();
	            if (combatEditor.CurrentGroupIndex < combatDatas.arraySize && combatEditor.CurrentGroupIndex >= 0)
	            {
	                SerializedProperty ObjsProperty = combatDatas.GetArrayElementAtIndex(combatEditor.CurrentGroupIndex).FindPropertyRelative("CombatObjs");
	                if (combatEditor.CurrentAbilityIndexInGroup < ObjsProperty.arraySize && combatEditor.CurrentAbilityIndexInGroup >= 0)
	                {
	                    SerializedProperty TargetObj = ObjsProperty.GetArrayElementAtIndex(combatEditor.CurrentAbilityIndexInGroup);
	                    EditorGUI.BeginChangeCheck();
	                    EditorGUILayout.PropertyField(TargetObj,new GUIContent("ConfigFile"));
	                    if(EditorGUI.EndChangeCheck())
	                    {
	                        combatEditor.SelectedAbilityObj = (AbilityScriptableObject)TargetObj.objectReferenceValue;
	                        combatEditor.LoadL3();
	                        combatEditor.Repaint();
	                        combatEditor.FlushAndInsPreviewToFrame0();
	                        Repaint();
	                    }
	                    if(TargetObj.objectReferenceValue == null)
	                    {
	                       if( GUILayout.Button("CreatConfig"))
	                        {
	                            TargetObj.objectReferenceValue = CreateAbilityScriptableObject();
	                            combatEditor.SelectedAbilityObj = (AbilityScriptableObject)TargetObj.objectReferenceValue;
	                            combatEditor.LoadL3();
	                            combatEditor.Repaint();
	                            combatEditor.FlushAndInsPreviewToFrame0();
	                        }
	                    }
	
	                    if (TargetObj.objectReferenceValue != null)
	                    {
	                        DrawAnimationClipSelector((AbilityScriptableObject)TargetObj.objectReferenceValue);
	                        DrawAbilityEventSelector((AbilityScriptableObject)TargetObj.objectReferenceValue);
	                    }
	                }
	            }
	            so.ApplyModifiedProperties();
	            EditorGUIUtility.labelWidth = DefaultWidth;
	        }
	        if (inspectedType == CombatEditor.InspectedType.Track)
	        {
	            //myRect.center = Vector2.one * 200;
	            float DefaultWidth = EditorGUIUtility.labelWidth;
	            EditorGUIUtility.labelWidth = 80;
	            if (CurrentAbilityObj != null)
	            {
	                if (SelectedTrackIndex - 1 < CurrentAbilityObj.events.Count && SelectedTrackIndex - 1 >= 0 && CurrentAbilityObj.events.Count > 0)
	                {
	                    string name = CurrentAbilityObj.events[SelectedTrackIndex - 1].Obj.name;
	                    CurrentAbilityObj.events[SelectedTrackIndex - 1].Obj.name = EditorGUILayout.TextField("Name", name);
	                }
	                if (InspectedEditor != null)
	                {
	                    if (InspectedEditor.target != null)
	                    {
	                        InspectedEditor.OnInspectorGUI();
	                    }
	                }
	                EditorGUIUtility.labelWidth = DefaultWidth;
	            }
	        }
	        if(inspectedType == CombatEditor.InspectedType.CombatConfig)
	        {
	            if (combatEditor.SelectedController != null)
	            {
	                CombatController controller = combatEditor.SelectedController;
	                SerializedObject so = new SerializedObject(controller);
                    //CombatControllerSO.Update();
                    EditorGUILayout.PropertyField(so.FindProperty("_animator"));
                    if(combatEditor.SelectedController._animator != null)
                    {
                        if(combatEditor.SelectedController._animator.transform == combatEditor.SelectedController.transform)
                        {
                            EditorGUILayout.HelpBox("Animator transform should be the child transform of Combatcontroller!",MessageType.Error);
                        }
                    }

	                if (NodeList == null || CombatControllerSO == null)
	                {
	                    InitNodeReorableList();
	                }
	                CombatControllerSO.Update();
	                NodeList.DoLayoutList();
	                CombatControllerSO.ApplyModifiedProperties();
	                so.ApplyModifiedProperties();
	
	
	            }
	        }
	        EditorGUILayout.EndScrollView();
	    }
	
	    public AbilityScriptableObject CreateAbilityScriptableObject()
	    {
	        if (!System.IO.Directory.Exists(CombatEditor.SandBoxPath))
	        {
	            System.IO.Directory.CreateDirectory(CombatEditor.SandBoxPath);
	        }
	        AbilityScriptableObject InsObj = CreateInstance("AbilityScriptableObject") as AbilityScriptableObject;
	        InsObj.name = "NewAbilityScriptableObject";
	        string path = CombatEditor.SandBoxPath;
	        int index = 0;
	        string TargetPath = path + InsObj.name + ".asset";
	        while (true)
	        {
	            //����ͬ�ļ��͸���
	            if (File.Exists(TargetPath))
	            {
	                TargetPath = path + InsObj.name + "_" + index + ".asset";
	            }
	            else
	            {
	                break;
	            }
	            index += 1;
	        }
	        AssetDatabase.CreateAsset(InsObj, TargetPath);
	        return InsObj;
	    }
	
	
	
	
	    public Rect WindowRect = new Rect(20, 20, 120, 50);
	
	    public void CreateInspectedObj(Object InspectedObj)
	    {
	        ClearInspectedObj();
	
	        InspectedEditor = Editor.CreateEditor(InspectedObj);
	        Repaint();
	    }
	    public void ClearInspectedObj()
	    {
	        if (InspectedEditor != null)
	        {
	            DestroyImmediate(InspectedEditor);
	        }
	    }
	    public static CombatInspector GetInspector()
	    {
	        return EditorWindow.GetWindow<CombatInspector>(false);
	    }
	
	    public void DrawAbilityConfigSelector(AbilityScriptableObject CurrentAbilityObj)
	    {
	        EditorGUI.BeginDisabledGroup(true);
	        EditorGUILayout.ObjectField("ConfigObj", CurrentAbilityObj, typeof(AbilityScriptableObject), false);
	        EditorGUI.EndDisabledGroup();
	    }
	    public void DrawAbilityEventSelector(AbilityScriptableObject CurrentAbilityObj)
	    {
	        if (EditorGUILayout.DropdownButton(new GUIContent("Copy Events From Template"), FocusType.Passive))
	        {
	            if (!System.IO.Directory.Exists(CombatEditor.TemplatesPath))
	            {
	                System.IO.Directory.CreateDirectory(CombatEditor.TemplatesPath);
	            }
	            AbilityScriptableObject[] TemplatesObjs = CombatEditor.GetAtPath<AbilityScriptableObject>(CombatEditor.TemplatesPath);
	            List<string> TemplatesObjNames = new List<string>();
	            GenericMenu menu = new GenericMenu();
	            for (int i = 0; i < TemplatesObjs.Length; i++)
	            {
	                TemplatesObjNames.Add(TemplatesObjs[i].name);
	                menu.AddItem(new GUIContent(TemplatesObjs[i].name), false, CopyAbilityEvent, TemplatesObjs[i]);
	            }
	            menu.ShowAsContext();
	        }
	    }
	
	    public void CopyAbilityEvent(object obj)
	    {
	        var editor = CombatEditorUtility.GetCurrentEditor();
	        AbilityScriptableObject CurrentObj = editor.SelectedAbilityObj;
	        for(int i =0;i<CurrentObj.events.Count;i++)
	        {
	            string path = AssetDatabase.GetAssetPath(CurrentObj.events[i].Obj);
	            var EveObj = CurrentObj.events[i].Obj;
	            AssetDatabase.RemoveObjectFromAsset(EveObj);
	            DestroyImmediate( EveObj, true);
	        }
	        CurrentObj.events = new List<AbilityEvent>();
	        
	
	
	        List<AbilityEvent> TargetEves = new List<AbilityEvent>();
	        TargetEves = (obj as AbilityScriptableObject).events;
	
	        for (int i = 0; i < TargetEves.Count; i++)
	        {
	            if (TargetEves[i].Obj == null) continue;
	            AbilityEvent eve = new AbilityEvent();
	            var EveObj = Instantiate(TargetEves[i].Obj);
	            eve.Obj = EveObj;
	            string path = AssetDatabase.GetAssetPath(editor.SelectedAbilityObj);
	            eve.Obj.name = eve.Obj.name.Replace("(Clone)","");
	            AssetDatabase.AddObjectToAsset(EveObj, path);
	            CurrentObj.events.Add(eve);
	        }
	
	        AssetDatabase.SaveAssets();
	        AssetDatabase.Refresh();
	
	        editor.LoadL3();
	        //Debug.Log("CopyAbility!");
	    }
	    public void DrawAnimationClipSelector(AbilityScriptableObject CurrentAbilityObj)
	    {
	        if(CurrentAbilityObj == null)
	        {
	            return;
	        }
	       
	
	        CurrentAbilityObj.Clip = (AnimationClip)EditorGUILayout.ObjectField("Clip", CurrentAbilityObj.Clip, typeof(AnimationClip), false);
	
	        CombatController controller = combatEditor.SelectedController;
	        Animator animator = controller._animator;
	        if (animator == null) return;
	        var clips = animator.runtimeAnimatorController.animationClips;
	
	        //Animator clips may change, so the index should auto change.
	        bool ClipExist = false;
	        for (int i = 0; i < clips.Length; i++)
	        {
	            if (CurrentAbilityObj.Clip == clips[i])
	            {
	                SelectedClipIndex = i + 1;
	                ClipExist = true;
	            }
	        }
	        if (!ClipExist)
	        {
	            SelectedClipIndex = 0;
	        }
	
	        List<string> clipsNames = new List<string>();
	        //clipsNames.Add("Null");
	        for (int i = 0; i < clips.Length; i++)
	        {
	            clipsNames.Add(clips[i].name);
	        }
	
	
	        string[] ClipNamesArray = clipsNames.ToArray();
	
	
	        GenericMenu menu = new GenericMenu();
	        if (EditorGUILayout.DropdownButton(new GUIContent("Select Clip From Animator"), FocusType.Passive))
	        {
	            for (int i = 0; i < clipsNames.Count; i++)
	            {
	                menu.AddItem(new GUIContent(clipsNames[i]), false, (object index) =>
	                {
	                    int ClipIndex = 0;
	                    int.TryParse(index.ToString(), out ClipIndex);
	                    CurrentAbilityObj.Clip = clips[ClipIndex];
	
	                    combatEditor.LoadL3();
	                    //combatEditor.Repaint();
	                }, i);
	                menu.ShowAsContext();
	            }
	        }
	        
	        combatEditor.LoadL3();
	    }
	    ReorderableList NodeList;
	
	    SerializedObject CombatControllerSO;
	    public void InitNodeReorableList()
	    {
	        CombatControllerSO = new SerializedObject(combatEditor.SelectedController); 
	        NodeList = new ReorderableList(CombatControllerSO, CombatControllerSO.FindProperty("Nodes"), true, true, true, true);
	        NodeList.drawHeaderCallback = (Rect rect) =>
	             {
	                 GUI.Label(rect, "CharacterNodes");
	             };
	        NodeList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
	        {
	            EditorGUI.PropertyField(new Rect(rect.x,rect.y+2,rect.width,rect.height), NodeList.serializedProperty.GetArrayElementAtIndex(index));
	        };
	
	    }
	    public void SelectCombatConfig()
	    {
	        //CombatControllerSO = new SerializedObject(combatEditor.SelectedController);
	        combatEditor.CurrentInspectedType = CombatEditor.InspectedType.CombatConfig;
	        Repaint();
	        InitNodeReorableList();
	    }
	}
}
