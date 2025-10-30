using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageArea
{
    [System.Serializable]
    public class DamageAreaData
    {
        public int Damage;
        public float Size;
        public float SpawnTime;
        public float GaugeTime;
        public float AttackTime;
        public float DeleteTime;
    }

    [System.Serializable]
    public enum GaugeType
    { 
        Normal,
        Width
    }
}