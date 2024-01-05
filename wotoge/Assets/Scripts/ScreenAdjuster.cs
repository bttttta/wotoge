using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAdjuster : MonoBehaviour
{
    public float SceneWidth = 1080.0f;
    public float SceneHeight = 1920.0f;

    void AdjustScreen() {
        Camera camera = GetComponent<Camera>();

        // Šg‘å—¦‚ÌŒvŽZ
        float scaleWidth = (float)Screen.width / SceneWidth;
        float scaleHeight = (float)Screen.height / SceneHeight;
        
        if(scaleWidth < scaleHeight) { // c’·¨ƒTƒCƒY‚Í‰¡‚É‡‚í‚¹‚é
            camera.orthographicSize = (float)Screen.height / scaleWidth / 2;
        } else { // ‰¡’·¨ƒTƒCƒY‚Íc‚É‡‚í‚¹‚é
            camera.orthographicSize = 960;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AdjustScreen();
    }
}
