using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ↓ これを書くと、Unityの右クリックメニューからこのデータファイルを作れるようになります
[CreateAssetMenu(fileName = "NewEnemyParam", menuName = "Game/EnemyParam")]
public class EnemyParam : ScriptableObject
{
[Header("基本ステータス")]
    public int MaxHp = 1;
    public float MoveSpeed = 3.0f;
    public int ScoreValue = 100; // 倒したときのスコア

    [Header("見た目・演出")]
    public float Scale = 1.0f;   // 体の大きさ
    public Color BodyColor = Color.white; // 体の色
}
