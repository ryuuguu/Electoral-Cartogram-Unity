﻿using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

/// <summary>
/// Add UitHex MakeHex(Vector3 coord,Vector2 location, VisualElement aHolder)
/// :: Retrieve UitHex hexes[aLocalSpaceId][coord.cubeCoord] 
/// 
/// </summary>
public class UitHexGrid :MonoBehaviour {

    public Texture2D cellBackground;

    public float hexRadius = 50f;
    public Vector2 offsetCoord =new Vector2(4,-3);
    
    public string localSpaceId;
    
    protected static string AllToken;
    
    // CubeCoordinates manages hexes in 3 a dimensional coordinate space
    // has utility functions like finding pathes and translating between cude coordinates and 2D local space for drawing
    public CubeCoordinates cubeCoordinates = new CubeCoordinates();
    
    public VisualElement hexHolder;

    // the inital string is a localSpaceID it allows multiple hex grids to handled separately 
    // inner dictionary is to find UI Toolkit elements from their coordinates
    public Dictionary<string,Dictionary<Vector3, UitHex>> hexes = new Dictionary<string,Dictionary<Vector3, UitHex>>();
    
    
    /// <summary>
    /// setup a local space
    /// </summary>
    /// <param name="aHexHolder"></param>
    public void Init(VisualElement aHexHolder) {
        hexHolder = aHexHolder;
        cubeCoordinates = new CubeCoordinates();
        AllToken = CubeCoordinates.AllContainer;
        localSpaceId =  CubeCoordinates.NewLocalSpaceId(hexRadius/2, Vector2.one,
            CubeCoordinates.LocalSpace.Orientation.XY,null,offsetCoord);
    }

    /// <summary>
    /// Makes a single UI ToolKit Hex at the correct location for its coord
    /// under the default VisualElement
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    protected UitHex MakeHex(Vector3 coord) {
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        var location= CubeCoordinates.ConvertPlaneToLocalPosition(coord, ls);
        return MakeHex(coord, location);
    }
    
    /// <summary>
    /// Makes a single UI ToolKit Hex at a given location 
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="location"></param>
    /// <param name="aHolder"></param>
    /// <returns></returns>
    protected UitHex  MakeHex(Vector3 coord,Vector2 location, VisualElement aHolder = null) {
        if (aHolder == null) {
            aHolder = hexHolder;
        } 
        var hex = MakeUitHex();
        aHolder.Add(hex);
        hex.name = coord.ToString();
        SetupHex(hex, location);
        // quick hack to test if this works, in later version need to assign it in a separate method
        hex.clickable.clicked += () => Debug.Log("Clicked! " + coord);
        return hex;
    }
    
    /// <summary>
    /// returns a single new UitHex
    /// </summary>
    /// <returns></returns>
    public UitHex MakeUitHex() {
        var hex = new UitHex();
        hex.EnableInClassList("HexGrid-Hex", true);
        return hex;
    }
    
    
    /// <summary>
    /// Make All hexes in a local space
    /// </summary>
    /// <param name="aLocalSpaceId"></param>
    public void MakeAllHexes(string aLocalSpaceId) {
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        if (!hexes.ContainsKey(localSpaceId)) {
            hexes[aLocalSpaceId] = new Dictionary<Vector3, UitHex>();
        }
         
        foreach (var coord in allCoords) {
            var localCoord = CubeCoordinates.ConvertPlaneToLocalPosition(coord.cubeCoord, ls);
            var hex = MakeHex(coord.cubeCoord, localCoord, hexHolder);
            hexes[aLocalSpaceId][coord.cubeCoord] = hex;
        }
    }
    
    /// <summary>
    /// setup location specific parts of the hex
    /// </summary>
    /// <param name="hex"></param>
    /// <param name="location"></param>
    public void SetupHex(UitHex hex, Vector2 location) {
        hex.transform.position = (Vector3) location;
        hex.style.backgroundImage = cellBackground;
        // Hack for initial testing
        //    tooltip is not implemented in the runtime system
        //    this might be used when I implement tooltip  
        hex.tooltip = hex.parent.name;
    }
    
}