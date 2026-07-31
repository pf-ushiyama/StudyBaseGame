using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// ヒットストップを行うクラス
/// </summary>
public class HitStop
{
    // 複数同時に呼び出されることを防ぐフラグ
    private static bool _isStopping = false;

    /// <summary>
    /// 時間を一瞬止める
    /// </summary>
    /// <param name="duration">止める時間</param>
    public static async void Freeze(float duration)
    {
        if(_isStopping)
        {
            return;
        }
        _isStopping = true;

        // 1. 時間をほぼ止める
        // 完全に 0 にするとバグることもあるので、0.01倍速くらいにするのがコツ
        float originalScale = Time.timeScale; // 元の速度を覚えておく
        Time.timeScale = 0.05f;

        // 2. 指定時間だけ待つ
        // ★重要: ignoreTimeScale: true にしないと、自分も止まって永遠に動かなくなる！
        await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: true);

        // 3. 時間を戻す
        Time.timeScale = originalScale;
        
        _isStopping = false;
    }
}
