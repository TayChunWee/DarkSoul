using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] protected int _maxHP;
    protected int _currentHP;
}