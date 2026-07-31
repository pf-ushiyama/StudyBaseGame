using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Threading;


/// <summary>
/// PlayerViewとPlayerModelを引き合わせるクラス
/// </summary>
public class PlayerPresenter
{
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    // 音声管理オブジェクト
    private AudioManager _audioManager;

    // 弾丸たち
    private readonly BulletPool _bulletPool;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlayerPresenter(PlayerModel model, PlayerView view,PlayerData playerData, AudioManager audioManager,BulletPool bulletPool)
    {
        _audioManager = audioManager;
        _bulletPool = bulletPool;
        
        // 移動入力時ストリーム
        view.OnMoveInput.Subscribe(input =>
            {
                view.Move(input);
            }).AddTo(view.destroyCancellationToken);

        // 射撃ストリーム
        Observable.Interval(System.TimeSpan.FromSeconds(0.2f))
            .Where(_ => !model.IsDead.CurrentValue)
            // 👇 ここで魔法をかけます。「この後の処理はメインスレッドでやってね」という命令です
            .ObserveOn(SynchronizationContext.Current)
            .Subscribe(_ =>
            {
                // 弾丸を生成する
                var bullet = _bulletPool.Spawn(view.transform.position, view.transform.rotation,"Enemy");

                if(bullet != null)
                {
                    _audioManager.PlaySE("Explosion13_8bitSFX_AudioJackGames");
                }
            }).AddTo(view.destroyCancellationToken);

        // ボム使用入力時ストリーム
        view.OnUseBombInput
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.2f)) // 連打を無効化
            .Subscribe(async _ =>
            {
                if(model.TryUseBomb())
                {
                    Debug.Log("💣 ドカーン！ボム発動！");
                    // ここで _audioManager.PlaySE("Bomb"); とかも呼べますね

                    // 2. ボムのエフェクトを生成
                    // (頻繁に出ないのでInstantiateでOK。こだわるならPoolでも可)
                    var obj = await Addressables.InstantiateAsync(
                        "BombEffectPrefab", 
                        view.transform.position, 
                        Quaternion.identity
                    );

                    var bombView = obj.GetComponent<BombView>();
                    bombView.Initialize(
                        playerData.BombDamage, 
                        playerData.BombRadius, 
                        playerData.BombDuration
                    );
                }
                else
                {
                    Debug.Log("カチッ...（弾切れ）");
                }
            }).AddTo(view.destroyCancellationToken);

        // 被ダメージ時ストリーム
        view.OnDameged
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.5f)) // 0.5秒間無敵を入れる
            .Subscribe(_=>{
                // ダメージを受ける
                model.TakeDamege(5);
            }).AddTo(view.destroyCancellationToken);
    }

    /// <summary>
    /// クラスが破棄される時に呼ばれる（後片付け）
    /// </summary>
    public void Dispose()
    {
        _disposables.Dispose(); // 登録したSubscribeを全部止める
    }
}
