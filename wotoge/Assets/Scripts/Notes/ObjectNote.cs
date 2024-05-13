using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectNote : MonoBehaviour {
    private Vector3 eulerAngles;
    public Vector3 EulerAngles { // Flickでの角度
        get { return eulerAngles; }
        set {
            transform.eulerAngles = value;
            eulerAngles = value;
        }
    }

    private Vector3 position;
    public Vector3 Position {
        get { return position; }
        set {
            transform.position = value;
            position = value;
        }
    }


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    // 表示/非表示の変更
    public void SetActive(bool value) {
        gameObject.SetActive(value);
    }
}
