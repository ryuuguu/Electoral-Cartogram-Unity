using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestPanelScreen : MonoBehaviour {
    private VisualElement root;

    // Start is called before the first frame update
    void Start() {
        var camara = Camera.main;
        root = GetComponent<UIDocument>().rootVisualElement;

        
        var corner = Screen.safeArea.max;
        root.styleSheets.Add(Resources.Load<StyleSheet>("HexGrid_Style"));
        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("HexGrid_Main");
        quickToolVisualTree.CloneTree(root);
        var panel = root.panel;
        var scale = UitUtility.ResolveScale(GetComponent<UIDocument>().panelSettings,
            new Rect(0, 0, Screen.width, Screen.height),
            Screen.dpi);
        Debug.Log("safeSize: " + corner + " : " + UitUtility.ScreenToPanel(scale, corner));
        Debug.Log("0,0 : " + UitUtility.ScreenToPanel(scale, Vector2.zero));
        Debug.Log("100,100 : " +
                  UitUtility.ScreenToPanel(scale, Vector2.one * 100f));
        Debug.Log("panel scale: " + panel.visualTree.transform.scale);
        Debug.Log("hacked scale: " + scale);
        Debug.Log("20:20:20 scale: " + UitUtility.ScreenToPanel(scale,Vector3.one * 20));

        NewAt(Vector3.one * 100, Vector3.one * 20);
        NewAt(UitUtility.ScreenToPanel(scale, corner - (Vector2.one * 30))
            , UitUtility.ScreenToPanel(scale,Vector3.one * 20));
    }

    public void NewAt(Vector3 location, Vector3 scale) {
        var ve = new UitHex();
        ve.EnableInClassList("HexGrid-Hex", true);
        UitHexGrid.SetupHex(ve, location, scale);
        root.Add(ve);
    }

}
