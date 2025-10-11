using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS
{
    public class Item : ScriptableObject
    {
        [Header("Item Information")]
        public Sprite itemIcon;
        public string itemName;
        public string itemDescription;
        public int itemID;
        // Additional item properties can be added here
    }
}