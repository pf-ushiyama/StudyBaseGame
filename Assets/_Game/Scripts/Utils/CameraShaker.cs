using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class CameraShaker : MonoBehaviour
{
    // シングルトン化
    public static CameraShaker Instance;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 画面を揺らす
    /// </summary>
    /// <param name="duration">揺れる時間（秒）</param>
    /// <param name="magnitude">揺れの強さ（0.1 ~ 0.5くらい）</param>
    public async void Shake(float duration, float magnitude)
    {
        // 元の位置を覚えておく
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // ランダムな位置にズラす
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            // 1フレーム待つ
            await UniTask.Yield(); 
            
            // 時間を進める（HitStop中はここも止まるので、ignoreTimeScaleは使わないのがミソ！）
            // ※ヒットストップ中に揺れも止まると「時が止まった空間で衝撃だけが残っている」演出になります
            // ※もしヒットストップ中も揺らしたければ、UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: default)などを検討
            elapsed += Time.unscaledDeltaTime; 
        }

        // 終わったら元の位置に戻す
        transform.localPosition = originalPos;
    }
}
