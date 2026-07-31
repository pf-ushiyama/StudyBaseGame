using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

public class BulletPool
{
    // 待機中の弾を入れておく列（キュー）
    private readonly Queue<BulletView> _pool = new Queue<BulletView>();

    // コピー元のプレハブ（一度ロードしたらここに保持）
    private GameObject _prefab;

    // 弾をまとめておく親（Hierarchy整理用）
    private Transform _parentTransform;

    // 初期化済みフラグ
    private bool _isInitialized = false;

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="prefabName">ロードする弾丸プレハブのファイル名</param>
    /// <param name="parent">弾をまとめるオブジェクト</param>
    /// <param name="initialCount">最初に生成する個数</param>
    /// <returns></returns>
    public async UniTask InitializeAsync(string prefabName, Transform parent, int initialCount = 20)
    {
        Debug.Log("Debug:弾丸たちの初期化");

        _parentTransform = parent;

        // アドレス指定でプレハブをロード（ここだけ少し時間がかかる）
        _prefab = await Addressables.LoadAssetAsync<GameObject>(prefabName);

        // 最初に20個くらい作ってプールに入れておく
        for (int i = 0; i < initialCount; i++)
        {
            CreateNewBullet();
        }

        _isInitialized = true;
    }


    /// <summary>
    /// 出現させる
    /// </summary>
    /// <param name="position">出現位置</param>
    /// <param name="rotation">出現角度</param>
    /// <param name="targetTag">当てたいオブジェクトのタグ</param>
    /// <returns></returns>
    public BulletView Spawn(Vector3 position, Quaternion rotation,string targetTag)
    {
        // 初期化していなければ生成しない
        if(!_isInitialized)
        {
            return null;
        }

        // プールに在庫がなければ新しく作る（不足時の自動補充）
        if (_pool.Count == 0)
        {
            CreateNewBullet();
        }

        // 列の先頭から取り出す
        var bullet = _pool.Dequeue();

        // 座標などをセットして有効化
        bullet.Activate(position, rotation, targetTag, this.Return);
        
        return bullet;
    }

    // 返却：使い終わった弾をプールに戻す
    public void Return(BulletView bullet)
    {
        bullet.Deactivate(); // 非表示にする
        _pool.Enqueue(bullet); // 列の一番後ろに並ばせる
    }

    // 内部用：新しく弾を作ってプールに入れる
    private void CreateNewBullet()
    {
        if (_prefab == null) return;

        // Instantiateはここでやる
        var obj = Object.Instantiate(_prefab, _parentTransform);
        var view = obj.GetComponent<BulletView>();
        
        // 最初は非表示
        view.Deactivate();
        
        _pool.Enqueue(view);
    }
}
