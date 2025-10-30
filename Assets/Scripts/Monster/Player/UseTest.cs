using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseTest : MonoBehaviour
{
    [SerializeField] private PlayerPosFinder posFinder;

    private void Start()
    {
        Invoke(nameof(Test), 0.1f);
    }

    private void Test()
    {

        Transform t = posFinder.GetNearestPlayer(this.transform.position);
        Debug.Log(t.position);
    }
}
