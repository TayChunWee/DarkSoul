using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{	
	public partial class CombatEditor : EditorWindow
	{
	    public class VerticalSplitLine
	    {
	        public float XPos;
	        public TimeLineHelper helper;
	        public VerticalSplitLine(float pos, EditorWindow window)
	        {
	            XPos = pos;
	            helper = new TimeLineHelper(window);
	        }
	    }
	    VerticalSplitLine SplitLine_Ability_Field;
	    VerticalSplitLine SplitLine_Field_Track;
	    VerticalSplitLine SplitLine_Track_Inspector;
	    public void InitSplitLine()
	    {
	        SplitLine_Ability_Field = new VerticalSplitLine(Width_Ability, this);
	        SplitLine_Field_Track = new VerticalSplitLine(Width_Ability + Width_TrackLabel, this);
	        SplitLine_Track_Inspector = new VerticalSplitLine(position.width - Width_Inspector, this);
	    }
	
	    //SplitLines Need Min dis ,or it will stick together
	    float MinDistanceBetweenSplitLine;
	    //������ק�ķֽ���
	    public void PaintSplitLine()
	    {
	        if (SplitLine_Ability_Field == null)
	        {
	            SplitLine_Ability_Field = new VerticalSplitLine(Width_Ability, this);
	        }
	        if (SplitLine_Field_Track == null)
	        {
	            SplitLine_Field_Track = new VerticalSplitLine(Width_Ability + Width_TrackLabel, this);
	        }
	        if (SplitLine_Track_Inspector == null)
	        {
	            SplitLine_Track_Inspector = new VerticalSplitLine(position.width - Width_Inspector, this);
	        }
	
	        MinDistanceBetweenSplitLine = 140;
	        // Draw and Limit
	        SplitLine_Ability_Field.XPos = SplitLine_Ability_Field.helper.DrawSplitLine(SplitLine_Ability_Field.XPos, 2, 
	            MinDistanceBetweenSplitLine, SplitLine_Field_Track.XPos - MinDistanceBetweenSplitLine);
	
	        Width_Ability = SplitLine_Ability_Field.XPos;
	        //Draw and Limit
	        SplitLine_Field_Track.XPos = SplitLine_Field_Track.helper.DrawSplitLine(SplitLine_Field_Track.XPos, 2, 
	            SplitLine_Ability_Field.XPos + MinDistanceBetweenSplitLine, SplitLine_Track_Inspector.XPos);
	
	        Width_TrackLabel = SplitLine_Field_Track.XPos - Width_Ability;
	
	        //SplitLine_Track_Inspector.XPos = SplitLine_Track_Inspector.helper.DrawSplitLine(SplitLine_Track_Inspector.XPos, 2, SplitLine_Field_Track.XPos, position.width);
	        Width_Inspector = 0;
	        InitRect();
	    }
	
	
	
	
	}
}
