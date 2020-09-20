using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestCameraPanel : MonoBehaviour {
    private VisualElement root;
    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        var panel = root.panel;
        root.transform.scale = Vector3.one * 2f;
        var corner = Screen.safeArea.max;
        root.styleSheets.Add(Resources.Load<StyleSheet>("HexGrid_Style"));
        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("HexGrid_Main");
        quickToolVisualTree.CloneTree(root);
        Debug.Log("safeSize: " + corner);
        Debug.Log("0,0 : " + RuntimePanelUtils.ScreenToPanel(panel, Vector2.zero));  
        Debug.Log("100,100 : " +
                  RuntimePanelUtils.ScreenToPanel(panel, Vector2.one*100f)); 
        Debug.Log("root scale: " + root.transform.scale);
       
        NewAt(Vector3.one * 100,Vector3.one * 20);
        NewAt(RuntimePanelUtils.ScreenToPanel(panel,corner)-(Vector2.one * 30)
            ,Vector3.one * 20);
    }

    public void NewAt(Vector3 location, Vector3 scale) {
        var ve =   new UitHex();
        ve.EnableInClassList("HexGrid-Hex", true);
        UitHexGrid.SetupHex(ve,location,scale);
        root.Add(ve);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
