using DamageArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCrirleType : DamageBase
{
    [SerializeField] private float _AreaAngle;
    [SerializeField] private Collider _Collider;

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
            Vector3 posDelta = other.transform.position - transform.position; // äpìxÇ≈îªï Ç∑ÇÈåvéZéÆÇ™É_ÉÅ
            float targetAngle = Vector3.Angle(transform.forward, posDelta);
            if (targetAngle < _AreaAngle)
            {
                Debug.Log("damage " + _data.damage);
            }
        }
    }

    private IEnumerator EnableColliderTemporarily(float activateTime)
    {
        yield return new WaitForSeconds(activateTime);
        _Collider.enabled = true;
    }
}