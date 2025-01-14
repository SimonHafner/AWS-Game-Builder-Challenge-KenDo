using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        // Framerate für alle Plattformen setzen
        #if UNITY_EDITOR || UNITY_IOS
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;  // VSync ausschalten für konsistente Framerate
        #endif
    }

}
