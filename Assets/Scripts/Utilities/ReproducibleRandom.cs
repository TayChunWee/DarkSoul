public struct ReproducibleRandom
{
    private UnityEngine.Random.State _state;

    /// <summary>
    /// 指定されたシード値で初期化するコンストラクタ
    /// </summary>
    /// <param name="seed">シード値</param>
    public ReproducibleRandom(int seed)
    {
        var prevState = UnityEngine.Random.state;

        UnityEngine.Random.InitState(seed);

        _state = UnityEngine.Random.state;
        UnityEngine.Random.state = prevState;
    }

    /// <summary>
    /// 指定されたint型の範囲の乱数取得
    /// </summary>
    /// <param name="minInclusive">下限（この値も範囲に含まれる）</param>
    /// <param name="maxExclusive">上限（この値は範囲に含まれない）</param>
    /// <returns>乱数値</returns>
    public int Range(int minInclusive, int maxExclusive)
    {
        var prevState = UnityEngine.Random.state;
        UnityEngine.Random.state = _state;

        var result = UnityEngine.Random.Range(minInclusive, maxExclusive);

        _state = UnityEngine.Random.state;
        UnityEngine.Random.state = prevState;

        return result;
    }

    /// <summary>
    /// 指定されたfloat型の範囲の乱数取得
    /// </summary>
    /// <param name="minInclusive">下限（この値も範囲に含まれる）</param>
    /// <param name="maxExclusive">上限（この値は範囲に含まれない）</param>
    /// <returns>乱数値</returns>
    public float Range(float minInclusive, float maxExclusive)
    {
        var prevState = UnityEngine.Random.state;
        UnityEngine.Random.state = _state;

        var result = UnityEngine.Random.Range(minInclusive, maxExclusive);

        _state = UnityEngine.Random.state;
        UnityEngine.Random.state = prevState;

        return result;
    }
}