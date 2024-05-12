using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ObjectTiming : MonoBehaviour {
    private Vector3 eularAngles;
    public Vector3 EularAngles { // Flickでの角度
        get { return eularAngles; }
        set {
            transform.eulerAngles = value;
            eularAngles = value;
        }
    }


    float timingScale = 4f; // タイミングに依存する枠の大きさ倍率

    // Start is called before the first frame update
    // ステージ最初に呼ばれる
    void Start() {
        transform.localScale = Vector3.one * timingScale;
    }

    // Update is called once per frame
    void Update() {
        
    }

    // 表示/非表示の変更
    public void SetActive(bool value) {
        gameObject.SetActive(value);
    }

    // タイミングに依存する枠の大きさ倍率を変更
    public void SetTimingScale(float deltaBeat) {
        timingScale = deltaBeat * 2f;
        transform.localScale = Vector3.one * timingScale;
    }
}
