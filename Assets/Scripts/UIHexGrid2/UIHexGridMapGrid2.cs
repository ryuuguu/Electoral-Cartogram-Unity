using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;

public class UIHexGridMapGrid2 : UIHexGrid {
    
    public Dictionary<Vector3, RegionList> cellDataDict = new Dictionary<Vector3, RegionList>();
    
    
    public UIHexGridMapCell2 CreateCell(Vector3 v3, bool isrectangle = false) {
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        var cell = (UIHexGridMapCell2)  AddCell(v3,ls);
        
        return cell;
    }
    
    public  void CreateCellRegion(Vector3Int v3,RegionList rl) {
        //hack to not draw WATER & USA because of speed problems
        if (rl.id == "Water" || rl.id == "USA" || rl.id == "Land") return;
        var cell = CreateCell(v3, false);
        cell.SetRegion(rl);
        cellDataDict[v3] = rl;
    }

    public void SetBorders() {
        foreach (var kvp in hexes[localSpaceId]) {
            var mapCell =(UIHexGridMapCell2)kvp.Value;
            mapCell.SetBorder(cellDataDict[kvp.Key]);
        }
    }
    
    public UIHexGridMapCell2 GetCellAt(Vector3 v3) {
        if (!hexes[localSpaceId].ContainsKey(v3)) return null;
        return (UIHexGridMapCell2) hexes[localSpaceId][v3];
    }

    public void HideVotes(bool val) {
        foreach (UIHexGridMapCell2 mapCell in hexes[localSpaceId].Values) {
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
    
}
