using UnityEngine;

public class PlayerConditionBase : MonoBehaviour
{
    protected bool _isSetUp = false;
    protected TestPlayerController _pController;
    protected float _procTime;
    float _timer;

    /// <summary>
    /// SetUp呼び出しで処理開始
    /// </summary>
    /// <param name="pcon"></param>
    /// <param name="effectiveTime"></param>
    public virtual void SetUp(TestPlayerController pcon, float effectiveTime)
    {
        _pController = pcon;
        _procTime = effectiveTime;
        _isSetUp = true;
        ProcStartCondition();
    }

    void Update()
    {
        if (!_isSetUp) return;
        _timer += Time.deltaTime;
        if (_timer > _procTime)
        {
            ProcAfterEndCondition();
            Destroy(this);
        }
    }

    /// <summary>
    /// 状態異常が開始する時に行う処理
    /// </summary>
    public virtual void ProcStartCondition() { }

    /// <summary>
    /// 場外異常が終了する時に行う処理
    /// </summary>
    public virtual void ProcAfterEndCondition() { }
}
