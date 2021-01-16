using UnityEngine;
using UnityEngine.UIElements;

public class UIDocExampleManager : MonoBehaviour {
    protected VisualElement root;
    protected VisualElement hexHolder;
    protected VisualElement borderHolder;
    
    public UitHexGridExample uitHexGrid;
    public UitHexBorderGridExample uitHexBorderGrid;
    
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
        if (uitHexGrid != null) {
            hexHolder = new VisualElement();
            root.Add(hexHolder);
            uitHexGrid.Init(hexHolder);
            uitHexGrid.SetupHexes();
        }

        // adding borderHolder after hexHolder
        // places all borders over all hexes
        if (uitHexBorderGrid != null) {
            borderHolder = new VisualElement();
            root.Add(borderHolder);
            uitHexBorderGrid.Init(borderHolder);
            uitHexBorderGrid.SetupHexBorders();
        }

    }

    public void ShowDebug() {
        root.Query(className: "Hex").ForEach(element => Debug.Log("Hex: " + element.worldBound));
        root.Query(className: "Border").ForEach(element => Debug.Log("Border: " + element.worldBound));

    }

}
