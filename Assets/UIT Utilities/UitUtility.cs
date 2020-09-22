using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UitUtility : MonoBehaviour {
    public static Vector2 ScreenToPanel(float scale, Vector2 screen) {
       return screen*scale;
    }
    
    public static Vector3 ScreenToPanel(float scale, Vector3 screen) {
        return screen*scale;
    }
    
    public static float ResolveScale(PanelSettings ps, Rect targetRect, float screenDpi) {
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
