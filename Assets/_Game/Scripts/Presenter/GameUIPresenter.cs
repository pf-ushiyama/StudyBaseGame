using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using R3;
using VContainer.Unity;

/// <summary>
/// GameUIViewのストリームを扱う
/// </summary>
public class GameUIPresenter: IStartable
{
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public GameUIPresenter(GameModel gameModel,GameUIView gameUIView,PlayerModel playerModel)
    {
        // スコア表示を初期化
        gameUIView.UpdateScore(gameModel.Score.CurrentValue);

        // リトライボタン押下時ストリーム
        gameUIView.OnRetryClick
            .Subscribe(_ =>
            {
                // 時間を動き出すように戻す（これを忘れると次も止まったまま！）
                Time.timeScale = 1;

                // 現在のシーンを再読み込みしてリセット
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }).AddTo(_disposables);

        // スコア獲得時ストリーム
        gameModel.Score
            .Subscribe(score => 
            {
                // 表示を更新する
                gameUIView.UpdateScore(score);
            }).AddTo(_disposables);

        // HP減少時ストリーム
        playerModel.HitPoint
            .Subscribe(currentHp =>
            {
                gameUIView.UpdateHpBar(currentHp);
            }).AddTo(_disposables);

        // ボム残弾数変更時ストリーム
        playerModel.BombCont
            .Subscribe(bombCount =>
            {
                gameUIView.UpdateBombCount(bombCount);
            }).AddTo(_disposables);

        // ゲームの状態切り替わり時ストリーム
        gameModel.CullnetGameState.
            Subscribe(cullentGameState =>
            {
                switch (cullentGameState)
                {
                    case GameModel.GameState.PLAYING:
                        gameUIView.HideResultPanel();
                        break;
                    case GameModel.GameState.CLEAR:
                        gameUIView.ShowGameClear();
                        break;
                    case GameModel.GameState.OVER:
                        gameUIView.ShowGameOver();
                        break;
                    default:
                        // ここは通るはずがない
                        Debug.LogWarning("存在しないGameModel.GameStateが指定されました");
                        break;
                }
            });
    }

    public void Start()
    {
        UnityEngine.Debug.Log("[GameUIPresenter] 自動起動しました！");
        
        // ※ もしコンストラクタの処理を移すならここに書きます
    }
}
