using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotTest : MonoBehaviour {

    public bool takeShot = false;
    public string fileName = "testScreenShot";
    
    // Update is called once per frame
    void Update()
    {
        if (takeShot) {
            takeShot = false;
            ScreenCapture.CaptureScreenshot(fileName);
            Debug.Log("ScreenShotTest");
        } 
    }
}
