using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("スピーカー設定")]
    [SerializeField] 
    AudioSource _bgmSource; // BGM用（ループする）
    [SerializeField]
    AudioSource _seSource;  // SE用（使い捨て）

    [Header("音源登録")]
    [SerializeField]
    List<AudioClip> _bgmClips; // BGM素材リスト
    [SerializeField]
    List<AudioClip> _seClips;  // SE素材リスト

    // 名前で検索しやすくするための辞書
    private Dictionary<string, AudioClip> _bgmDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _seDict = new Dictionary<string, AudioClip>();

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        // リストを辞書に変換（"Attack" という名前で検索できるようにする）
        foreach (var clip in _bgmClips) _bgmDict[clip.name] = clip;
        foreach (var clip in _seClips) _seDict[clip.name] = clip;
    }

    /// <summary>
    /// BGMを再生
    /// </summary>
    /// <param name="bgmName"></param>
    public void PlayBGM(string bgmName)
    {
        if (_bgmDict.ContainsKey(bgmName))
        {
            // 既に同じ曲が流れていたら何もしない
            if (_bgmSource.clip != null && _bgmSource.clip.name == bgmName)
            {
                return;   
            }

            _bgmSource.clip = _bgmDict[bgmName];
            _bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGMが見つかりません: {bgmName}");
        }
    }

    /// <summary>
    /// BGMストップ
    /// </summary>
    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    /// <summary>
    /// SEを再生
    /// </summary>
    /// <param name="seName"></param>
    public void PlaySE(string seName)
    {
        if (_seDict.ContainsKey(seName))
        {
            // PlayOneShotは「重ねがけ」ができる（マシンガンみたいに連打しても音が途切れない）
            _seSource.PlayOneShot(_seDict[seName]);
        }
        else
        {
            Debug.LogWarning($"SEが見つかりません: {seName}");
        }
    }
}
