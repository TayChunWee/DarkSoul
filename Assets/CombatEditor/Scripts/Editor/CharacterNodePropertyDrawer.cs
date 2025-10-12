using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

 namespace CombatEditor
{
    [CustomPropertyDrawer(typeof(CharacterNode))]
	public class CharacterNodePropertyDrawer : PropertyDrawer
	{
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	    {
	        property.serializedObject.Update();
	
	        EditorGUI.BeginChangeCheck();
	
	        EditorGUI.BeginProperty(position, label, property);
	
	        CombatEditor editor = CombatEditorUtility.GetCurrentEditor();
	        var controller = editor.SelectedController;
	
	
	        var height = EditorGUIUtility.singleLineHeight;
	        Rect propertyRect = new Rect(position.x, position.y, position.width * 0.5f - 20, position.height * 0.5f);
	        Rect NodeRect = new Rect(position.x + position.width * 0.5f - 15, position.y, position.width * 0.5f - 5, height);
	        Rect SelectButtonRect = new Rect(position.x + position.width - position.height, position.y, height, height);
	        EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("type"), new GUIContent(""));
	        EditorGUI.PropertyField(NodeRect, property.FindPropertyRelative("NodeTrans"), new GUIContent(""));
	
	        if (GUI.Button(SelectButtonRect, ""))
	        {
	            TransformSearchProvider provider = ScriptableObject.CreateInstance("TransformSearchProvider") as TransformSearchProvider;
	            provider.Transforms = controller.transform.GetComponentsInChildren<Transform>().ToList();
	            provider.OnSetIndexCallBack = (trans) =>
	            {
	
	                property.serializedObject.Update();
	                property.FindPropertyRelative("NodeTrans").objectReferenceValue = trans;
	                Object.DestroyImmediate(provider);
	                property.serializedObject.ApplyModifiedProperties();
	                //so.ApplyModifiedProperties();
	            };
	            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition + new Vector2(120, 10))), provider);
	        }
	        CombatEditorUtility.DrawEditorTextureOnRect(SelectButtonRect, 1, "_Menu@2x");
	
	        EditorGUI.EndProperty();
	
	        if(EditorGUI.EndChangeCheck())
	        {
	            if(CombatEditorUtility.EditorExist())
	            {
	                CombatEditorUtility.GetCurrentEditor().RequirePreviewReload();
	            }
	        }
	        property.serializedObject.ApplyModifiedProperties();
	    }
	    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	    {
	        return 20;
	    }
	}
}
