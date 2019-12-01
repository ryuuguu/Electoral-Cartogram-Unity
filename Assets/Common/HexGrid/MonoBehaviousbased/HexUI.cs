using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexUI : MonoBehaviour{
    public Image  center;
    
    // Hides the Hex
    public void Hide() {
        center.enabled= false;
        //Debug.Log("hide: "+ name);
    }

    // Shows the hex
    public void Show() {
        center.enabled = true;
        //Debug.Log("show: "+ name);
    }
}
