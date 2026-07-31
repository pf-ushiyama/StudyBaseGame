using R3;

/// <summary>
/// ゲーム全体で管理するデータを扱うクラス
/// </summary>
public class GameModel
{
    // ゲームの状態
    public enum GameState
    {
        PLAYING,
        CLEAR,
        OVER
    }

    // スコア
    public ReadOnlyReactiveProperty<int> Score => _score;
    private readonly ReactiveProperty<int> _score = new ReactiveProperty<int>(0);

    // 撃破数カウント
    public ReadOnlyReactiveProperty<int> KillCount => _killCount;
    private readonly ReactiveProperty<int> _killCount = new ReactiveProperty<int>(0);

    // 現在の状態
    public ReactiveProperty<GameState> CullnetGameState { get; } = new(GameState.PLAYING);
    
    /// <summary>
    /// スコアのリセット
    /// </summary>
    public void Reset()
    {
        CullnetGameState.Value = GameState.PLAYING;
        _score.Value = 0;
        _killCount.Value = 0;
    }

    /// <summary>
    /// スコアを加算する
    /// </summary>
    /// <param name="amount">加算するスコア</param>
    public void AddScore(int amount)
    {
        _score.Value += amount;
    }

    /// <summary>
    /// 撃破数加算
    /// </summary>
    public void AddKillCount()
    {
        _killCount.Value++;
    }
}
