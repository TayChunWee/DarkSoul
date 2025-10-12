using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

 namespace CombatEditor
{	
	public class AnimEventSearchProvider: ScriptableObject, ISearchWindowProvider
	{
	    public Type[] types;
	    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
	    {
	        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
	        searchList.Add(new SearchTreeGroupEntry(new GUIContent("List"), 0));
	
	        for (int i = 0; i < types.Length; i++)
	        {
	            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(types[i].Name .Replace("AbilityEventObj_","")));
	            entry.level = 1;
	            entry.userData = types[i];
	            searchList.Add(entry);
	        }
	        return searchList;
	    }
	
	    public Action<Type> OnSetIndexCallBack;
	    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
	    {
	        OnSetIndexCallBack?.Invoke(SearchTreeEntry.userData as Type);
	        return true;
	    }
	}
}
