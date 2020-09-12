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
public class UitHexBorderGridExample :UitHexBorderGrid {
    
    [FormerlySerializedAs("exampleRadius")] public int superHexRadius = 1;
  
    public void SetupHexBorders() {
        cubeCoordinates = new CubeCoordinates();
        AllToken = CubeCoordinates.AllContainer;
        localSpaceId =  CubeCoordinates.NewLocalSpaceId(hexRadius/2, Vector2.one, CubeCoordinates.LocalSpace.Orientation.XY,null,offsetCoord);
        var coordList = cubeCoordinates.Construct(superHexRadius);
        MakeAllHexBorders(localSpaceId);
    }
    
}