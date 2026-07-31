using System;
using UnityEngine;
using UnityEngine.AddressableAssets; // Addressables
using Cysharp.Threading.Tasks;       // UniTask
using R3;                            // R3
using VContainer;


/// <summary>
/// ゲームを開始するクラス
/// </summary>
public class GameEntry : MonoBehaviour
{
    // ゲーム全体のデータ
    private GameModel _gameModel;
    // ゲームUI
    private GameUIView _gameUIView;
    private LevelData _levelData;
    // プレイヤーのデータ
    private PlayerModel _playerModel;
    // プレイヤーオブジェクトを生成するオブジェクト
    private PlayerSpawner _playerSpawner;
    // 敵キャラオブジェクトを生成するオブジェクト
    private EnemySpawner _enemySpawner;
    // プレイヤーのプレゼンター
    private PlayerPresenter _playerPresenter;

    private BulletPool _bulletPool;
    private BattleContext _context;

    /// <summary>
    /// 初期化
    ///  GameLifetimeScopeから必要な情報を引数で仕入れている
    /// </summary>
    /// <param name="gameModel">ゲーム全体のデータ</param>
    /// <param name="gameUIView">ゲームUI</param>
    /// <param name="levelData"></param>
    /// <param name="playerSpawner">プレイヤーオブジェクトを生成するオブジェクト</param>
    /// <param name="playerModel">プレイヤーのデータ</param>
    /// <param name="enemySpawner">敵キャラオブジェクトを生成するオブジェクト</param>
    [Inject]
    public void Construct(GameModel gameModel,GameUIView gameUIView,LevelData levelData, PlayerSpawner playerSpawner,PlayerModel playerModel,EnemySpawner enemySpawner,BulletPool bulletPool,BattleContext context)
    {
        _gameModel = gameModel;
        _gameUIView = gameUIView;
        _levelData = levelData;
        _playerSpawner = playerSpawner;
        _playerModel = playerModel;
        _enemySpawner = enemySpawner;
        _bulletPool = bulletPool;
        _context = context;
    }
    
    /// <summary>
    /// 開始時処理
    /// </summary>
    async void Start()
    {
        Debug.Log("ゲーム開始");

        // プレイヤーを生成
        _playerPresenter = await _playerSpawner.SpawnPlayer();
        Debug.Log($"② プレイヤー生成完了: PlayerPresenterは？ -> {(_playerPresenter != null ? "あり" : "null")}");

        // ゲーム全体の初期化
        Initialize();
        Debug.Log($"③ 初期化完了: GameModelは？ -> {(_gameModel != null ? "あり" : "null")}");
        
        // 敵キャラの生成を開始する
        SpawnEnemyLoop(this.destroyCancellationToken).Forget();

        // 弾丸たちの初期化
        await _bulletPool.InitializeAsync("BulletPrefab", _context.ProjectileContainer, 20);
    }

    /// <summary>
    /// ゲーム全体の初期化
    /// </summary>
    private void Initialize()
    {
        _gameUIView.SetBossHpBarActive(false,0);

        // 死亡時ストリーム
        _playerModel.IsDead
            .Where(isDead => isDead == true) // falseになった時(よみがえった時)には呼ばない
            .Subscribe(isDead =>
            {
                // ゲームオーバーへ進む
                _gameModel.CullnetGameState.Value = GameModel.GameState.OVER;

                // (おまけ) ゲームの時間を止める
                Time.timeScale = 0;
            }).AddTo(this.destroyCancellationToken);
    }

    /// <summary>
    /// 敵キャラを生成し続ける
    /// </summary>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns></returns>
    private async UniTask SpawnEnemyLoop(System.Threading.CancellationToken ct)
    {
        // ゲームが終了するまでループする
        while (_gameModel.KillCount.CurrentValue < _levelData.EnemiesToKillForBoss)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            // 生成位置をランダムに決定
            Vector3 spawnPos = new Vector3(
                UnityEngine.Random.Range(-_levelData.SpawnAreaSize.x,_levelData.SpawnAreaSize.x),
                0.5f,
                UnityEngine.Random.Range(-_levelData.SpawnAreaSize.y,_levelData.SpawnAreaSize.y));

            // 敵を生成
            await _enemySpawner.SpawnEnemy(EnemySpawner.EnemyType.ZAKO,spawnPos,null,
                // 死亡時ストリーム
                (getScore) =>
                {
                    // スコアを獲得する
                    _gameModel.AddScore(getScore);
                    _gameModel.AddKillCount();
                });

            // ２秒待つ
            await UniTask.Delay(TimeSpan.FromSeconds(_levelData.SpawnInterval),cancellationToken:ct);
        }

        // ------------------------------------------------
        // 敵を10体倒し終わっていると、この先の処理に進む
        // ------------------------------------------------

        Debug.Log("⚠️ 警告: ボスが出現します！");

        // ちょっと間をおく
        await UniTask.Delay(System.TimeSpan.FromSeconds(2.0f), cancellationToken: ct);

        // ボス生成！
        EnemyView bossView =  await _enemySpawner.SpawnEnemy(EnemySpawner.EnemyType.BOSS,_levelData.BossSpawnPosition,
            // 被ダメージ時ストリーム
            (currentHp) =>
            {
                // ここでUIのバーの長さを変えるメソッドを呼ぶ
                _gameUIView.UpdateBossHpBar(currentHp);
            },
            // 死亡時ストリーム
            (getScore) => {
                Debug.Log("🎉 GAME CLEAR!!");
                
                // スコア入手
                _gameModel.AddScore(getScore);

                // クリア画面表示
                _gameModel.CullnetGameState.Value = GameModel.GameState.CLEAR;

                // ゲーム停止
                Time.timeScale = 0;
                
                // プレイヤー操作停止
                _playerPresenter.Dispose();
            });

        // ボス用のUI表示
        if(bossView != null)
        {
            _gameUIView.SetBossHpBarActive(true,bossView.Health.Value);
            _gameUIView.UpdateBossHpBar(bossView.Health.Value);
        }
    }
}
