using R3;

/// <summary>
/// プレイヤーのデータを扱うクラス
/// </summary>
public class PlayerModel
{
    // ヒットポイント
    public ReadOnlyReactiveProperty<int> HitPoint => _hitPoint;
    private readonly ReactiveProperty<int> _hitPoint = new ReactiveProperty<int>();

    // 死亡フラグ
    public ReadOnlyReactiveProperty<bool> IsDead => _isDead;
    private readonly ReactiveProperty<bool> _isDead = new ReactiveProperty<bool>();

    // ボムの弾数
    public ReadOnlyReactiveProperty<int> BombCont => _bombCount;
    private readonly ReactiveProperty<int> _bombCount = new ReactiveProperty<int>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="maxHitPoint">最大HP</param>
    /// <param name="initBombCount">ボムの初期弾数</param>
    public PlayerModel(int maxHitPoint, int initBombCount)
    {
        _hitPoint.Value = maxHitPoint;
        _bombCount.Value = initBombCount;
    }

    /// <summary>
    /// ボムを使用する
    /// </summary>
    /// <returns>使用に成功したらtrue</returns>
    public bool TryUseBomb()
    {
        // ボムの残弾があるなら、1つ使用してtrue
        if(_bombCount.Value > 0)
        {
            _bombCount.Value--;
            return true;
        }
        // 無いならfalse
        else
        {
            return false;
        }
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    public void TakeDamege(int damage)
    {
        if(_isDead.Value)
        {
           return; 
        }

        // HPを減少させる
        _hitPoint.Value -= damage;
        // 0になったら死亡
        if(_hitPoint.Value <= 0)
        {
            _hitPoint.Value = 0;
            _isDead.Value = true;
        }
    }
}
