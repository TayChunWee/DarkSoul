using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{	
	public class Node_Sp
	{
	    public CharacterNode.NodeType type;
	    public SerializedProperty sp;
	}
	
	[CustomPropertyDrawer(typeof(InsedObject))]
	public class HandleSelectorProperty : PropertyDrawer
	{
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	    {
	        EditorGUI.BeginProperty(position, label, property);
	        GUILayout.BeginVertical("TransformData", "window",GUILayout.Height(120));
	        float TargetlabelWidth = 100;
	        float TabHeight = 20;
	        float TabWidth = 60;
	        
	        //Property
	        var labelWidth = EditorGUIUtility.labelWidth;
	        EditorGUIUtility.labelWidth = TargetlabelWidth;
	        //UpdatePreviewIfChange
	        EditorGUI.BeginChangeCheck();
	        EditorGUILayout.PropertyField(property.FindPropertyRelative("TargetObj"));
	        if (EditorGUI.EndChangeCheck())
	        {
	            property.serializedObject.ApplyModifiedProperties();
	            if (CombatEditorUtility.EditorExist())
	            {
	                var edit = CombatEditorUtility.GetCurrentEditor();
	                edit.RequirePreviewReload();
	                //edit.HardResetPreviewToCurrentFrame();
	            }
	        }
	
	        //Offset and Rot
	        EditorGUILayout.BeginHorizontal(GUILayout.Height(TabHeight));
	        EditorGUILayout.LabelField("Pos",GUILayout.Width(TargetlabelWidth));
	        EditorGUILayout.PropertyField(  property.FindPropertyRelative("Offset"),new GUIContent(""));
	        if (GUILayout.Button("Reset", "Button", GUILayout.Width(TabWidth)))
	        {
	            property.FindPropertyRelative("Offset").vector3Value = Vector3.zero;
	        }
	        EditorGUILayout.EndHorizontal();
	
	
	        EditorGUILayout.BeginHorizontal(GUILayout.Height(TabHeight));
	        EditorGUILayout.LabelField("Rot", GUILayout.Width(TargetlabelWidth));
	        var v3 = EditorGUILayout.Vector3Field("", ((Quaternion)property.FindPropertyRelative("Rot").quaternionValue).eulerAngles);
	        property.FindPropertyRelative("Rot").quaternionValue = Quaternion.Euler(v3);
	        if (GUILayout.Button("Reset", "Button", GUILayout.Width(TabWidth)))
	        {
	            property.FindPropertyRelative("Rot").quaternionValue = Quaternion.identity;
	        }
	        EditorGUILayout.EndHorizontal();
	        //EditorGUILayout.EndHorizontal();
	
	
	
	        var editor = CombatEditorUtility.GetCurrentEditor();
	        List<string> NodeTypesInController = new List<string>();
	        var nodes = editor.SelectedController.Nodes;
	
	        var DefaultNodeName = System.Enum.GetName(typeof(CharacterNode.NodeType), 0);
	        NodeTypesInController.Add(DefaultNodeName);
	        for (int i = 0; i < nodes.Count; i++)
	        {
	            string EnumName = System.Enum.GetName(typeof(CharacterNode.NodeType), (int)nodes[i].type);
	            NodeTypesInController.Add(System.Enum.GetName(typeof(CharacterNode.NodeType), (int)nodes[i].type));
	        }
	
	        GenericMenu menu = new GenericMenu();
	        string name = property.FindPropertyRelative("TargetNode").enumNames[property.FindPropertyRelative("TargetNode").enumValueIndex];
	
	        EditorGUILayout.BeginHorizontal();
	        EditorGUILayout.LabelField("TargetNode", GUILayout.Width(TargetlabelWidth - 1));
	        if (EditorGUILayout.DropdownButton(new GUIContent(name), FocusType.Passive))
	        {
	            for (int i = 0; i < NodeTypesInController.Count; i++)
	            {
	                Node_Sp node_So = new Node_Sp();
	                node_So.sp = property;
	                if (i == 0)
	                {
	                    node_So.type = CharacterNode.NodeType.Animator;
	                    menu.AddItem(new GUIContent(NodeTypesInController[i]), false, SetNode, node_So);
	                }
	                else if (nodes[i - 1].type != CharacterNode.NodeType.Animator)
	                {
	                    node_So.type = nodes[i - 1].type;
	                    menu.AddItem(new GUIContent(NodeTypesInController[i]), false, SetNode, node_So);
	                }
	            }
	            menu.ShowAsContext();
	        }
	
	
	        //Texture Config = EditorGUIUtility.IconContent("_Popup@2x").image;
	        //Config.filterMode = FilterMode.Bilinear;
	        if(GUILayout.Button("Config", GUILayout.Width(60), GUILayout.Height(18)))
	        {
	            //CombatEditorUltilies.GetCurrentEditor().select
	            CombatInspector.GetInspector().SelectCombatConfig();
	        }
	
	        EditorGUILayout.EndHorizontal();
	        EditorGUILayout.PropertyField(property.FindPropertyRelative("FollowNode"));
	        EditorGUILayout.PropertyField(property.FindPropertyRelative("RotateByNode"));
	
	        //SerializedObject so = new SerializedObject(editor.SelectedController);
	        //so.Update();
	        //EditorGUILayout.PropertyField(so.FindProperty("Nodes"));
	        //so.ApplyModifiedProperties();
	
	        EditorGUIUtility.labelWidth = labelWidth;
	
	        EditorGUI.EndProperty();
	        GUILayout.EndVertical();
	        //base.OnGUI(position, property, label);
	
	    }
	
	    public void SetNode(object nodetype)
	    {
	        Node_Sp nodeSO = nodetype as Node_Sp;
	        SerializedObject so = nodeSO.sp.serializedObject;
	        so.Update();
	
	        nodeSO.sp.FindPropertyRelative("TargetNode").enumValueIndex = (int)nodeSO.type;
	        //SceneView.RepaintAll();
	        so.ApplyModifiedProperties();
	    }
	
	}
}
