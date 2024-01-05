using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAdjuster : MonoBehaviour
{
    public float SceneWidth = 1080.0f;
    public float SceneHeight = 1920.0f;

    void AdjustScreen() {
        Camera camera = GetComponent<Camera>();

        // �g�嗦�̌v�Z
        float scaleWidth = (float)Screen.width / SceneWidth;
        float scaleHeight = (float)Screen.height / SceneHeight;
        
        if(scaleWidth < scaleHeight) { // �c�����T�C�Y�͉��ɍ��킹��
            camera.orthographicSize = (float)Screen.height / scaleWidth / 2;
        } else { // �������T�C�Y�͏c�ɍ��킹��
            camera.orthographicSize = 960;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AdjustScreen();
    }
}
