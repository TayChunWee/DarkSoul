using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatEditor
{
    public partial class CombatEditor
    {
        int ShownAbilityCount;
        int ElementCount;
        int HeightCounter = 1;
        /// <summary>
        /// CurrentSelectedController;
        /// </summary>
        public CombatController SelectedController;
        /// <summary>
        /// Used to load controller after exit playmode.
        /// </summary>
        public string LastSelectedControllerName;
        /// <summary>
        /// the rect to paint character selection and config.
        /// </summary>
        Rect CharacterConfigRect;
        /// <summary>
        /// the rect to paint abilities.
        /// </summary>
        Rect AbilityRect;
        /// <summary>
        /// Used to swap abilities
        /// </summary>
        /// 
        Rect L1DraggingTargetRect;

        static float DragRectHeight = 2;
        bool IsDraggingL1;
        bool TargetChangeThisFrame;
        bool SwapRequired;
        public int CurrentGroupIndex = -1;
        public int CurrentAbilityIndexInGroup = -1;
        public int SwapGroupIndexBefore;
        public int SwapArrayIndexBefore;
        public int SwapGroupIndexAfter;
        public int SwapArrayIndexAfter;
    }
}
