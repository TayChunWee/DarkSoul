using Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSummoner : MonoBehaviour
{
    public void Summon(MonsterSummonData data)
    {
        // ñÇñ@êwÇÃê∂ê¨
        GameObject MagicCircle = Instantiate(data.MagicCircle, data.SummonPos, Quaternion.identity);
        MagicCircle.transform.localScale = Vector3.one * data.CircleScale;

        Vector3 summonPos = data.SummonPos;
        summonPos.y -= data.MonsterSize;
        GameObject Monster = Instantiate(data.SummonMonster, summonPos, Quaternion.identity);
    }
}
