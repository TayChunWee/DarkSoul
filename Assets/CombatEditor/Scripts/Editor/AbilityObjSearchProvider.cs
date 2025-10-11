using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

 namespace CombatEditor {	
	public class AbilityObjSearchProvider : ScriptableObject, ISearchWindowProvider
	{
	    public List<AbilityScriptableObject> TemplateObjs;
	    public SerializedObject so;
	    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
	    {
	        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
	        searchList.Add(new SearchTreeGroupEntry(new GUIContent("CreateFrom..."), 0));
	
	        for (int i = 0; i < TemplateObjs.Count; i++)
	        {
	            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(TemplateObjs[i].name));
	            entry.level = 1;
	            entry.userData = TemplateObjs[i];
	            searchList.Add(entry);
	        }
	        return searchList;
	    }
	    public Action<AbilityScriptableObject> OnSetIndexCallBack;
	    //public Action<AbilityScriptableObject> OnSetIndexCallBack;
	    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
	    {
	        OnSetIndexCallBack?.Invoke(SearchTreeEntry.userData as AbilityScriptableObject);
	        //OnSetIndexCallBack?.Invoke(SearchTreeEntry.userData as Type);
	        return true;
	    }
	
	}
}
