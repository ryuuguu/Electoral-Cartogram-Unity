using System;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class WebGLScroll : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void ScrollBy(int x, int y);

    public Text debugDisplay;

    public float sensitivity = 100;

#if UNITY_WEBGL 
    private void Update() {

        var x = Input.GetAxis("Mouse ScrollWheel");
        if (x != 0) {
            ScrollBy(0,(int)(sensitivity*x*-1));
            debugDisplay.text = x.ToString();
            //Hello();
            ;
        }

    }
#endif
}