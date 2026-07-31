using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キー入力が効かない問題の調査用クラス
/// </summary>
public class TestKeyInput : MonoBehaviour
{
    void Start()
    {
        Debug.Log("キー入力のテストを開始します");
    }

    void Update()
    {
        /// スペースキーの入力を取得する
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Spaseキーを押しました");
        }
    }
}
