using Com.Ryuuguu.HexGridCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Make a hex of superHexRadius
/// 
/// </summary>
public class UitHexGridExample : UitHexGrid {
    
    public int superHexRadius = 1;

    /// <summary>
    /// Makes all hexes in a given local space
    /// </summary>
    public void SetupHexes() {
        //make a hex or hexes 
        // FYI Construct returns a list of the hexes it makes as well as storing them in the local space
        cubeCoordinates.Construct(superHexRadius);
        MakeAllHexes(localSpaceId);
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        var hex = hexes[localSpaceId][allCoords[0].cubeCoord];
        ColorSubGrid(hex, true);
    }
    
    public UitSubHex MakeSubHex( Vector3 location,Vector3 scale) {
        var hex = new UitSubHex();
        hex.EnableInClassList("HexGrid-Hex", true);
        hex.transform.position = location;
        hex.transform.scale = scale ;
        hex.style.backgroundImage = null;
        return hex;
    }
    public List<VisualElement> MakeSquareSubHexes() {
        var scale = Vector3.one * 0.5f ;
       
        var result = new List<VisualElement>();
        var pos = Vector3.one*2f;
        //squares seem to twice the size of thir scale so *2
        for (int i = 0;i<2;i++) {
            for (int j = 0; j < 2; j++) {
                pos = new Vector3(i, j, 1)*2f;
                pos.Scale(scale);
                result.Add(MakeSubHex( pos,scale));
            }
        }
        return result;
    }
    public void  ColorSubGrid( VisualElement aParent, bool isSquare) {

        List<VisualElement> subHexes = new List<VisualElement>();
        if (isSquare) {
            subHexes = MakeSquareSubHexes();
        }
        
        for (int i = 0; i<2; i++) {
            var hex = subHexes[i];
            aParent.Add(hex);
            hex.style.backgroundColor= Color.red;
            
        }
        // Debug.Log("Subhex:" + childIndex + " : "  +hexDebug.transform.position + " : " 
        //            + hexDebug.transform.scale + " : " + hexDebug.style.backgroundColor  );
    
    }
}