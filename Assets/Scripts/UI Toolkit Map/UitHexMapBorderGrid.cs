using System;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
public class UitHexMapBorderGrid : UitHexBorderGrid {
    
    
    //todo: refactor to use make only one localSpaceId
    // use it both grid and map
    
    public void SetupHexBorders() {
        cubeCoordinates = new CubeCoordinates();
        AllToken = CubeCoordinates.AllContainer;
        localSpaceId =  CubeCoordinates.NewLocalSpaceId(hexRadius/2, Vector2.one,
            CubeCoordinates.LocalSpace.Orientation.XY,null,offsetCoord);
    }
}
