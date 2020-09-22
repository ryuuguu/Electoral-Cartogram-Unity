using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseUILayout : MonoBehaviour {
	public Vector2 MapRatio;
    public Vector2 DetailsTopRightcorner;
    
    private VisualElement root;
    private Vector2 _screenCorner;
    private float screenScale;
    
    void Start() {
	    Init();
	    var box =NewScaledAt(Vector3.one * 0, Vector3.one * 20,
	        Color.green);
        ScalePositionMap(box, MapRatio.x / MapRatio.y,
	        UitUtility.ScreenToPanel(screenScale, _screenCorner));
       
        var detailTopRightPos = box.transform.matrix.MultiplyPoint(DetailsTopRightcorner);
        // detail box goes bottom left to detailTopRightPos
        // need top left and bottom right
        // that is (0,detailTopRightPos.y)  (detailTopRightPos.x,max)
        var rect = new Rect(0, detailTopRightPos.y,
	        detailTopRightPos.x, _screenCorner.y - detailTopRightPos.y);
        Debug.Log("_screenCorner " + _screenCorner);
        Debug.Log("detailTopRightPos " + detailTopRightPos);
        Debug.Log("rect "+ rect);
        var detailBox = NewScaledAt(rect, Color.blue);
    }

    /// <summary>
    /// Setup root and screen scale
    /// </summary>
    private void Init() {
	    root = GetComponent<UIDocument>().rootVisualElement;
	    _screenCorner = Screen.safeArea.max;
	    root.styleSheets.Add(Resources.Load<StyleSheet>("HexGrid_Style"));
	    var quickToolVisualTree = Resources.Load<VisualTreeAsset>("HexGrid_Main");
	    quickToolVisualTree.CloneTree(root);
	    screenScale = UitUtility.ResolveScale(GetComponent<UIDocument>().panelSettings,
		    new Rect(0, 0, Screen.width, Screen.height),
		    Screen.dpi); 
    }
    
    
    public void ScalePositionMap(VisualElement ve,float boxRatio, Vector2 HolderSize) {
	    var holderRatio = HolderSize.x / HolderSize.y;
	    var scale = HolderSize; 
	    if (boxRatio > holderRatio) {
		   scale.y = scale.x/boxRatio;
	    }
	    else {
		    scale.x = scale.y*boxRatio;
		    ve.transform.position = new Vector2((HolderSize.x - scale.x) / 2, 0);
	    }
	    ve.transform.scale = scale;
    }

	/// <summary>
	/// place a scaled box on the root.
	/// </summary>
	/// <param name="rect"></param>
	/// <param name="color"></param>
	/// <returns></returns>
    public VisualElement NewScaledAt(Rect rect, Color color) {
		return NewScaledAt(rect.position, rect.size, color);
	}
	
    /// <summary>
   /// place a scaled box on the root.
   /// </summary>
   /// <param name="location"></param>
   /// <param name="scale"></param>
   /// <param name="color"></param>
   /// <returns></returns>
    public VisualElement NewScaledAt(Vector3 location, Vector3 scale, Color color) {
        var ve = new UitHex();
        ve.EnableInClassList("HexGrid-Hex", true);
        UitHexGrid.SetupHex(ve, location, scale);
        ve.style.backgroundColor = color;
        root.Add(ve);
        return ve;
    }

}
