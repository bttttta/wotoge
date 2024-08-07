using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CRLabel : MonoBehaviour {
    ScoreManager scoreManager;
    TextMeshProUGUI textMeshProUGUI;
    // Start is called before the first frame update
    void Start() {
        scoreManager = ScoreManager.Instance;
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update() {
        float CR = scoreManager.CR * 100;
        textMeshProUGUI.text = CR.ToString("F2") + "%";
    }
}
