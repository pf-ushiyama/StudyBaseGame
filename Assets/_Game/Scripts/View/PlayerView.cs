using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R3;

/// <summary>
/// プレイヤーの表示と入力の検知を行うクラス
/// </summary>
public class PlayerView : MonoBehaviour , IDamageable
{
    // 移動速度
    [SerializeField]
    private float _moveSpeed = 5.0f;

    // 移動入力ストリーム
    public Observable<Vector2> OnMoveInput => _onMoveInput;
    private readonly Subject<Vector2> _onMoveInput = new Subject<Vector2>();

    // ボム使用ストリーム
    public Observable<Unit> OnUseBombInput => _onUseBombInput;
    private readonly Subject<Unit> _onUseBombInput = new Subject<Unit>();

    // 敵との接触ストリーム
    // public Observable<Unit> OnCollistionEnemy => _onCollistionEnemy;
    // private readonly Subject<Unit> _onCollistionEnemy = new Subject<Unit>();

    public Observable<Unit> OnDameged => _onDameged;
    private readonly Subject<Unit> _onDameged = new Subject<Unit>();


    // トランスフォーム
    public Transform Transform
    {
        get
        {
            return gameObject.transform;
        }
    }

    /// <summary>
    ///  毎フレーム処理
    /// </summary>
    void Update()
    {
        // 移動入力を検知
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical"); 
        _onMoveInput.OnNext(new Vector2(x, y));

        // クリックを検知
        if (Input.GetButtonDown("Bomb")) // マウス左クリック or Ctrlキー
        {
            _onUseBombInput.OnNext(Unit.Default);
        }
    }

    /// <summary>
    /// 接触時処理
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            // ダメージを受ける
            _onDameged.OnNext(Unit.Default);
        }
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        _onDameged.OnNext(Unit.Default);
    }

    /// <summary>
    /// 移動する
    /// </summary>
    /// <param name="direction">移動方向</param>
    public void Move(Vector2 direction)
    {
        if(direction == Vector2.zero)
        {
            return;
        }

        Vector3 moveDir = new Vector3(direction.x, 0, direction.y).normalized;

        transform.position += moveDir * _moveSpeed * Time.deltaTime;

        transform.LookAt(transform.position + moveDir);
    }
}
