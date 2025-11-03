using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    [System.Serializable]
    public class MonsterSummonData
    {
        public GameObject SummonMonster;
        public GameObject MagicCircle;
        public float CircleScale  = 1f;
        public float MonsterSclae = 1f;
        public float SummonTime   = 3f;
        public float MonsterSize  = 1f;
        [HideInInspector]
        public Vector3 SummonPos;
    }
}