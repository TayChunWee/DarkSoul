using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatEditor
{
    public partial class CombatEditor
    {
        static int DragIndicatorHeight = 2;

        //PreviewPlayDatas
        public bool IsPlaying;
        public bool IsLooping;
        float StartTime;
        float CurrentPlayTime;
        float LastTickTime;
        public PreviewObject_AnimSpeed[] SpeedModifiers;
        public float PlayTimeMultiplier = 1;
        public float LoopWaitTime = 0;

        Rect TrackRect;

        //DragDatas
        bool DragHandleRequired;
        int CurrentDraggingID;
        int DraggingFieldStartIndex;
        int L2DragEndIndex;
        Vector2 DragStartPosition;
        Rect DragEndRect;

        public Rect L2DragRect;
        int LastIndex;


    }
}
