using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexMesh : MonoBehaviour{
    
    public MeshRenderer  center;
    
    // Hides the Hex
    public void Unhighlight() {
        Highlight(false);
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
