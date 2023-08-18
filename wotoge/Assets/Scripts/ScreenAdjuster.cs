using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAdjuster : MonoBehaviour
{
    void AdjustScreen() {
        Screen.SetResolution(1080, 1920, FullScreenMode.FullScreenWindow, 60);
    }

    // Start is called before the first frame update
    void Start()
    {
        AdjustScreen();
    }
}
