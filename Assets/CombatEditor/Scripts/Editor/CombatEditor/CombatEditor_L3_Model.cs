using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CombatEditor
{
    public partial class CombatEditor : EditorWindow
    {
        Rect L3SurfaceRect;
        Rect L3ViewRect;
        float TimePointWidth = 10;

        float TimeLineScaler = 1;
        Rect AnimTrackRect;
        Vector2 Scroll_Ruler;
        float MaxWidth;
    }
}
