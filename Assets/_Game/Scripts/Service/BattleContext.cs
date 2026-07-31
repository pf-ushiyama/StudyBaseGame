using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バトル中のコンテキスト
/// </summary>
public class BattleContext
{
    // プレイヤーの場所（敵が追いかける用）
    public Transform PlayerTransform { get; set; }

    // 弾丸の親オブジェクト
    public Transform ProjectileContainer { get; set; }
}
