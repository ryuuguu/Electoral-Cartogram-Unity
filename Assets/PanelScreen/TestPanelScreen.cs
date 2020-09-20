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
        var scale = ResolveScale(GetComponent<UIDocument>().panelSettings,
            new Rect(0, 0, Screen.width, Screen.height),
            Screen.dpi);
        Debug.Log("safeSize: " + corner);
        Debug.Log("0,0 : " + ScreenToPanel(scale, Vector2.zero));
        Debug.Log("100,100 : " +
                  ScreenToPanel(scale, Vector2.one * 100f));
        Debug.Log("root scale: " + root.transform.scale);
        Debug.Log("panel scale: " + panel.visualTree.transform.scale);
        Debug.Log("hacked scale: " + panel.visualTree.transform.scale);

        NewAt(Vector3.one * 100, Vector3.one * 20);
        NewAt(ScreenToPanel(scale, corner) - (Vector2.one * 30)
            , Vector3.one * 20);
    }

    public Vector2 ScreenToPanel(float scale, Vector2 screen) {
       return screen*scale;
    }
    
    public void NewAt(Vector3 location, Vector3 scale) {
        var ve = new UitHex();
        ve.EnableInClassList("HexGrid-Hex", true);
        UitHexGrid.SetupHex(ve, location, scale);
        root.Add(ve);
    }

    float ResolveScale(PanelSettings ps, Rect targetRect, float screenDpi) {
        // Calculate scaling
        float resolvedScale = 1.0f;
        switch (ps.scaleMode) {
            case PanelScaleModes.ConstantPixelSize:
                break;
            case PanelScaleModes.ConstantPhysicalSize: {
                var dpi = screenDpi == 0.0f ? ps.fallbackDpi : screenDpi;
                if (dpi != 0.0f)
                    resolvedScale = ps.referenceDpi / dpi;
            }
                break;
            case PanelScaleModes.ScaleWithScreenSize:
                if (ps.referenceResolution.x * ps.referenceResolution.y != 0) {
                    var refSize = (Vector2) ps.referenceResolution;
                    var sizeRatio = new Vector2(targetRect.width / refSize.x, targetRect.height / refSize.y);

                    var denominator = 0.0f;
                    switch (ps.screenMatchMode) {
                        case PanelScreenMatchModes.Expand:
                            denominator = Mathf.Min(sizeRatio.x, sizeRatio.y);
                            break;
                        case PanelScreenMatchModes.Shrink:
                            denominator = Mathf.Max(sizeRatio.x, sizeRatio.y);
                            break;
                        default: // PanelScreenMatchModes.MatchWidthOrHeight:
                            var widthHeightRatio = Mathf.Clamp01(ps.match);
                            denominator = Mathf.Lerp(sizeRatio.x, sizeRatio.y, widthHeightRatio);
                            break;
                    }

                    if (denominator != 0.0f)
                        resolvedScale = 1.0f / denominator;
                }

                break;
        }

        return resolvedScale;
    }
}
