using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("基本ステータス")]
    public float MoveSpeed = 5.0f;
    public int MaxHp = 3;

    [Header("ボムの設定")]
    public int BombDamage = 100; // 雑魚は即死、ボスには大ダメージ
    public float BombRadius = 5.0f; // 衝撃波の最終的な大きさ
    public float BombDuration = 1.0f; // 衝撃波が広がりきるまでの時間
}
