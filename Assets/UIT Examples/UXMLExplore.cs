using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UXMLExplore : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 offset;
    
    
    void Start() {
        // Reference to the root of the window.

        
    
        var uiDoc= GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;
        
        // Associates a stylesheet to our root. Thanks to inheritance, all root’s
        // children will have access to it.
        //root.styleSheets.Add(Resources.Load<StyleSheet>("HexGrid_Style"));

        // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
        //var tree = Resources.Load<VisualTreeAsset>("HexGrid_Main");
        //tree.CloneTree(root);
    /*
        root.style.width = 50;
        root.style.height = 50;
        root.style.backgroundColor = Color.red;
        */
        var debugAVE = new VisualElement();
        var debugBVE = new VisualElement();
        root.Add(debugAVE);
        root.Add(debugBVE);
        debugAVE.style.width = 50;
        debugAVE.style.height = 50;
        debugAVE.style.backgroundColor = Color.green;
        debugAVE.transform.position = offset;
        //debugAVE.style.position = Position.Absolute;
        /*
        debugBVE.style.width = 50;
        debugBVE.style.height = 50;
        debugBVE.style.backgroundColor = Color.blue;
        debugBVE.transform.position = Vector3.one * 200;
        debugBVE.style.position = Position.Absolute;
        */
    }

   
}
