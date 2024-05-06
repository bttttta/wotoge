using System;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
///　MonoBehaviour継承したSingleton
/// </summary>
/// <typeparam name="TYpe">自身のクラス</typeparam>
/// <seealso href="https://qiita.com/Cova8bitdot/items/29b7064c7472a6f34972">【Unity】Unityで学ぶデザインパターン05: Singleton パターン【デザパタ】 #Unity - Qiita</seealso>
public abstract class SingletonMonoBehaviour<TYpe> : MonoBehaviour, IDisposable where TYpe : MonoBehaviour {
    private static TYpe instance;

    public static TYpe Instance {
        get {
            Assert.IsNotNull(instance, "There is no object attached " + typeof(TYpe).Name);
            return instance;
        }
    }

    /// <summary>
    /// 存在チェック
    /// </summary>
    /// <returns>True:存在, False:インスタンスが無い</returns>
    public static bool IsExist() { return instance != null; }

    private void Awake() {
        if(instance != null && instance.gameObject != null) {
            Destroy(this.gameObject);
            return;
        }

        instance = this as TYpe;
        OnAwakeProcess();
    }

    /// <summary>
    /// 派生先でも初期化処理を書くためのAPI
    /// </summary>
    protected virtual void OnAwakeProcess() { }


    /// <summary>
    /// Destroy時処理
    /// 派生先で実行漏れが無いように意図的にPrivate
    /// </summary>
    private void OnDestroy() {
        // 自身以外のインスタンスが作成→即時破棄されるときに間違って実行されないようにブロック
        if(instance != (this as TYpe)) return;
        OnDestroyProcess();
        Dispose();
    }

    /// <summary>
    /// Destroy時処理
    /// </summary>
    protected virtual void OnDestroyProcess() {
    }


    public virtual void Dispose() {
        if(IsExist()) instance = null;
    }

}