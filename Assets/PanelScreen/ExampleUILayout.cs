using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class ExampleUILayout : MonoBehaviour {
	[FormerlySerializedAs("MapRatio")] public Vector2 mapRatio;
    [FormerlySerializedAs("DetailsTopRightCorner")] public Vector2 detailsTopRightCorner; //in MapHolder space
    
    private VisualElement _root;
    private VisualElement _mapHolder;
    private VisualElement _detailsHolder;

    void Start() {
	    Init();
    }

    /// <summary>
    /// scale and position a toplevel elements after geomtry change
    /// create them if they do not exist.
    /// </summary>
    /// <param name="screenRect"></param>
    private void TopLevelLayout(Rect screenRect) {
	    if (_mapHolder == null) {
		    _mapHolder = NewHolder(Color.green);
	    }

	    ScalePositionMapHolder(_mapHolder, mapRatio.x / mapRatio.y,
		    screenRect.max);

	    if (_detailsHolder == null) {
		    _detailsHolder = NewHolder( Color.blue);
	    }
	    
	    var detailsTopRightPos = _mapHolder.transform.matrix.MultiplyPoint(detailsTopRightCorner);
	    var rect = new Rect(0, detailsTopRightPos.y,
		    detailsTopRightPos.x, screenRect.yMax - detailsTopRightPos.y);
	    ScaledAt(_detailsHolder,rect);
    }

    /// <summary>
    /// Setup root and callbacks
    /// </summary>
    private void Init() {
	    _root = GetComponent<UIDocument>().rootVisualElement;
	    //normally a style sheet would loaded here
	    //_root.styleSheets.Add(Resources.Load<StyleSheet>("HexGrid_Style"));
	    //normally a tree would loaded here
	    //var treeree = Resources.Load<VisualTreeAsset>("HexGrid_Main");
	    //tree.CloneTree(_root);
	    _root.RegisterCallback<GeometryChangedEvent>( (evt) => TopLevelLayout(evt.newRect));
    }
    
    /// <summary>
    /// set scale and position of map holder
    /// based on 
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="boxRatio"></param>
    /// <param name="parentSize"></param>
    private void ScalePositionMapHolder(VisualElement ve,float holderRatio, Vector2 parentSize) {
	    var parentRatio = parentSize.x / parentSize.y;
	    var scale = parentSize; 
	    if (holderRatio > parentRatio) {
		   scale.y = scale.x/holderRatio;
	    }
	    else {
		    scale.x = scale.y*holderRatio;
		    ve.transform.position = new Vector2((parentSize.x - scale.x) / 2f, 0);
	    }
	    ve.transform.scale = scale;
    }

    
    /// <summary>
	/// place a colored box on the root.
	/// </summary>
	/// <param name="location"></param>
	/// <param name="scale"></param>
	/// <param name="color"></param>
	/// <returns></returns>
    private VisualElement NewHolder( Color color) {
        var ve = new VisualElement();
        // normally style position, width and height would be set with a class from a stylesheet
        ve.style.position = Position.Absolute;
        ve.style.width = 1;
        ve.style.height = 1;
        ve.style.backgroundColor = color;
        _root.Add(ve);
        return ve;
    }

    /// <summary>
    /// scale and position a box
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="rect"></param>
    private void ScaledAt(VisualElement  ve,Rect rect) {
	    ScaledAt(ve,rect.position, rect.size);
    }
    
    /// <summary>
    /// scale and position a box
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="position"></param>
    /// <param name="scale"></param>
    private void ScaledAt(VisualElement ve, Vector3 position, Vector3 scale) {
	    ve.transform.position = position;
	    ve.transform.scale = scale;
    }
    
}
