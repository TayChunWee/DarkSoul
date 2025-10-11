using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

 namespace CombatEditor
{	
	public class TransformSearchProvider : ScriptableObject, ISearchWindowProvider
	{
	    public List<Transform> Transforms = new List<Transform>();
	    public CombatController controller;
	    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
	    {
	        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
	        searchList.Add(new SearchTreeGroupEntry(new GUIContent("SelectTransform"), 0));
	        //Debug.Log("CreateSearchTree");
	        for (int i = 0; i < Transforms.Count; i++)
	        {
	            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(Transforms[i].name));
	            entry.level = 1;
	            entry.userData = Transforms[i];
	
	            searchList.Add(entry);
	        }
	        return searchList;
	    }
	    public Action<Transform> OnSetIndexCallBack;
	    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
	    {
	        OnSetIndexCallBack?.Invoke(SearchTreeEntry.userData as Transform);
	        return true;
	    }
	}
}
