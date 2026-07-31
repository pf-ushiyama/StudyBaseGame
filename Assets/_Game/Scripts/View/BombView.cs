using System;
using UnityEngine;
using R3;
using VContainer;

/// <summary>
/// ボムとその衝撃波の表示を扱う
/// </summary>
public class BombView : MonoBehaviour
{
    // 与ダメージ
    private int _damage;
    // 爆風(衝撃波)半径
    private float _maxRadius;
    // 衝撃波が広がるのにかかる時間
    private float _duration;        

    /// <summary>
    ///  初期化
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="maxRadius"></param>
    /// <param name="duration"></param>
    public void Initialize(int damage, float maxRadius, float duration)
    {
        _damage = damage;
        _maxRadius = maxRadius;
        _duration = duration;

        // スタート時は小さく
        transform.localScale = Vector3.zero;

        // R3を使って、時間をかけて大きくするアニメーション
        Observable.EveryUpdate(UnityFrameProvider.Update)
            .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(_duration))) // 指定時間まで実行
            .Subscribe(
                _ => 
                {
                    // 徐々に大きくする (Lerpなどは使わず簡易的に)
                    // フレームごとの増加量 = (最大サイズ / 時間) * 経過時間
                    float speed = _maxRadius / _duration;
                    transform.localScale += Vector3.one * speed * Time.deltaTime;
                },
                onCompleted: _ => Destroy(gameObject) // 終わったら消滅
            )
            .RegisterTo(this.destroyCancellationToken);
    }

    // 当たり判定（IsTrigger推奨）
    private void OnTriggerEnter(Collider other)
    {
        // 敵かどうか判定（タグ名はプロジェクトに合わせてください）
        if (other.CompareTag("Enemy"))
        {
            // 敵のViewなどを取得してダメージを与える
            // ※ここでは簡易的に SendMessage を使っていますが、
            // 本来は EnemyView.TakeDamage() のようなメソッドを呼ぶのが理想です
            var enemy = other.GetComponent<EnemyView>(); // インターフェースがあると便利
            if (enemy != null)
            {
                enemy.TakeDamage(_damage);
            }
            else
            {
                // インターフェースがない場合の簡易処理
                // other.gameObject.SendMessage("ApplyDamage", _damage, SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (other.CompareTag("EnemyBullet"))
        {
            // おまけ：敵の弾も消せるとボムっぽい！
            Destroy(other.gameObject);
        }
    }
}
