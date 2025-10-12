using System;
using System.Collections;
using UnityEngine;
using DamageArea;

namespace Monster
{
    public class SpiderKingMove : MonoBehaviour
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
            float size = 10f;
            DamageAreaData Data = new DamageAreaData();
            Data.damage = 10f;
            Data.size = size;
            Data.SpawnTime = 0.5f;
            Data.GaugeTime = 3;
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