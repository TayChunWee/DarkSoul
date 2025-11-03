using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private float _stageSize;

    public float GetStageSize()
    {
        return _stageSize;
    }
}