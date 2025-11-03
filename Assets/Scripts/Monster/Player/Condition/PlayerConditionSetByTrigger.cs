using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConditionSetByTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _pConditionPoison;
    [SerializeField] private float _effectiveTime;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject prefab = Instantiate(_pConditionPoison, other.transform);

            TestPlayerController pController = other.GetComponent<TestPlayerController>();
            PlayerConditionBase pCondition = prefab.GetComponent<PlayerConditionBase>();
            pCondition.SetUp(pController, _effectiveTime);
        }
    }
}
