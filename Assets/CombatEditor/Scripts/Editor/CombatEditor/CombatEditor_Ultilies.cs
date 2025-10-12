using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{	
	public partial class CombatEditor : EditorWindow
	{
	    private Texture2D MakeTex(int width, int height, Color col)
	    {
	        Color[] pix = new Color[width * height];
	        for (int i = 0; i < pix.Length; ++i)
	        {
	            pix[i] = col;
	        }
	        Texture2D result = new Texture2D(width, height);
	        result.SetPixels(pix);
	        result.Apply();
	        return result;
	    }
	
	    public void InitGUIStyle()
	    {
	        InitDeleteButtonStyle();
	        InitBoxGUIStyle();
	        InitHeaderStyle();
	    }
	
	    //public GUIStyle OnInspectedButtonStyle;
	    //public void InitOnInspectedButtonStyle()
	    //{
	    //    if(OnInspectedButtonStyle == null)
	    //    {
	    //        OnInspectedButtonStyle = new GUIStyle(GUI.skin.button);
	    //    }
	    //}
	    //public void InitOnSelectedButtonStyle()
	    //{
	
	    //}
	
	    public void HighlightBGIfInspectType(InspectedType type)
	    {
	        if (CurrentInspectedType == type)
	        {
	            GUI.backgroundColor = OnInspectedColor;
	        }
	        else
	        {
	            GUI.backgroundColor = SelectedColor;
	        }
	    }
	
	    public void InitDeleteButtonStyle()
	    {
	        if (MyDeleteButtonStyle == null)
	        {
	            MyDeleteButtonStyle = new GUIStyle(GUI.skin.button);
	            MyDeleteButtonStyle.margin = new RectOffset(0, 0, 0, 0);
	            MyDeleteButtonStyle.fontSize = 20;
	            MyDeleteButtonStyle.padding = new RectOffset(0,0,0,0);
	            MyDeleteButtonStyle.alignment = TextAnchor.MiddleCenter;
	            MyDeleteButtonStyle.fontStyle = FontStyle.Bold;
	            MyDeleteButtonStyle.contentOffset = new Vector2(0, 0);
	        }
	    }
	
	
	    public void InitBoxGUIStyle()
	    {
	        if (MyBoxGUIStyle == null)
	        {
	            MyBoxGUIStyle = new GUIStyle(GUI.skin.box);
	            MyBoxGUIStyle.normal.background = MakeTex(2, 2, Color.white);
	            MyBoxGUIStyle.border = new RectOffset(2, 2, 2, 2);
	        }
	    }
	
	
	    public void InitHeaderStyle()
	    {
	        if (HeaderStyle == null)
	        {
	            HeaderStyle = new GUIStyle(EditorStyles.helpBox);
	            HeaderStyle.alignment = GUI.skin.button.alignment;
	            HeaderStyle.fontSize = HeaderFontSize;
	            HeaderStyle.fontStyle = FontStyle.Bold;
	        }
	    }
	
	
	    bool IsPaintingRenameField;
	    Rect RenameFieldRect;
	    Rect RenameTargetRect;
	    string NameOfRename = "";
	
	
	    public void StartPaintRenameField(Rect TargetRect, string DefaultName, System.Action finishRenameAction)
	    {
	        FinishRenameAction = finishRenameAction;
	        NameOfRename = DefaultName;
	        RenameTargetRect = TargetRect;
	        Event e = Event.current;
	        //RenameFieldRect = new Rect(e.mousePosition.x, e.mousePosition.y, 200, 100);
	        RenameFieldRect = TargetRect;
	        //Vector2 StartPos = new Vector2(e.mousePosition.x, e.mousePosition.y);
	        IsPaintingRenameField = true;
	        PaintRenameField();
	
	        GUI.FocusControl("RenameField");
	        //Debug.Log(GUI.GetNameOfFocusedControl());
	    }
	    System.Action FinishRenameAction;
	
	    public void PaintRenameField()
	    {
	        //EditorGUI.DrawRect(RenameTargetRect,Color.green);
	        if (!IsPaintingRenameField)
	        {
	            return;
	        }
	
	        Event e = Event.current;
	        if (e.isKey && e.keyCode == KeyCode.Return)
	        {
	            StopRename();
	        }
	        if (e.isMouse)
	        {
	            if (!RenameFieldRect.Contains(e.mousePosition))
	            {
	                StopRename();
	            }
	        }
	        GUI.SetNextControlName("RenameField");
	
	        Rect InputRect = new Rect(RenameFieldRect.x, RenameFieldRect.y, RenameFieldRect.width, RenameFieldRect.height);
	        GUI.FocusControl("RenameField");
	
	        GUIStyle RenameStyle = EditorStyles.textField;
	        RenameStyle.alignment = TextAnchor.MiddleLeft;
	        NameOfRename = EditorGUI.TextField(InputRect, NameOfRename, RenameStyle);
	
	
	        //GUI.depth = 1;
	
	        //Repaint();
	    }
	
	   
	
	
	    public void StopRename()
	    {
	        IsPaintingRenameField = false;
	        if (FinishRenameAction != null)
	        {
	            FinishRenameAction.Invoke();
	        }
	    }
	    public static T[] GetAtPath<T>(string path)
	    {
	
	        ArrayList al = new ArrayList();
	
	        path = path.Remove(0, 6);
	        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);
	        foreach (string fileName in fileEntries)
	        {
	            int index = fileName.LastIndexOf("/");
	            string localPath = "Assets/" + path;
	
	            if (index > 0)
	                localPath += fileName.Substring(index);
	            //Debug.Log(path);
	            Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
	
	            if (t != null)
	                al.Add(t);
	        }
	        T[] result = new T[al.Count];
	        for (int i = 0; i < al.Count; i++)
	            result[i] = (T)al[i];
	
	        return result;
	    }
	
	    public void DrawHorizontalLine(Vector3 p1, Vector3 p2, Color color, float Width)
	    {
	        EditorGUI.DrawRect(new Rect(p1.x, p1.y - Width / 2, (p2 - p1).x, Width), color);
	    }
	    public void DrawVerticalLine(Vector3 p1, Vector3 p2, Color color, float Width)
	    {
	        EditorGUI.DrawRect(new Rect(p1.x - Width / 2, p1.y, Width, (p2 - p1).y), color);
	    }
	    public void UpdateAsset(Object obj)
	    {
	
	        EditorUtility.SetDirty(SelectedAbilityObj);
	        AssetDatabase.SaveAssets();
	        //AssetDatabase.Refresh();
	    }
	    
	
	    public void LoadL3()
	    {
	        AnimEventTracks = new List<AnimEventTrack>();
	        if (SelectedAbilityObj != null)
	        {
	            for (int i = 0; i < SelectedAbilityObj.events.Count; i++)
	            {
	                AnimEventTracks.Add(new AnimEventTrack(SelectedAbilityObj.events[i], this));
	            }
	            if (SelectedAbilityObj.Clip != null)
	            {
	                AnimFrameCount = (int)(SelectedAbilityObj.Clip.length * 60);
	            }
	            else
	            {
	                AnimFrameCount = 0;
	            }
	        }
	        InitRect();
	    }
	   
	}
	
	public static class CombatEditorUtility
	{
	    public static void ReloadAnimEvents()
	    {
	        
	        GetCurrentEditor().LoadL3();
	    }
	    public static CombatEditor GetCurrentEditor()
	    {
	        return EditorWindow.GetWindow<CombatEditor>(false,"",false);
	    }
	    public static bool EditorExist()
	    {
	        return EditorWindow.HasOpenInstances<CombatEditor>();
	    }
	
	    public static Rect ScaleRect(Rect rect,float Scale)
	    {
	        Rect RectAfterScale = new Rect
	            (rect.x + 0.5f* (rect.width - rect.width * Scale),
	            rect.y + 0.5f * (rect.height - rect.height * Scale),
	            rect.width * Scale,rect.height*Scale);
	        return RectAfterScale;
	    }
	    public static void DrawEditorTextureOnRect(Rect rect,float Scale, string name)
	    {
	        rect = CombatEditorUtility.ScaleRect(rect, Scale);
	        var texture = EditorGUIUtility.IconContent(name).image;
	        if(texture == null)
	        {
	            return;
	        }
	
	        texture.filterMode = FilterMode.Bilinear;
	        GUI.DrawTexture(rect, EditorGUIUtility.IconContent(name).image);
	    }
	
	
	}


    public class TimeLineHelper
    {
        bool IsDraggingthis;
        public EditorWindow TargetWindow;
        public TimeLineHelper(EditorWindow window)
        {
            TargetWindow = window;
        }
        public int DrawHorizontalDraggablePoint(int Value,
            int MaxValue,
            Rect rect,
            Color color,
            GUIStyle style,
            float Width = 5,
            bool LeftMouse = true,
            bool DrawPoint = true,
            bool DragStartOnMouseIn = false,
            System.Action<float> DragAction = null,
            System.Action FinishAction = null)
        {
            Event e = Event.current;
            float Percentage = (float)Value / (float)MaxValue;

            Rect PointRect = new Rect(rect.x + Percentage * rect.width - Width / 2, rect.y , Width, rect.height );
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            //Draw white background when selected.
            if (GUIUtility.hotControl == controlID)
            {
                //if (rect.Contains(e.mousePosition))
                //{
                    EditorGUI.DrawRect(rect, 0.5f * Color.white);
                //}
            }

            //GUI.depth = 1;
            if (DrawPoint)
            {
                GUI.Box(PointRect, "", style);
            }
            //GUI.depth = 0;

            int TargetMouseButton = LeftMouse ? 0 : 1;

            //Paint On Focus?
        
            switch (e.GetTypeForControl(controlID))
            {
                case (EventType.MouseDown):
                    if (e.button == TargetMouseButton)
                    {
                        if ((DragStartOnMouseIn && PointRect.Contains(e.mousePosition)) || (!DragStartOnMouseIn && rect.Contains(e.mousePosition)))
                        {
                            GUIUtility.hotControl = controlID;
                            e.Use();
                        }

                    }
                    break;
                case (EventType.MouseDrag):
                    {
                        if (GUIUtility.hotControl == controlID && e.button == TargetMouseButton)
                        {
                            Percentage = ((e.mousePosition.x - rect.x) / rect.width);
                            Percentage = Mathf.Clamp(Percentage, 0, 1);
                            Value = Mathf.RoundToInt(Percentage * MaxValue);
                            Percentage = (float)Value / (float)MaxValue;
                            PointRect = new Rect(rect.x + Percentage * rect.width - Width / 2, rect.y, Width, rect.height);
                            if (DrawPoint)
                            {
                                EditorGUI.DrawRect(PointRect, color);
                            }
                            if (DragAction != null)
                            {
                                DragAction(Percentage);
                            }
                            TargetWindow.Repaint();
                        }
                    }
                    break;
                case (EventType.MouseUp):
                    {
                        if (e.button == TargetMouseButton)
                        {
                            if (GUIUtility.hotControl == controlID)
                            //if (IsDraggingthis)
                            {
                                GUIUtility.hotControl = 0;
                                if (FinishAction != null)
                                {
                                    FinishAction.Invoke();
                                }
                            }
                            //IsDraggingthis = false;
                            
                        }
                    }
                    break;
            }


            EditorGUIUtility.AddCursorRect(PointRect, MouseCursor.SlideArrow, controlID);
            return Value;
        }

        public int[] DrawHorizontalDraggableRange(int Value1, int Value2, int MaxValue, Rect rect, Color color, GUIStyle boxStyle, float Width = 5, System.Action FinishDragAction = null)
        {
            if (IsDraggingthis)
            {
            }
            //Right handle using left mouse, must start from right handle 
            int RightValue = DrawHorizontalDraggablePoint(Value2, MaxValue, rect, color, boxStyle, Width, true, false, true, null, FinishDragAction);
            //Right handle using right mouse, can start from anywhere in rect.
            RightValue = DrawHorizontalDraggablePoint(RightValue, MaxValue, rect, color, boxStyle, Width, false, false, false, null, FinishDragAction);

            int LeftValue = DrawHorizontalDraggablePoint(Value1, MaxValue, rect, color, boxStyle, Width, true, false, false, null, FinishDragAction);


            float Percentage1 = (float)Value1 / (float)MaxValue;
            float Percentage2 = (float)Value2 / (float)MaxValue;

            Color defaultColor = GUI.color;
            GUI.color = color;

            Rect TargetRect = new Rect(rect.x + Percentage1 * rect.width, rect.y, (Percentage2 - Percentage1) * rect.width, rect.height);
            if (Percentage2 > Percentage1)
            {
                GUI.Box(TargetRect, "", boxStyle);
            }
            else
            {
                GUI.Box(TargetRect, "", boxStyle);
            }
            GUI.color = defaultColor;
            return new int[] { LeftValue, RightValue };
        }


        //int BoxEdgeWidth = 1;
        //static Color[] MultiColors= new Color{Color.blue,Color.cyan};
        
        public int[] DrawHorizontalMultiDraggable(int[] Values,string[] Names, int MaxValue, Rect rect, Color color, float Width = 5, System.Action FinishDragAction = null)
        {
            int[] ModifiedValue = Values;
          
            Color defaultColor = GUI.color;
            GUI.color = color;

            float[] VisiableValue = new float[Values.Length + 2];
            VisiableValue[0] = 0;
            VisiableValue[Values.Length + 1] = 1;
            for(int i =0;i<Values.Length;i++)
            {
                VisiableValue[i + 1] = (float)Values[i] / (float)MaxValue;
            }
            for (int i = 0; i < Values.Length; i++)
            {
                ModifiedValue[i] = DrawHorizontalDraggablePoint(Values[i], MaxValue, rect, color, "flow node " + 4, Width, true, false, true, null, FinishDragAction);
            }
            for (int i = 0; i < VisiableValue.Length - 1; i++)
            {
                Rect TargetRect = new Rect(rect.x + VisiableValue[i] * rect.width + 1, rect.y, (VisiableValue[i + 1] - VisiableValue[i]) * rect.width - 2 , rect.height);
                //Rect InnerRect = new Rect(TargetRect.x + BoxEdgeWidth, TargetRect.y + BoxEdgeWidth, TargetRect.width - 2 * BoxEdgeWidth, TargetRect.height - 2 * BoxEdgeWidth);

                if (Names[i] != "" && Names[i]!=null)
                {
                    GUI.Box(TargetRect, Names[i], "flow node " + 5);
                }
                else
                {
                    GUI.Box(TargetRect, Names[i], "flow node " + 0);
                }
               
            }
          


            return ModifiedValue;
        }



	
	    public float DrawSplitLine(float X, float width, float MinX, float MaxX)
	    {
	        //DrawVerticalLine();
	        float Percentage = X / TargetWindow.position.width;
	
	        //TriggerField
	        Rect DraggableStartField = new Rect(X - 8, 0, 16, TargetWindow.position.height);
	        //GUI.Box(DraggableStartField,"LineTrigger");
	        Event e = Event.current;
	        Rect rect = new Rect(0, 0, TargetWindow.position.width, TargetWindow.position.height);
	        Rect TargetRect = new Rect(rect.x + Percentage * rect.width, rect.y, width, rect.height);
	        EditorGUI.DrawRect(TargetRect, Color.grey);
	        int controlID = GUIUtility.GetControlID(FocusType.Passive);
            EditorGUIUtility.AddCursorRect(DraggableStartField, MouseCursor.SlideArrow, controlID);


            if (e.GetTypeForControl(controlID) == EventType.MouseDown)
	        {
	            if (e.button == 0)
	            {
	                if (DraggableStartField.Contains(e.mousePosition))
	                {
                        GUIUtility.hotControl = controlID;
                        e.Use();
                    }
	            }
	        }
	        if (e.GetTypeForControl(controlID) == EventType.MouseDrag)
	        {
                if (GUIUtility.hotControl == controlID)
                {
                    Percentage = ((e.mousePosition.x - rect.x) / rect.width);
                    Percentage = Mathf.Clamp(Percentage, MinX / rect.width, MaxX / rect.width);
                    TargetRect = new Rect(rect.x + Percentage * rect.width, rect.y, 10, rect.height);
                    EditorGUI.DrawRect(TargetRect, Color.grey);
                    TargetWindow.Repaint();
                }
            }
            
            return Percentage * TargetWindow.position.width;
        }
	
	 
	}
}
