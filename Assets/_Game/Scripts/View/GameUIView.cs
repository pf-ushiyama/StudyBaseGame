using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UIを扱うために必要

/// <summary>
/// ゲーム中のUIを扱うビュー
/// </summary>
public class GameUIView : MonoBehaviour
{
    /// プレイ中
    // プレイヤーのHPスライダー
    [SerializeField]
    Slider _playerHpBar;

    // ボムの残弾数
    [SerializeField]
    TMP_Text _bombCountText;

    // ボスのHPスライダー
    [SerializeField]
    Slider _bossHpBar;

    // ボスの最大HP
    // 表示の為に記録しておく
    private int _bossMaxHP;

    // スコア表示テキスト
    [SerializeField]
    TMP_Text _scoreText; 

    /// クリア画面
    // ゲームクリア時の表示パネル
    [SerializeField]
    GameObject _gameClearPanel;

    // ゲームクリアパネル上のリトライボタン
    [SerializeField]
    Button _retryButtonClear;

    /// ゲームオーバー画面
    // ゲームオーバー時の表示パネル
    [SerializeField]
    GameObject _gameOverPanel;

    // ゲームオーバーパネル上のリトライボタン
    [SerializeField]
    Button _retryButtonOver;


    // リトライボタンクリック時ストリーム
    public Observable<Unit> OnRetryClick => _onRetryClick;
    private Subject<Unit> _onRetryClick = new Subject<Unit>();
    
    void Start()
    {
        // ボタンが押されたら、Subjectを通して通知を送る
        _retryButtonClear.onClick.AddListener(() => 
        {
            _onRetryClick.OnNext(Unit.Default);
        });

        _retryButtonOver.onClick.AddListener(() => 
        {
            _onRetryClick.OnNext(Unit.Default);
        });
    }

    /// <summary>
    /// HPの表示を更新する
    /// </summary>
    /// <param name="currentHp">現在HP</param>
    public void UpdateHpBar(int currentHp)
    {
        _playerHpBar.value = currentHp;
    }

    /// <summary>
    /// ボムの残弾数表示を更新する
    /// </summary>
    /// <param name="bombCount"></param>
    public void UpdateBombCount(int bombCount)
    {
        _bombCountText.text = "Bomb:";

        // 残弾数を"●"の数で表示する
        for (int i = 0; i < bombCount; i++)
        {
            _bombCountText.text += "●";
        }
    }

    /// <summary>
    /// ボスのHPバーの表示非表示を切り替える
    /// </summary>
    /// <param name="isActive"></param>
    public void SetBossHpBarActive(bool isActive, int maxHp)
    {
        _bossMaxHP = maxHp;
        _bossHpBar.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// ボスのHPの表示を更新する
    /// </summary>
    public void UpdateBossHpBar(int currentHp)
    {
        if(_bossMaxHP <= 0)
        {
            Debug.LogWarning("GameUIView.UpdateBossHpBar ボスの最大HPを設定していません");
            return;
        }

        // 0.0 ~ 1.0 の割合に変換してセット
        _bossHpBar.value = (float)currentHp / _bossMaxHP;
    }

    /// <summary>
    /// スコア表示を更新する
    /// </summary>
    /// <param name="newScore"></param>
    public void UpdateScore(int newScore)
    {
        _scoreText.text = $"Score: {newScore}";
    }

    /// <summary>
    /// ゲームクリアパネルを表示
    /// </summary>
    public void ShowGameClear()
    {
        _gameClearPanel.SetActive(true);
        SetBossHpBarActive(false,0);
    }

    /// <summary>
    /// ゲームオーバーパネルを表示
    /// </summary>
    public void ShowGameOver()
    {
        _gameOverPanel.SetActive(true);
    }

    public void HideResultPanel()
    {
        _gameClearPanel.SetActive(false);
        _gameOverPanel.SetActive(false);
    }
}