using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R3;
using Cysharp.Threading.Tasks; // UniTask
using UnityEngine.AddressableAssets; // Addressables

/// <summary>
/// 敵キャラの表示を行う
/// </summary>
public class EnemyView : MonoBehaviour , IDamageable
{
    // 移動速度
    // [SerializeField]
    // float _moveSpeed = 3.0f;

    // データファイル
    [SerializeField]
    EnemyParam _param;

    // 二重死亡防止フラグ
    private bool _isAlreadyDead = false;

    // HP
    public readonly ReactiveProperty<int> Health = new ReactiveProperty<int>(1);
    private int _maxHp = 1;

    // 倒した時に得られるスコア
    private int _score;
    public int Score
    {
        get
        {
            return _score;
        }
    }

    // 追いかける対象
    private Transform _target;

    // 音声管理オブジェクト
    private AudioManager _audioManager;

    // 死亡時ストリーム
    public Observable<Unit> OnDead => _onDead;
    private readonly Subject<Unit> _onDead = new Subject<Unit>();

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(Transform targetTransform, AudioManager audioManager)
    {
        _target = targetTransform;
        _audioManager = audioManager;

        if (_param != null)
        {
            _maxHp = _param.MaxHp;
            Health.Value = _param.MaxHp;

            _score = _param.ScoreValue;
            
            // ついでに見た目もデータに合わせて変えてみる（任意）
            transform.localScale = Vector3.one * _param.Scale;
            
            // 色を変える（Rendererがあれば）
            var renderers = GetComponentsInChildren<Renderer>();
            foreach(var r in renderers) r.material.color = _param.BodyColor;
        }
        else
        {
            // HP設定
            _maxHp = 1;
            Health.Value = 1;          
        }

    }

    /// <summary>
    /// 毎フレーム処理
    /// </summary>
    void Update()
    {
        Move();
    }

    /// <summary>
    /// 移動する
    /// </summary>
    private void Move()
    {
        // ターゲットがいなければ移動しない
        if(_target == null)
        {
            return;
        }

        // ターゲットの方向を向き
        transform.LookAt(_target);

        // そちらへ進む
        transform.position += transform.forward * _param.MoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (_isAlreadyDead) return;

        Health.Value -= damage;

        // HPが0以下になったら死亡
        if (Health.Value <= 0)
        {
            Die();

            HitStop.Freeze(0.1f);
            CameraShaker.Instance?.Shake(0.1f,0.5f);
        }
        else
        {
            // 生きていれば、被弾演出（ヒットストップなど）だけ入れるのもアリ
            // 今回は前回の爆発エフェクトが死亡時用なので、ここには特に書きません
        }
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    public void Die()
    {
        if(_isAlreadyDead)
        {
            return;
        }
        _isAlreadyDead = true;

        // 死亡イベントを発行する
        _onDead.OnNext(Unit.Default);

        SpawnExplosion().Forget();
        _audioManager.PlaySE("Explosion1_8bitSFX_AudioJackGames");
        
        // 自分自身を削除
        Destroy(this.gameObject);
    }

    private async UniTaskVoid SpawnExplosion()
    {
        // 自分のいた場所に爆発を出す
        await Addressables.InstantiateAsync("ExplosionPrefab", this.transform.position, Quaternion.identity);
    }
}
