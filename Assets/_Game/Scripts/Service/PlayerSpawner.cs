using System;
using UnityEngine;
using UnityEngine.AddressableAssets; // Addressables
using Cysharp.Threading.Tasks;       // UniTask
using VContainer;

/// <summary>
/// プレイヤーオブジェクトを生成する
/// </summary>
public class PlayerSpawner
{
    // ファクトリーメソッド
    private Func<PlayerView, PlayerPresenter> _playerPresenterFactory;

    // バトル中のコンテキスト
    private BattleContext _battleContext;

    [Inject]
    public void Construct(Func<PlayerView, PlayerPresenter> factory, BattleContext battleContext)
    {
        _playerPresenterFactory = factory;
        _battleContext = battleContext;
    }

    /// <summary>
    /// プレイヤーを生成する
    /// </summary>
    public async UniTask<PlayerPresenter> SpawnPlayer()
    {
        // プレイヤーを生成(Addressabelsの機能を使う)
        Debug.Log("プレイヤーをロードします");
        GameObject playerObj = await Addressables.InstantiateAsync("PlayerPrefab");
        Transform playerTransform = playerObj.transform;
        _battleContext.PlayerTransform = playerTransform;
        Debug.Log("プレイヤーのロード完了");

        // プレイヤーのビューを取得
        var playerView = playerObj.GetComponent<PlayerView>();
        if(playerView == null)
        {
            Debug.LogWarning("ロードしたプレイヤーからPlayerViewコンポーネントを見つけられませんでした");
            return null;
        }

        // プレイヤーのプレゼンターを生成
        return _playerPresenterFactory.Invoke(playerView);
    }
}
