using DamageArea;
using System;
using System.Collections;
using UnityEngine;

namespace Monster
{
    public class SpiderChild : MonoBehaviour
    {
        [Header("ステータス")]
        [SerializeField] private float moveSpeed = 3f;        // 移動速度
        [SerializeField] private float attackRange = 2f;      // 攻撃範囲（水平距離）
        [SerializeField] private float attackCooldown = 5f;   // 攻撃間隔
        [SerializeField] private float attackAngle = 60f; // 前方攻撃角度

        [Header("攻撃")]
        [SerializeField] private DamageAreaData biteData;

        private Transform _player;          // プレイヤーのTransform
        private float _lastAttackTime;      // 最後に攻撃した時間
        private Animator _animator;         // アニメーション制御用
        private DamageAreaSpaner _areaSpaner;
        private bool _isAttacking = false;  // 攻撃中フラグ

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _areaSpaner = GetComponent<DamageAreaSpaner>();
        }

        private void Update()
        {
            if (_player == null) return;

            // 攻撃中は一切移動・攻撃判定をしない
            if (_isAttacking) return;

            // プレイヤーとの水平距離
            Vector3 playerFlat = new Vector3(_player.position.x, transform.position.y, _player.position.z);
            float distance = Vector3.Distance(transform.position, playerFlat);

            if (distance > attackRange)
            {
                MoveTowardPlayer();
            }
            else
            {
                TryAttack(playerFlat);
            }
        }

        public void Init()
        {
            string finderName = "PlayerManager";
            GameObject finder = GameObject.Find(finderName);

            PlayerPosFinder posFinder = finder.GetComponent<PlayerPosFinder>();
            _player = posFinder.GetNearestPlayer(transform.position);
        }

        private void MoveTowardPlayer()
        {
            // Y軸を無視した移動
            Vector3 targetPos = new Vector3(_player.position.x, transform.position.y, _player.position.z);
            Vector3 dir = (targetPos - transform.position).normalized;

            transform.position += dir * moveSpeed * Time.deltaTime;

            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

            if (_animator)
                _animator.SetBool("isMoving", true);
        }

        private void TryAttack(Vector3 playerFlat)
        {
            if (_animator)
                _animator.SetBool("isMoving", false);

            // 前方角度チェック
            Vector3 toPlayer = (playerFlat - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, toPlayer);

            if (angleToPlayer <= attackAngle / 2f)
            {
                if (Time.time - _lastAttackTime >= attackCooldown)
                {
                    _lastAttackTime = Time.time;
                    StartCoroutine(AttackRoutine(toPlayer));
                }
            }
        }

        private IEnumerator AttackRoutine(Vector3 toPlayer)
        {
            _isAttacking = true; // 攻撃開始 → 移動停止
            if (toPlayer != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(toPlayer);

            BiteAttack();

            // 攻撃アニメーション全体の待ち時間
            float totalAttackTime = 4.5f; // モーション長に合わせて調整
            yield return new WaitForSeconds(totalAttackTime);

            _isAttacking = false; // 攻撃終了 → 再び移動可能
        }

        private void BiteAttack()
        {
            
            _areaSpaner.Spawn60(biteData);

            float delayTime = biteData.SpawnTime + biteData.GaugeTime;
            StartCoroutine(DelayCoroutine(delayTime, () =>
            {
                _animator.SetTrigger("BiteAttack");
            }));
        }

        private IEnumerator DelayCoroutine(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}