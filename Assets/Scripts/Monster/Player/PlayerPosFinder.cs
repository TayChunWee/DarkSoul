using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosFinder : MonoBehaviour
{
    private List<Transform> _playerList = new List<Transform>();

    private void Start()
    {
        SetPlayer();
    }

    /// <summary>
    /// 全てのプレイヤの座標を取得
    /// </summary>
    /// <returns>List<Vector3></returns>
    public List<Vector3> GetPlayerPosList()
    {
        if(_playerList == null || _playerList.Count == 0) return null;

        List<Vector3> posList = new List<Vector3>();
        foreach (Transform t in _playerList)
        {
            if(t == null) continue;
            posList.Add(t.position);
        }

        return posList;
    }

    /// <summary>
    /// 指定した座標に最も近いプレイヤーを返す
    /// </summary>
    /// <param name="selectPos">指定する座標</param>
    /// <returns>Transform</returns>
    public Transform GetNearestPlayer(Vector3 selectPos)
    {
        if(_playerList == null || _playerList.Count == 0) return null;

        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (Transform player in _playerList)
        {
            if(player == null) continue;

            float distance = Vector3.Distance(selectPos, player.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = player;
            }
        }

        return nearest;
    }

    private void SetPlayer()
    {
        foreach (Transform player in this.transform.GetComponentsInChildren<Transform>())
        {
            if (player.tag == "Player")
            {
                _playerList.Add(player);
            }
        }
    }
}