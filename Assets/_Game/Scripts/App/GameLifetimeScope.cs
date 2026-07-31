using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// インスタンス管理を行うクラス
/// </summary>
public class GameLifetimeScope : LifetimeScope
{
    // 音声管理オブジェクト
    [SerializeField]
    AudioManager _audioManager;

    // ゲーム画面UIオブジェクト
    [SerializeField]
    GameUIView _gameUIView;

    // ゲーム難易度のデータ
    [SerializeField]
    LevelData _levelData;
    // プレイヤーデータ
    [SerializeField]
    PlayerData _playerData;
    
    // プレイヤーキャラの最大体力
    [SerializeField]
    int _playerMaxHp = 30;

    /// <summary>
    /// インスタンスの生成
    /// </summary>
    /// <param name="builder"></param>
    protected override void Configure(IContainerBuilder builder)
    {
        /// インスペクターで指定したインスタンス
        // 音声管理オブジェクト
        builder.RegisterComponent(_audioManager);
        // ゲーム画面UIオブジェクト
        builder.RegisterComponent(_gameUIView);
        // ゲーム難易度のデータ
        builder.RegisterComponent(_levelData);
        // プレイヤーデータ
        builder.RegisterComponent(_playerData);


        /// 0から生成するインスタンス
        // ゲーム全体で扱うデータ
        builder.Register<GameModel>(Lifetime.Singleton);
        // プレイヤーキャラが扱うデータ
        builder.Register<PlayerModel>(Lifetime.Singleton)
            .WithParameter("maxHitPoint", _playerMaxHp) // 引数指定：最大HP
            .WithParameter("initBombCount", 3);         // 引数指定：ボムの初期残弾数
        // ゲーム画面UIのストリームを扱うオブジェクト
        builder.RegisterEntryPoint<GameUIPresenter>(Lifetime.Singleton);
        // プレイヤーを生成するオブジェクト
        builder.Register<PlayerSpawner>(Lifetime.Singleton);
        // 敵キャラを生成するオブジェクト
        builder.Register<EnemySpawner>(Lifetime.Singleton);
        // バトル中に使用する情報
        builder.Register<BattleContext>(Lifetime.Singleton);
        // 弾丸のオブジェクトプール
        builder.Register<BulletPool>(Lifetime.Singleton);

        // ファクトリーメソッド
        builder.Register<System.Func<PlayerView, PlayerPresenter>>(container =>
        {
            // ここで「関数」を作って返す
            return (PlayerView view) =>
            {
                var model = container.Resolve<PlayerModel>();
                var data = container.Resolve<PlayerData>();
                var audio = container.Resolve<AudioManager>();
                var bulletPool = container.Resolve<BulletPool>();
                
                // ここで new する
                return new PlayerPresenter(model, view, data, audio,bulletPool);
            };
        }, Lifetime.Singleton); // ※Lifetimeの設定も忘れずに（通常はSingletonかScoped）
    }
}
