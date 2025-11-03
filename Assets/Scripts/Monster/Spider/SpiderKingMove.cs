using System;
using System.Collections;
using UnityEngine;
using DamageArea;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;

namespace Monster
{
    public class SpiderKingMove : MonsterBase
    {
        private Rigidbody _rb;
        private Animator _animator;
        DamageArea.DamageAreaSpaner _areaSpaner;
        MonsterSummoner _monsterSummoner;

        private int _randomSeed = 3627;
        private ReproducibleRandom _random;

        [Header("AttackPrefab")]
        [SerializeField] private GameObject _poisonArea;

        [Header("SummonData")]
        [SerializeField] private MonsterSummonData _spider;
        [SerializeField] private MonsterSummonData _toxinSpider;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _areaSpaner = GetComponent<DamageAreaSpaner>();
            _monsterSummoner = GetComponent<MonsterSummoner>();

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
                CreatePoisonArea();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SpawnChild();
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
            _areaSpaner.Spawn120(Data);

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
            _areaSpaner.SpawnRect(longth, Data);

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
            // 値を設定
            DamageAreaData Data = new DamageAreaData();
            Data.Damage = 0;
            Data.Size = 10;
            Data.SpawnTime = 0.3f;
            Data.GaugeTime = 1.5f;
            Data.DeleteTime = 0.3f;

            // Player位置取得
            GameObject pManager = GameObject.Find("PlayerManager");
            PlayerPosFinder pPosFinder = pManager.GetComponent<PlayerPosFinder>();
            List<Vector3> posList = pPosFinder.GetPlayerPosList();

            // Delay時間の設定
            int pCount = posList.Count;
            float delayTime = 2 * ( Data.SpawnTime + Data.GaugeTime + Data.DeleteTime);

            int counter = 0;
            StartCoroutine(RepeatWithDelayCountType(delayTime, pCount, () =>
            {
                Vector3 spawnPos = posList[counter];
                spawnPos.y = 0.01f;

                _areaSpaner.Spawn360(spawnPos, Data);
                float delayTime1 = Data.SpawnTime + Data.GaugeTime;
                StartCoroutine(DelayCoroutine(delayTime1, () =>
                {
                    _animator.SetTrigger("AttackTrigger");
                    _animator.SetInteger("AttackType", 2);

                }));
                float delayTime2 = Data.SpawnTime + Data.GaugeTime + Data.DeleteTime;
                StartCoroutine(DelayCoroutine(delayTime2, () =>
                {
                    Quaternion poisonAreaRot = Quaternion.Euler(-90, 0, 0);
                    Instantiate(_poisonArea, spawnPos, poisonAreaRot);
                }));

                ++counter;
            }));
        }

        private void SpawnChild()
        {
            _animator.SetTrigger("AttackTrigger");
            _animator.SetInteger("AttackType", 1);

            // 子モンスターの生成
            MonsterSummoner Summoner = GetComponent<MonsterSummoner>();
            _spider.SummonPos = transform.position + transform.forward * 5f;
            GameObject child = Summoner.Summon(_spider);

            SpiderChild spiderChild = child.GetComponent<SpiderChild>();
            StartCoroutine(DelayCoroutine(_spider.SummonTime, () =>
            {
                spiderChild.Init();
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
                yield return null; // 1フレーム待つ
            }
        }

        private IEnumerator DelayCoroutine(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }

        private IEnumerator RepeatWithDelayCountType(float delayTime, int repeatCount, Action action)
        {
            int count = 0;
            while (count < repeatCount)
            {
                action?.Invoke();
                yield return new WaitForSeconds(delayTime);
                ++count;
            }
        }
    }
}