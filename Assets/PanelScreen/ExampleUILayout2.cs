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
	    //TestBox();
	    // add first call using panel utility
    }

    /// <summary>
    /// scale and position a toplevel elements after geomtry change
    /// create them if they do not exist.
    /// </summary>
    /// <param name="screenRect"></param>
    private void TopLevelLayout(Rect screenRect) {
	    if (_mapHolder == null) {
		    _mapHolder = new VisualElement();
		    _root.Add(_mapHolder);
		     MakeGrid(_mapHolder);
		     Debug.Log( "made");
	    }

	    // ScaleMapHolder(_mapHolder, mapRatio, screenRect.max);

    }

    private void MakeGrid(VisualElement parent) {
	    
	    for (int i = 0; i < 3; i++) {
		   
		    for (int j = 0; j < 2; j++) {
			    var color = Color.red;
			    if ((i + j) % 2 ==0) {
				    color = Color.cyan;
			    }
			    var ve =  SquareVE(color);

			    
			    var pos = (new Vector3(i, j , 0))*10;
			   
			    ve.transform.position = pos;
			    ve.transform.scale = new Vector3(scale,scale,0);
			    _root.Add(ve);
			   
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
	    ve.transform.scale = rescale * Vector3.one;
	   
	    Debug.Log("recale: "+ rescale);
    }

    private VisualElement SquareVE( Color color) {
        var ve = new VisualElement();
        ve.style.position = Position.Absolute;
        ve.style.width = 1;
        ve.style.height = 1;
        ve.style.backgroundColor = color;
        return ve;
    }
    //==================
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
    
    

    private void TestBox() {
	    if (_detailsHolder == null) {
		    _detailsHolder = NewHolder( Color.magenta);
	    }
	    
	    var rect = new Rect(0, 100,
		    400, 400);
	    ScaledAt(_detailsHolder, rect );
	    MakeGrid(_root);
    }
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
