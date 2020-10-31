using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// access visual element and region data by coord
/// </summary>
public class UitHexMapGrid : UitHexGrid {

    public bool debugSubGridOff;
    
    public Texture2D hexBackground;
    public Texture2D squareBackgound;
    public Texture2D borderImage;
    public float subHexScale;
    
    public Vector2 mapSize = new Vector2(1600,800);
    public Rect mapCubeRect = new Rect(0, 0, 55, 21); 
    
    //private List<VisualElement> subGridHolders = new List<VisualElement>();
    
    public UitHex CreateCell(Vector3 v3, bool isRectangle = false) {
        var cell =  MakeHex(v3); // this creates the Visual element but not the region
        return cell;
    }

    public  UitHexGridMapCell CreateCellRegion(Vector3 v3,RegionList rl) {
        //Do not draw unnassignable region types
        if (!rl.isAssignable) return null;
        var uitHex = CreateCell(v3, false);
        
        UitHexGridMapCell mapCell =  new UitHexGridMapCell() {
            centerRiding = isSquare? squareBackgound : hexBackground,
                isSquare = isSquare,
                localSpaceId = localSpaceId,
                subHexScale = subHexScale,
                borderImage = borderImage
        };
        mapCell.uitCell = uitHex;
        mapCell.SetRegion(rl, v3);
        return mapCell;
    }
    
    public UitHexGridMapCell GetCellAt(Vector3 v3) {
        if (!hexes[localSpaceId].ContainsKey(v3)) return null;
        return (UitHexGridMapCell) hexes[localSpaceId][v3];
    }
    
}
