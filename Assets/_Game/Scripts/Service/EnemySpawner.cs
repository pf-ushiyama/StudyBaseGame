using System;
using UnityEngine;
using UnityEngine.AddressableAssets; // Addressables
using Cysharp.Threading.Tasks;       // UniTask
using R3;                            // R3
using VContainer;
using System.Threading.Tasks;
using System.Collections.Generic;

public class EnemySpawner
{
    // 敵キャラのタイプ
    public enum EnemyType
    {
        ZAKO,
        BOSS
    }

    // ロードするプレハブのファイル名(敵のタイプ毎に管理)
    Dictionary<EnemyType,string> _enemyPrefabNames = new Dictionary<EnemyType, string>();

    // バトル中のコンテキスト
    private BattleContext _battleContext;

    // 音声管理オブジェクト
    private AudioManager _audioManager;

    [Inject]
    public void Construct(BattleContext battleContext,AudioManager audioManager)
    {
        _battleContext = battleContext;
         _audioManager = audioManager;

         _enemyPrefabNames = new Dictionary<EnemyType, string>
        {
            { EnemyType.ZAKO, "EnemyPrefab" },
            { EnemyType.BOSS, "BossEnemyPrefab" }
        };
    }

    /// <summary>
    /// 敵キャラを生成する
    /// </summary>
    /// <param name="enemyType">生成する敵のタイプ</param>
    /// <param name="spawnPosition">出現させる座標</param>
    /// <param name="isDameged">被ダメージ時のコールバック(被ダメージ後の現在HP)</param>
    /// <param name="isDead">死亡時のコールバック(引数は倒すと貰えるスコア)</param>
    /// <returns></returns>
    public async Task<EnemyView> SpawnEnemy(EnemyType enemyType,Vector3 spawnPosition,Action<int> isDameged,Action<int> isDead)
    {
        string loadPrefabName = _enemyPrefabNames[enemyType];

        // 敵を生成
        GameObject enemyObj = await Addressables.InstantiateAsync(loadPrefabName,spawnPosition, Quaternion.identity);
        EnemyView enemyView = enemyObj.GetComponent<EnemyView>();
        if(enemyObj != null)
        {
            // 生成
            enemyView.Initialize(_battleContext.PlayerTransform,_audioManager);

            // 被ダメージストリームを登録
            enemyView.Health.Subscribe(currentHp =>
            {
                isDameged?.Invoke(currentHp);
            }).RegisterTo(enemyView.destroyCancellationToken);

            // 死亡時ストリームを登録
            enemyView.OnDead.Subscribe(_ =>
            {
                isDead?.Invoke(enemyView.Score);
            }).RegisterTo(enemyView.destroyCancellationToken);
            return enemyView;
        }
        else
        {
            return null;
        }

    }
}
