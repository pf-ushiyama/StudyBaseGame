using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("雑魚敵の設定")]
    [Tooltip("雑魚敵が湧く範囲（X:幅, Z:奥行き）")]
    public Vector2 SpawnAreaSize = new Vector2(20f, 20f);

    [Tooltip("雑魚敵が湧く間隔（秒）")]
    public float SpawnInterval = 2.0f;

    [Header("ボスの設定")]
    [Tooltip("ボスが出現する座標")]
    public Vector3 BossSpawnPosition = new Vector3(0, 0, 10f);

    [Tooltip("ボスが出てくるまでに倒す敵の数")]
    public int EnemiesToKillForBoss = 10;
}