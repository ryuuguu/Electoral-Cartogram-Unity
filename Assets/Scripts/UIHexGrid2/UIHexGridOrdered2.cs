using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHexGridOrdered2 : UIHexGrid {

    public List<HexUI> orderedCoords;


    public override void Init() {
        base.Init();
        foreach (var hex in orderedCoords) {
            var coord = cubeCoordinates.GetAddCoordinateFromContainer(hex.cubeCoord, AllToken);
            hexes[localSpaceId][coord.cubeCoord] = hex;
        }
    }
}
