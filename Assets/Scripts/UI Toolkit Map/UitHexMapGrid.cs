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
    public float subHexScale;
    
    public Vector2 mapSize = new Vector2(1600,800);
    
    private List<VisualElement> subGridHolders = new List<VisualElement>();
    
    public UitHex CreateCell(Vector3 v3, bool isRectangle = false) {
        var cell =  MakeHex(v3); // this creates the Visual element but not the region
        return cell;
    }

    public  void CreateCellRegion(Vector3Int v3,RegionList rl) {
        //hack to not draw WATER & USA because of speed problems
        if (rl.id == "Water" || rl.id == "USA" || rl.id == "Land") return;
        var uitHex = CreateCell(v3, false);
        
        UitHexGridMapCell mapCell =  new UitHexGridMapCell() {
            centerRiding = isSquare? squareBackgound : hexBackground,
                isSquare = isSquare,
                localSpaceId = localSpaceId,
                subHexScale = subHexScale
        };
        mapCell.uitCell = uitHex;
        var subgridHolder = mapCell.SetRegion(rl);
        if (subgridHolder != null) {
            subGridHolders.Add(subgridHolder);
        }
        
    }
    
    //TODO* highlights
   
    /*
    public void ClearHighLight() {
        
        foreach (UitHexGridMapCell mapCell in hexes[localSpaceId].Values) {
            mapCell.SetHighLight(false);
        }
        
    }
    */
    
    public UitHexGridMapCell GetCellAt(Vector3 v3) {
        if (!hexes[localSpaceId].ContainsKey(v3)) return null;
        return (UitHexGridMapCell) hexes[localSpaceId][v3];
    }
    
    
    
   
    //TODO: subgrid
    /*
    public void HideVotes(bool val) {
        foreach (UitHexGridMapCell mapCell in hexes[localSpaceId].Values) {
            if (!(mapCell.subGrid is null)) {
                if (val) {
                    mapCell.subGridHolder.SetSiblingIndex(mapCell.subGridPosition);  
                }
                else {
                    mapCell.subGridHolder.SetSiblingIndex(0);
                }
                //mapCell.subGrid.gameObject.SetActive(val);
            }
        }
    }
    */
}
