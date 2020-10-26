using System;
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
    public int subHexRadius = 1;

    /// <summary>
    /// Makes all hexes in a given local space
    /// </summary>
    public void SetupHexes() {
        //make a hex or hexes 
        // FYI Construct returns a list of the hexes it makes as well as storing them in the local space
        cubeCoordinates.Construct(superHexRadius);
        
        foreach (var hex in hexes[localSpaceId].Values) {
            hex.style.backgroundImage = cellBackground;
            hex.style.unityBackgroundImageTintColor = Color.red;
            ColorSubGrid(hex, false);
        }
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
        //squares seem to twice the size of this scale so *2
        for (int i = 0;i<2;i++) {
            for (int j = 0; j < 2; j++) {
                pos = new Vector3(i, j, 1)*2f;
                pos.Scale(scale);
                result.Add(MakeSubHex( pos,scale));
            }
        }
        return result;
    }
    public List<VisualElement>  ColorSubGrid( VisualElement aParent, bool isSquare) {
        var holder = new VisualElement();
        aParent.Add(holder);
        holder.transform.rotation = Quaternion.Euler(0,0,30);
        // need to shift holder because hex is placed using 
        // top left not center
        holder.transform.position += new Vector3(1, 1, 0) * 0.5f;
        List<VisualElement> subHexes = new List<VisualElement>();
        subHexes = MakeSubHexes(holder, subHexRadius, aParent.transform.scale.x);
        var color = new Color(0, 1, 1, 1f);
        foreach(var hex in subHexes){
            holder.Add(hex);
            hex.style.backgroundImage = cellBackground;
            hex.style.unityBackgroundImageTintColor = color;
        }

        return subHexes;
    }
    
    public List<Vector3> ConstructMegaHex(int radius) {
        //needs to be re-ordered by rings 
        var result = new List<Vector3>();
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                for (int z = -radius; z <= radius; z++) {
                    if ((x + y + z) == 0) {
                        result.Add(new Vector3(x, y, z));
                    }
                }
            }
        }
        return result;
    }

    UitSubHex MakeSubHex(Vector3 coord, float hexScale, float coordScale, Vector2 offset) {
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        var location = CubeCoordinates.ConvertPlaneToLocalPosition(coord, ls, offset);
        return MakeSubHex( location*coordScale, Vector3.one *hexScale);
    }

    List<VisualElement> MakeSubHexes(VisualElement parent, int radius, float coordScale ) {
        var result = new List<VisualElement>();
        var coords = ConstructMegaHex(radius);
        foreach (var coord in coords) {
           var hex =MakeSubHex(coord,1f / (2 * radius + 1f),
               1f / ((coordScale )*(2 * radius + 1)),
               new Vector2(0,0));
           // need to shift hexes because sub hex is placed using 
           // top left not center
           hex.transform.position +=  new Vector3(1,1,0) * (-0.5f/(2*radius+1) );
           
           result.Add(hex);
        }
        return result;
    }
    
}
