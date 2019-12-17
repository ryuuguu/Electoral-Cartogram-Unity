using System;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class UIEHexGridExampleScript : EditorWindow {
    
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

    public Dictionary<string,Dictionary<Vector3, UIEHex>> hexes = new Dictionary<string,Dictionary<Vector3, UIEHex>>();
    
    [MenuItem("HexGrid/Open _%#T")]
    public static void ShowWindow() {
        // Opens the window, otherwise focuses it if it’s already open.
        var window = GetWindow<UIEHexGridExampleScript>();

        // Adds a title to the window.
        window.titleContent = new GUIContent("HexGrid");

        // Sets a minimum size to the window.
        window.minSize = new Vector2(250, 250);
    }
    
    private void OnEnable() {
        // Reference to the root of the window.
        root = rootVisualElement;

        // Associates a stylesheet to our root. Thanks to inheritance, all root’s
        // children will have access to it.
        root.styleSheets.Add(Resources.Load<StyleSheet>("HexGrid_Style"));

        // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
        var quickToolVisualTree = Resources.Load<VisualTreeAsset>("HexGrid_Main");
        quickToolVisualTree.CloneTree(root);

        holder = root;
        
        SetupHexes();
    }
 
    private UIEHex  MakeHex(Vector3 coord,Vector2 location, VisualElement aHolder) {
        var hex = AddHex();
        aHolder.Add(hex);
        hex.name = coord.ToString();
        SetupHex(hex, location);
        return hex;
    }
    
    public void SetupHexes() {
        cubeCoordinates = new CubeCoordinates();
        AllToken = CubeCoordinates.AllContainer;
        localSpaceId =  CubeCoordinates.NewLocalSpaceId(hexRadius/2, CubeCoordinates.LocalSpace.Orientation.XY,null,offsetCoord);
        var coordList = cubeCoordinates.Construct(exampleRadius);
        MakeAllHexes(localSpaceId);
        //NewMap();
    }
    
    public UIEHex AddHex() {
        var hex = new UIEHex();
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
            hexes[aLocalSpaceId] = new Dictionary<Vector3, UIEHex>();
        }
        
        //if (ls.spaceRectTransform != null) { not used in editor mode
            foreach (var coord in allCoords) {
                var localCoord = CubeCoordinates.ConvertPlaneToLocalPosition(coord.cubeCoord, ls);
                var hex = MakeHex(coord.cubeCoord, localCoord, holder);
                hexes[aLocalSpaceId][coord.cubeCoord] = hex;
            }
        //}
        
    }
    
    
    
    
    private void SetupHex(UIEHex hex, Vector2 location) {
        hex.style.left = location.x;
        hex.style.top = location.y ;
        // Reference to the VisualElement inside thehex that serves
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
        
/*
        // Instantiates our primitive object on a left click.
        button.clickable.clicked += () => CreateObject(button.parent.name);
*/
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