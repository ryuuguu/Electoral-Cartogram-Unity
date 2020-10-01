using System;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

/// <summary>
/// Very similar to UitHexGrid but handles placing the from 0 to 6 borders around a hex.
/// </summary>
public class UitHexBorderGrid :MonoBehaviour {
    
    public Texture2D borderImage;
    
    public float hexRadius = 50f;
    public float hexScaleFactor = 2;
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
    
    private HexBorder  MakeHexBorder(Vector2 location, Vector3 scale, VisualElement aHolder) {
        //radius is to vertex not center of an edge
        // offset needs to be distance to center of the edge
        
        HexBorder hexBorder = new HexBorder();
        hexBorder.borderholder = new VisualElement();
        hexBorder.borderholder.transform.position = (Vector3) location;
        hexBorder.borders = new VisualElement[6];
        for (int i = 0; i < 6; i++) {
            var border1 = new VisualElement();
            hexBorder.borderholder.Add(border1);
            border1.transform. rotation = Quaternion.Euler(0,0,60*i); 
            border1.transform.position = new Vector3(hexRadius/2,hexRadius/2,0);
            
            var border2 = new VisualElement();
            border2.transform.scale = scale;
            border2.pickingMode = PickingMode.Ignore; // border will not receive or block mouse clicks
            border1.Add(border2);
            border2.EnableInClassList("HexGrid-Hex-Border", true);
            border2.AddToClassList("Border");
            var image = new Image();
            border2.Add(image);
            hexBorder.borders[i] = border2;
            border2.style.backgroundImage = borderImage;
            border2.transform.position = new Vector3(borderOffsetX, borderOffsetY,0);
        }
        return hexBorder;
    }
    
    public void MakeAllHexBorders(string aLocalSpaceId) {
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        if (!borders.ContainsKey(localSpaceId)) {
            borders[aLocalSpaceId] = new Dictionary<Vector3, HexBorder>();
        }
        var scale = Vector3.one * ls.gameScale*hexScaleFactor;
        foreach (var coord in allCoords) {
            var localCoord = CubeCoordinates.ConvertPlaneToLocalPosition(coord.cubeCoord, ls);
            var hexBorder = MakeHexBorder(localCoord,scale, borderHolder);
            borders[aLocalSpaceId][coord.cubeCoord] = hexBorder;
            borderHolder.Add(hexBorder.borderholder);
        }
    }
}