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
        if (rl.isRiding) {
            var partyId = rl.districtResult.candidateResults[0].partyId;
            Color color = PartyController.GetPartyData(partyId).color;
            ECSSpawner.Hex(v3, color);
        }
    }

    public void SetBorders() {
        foreach (var kvp in hexes[localSpaceId]) {
            var mapCell =(UIHexGridMapCell2)kvp.Value;
            var aRegionList = cellDataDict[kvp.Key];
                for (int i = 0;i<6;i++) {
                    int border = -1;
                    var hierarchy = aRegionList.hierarchyList;
                    var otherCoord = CubeCoordinates.CubeDirections[i] + kvp.Key;
                    if (!cellDataDict.ContainsKey(otherCoord)) {
                        continue; 
                    }
                    var otherHierarchy = cellDataDict[otherCoord].hierarchyList;
                    for (int j = 0; j < Mathf.Min(hierarchy.Count, otherHierarchy.Count);j++) {
                        if (hierarchy[j] != otherHierarchy[j]) {
                            border = j;
                            break;
                        }
                    }

                    mapCell.SetBorder(i, border);
            
                }
            
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
