using DamageArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCrirleType : DamageBase
{
    [SerializeField] private float _AreaAngle;

    private void Start()
    {
        _Collider.enabled = false;
        if (_data == null)
        {
            Debug.LogWarning("_data is not find !");
            return;
        }
        float activateTime = _data.SpawnTime + _data.GaugeTime;
        StartCoroutine(EnableColliderTemporarily(activateTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 posDelta = other.transform.position - transform.position;
            float targetAngle = Vector3.Angle(transform.forward, posDelta);
            if (targetAngle < _AreaAngle / 2)
            {
                Debug.Log("damage " + _data.Damage);
            }
        }
    }

    private IEnumerator EnableColliderTemporarily(float activateTime)
    {
        yield return new WaitForSeconds(activateTime);
        _Collider.enabled = true;
    }
}