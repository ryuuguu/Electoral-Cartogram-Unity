using System;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class UitHexBorderGrid :MonoBehaviour {

    public Texture2D cellBackground;
    public Texture2D borderImage;


    public int exampleRadius = 1;
    public float hexRadius = 50f;
    public Vector2 offsetCoord =new Vector2(4,-3);
    
    public string localSpaceId;
    
    protected static string AllToken;
    
    public CubeCoordinates cubeCoordinates;
    
    public VisualElement borderHolder;

    public struct HexBorder {
        public VisualElement borderholder;
        public VisualElement[] borders;
    }
    public Dictionary<string,Dictionary<Vector3, HexBorder>> borders = new Dictionary<string,Dictionary<Vector3, HexBorder>>();
    
    public void Init(VisualElement aHexHolder) {
        borderHolder = aHexHolder;
    }
    
    private HexBorder  MakeHexBorder(Vector2 location, VisualElement aHolder) {
        
        HexBorder hexBorder = new HexBorder();
        hexBorder.borderholder = new VisualElement();
        hexBorder.borders = new VisualElement[6];
        for (int i = 0; i < 6; i++) {
            var border = new VisualElement();
            hexBorder.borders[i] = border;
            hexBorder.borderholder.Add(border);
            border.style.backgroundImage = borderImage;
            border.transform.position = new Vector3(hexRadius,0,0);
            border.transform. rotation = Quaternion.Euler(60*i,0,0);
        }
        hexBorder.borderholder.transform.position = (Vector3) location;
        return hexBorder;
    }
    
    public void SetupHexes() {
        cubeCoordinates = new CubeCoordinates();
        AllToken = CubeCoordinates.AllContainer;
        localSpaceId =  CubeCoordinates.NewLocalSpaceId(hexRadius/2, Vector2.one, CubeCoordinates.LocalSpace.Orientation.XY,null,offsetCoord);
        var coordList = cubeCoordinates.Construct(exampleRadius);
        MakeAllHexes(localSpaceId);
        //NewMap();
    }
    
    public void MakeAllHexes(string aLocalSpaceId) {
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        if (!borders.ContainsKey(localSpaceId)) {
            borders[aLocalSpaceId] = new Dictionary<Vector3, HexBorder>();
        }
        
        // not in editor anymore should this change?
        //if (ls.spaceRectTransform != null) { not used in editor mode
        
            foreach (var coord in allCoords) {
                var localCoord = CubeCoordinates.ConvertPlaneToLocalPosition(coord.cubeCoord, ls);
                var hex = MakeHexBorder(localCoord, borderHolder);
                borders[aLocalSpaceId][coord.cubeCoord] = hex;
            }
        //}

    }
}