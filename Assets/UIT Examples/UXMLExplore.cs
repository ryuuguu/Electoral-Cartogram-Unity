using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UXMLExplore : MonoBehaviour
{
    // Start is called before the first frame update
   
    void Start() {
        // Reference to the root of the window.
    
        var uiDoc= GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;
        
        // Associates a stylesheet to our root. Thanks to inheritance, all root’s
        // children will have access to it.
        root.styleSheets.Add(Resources.Load<StyleSheet>("HexGrid_Style"));

        // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
        var tree = Resources.Load<VisualTreeAsset>("HexGrid_Main");
        tree.CloneTree(root);
    
        var topBar = new VisualElement();
        var treeTopBar = Resources.Load<VisualTreeAsset>("Electoral");
        treeTopBar.CloneTree(topBar);
        root.Add(topBar);   
    }

   
}
