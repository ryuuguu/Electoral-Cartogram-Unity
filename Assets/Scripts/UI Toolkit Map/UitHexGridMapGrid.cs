using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;

public class UitHexGridMapGrid : UitHexGrid {
 /*   
    public UitHexGridMapCell CreateCell(Vector3 v3, bool isrectangle = false) {
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        var cell = (UitHexGridMapCell)  AddCell(v3,ls);
        return cell;
    }

    public  void CreateCellRegion(Vector3Int v3,RegionList rl) {
        //hack to not draw WATER & USA because of speed problems
        if (rl.id == "Water" || rl.id == "USA" || rl.id == "Land") return;
        var cell = CreateCell(v3, false);
        cell.SetRegion(rl);
    }
    
    //TODO* highlights
   */ 
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
