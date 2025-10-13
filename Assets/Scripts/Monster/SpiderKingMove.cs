using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageArea;
using Unity.VisualScripting;

namespace Monster
{
    public class SpiderKingMove : MonsterBase
    {
        [SerializeField] private Animator _animator;

        DamageArea.Spaner areaSpaner;

        private void Start()
        {
            areaSpaner = GetComponent<Spaner>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                BiteAttack();
            }
        }

        private void BiteAttack()
        {
            DamageAreaData Data = new DamageAreaData();
            Data.Damage = 0;
            Data.Size = 10;
            Data.SpawnTime = 0.5f;
            Data.GaugeTime = 3f;
            Data.DeleteTime = 0.3f;
            areaSpaner.Spawn60(transform, Data);

            float delayTime = Data.SpawnTime + Data.GaugeTime;
            StartCoroutine(DelayCoroutine(delayTime, () =>
            {
                _animator.SetTrigger("AttackTrigger");
                _animator.SetInteger("AttackType", 0);
            }));
        }

        private IEnumerator DelayCoroutine(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}