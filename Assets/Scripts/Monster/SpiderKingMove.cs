using System;
using System.Collections;
using UnityEngine;
using DamageArea;

namespace Monster
{
    public class SpiderKingMove : MonsterBase
    {
        private Animator _animator;
        private Rigidbody _rb;

        DamageArea.DASpaner areaSpaner;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            areaSpaner = GetComponent<DASpaner>();
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                BiteAttack();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                RushAttack();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Rotate(170);
            }
        }

        private void BiteAttack()
        {
            // Attack Data & DamageArea spawn
            DamageAreaData Data = new DamageAreaData();
            Data.Damage = 10;
            Data.Size = 10;
            Data.SpawnTime = 0.5f;
            Data.GaugeTime = 3f;
            Data.DeleteTime = 0.3f;
            areaSpaner.Spawn60(Data);

            // animation
            float delayTime = Data.SpawnTime + Data.GaugeTime;
            StartCoroutine(DelayCoroutine(delayTime, () =>
            {
                _animator.SetTrigger("AttackTrigger");
                _animator.SetInteger("AttackType", 0);
            }));
        }

        private void RushAttack()   // ˆÚ“®‚Ì‘¬“x‚ª–]‚Ü‚µ‚­‚È‚¢
        {
            // Attack Data & DamageArea spawn
            int longth = 5;
            float moveSpeed = 21f;
            DamageAreaData Data = new DamageAreaData();
            Data.Damage = 0;
            Data.Size = 7;
            Data.SpawnTime = 0.3f;
            Data.GaugeTime = 1f;
            Data.AttackTime = 1.3f;
            Data.DeleteTime = 0.3f;
            areaSpaner.SpawnRect(longth, Data);

            float delayTime = Data.SpawnTime + Data.GaugeTime;
            float advanceStopTime = 0.4f;
            StartCoroutine(DelayCoroutine(delayTime, () =>
            {
                // Animation
                _animator.SetTrigger("AttackTrigger");
                _animator.SetBool("isRushAttack", true);
                StartCoroutine(DelayCoroutine(Data.AttackTime - advanceStopTime, () =>
                {
                    _animator.SetBool("isRushAttack", false);
                }));

                // Move
                StartCoroutine(RunForSeconds(Data.AttackTime, () =>
                {
                    _rb.velocity = transform.forward * moveSpeed;
                }));
                StartCoroutine(DelayCoroutine(Data.AttackTime, () =>
                {
                    _rb.velocity = Vector3.zero;
                }));
            }));
        }

        private void Rotate(float angle)
        {
            float rotateTime = 1f;
            float advanceStopTime = 0.5f;
            Vector3 rotateSpeed = new Vector3(0, angle / rotateTime, 0);
            _animator.SetBool("isRotate", true);
            StartCoroutine(RunForSeconds(rotateTime, () =>
            {
                transform.Rotate(rotateSpeed * Time.deltaTime);
            }));
            StartCoroutine(DelayCoroutine(rotateTime - advanceStopTime, () =>
            {
                _animator.SetBool("isRotate", false);
            }));

        }

        private IEnumerator RunForSeconds(float activeTime, Action action)
        {
            float timer = 0;

            while (timer < activeTime)
            {
                action?.Invoke();

                timer += Time.deltaTime;
                yield return null; // 1ƒtƒŒ[ƒ€‘Ò‚Â
            }
        }

        private IEnumerator DelayCoroutine(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}