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
        DamageArea.DamageAreaSpaner areaSpaner;

        [SerializeField] private int _randomSeed;
        private ReproducibleRandom _random;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            areaSpaner = GetComponent<DamageAreaSpaner>();

            _random = new ReproducibleRandom(_randomSeed);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                BiteAttack();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                RushAttack();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                // 乱数1の次の値取得
                var rand1Value = _random.Range(0, 1000);

                Debug.Log($"乱数1の値 : {rand1Value}");
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Rotate(10);
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

        private void RushAttack()
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

        private void CreatePoisonArea()
        {
            Vector2 myPos = this.transform.position;

        }

        private void SpawnChild()
        {

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
                yield return null; // 1フレーム待つ
            }
        }

        private IEnumerator DelayCoroutine(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}