using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexUI : MonoBehaviour{
    public Image  center;
    public Vector3 cubeCoord;
    
    // Hides the Hex
    public void Unhighlight() {
        center.enabled= false;
    }

    // Shows the hex
    public void Highlight(bool val = true) {
        center.enabled = val;
    }
    
    public bool ToggleHighlight() {
        Highlight(!center.enabled);
        return center.enabled;
        
    }
}
