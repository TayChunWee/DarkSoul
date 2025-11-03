using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace Monster
{
    public class MonsterSummoner : MonoBehaviour
    {
        private float _perMagicCircleTime = 0.3f;
        private float _perMonsterSummonTime = 0.7f;
        public GameObject Summon(MonsterSummonData data)
        {
            // 魔法陣の生成
            Vector3 liftUp = new Vector3(0, 0.01f, 0);
            Quaternion rot = Quaternion.Euler(-90f, 0, 0);
            GameObject MagicCircle = Instantiate(data.MagicCircle, data.SummonPos + liftUp, rot);
            MagicCircle.transform.localScale = Vector3.one * data.CircleScale;
            StartCoroutine(DelayCoroutine(data.SummonTime, () =>
            {
                Destroy(MagicCircle);
            }));

            // モンスターを生成
            Vector3 summonPos = data.SummonPos;
            summonPos.y -= data.MonsterSize;
            GameObject Monster = Instantiate(data.SummonMonster, summonPos, Quaternion.identity);
            Monster.transform.localScale = Vector3.one * data.MonsterSclae;

            // 地面からモンスターを出現させる
            Monster.transform.DOMove(data.SummonPos, data.SummonTime * _perMonsterSummonTime)
                .SetDelay(data.SummonTime * _perMagicCircleTime);

            return Monster;
        }
        private IEnumerator DelayCoroutine(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}