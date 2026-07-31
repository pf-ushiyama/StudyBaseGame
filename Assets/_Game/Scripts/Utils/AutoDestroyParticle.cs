using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 勝手に消滅するパーティクル
/// </summary>
public class AutoDestroyParticle : MonoBehaviour
{
    void Start()
    {
        var particle = GetComponent<ParticleSystem>();
        // パーティクルの再生が終わる時間を計算して、その時に自分を消す
        float duration = particle.main.duration + particle.main.startLifetime.constantMax;
        Destroy(gameObject, duration);
    }
}