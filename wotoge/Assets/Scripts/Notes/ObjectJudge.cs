using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectJudge : MonoBehaviour {
    const float time_result = 1f; // ”»’è•\¦‚³‚ê‚éŠÔB•b

    // Start is called before the first frame update
    // ”»’è‚ª•\¦‚³‚ê‚é‚Æ‚«‚ÉŒÄ‚Î‚ê‚é
    void Start() {
        // time_resultŒã‚ÉÁ‹
        Destroy(gameObject, time_result);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
