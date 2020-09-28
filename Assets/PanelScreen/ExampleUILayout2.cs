using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class ExampleUILayout2 : MonoBehaviour {
	public Vector2 mapRatio;
	public float scale = 10;
    
    private VisualElement _root;
    private VisualElement _mapHolder;
    private VisualElement _detailsHolder;

    void Start() {
	    Init();
	    
	    // add first call using panel utility
    }

    /// <summary>
    /// scale and position a toplevel elements after geomtry change
    /// create them if they do not exist.
    /// </summary>
    /// <param name="screenRect"></param>
    private void TopLevelLayout(Rect screenRect) {
	    if (_mapHolder == null) {
		    _mapHolder = SquareVE( Color.green);
		    _root.Add(_mapHolder);
		     MakeGrid(_mapHolder);
		     Debug.Log( "made");
	    }
	    ScaleMapHolder(_mapHolder, mapRatio, screenRect.max);
    }

    private void MakeGrid(VisualElement parent) {
	    for (int i = 0; i < mapRatio.x; i++) {
		    for (int j = 0; j < mapRatio.y; j++) {
			    var color = new Color(1,0,0,0.5f);
			    if ((i + j) % 2 ==0) {
				    color = new Color(0,1,1,0.5f);
			    }
			    var ve =  SquareVE(color);
			    var pos = (new Vector3(i, j , 0))*scale;
			    ve.transform.position = pos;
			    ve.transform.scale =  Vector3.one*scale*0.8f;
			    parent.Add(ve);
		    }
	    }
	    
    }
    /// <summary>
    /// Setup root and callbacks
    /// </summary>
    private void Init() {
	    _root = GetComponent<UIDocument>().rootVisualElement;
	    _root.RegisterCallback<GeometryChangedEvent>( (evt) => TopLevelLayout(evt.newRect));
    }
    
    private void ScaleMapHolder(VisualElement ve,Vector2 holderSize, Vector2 parentSize) {
	    var parentRatio = parentSize.x / parentSize.y;
	    var holderRatio = holderSize.x / holderSize.y;
	    var rescale = 1f; 
	    if (holderRatio > parentRatio) {
		    rescale = parentSize.x/holderSize.x;
	    }
	    else {
		    rescale = parentSize.y/holderSize.y;
	    }
	    Debug.Log("holderElement: "+ ve.localBound  + " _root (evt.newRect: "+ parentSize);
	    ve.transform.scale = rescale * Vector3.one;
	    
	    Debug.Log("Screen.safeArea: "+ Screen.safeArea  + " rescale: "+ rescale);
    }

    private VisualElement SquareVE( Color color) {
        var ve = new VisualElement();
        ve.style.position = Position.Absolute;
        ve.style.width = 1;
        ve.style.height = 1;
        ve.style.backgroundColor = color;
        return ve;
    }
}
