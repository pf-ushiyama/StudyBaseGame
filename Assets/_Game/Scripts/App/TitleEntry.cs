using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必要
using UnityEngine.UI;              // ボタン操作に必要

/// <summary>
/// タイトルシーンを開始するクラス
/// </summary>
public class TitleEntry : MonoBehaviour
{
    // ゲーム開始ボタン
    [SerializeField]
    Button _startButton;

    // ゲーム終了ボタン
    [SerializeField]
    Button _exitButton;

    private const string GameSceneName = "TestScene";

    void Start()
    {
        // スタートボタンが押されたら
        _startButton.onClick.AddListener(() =>
        {
            // スタート音を鳴らしても良い
            // AudioManager.Instance.PlaySE("Select");

            // シーンを読み込む（ゲーム開始！）
            SceneManager.LoadScene(GameSceneName);
        });

        // 終了ボタンが押されたら
        _exitButton.onClick.AddListener(() =>
        {
            // アプリを終了する（エディタ上では反応しません）
            Application.Quit();
            Debug.Log("アプリ終了");
        });
    }
}
