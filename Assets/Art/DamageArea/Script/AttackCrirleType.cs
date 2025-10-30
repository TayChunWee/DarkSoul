using DamageArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCrirleType : AttackBase
{
    [SerializeField] private float _attackAreaAngle;

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
            Debug.Log("Hit!");
            Vector3 posDelta = other.transform.position - transform.position;
            float targetAngle = Vector3.Angle(transform.forward, posDelta);
            Debug.Log("targetAngle " + targetAngle);
            if (targetAngle < _attackAreaAngle / 2)
            {
                Debug.Log("Call TakeDamage");
                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable != null) damageable.TakeDamage(_data.Damage);
            }
        }
    }

    private IEnumerator EnableColliderTemporarily(float activateTime)
    {
        yield return new WaitForSeconds(activateTime);
        _Collider.enabled = true;
    }
}