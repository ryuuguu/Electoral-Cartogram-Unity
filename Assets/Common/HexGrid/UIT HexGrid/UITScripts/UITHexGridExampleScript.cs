using System;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class UITHexGridExampleScript :MonoBehaviour {
    
    public PointerTransform pointerTransform;

    public int exampleRadius = 1;
    public float hexRadius = 50f;
    public Vector2 offsetCoord =new Vector2(4,-3);

    public VisualElement holder;
    
    public Vector3 mouseCoord;

    public string localSpaceId;
    
    protected static string AllToken;
    
    public CubeCoordinates cubeCoordinates;
    private VisualElement root;

    public Dictionary<string,Dictionary<Vector3, UitHex>> hexes = new Dictionary<string,Dictionary<Vector3, UitHex>>();
    
    private void OnEnable() {
        // Reference to the root of the window.
        var uiDoc= GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // Associates a stylesheet to our root. Thanks to inheritance, all root’s
        // children will have access to it.
        root.styleSheets.Add(Resources.Load<StyleSheet>("HexGrid_Style"));

        // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("HexGrid_Main");
        quickToolVisualTree.CloneTree(root);

        holder = root;
        
        SetupHexes();
    }
 
    private UitHex  MakeHex(Vector3 coord,Vector2 location, VisualElement aHolder) {
        var hex = AddHex();
        aHolder.Add(hex);
        hex.name = coord.ToString();
        SetupHex(hex, location);
        hex.clickable.clicked += () => Debug.Log("Clicked! " + coord);
        return hex;
    }
    
    public void SetupHexes() {
        cubeCoordinates = new CubeCoordinates();
        AllToken = CubeCoordinates.AllContainer;
        localSpaceId =  CubeCoordinates.NewLocalSpaceId(hexRadius/2, Vector2.one, CubeCoordinates.LocalSpace.Orientation.XY,null,offsetCoord);
        var coordList = cubeCoordinates.Construct(exampleRadius);
        MakeAllHexes(localSpaceId);
        //NewMap();
    }
    
    public UitHex AddHex() {
        var hex = new UitHex();
        hex.EnableInClassList("HexGrid-Hex-icon", true);
        var image = new Image();
        image.EnableInClassList("HexGrid-Hex-highlight",true);
        hex.Add(image);
        return hex;
    }
    
    public void MakeAllHexes(string aLocalSpaceId) {
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        if (!hexes.ContainsKey(localSpaceId)) {
            hexes[aLocalSpaceId] = new Dictionary<Vector3, UitHex>();
        }
        
        // not in editor anymore should this change?
        //if (ls.spaceRectTransform != null) { not used in editor mode
        
            foreach (var coord in allCoords) {
                var localCoord = CubeCoordinates.ConvertPlaneToLocalPosition(coord.cubeCoord, ls);
                var hex = MakeHex(coord.cubeCoord, localCoord, holder);
                hexes[aLocalSpaceId][coord.cubeCoord] = hex;
            }
        //}
        
        
        
    }
    
    private void SetupHex(UitHex hex, Vector2 location) {
        hex.style.left = location.x;
        hex.style.top = location.y ;
        // Reference to the VisualElement inside the hex that serves
        // as thehighlight.
       var highlight = hex.Q<Image>(className: "HexGrid-Hex-highlight");
        
        // Icon’s path in our project.
        var filledPath = "Images/hex filled";
        var outlinePath = "Images/hex outline";

        // Loads the actual asset from the above path.
        var filledAsset = Resources.Load<Texture2D>(filledPath);
        var outlineAsset = Resources.Load<Texture2D>(outlinePath);

        // Applies the above asset as a background image for the icon.
        
        hex.style.backgroundImage = outlineAsset;
        highlight.image = filledAsset;
        highlight.EnableInClassList("Hide",true);
        if (Random.value > 0.5f) {
          //  highlight.EnableInClassList("Hide",false);
        }

        // Sets a basic tooltip to the button itself.
        hex.tooltip = hex.parent.name;
    }
    
    
    
    
    private void CreateObject(string primitiveTypeName)
    {    
        var pt = (PrimitiveType) Enum.Parse
            (typeof(PrimitiveType), primitiveTypeName, true);
        var go = ObjectFactory.CreatePrimitive(pt);
        go.transform.position = Vector3.zero;
    }
    
    
}