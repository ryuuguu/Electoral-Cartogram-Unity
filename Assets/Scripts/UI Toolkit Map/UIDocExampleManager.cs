using System;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIDocExampleManager : MonoBehaviour {
    protected VisualElement root;

    protected VisualElement hexHolder;
    protected VisualElement borderHolder;
    
    public UitHexGrid uitHexGrid;
    public UitHexBorderGrid uitHexBorderGrid;
    
    private void Start() {
        // Reference to the root of the window.
        var uiDoc= GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        

        // Associates a stylesheet to our root. Thanks to inheritance, all root’s
        // children will have access to it.
        root.styleSheets.Add(Resources.Load<StyleSheet>("HexGrid_Style"));

        // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("HexGrid_Main");
        quickToolVisualTree.CloneTree(root);

        //this acts as visual "Layer"
        hexHolder = new VisualElement();
        root.Add(hexHolder);
        uitHexGrid.Init(hexHolder);
        uitHexGrid.SetupHexes();
        
        borderHolder = new VisualElement();
        root.Add(borderHolder);
        uitHexBorderGrid.Init(borderHolder);
        uitHexBorderGrid.SetupHexBorders();
        
    }
}
