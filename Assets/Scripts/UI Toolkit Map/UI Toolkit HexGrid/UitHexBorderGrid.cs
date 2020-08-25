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
    protected float borderOffsetX; //calculated from hexRadius
    protected float borderOffsetY;  //calculated from hexRadius
    
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
        borderOffsetX = -1*hexRadius / 2;
        borderOffsetY = hexRadius * Mathf.Cos(Mathf.PI / 3) / 2;
        
    }
    
    private HexBorder  MakeHexBorder(Vector2 location, VisualElement aHolder) {
        //radius is to vertex
        // offset needs to be distance to center of the edge
       
        
        
        HexBorder hexBorder = new HexBorder();
        hexBorder.borderholder = new VisualElement();
        hexBorder.borders = new VisualElement[6];
        for (int i = 0; i < 6; i++) {
            var border1 = new VisualElement();
            hexBorder.borderholder.Add(border1);
            border1.transform. rotation = Quaternion.Euler(0,0,60*i); 
            border1.transform.position = new Vector3(hexRadius/2,hexRadius/2,0);
            
            var border2 = new VisualElement();
            border2.pickingMode = PickingMode.Ignore;
            border1.Add(border2);
            border2.EnableInClassList("HexGrid-Hex-Border", true);
            var image = new Image();
            border2.Add(image);
            hexBorder.borders[i] = border2;
            //border2.style.backgroundImage = borderImage;
            border2.transform.position = new Vector3(borderOffsetX, borderOffsetY,0);
           
        }
        hexBorder.borderholder.transform.position = (Vector3) location;
        return hexBorder;
    }
    
    public void SetupHexBorders() {
        cubeCoordinates = new CubeCoordinates();
        AllToken = CubeCoordinates.AllContainer;
        localSpaceId =  CubeCoordinates.NewLocalSpaceId(hexRadius/2, Vector2.one, CubeCoordinates.LocalSpace.Orientation.XY,null,offsetCoord);
        var coordList = cubeCoordinates.Construct(exampleRadius);
        MakeAllHexBorders(localSpaceId);
        //NewMap();
    }
    
    public void MakeAllHexBorders(string aLocalSpaceId) {
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        if (!borders.ContainsKey(localSpaceId)) {
            borders[aLocalSpaceId] = new Dictionary<Vector3, HexBorder>();
        }
        
        // not in editor anymore should this change?
        //if (ls.spaceRectTransform != null) { not used in editor mode
        
            foreach (var coord in allCoords) {
                var localCoord = CubeCoordinates.ConvertPlaneToLocalPosition(coord.cubeCoord, ls);
                var hexBorder = MakeHexBorder(localCoord, borderHolder);
                borders[aLocalSpaceId][coord.cubeCoord] = hexBorder;
                borderHolder.Add(hexBorder.borderholder);
            }
        //}

    }
}