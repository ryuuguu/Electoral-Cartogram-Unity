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
       
        //root.transform.scale = Vector3.one * 2f;
        var corner = Screen.safeArea.max;
        root.styleSheets.Add(Resources.Load<StyleSheet>("HexGrid_Style"));
        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("HexGrid_Main");
        quickToolVisualTree.CloneTree(root);
        var panel = root.panel;
        Debug.Log("safeSize: " + corner);
        Debug.Log("0,0 : " + RuntimePanelUtils.ScreenToPanel(panel, Vector2.zero));  
        Debug.Log("100,100 : " +
                  RuntimePanelUtils.ScreenToPanel(panel, Vector2.one*100f)); 
        Debug.Log("root scale: " + root.transform.scale);
        Debug.Log("panel scale: " + panel.visualTree.transform.scale);
       
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
    
    float ResolveScale(Rect targetRect, float screenDpi)
    {
        // Calculate scaling
        float resolvedScale = 1.0f;
        switch (scaleMode)
        {
            case PanelScaleModes.ConstantPixelSize:
                break;
            case PanelScaleModes.ConstantPhysicalSize:
            {
                var dpi = screenDpi == 0.0f ? fallbackDpi : screenDpi;
                if (dpi != 0.0f)
                    resolvedScale = referenceDpi / dpi;
            }
                break;
            case PanelScaleModes.ScaleWithScreenSize:
                if (referenceResolution.x * referenceResolution.y != 0)
                {
                    var refSize = (Vector2)referenceResolution;
                    var sizeRatio = new Vector2(targetRect.width / refSize.x, targetRect.height / refSize.y);

                    var denominator = 0.0f;
                    switch (screenMatchMode)
                    {
                        case PanelScreenMatchModes.Expand:
                            denominator = Mathf.Min(sizeRatio.x, sizeRatio.y);
                            break;
                        case PanelScreenMatchModes.Shrink:
                            denominator = Mathf.Max(sizeRatio.x, sizeRatio.y);
                            break;
                        default: // PanelScreenMatchModes.MatchWidthOrHeight:
                            var widthHeightRatio = Mathf.Clamp01(match);
                            denominator = Mathf.Lerp(sizeRatio.x, sizeRatio.y, widthHeightRatio);
                            break;
                    }
                    if (denominator != 0.0f)
                        resolvedScale = 1.0f / denominator;
                }
                break;
        }
}
