using UnityEngine;
using R3;
using System;

/// <summary>
/// 弾丸の表示を行う
/// </summary>
public class BulletView : MonoBehaviour
{
    // 弾速
    [SerializeField]
    float _speed = 50.0f;

    // 消滅するまでの時間(秒)
    [SerializeField]
    float _lifeTimeSeconds = 3.0f;

    // 当てたい相手のタグ
    //  このタグ以外のオブジェクトには当たらない
    private string _targetTag;

    // 自分をプールに返すためのアクション
    private Action<BulletView> _returnAction;
    
    // タイマーの管理用
    private IDisposable _timerDisposable;

    // 弾が何かに当たったり、寿命が来た時に呼ぶ処理
    public void Deactivate()
    {
        // 動作中のタイマーがあれば止める
        // (これをしないと、プールに戻った後にタイマーが発動してエラーになります！)
        _timerDisposable?.Dispose();
        _timerDisposable = null;

        _returnAction = null;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// プールから取り出した時に「新品」の状態に戻すメソッド
    /// </summary>
    /// <param name="position">出現位置</param>
    /// <param name="rotation">出現角度</param>
    /// <param name="targetTag">当てたいオブジェクトのタグ</param>
    /// <param name="returnAction">消滅時コールバック</param>
    public void Activate(Vector3 position, Quaternion rotation, string targetTag, Action<BulletView> returnAction)
    {
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);

        _targetTag = targetTag;

        // ※もしRigidbodyを使っているなら、速度のリセットなどもここで行う
        // var rb = GetComponent<Rigidbody>();
        // if (rb != null) rb.velocity = Vector3.zero;

        _returnAction = returnAction;

        // 前回のタイマーが残っていたら消す（念のため）
        _timerDisposable?.Dispose();

        // ✨ 新しいタイマー：指定時間待ったら、自分を返却する
        _timerDisposable = Observable.Timer(TimeSpan.FromSeconds(_lifeTimeSeconds))
            .Subscribe(_ => 
            {
                _returnAction?.Invoke(this);
            });

        // 寿命をViewに紐づける（シーン遷移などでDestroyされた時の安全策）
        _timerDisposable.RegisterTo(this.destroyCancellationToken);
    }

    /// <summary>
    /// 毎フレーム処理
    /// </summary>
    void Update()
    {
        // 前方へまっすぐ飛ぶ
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    /// <summary>
    /// 接触時処理
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 敵に当たったら
        if(other.CompareTag(_targetTag))
        {
            // 敵コンポーネントを取得し、敵を倒す
            IDamageable damegeable = other.GetComponent<IDamageable>();
            if (damegeable != null)
            {
                damegeable.TakeDamage(1);
            }

            // 自身も削除する
            Deactivate();
        }
    }
}
