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


    public override string Init(VisualElement aHexHolder) {
        hexRadius = 38.5f;
       
        
        var x = mapCubeRect.width - 1;
        var z =  (x/ 2f * -1) + mapCubeRect.height + 1;
        var y = -1 * (x + z);
        //add a kludged number to get edge of far right hex
        var bottomLeftCoord  = new Vector3( x+1.3f, y, z-1.3f) ;
        
        var calcSpaceId = CubeCoordinates.NewLocalSpaceId(1f / 2f, new Vector2(1,1), 
            CubeCoordinates.LocalSpace.Orientation.XY, null, offsetCoord);
        var cornerInVESpace =  CubeCoordinates.ConvertPlaneToLocalPosition(bottomLeftCoord,calcSpaceId);
        hexRadius = Mathf.Min((mapSize.x / cornerInVESpace.x), (mapSize.y/cornerInVESpace.y) );
        return base.Init(aHexHolder);
    }
    
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
