using System;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class UitHexBorderGrid :MonoBehaviour {

    public Texture2D cellBackground;

    public int exampleRadius = 1;
    public float hexRadius = 50f;
    public Vector2 offsetCoord =new Vector2(4,-3);
    
    public string localSpaceId;
    
    protected static string AllToken;
    
    public CubeCoordinates cubeCoordinates;
    
    public VisualElement hexHolder;

    public Dictionary<string,Dictionary<Vector3, UitHex>> hexes = new Dictionary<string,Dictionary<Vector3, UitHex>>();


    public void Init(VisualElement aHexHolder) {
        hexHolder = aHexHolder;
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
                var hex = MakeHex(coord.cubeCoord, localCoord, hexHolder);
                hexes[aLocalSpaceId][coord.cubeCoord] = hex;
            }
        //}

    }
    
    private void SetupHex(UitHex hex, Vector2 location) {
        //hex.style.left = location.x;
        //hex.style.top = location.y ;
        hex.transform.position = (Vector3) location;
        hex.style.backgroundImage = cellBackground;
        // Sets a basic tooltip to the button itself.
        hex.tooltip = hex.parent.name;
    }
    
}