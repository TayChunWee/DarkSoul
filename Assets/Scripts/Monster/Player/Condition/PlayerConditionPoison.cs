using System;
using System.Collections;
using UnityEngine;

public class PlayerConditionPoison : PlayerConditionBase
{
    [SerializeField] private int _poisonHitCount = 5;
    [SerializeField, Range(0, 1)] private float _rateTotalPoisonDamage = 0.4f;

    public override void SetUp(TestPlayerController pcon, float effectiveTime)
    {
        base.SetUp(pcon, effectiveTime);
    }

    public override void ProcStartCondition()
    {
        // 毒エフェクトを表示（未実装）

        int playerMaxHp = _pController.GetMaxHp();
        int poisonDamage = (int)(playerMaxHp * _rateTotalPoisonDamage / _poisonHitCount);
        StartCoroutine(RepeatWithDelayTimerType(_procTime, _poisonHitCount, () =>
        {
            _pController.TakeDamage(poisonDamage);
        }));
    }

    private IEnumerator RepeatWithDelayTimerType(float totalTime, int repeatCount, Action action)
    {
        float timer = 0;

        while (timer < totalTime)
        {
            action?.Invoke();

            timer += Time.deltaTime;
            float cycleTime = totalTime / repeatCount;
            yield return new WaitForSeconds(cycleTime);
        }
    }
}
