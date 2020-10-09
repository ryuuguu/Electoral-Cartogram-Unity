using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TopBarOnly : MonoBehaviour
{
    // Start is called before the first frame update

    private VisualElement mapLayer;
    void Start()
    {
        var uiDoc= GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;   
        root.RegisterCallback<GeometryChangedEvent>( (evt) =>
            GeometryChange(evt.newRect));
        var topBar = new VisualElement();
        var treeTopBar = Resources.Load<VisualTreeAsset>("TopBar");
        treeTopBar.CloneTree(topBar);
        mapLayer = new VisualElement();
        root.Add(mapLayer);
        mapLayer.Add(topBar);
    }

    private void GeometryChange(Rect screenRect) {
        TopLevelLayout(screenRect);
    }
    
    private void TopLevelLayout(Rect screenRect) {
        
        
        var scale = ScaleMapHolder(mapLayer, new Vector2(1600,850), 
            screenRect.max);
        Debug.Log("TopLevelLayout: "+ screenRect + " : " + scale);
        
        

    }
    
    private Vector3 ScaleMapHolder(VisualElement ve,Vector2 holderSize, Vector2 parentSize) {
        var parentRatio = parentSize.x / parentSize.y;
        var holderRatio = holderSize.x / holderSize.y;
        var scale = 1f; 
        
        if (holderRatio > parentRatio) {
            scale = parentSize.x/holderSize.x;
            ve.transform.position = Vector2.zero;
        }
        else {
            scale = parentSize.y/holderSize.y;
            ve.transform.position = new Vector2((parentSize.x - holderSize.x*scale) / 2f, 0);
        }
        ve.transform.scale = scale * Vector3.one;
        Debug.Log("ScaleMapHolder: " + scale);
        return ve.transform.scale;
    }
    
}
