using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreLabel : MonoBehaviour {
    ScoreManager scoreManager;
    TextMeshProUGUI textMeshProUGUI;

    // Start is called before the first frame update
    void Start() {
        scoreManager = ScoreManager.Instance;
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update() {
        textMeshProUGUI.text = scoreManager.Score.ToString();
    }
}
